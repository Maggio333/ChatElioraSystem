using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Application_.Services;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using ChatElioraSystem.Presentation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace ChatElioraSystem.Presentation.ViewModels
{
    public class ChatViewModel : ObservableObject, IChatViewModel
    {
        private IPromptTypeOrchiestratorService _promptTypeOrchiestratorService;
        //public IJudgeCategoryService _judgeCategoryService;
        public IChatLogService _chatLogService;
        //private IBgTaskQueueService _bgTaskQueueService;

        public ObservableCollection<IChatMessage> Messages { get; } = new();

        private IChatMessage? _selectedMessage = default;
        public IChatMessage? SelectedMessage { get => _selectedMessage; set => SetProperty(ref _selectedMessage, value); }

        public event Action ScrollToEndRequested;

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set => SetProperty(ref _inputText, value);
        }

        private SesjaTematu _wybranyTemat;
        public SesjaTematu WybranyTemat
        {
            get => _wybranyTemat;
            set => SetProperty(ref _wybranyTemat, value);
        }

        private bool _canEditContent = false;
        public bool CanEditContent
        {
            get => _canEditContent;
            set => SetProperty(ref _canEditContent, value);
        }


        private ObservableCollection<ChatSession> _topicsConversation = new ObservableCollection<ChatSession>();
        public ObservableCollection<ChatSession> TopicsConversation
        {
            get => _topicsConversation;
            set => SetProperty(ref _topicsConversation, value);
        }

        public ICommand SendCommand { get; }
        public ICommand CreateNewSessionCommand { get; }
        public ICommand OpenSessionCommand { get; }

        private bool _isSaveToDbVec = true;
        public bool IsSaveToDbVec 
        {
            get => _isSaveToDbVec;
            set => SetProperty(ref _isSaveToDbVec, value);
        }

        private bool _isLogOnView = true;
        public bool IsLogOnView
        {
            get => _isLogOnView;
            set => SetProperty(ref _isLogOnView, value);
        }

        public ChatViewModel(IPromptTypeOrchiestratorService promptTypeOrchiestratorService, IChatLogService chatLogService)
        {
            WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(this, nameof(PropertyChanged), ViewModelPropertyChanged);

            _promptTypeOrchiestratorService = promptTypeOrchiestratorService;
            _chatLogService = chatLogService;

            SendCommand = new AsyncRelayCommand(SendMessageAsync, CanSend);
            CreateNewSessionCommand = new RelayCommand(NewConversation);
            OpenSessionCommand = new RelayCommand<IEnumerable<IChatMessage>>(OpenSession);

            LoadConversationHistorian();
        }

        private void ViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InputText))
            {
                ((AsyncRelayCommand)SendCommand).NotifyCanExecuteChanged();
            }
        }


        private void LoadConversationHistorian()
        {
            List<string> fileNameList = _chatLogService.ListLogFiles();
            TopicsConversation.Clear();
            foreach (string fileName in fileNameList)
            {
                IDictionary<string, object>? metaData;
                ObservableCollection<IChatMessage> messages = new ObservableCollection<IChatMessage>();

                IEnumerable<IChatMessage> messagesData = _chatLogService.Load(fileName, out metaData);

                foreach (var messageData in messagesData)
                {
                    messageData.FileName = fileName;
                    messages.Add(messageData);               
                }

                if (messages.Count == 0) { continue; }

                var lastMessage = messages.Last();
                string title;

                if (lastMessage.Content.Length > 25) 
                {
                    title = $"{lastMessage.Timestamp} {lastMessage.Content.Substring(0, 25)}";
                }
                else
                {
                    title = $"{lastMessage.Timestamp} {lastMessage.Content}";
                }


                TopicsConversation.Add(new ChatSession() { Messages = messages, Title = title, FileName = fileName });
            }
        }

        private void OpenSession(IEnumerable<IChatMessage> chatMessages)
        {
            SaveLastConversation();
            Messages.Clear();
            foreach(var message in chatMessages)
            {
                Messages.Add(message);
            }

            LoadConversationHistorian();
        }

        private void NewConversation()
        {
            SaveLastConversation();

            _chatLogService.GenerateNewFileName();

            Clear();
        }

        private void SaveLastConversation()
        {
            var message = Messages.FirstOrDefault();
            if (Messages != null && message != null)
            {
                var date = _chatLogService.Save(Messages, message.FileName);
            }
        }

        private void Clear()
        {
            Messages.Clear();
            InputText = string.Empty;
        }


        private bool CanSend() => !string.IsNullOrWhiteSpace(InputText);

        private void RemoveToolMessages()
        {
            var messagesToRemove = new List<IChatMessage>();
            messagesToRemove.AddRange(Messages.Where(x => x.Role == Role.tool));

            foreach (var message in messagesToRemove)
            {
                Messages.Remove(message);
            }
        }

        public async Task<string> SaveFromDbVec(int llmNo, CancellationToken cancellationToken)
        {
            if (!IsSaveToDbVec)
            {
                return string.Empty;
            }

            IChatMessage? toAddView = null;

            ChatMessage toolResponseSaveMessage = new() { Content = "", Role = Role.tool };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Messages.Add(toolResponseSaveMessage);
            });

            List<IChatMessage> newMessage = [.. Messages.Where(x => x.IsUser || x.IsAssistant)];

            await foreach (var chunk in _promptTypeOrchiestratorService.SaveStreamDataFromVectorDb(newMessage, llmNo, cancellationToken))
            {
                if (chunk.TextChunk != null)
                {
                    toolResponseSaveMessage.Content = toolResponseSaveMessage.Content + chunk.TextChunk;
                }

                if (chunk.ChatMessages != null && chunk.ChatMessages.Last() != null && chunk.ChatMessages.Last().Content != string.Empty)
                {
                    toAddView = chunk.ChatMessages.Last();
                    toAddView.Role = Role.system;
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Messages.Add(toAddView);
                    });
                }
            };
            return toAddView == null ? string.Empty : toAddView.Content;
        }

        public async Task<bool> SetCategory(int llmNo, CancellationToken cancellationToken)
        {
            var tematRozmowy = _promptTypeOrchiestratorService.GetCategory(Messages.Where(x => x.IsUser || x.IsAssistant), llmNo, cancellationToken).Result;

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                WybranyTemat = tematRozmowy;
            });

            return true;
        }

        public async Task<string> LoadFromDbVec(int llmNo, CancellationToken cancellationToken)
        {
            if (!IsSaveToDbVec)
            {
                return string.Empty;
            }

            IChatMessage? toAddView = null;
            var toolResponseGetMessage = new ChatMessage { Content = "", Role = Role.tool };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Messages.Add(toolResponseGetMessage);
            });

            await foreach (var chunk in _promptTypeOrchiestratorService.GetStreamDataFromVectorDb(Messages.Where(x => x.IsUser || x.IsAssistant), llmNo, cancellationToken))
            {
                if (chunk.TextChunk != null)
                {
                    toolResponseGetMessage.Content = toolResponseGetMessage.Content + chunk.TextChunk;
                }

                if (chunk.ChatMessages != null && chunk.ChatMessages.Last() != null && chunk.ChatMessages.Last().Content != string.Empty)
                {
                    toAddView = chunk.ChatMessages.Last();
                    toAddView.Role = Role.system;

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Messages.Add(toAddView);
                    });
                }
            }

            List<IChatMessage> newMessage = [.. Messages];

            return toAddView == null ? string.Empty : toAddView.Content;
        }

        private async Task LoadCurrentTopic(CancellationToken cancellationToken)
        {
            var topicFromDb = await _promptTypeOrchiestratorService.LoadCurrentTopic(cancellationToken);
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Messages.Add(ChatMessageFactory.System($"Odczyt z bazy wektorowej ostatnich dostępnych tematów -\n\n {topicFromDb}"));
            });
        }

        public static class ObjectComparer
        {
            // Bufor (cache) przechowujący skompilowane funkcje dla każdego typu T.
            // ConcurrentDictionary jest bezpieczny wątkowo (thread-safe).
            private static readonly ConcurrentDictionary<Type, Delegate> _comparerCache =
                new ConcurrentDictionary<Type, Delegate>();

            /// <summary>
            /// Pobiera lub generuje superszybką funkcję do porównywania dwóch obiektów typu T.
            /// </summary>
            /// <typeparam name="T">Typ obiektu do porównania.</typeparam>
            /// <returns>Skompilowany delegat Func<T, T, bool>.</returns>
            public static Func<T, T, bool> GetComparer<T>()
                where T : class // Ograniczenie, aby pracować na typach referencyjnych (klasach)
            {
                // 1. Sprawdzenie bufora. Jeśli delegat istnieje, zwracamy go od razu.
                if (_comparerCache.TryGetValue(typeof(T), out Delegate cachedDelegate))
                {
                    return (Func<T, T, bool>)cachedDelegate;
                }

                // --- BUDOWANIE DRZEWA WYRAŻEŃ ---

                // 2. Definicja parametrów wejściowych (o1 i o2)
                ParameterExpression o1 = System.Linq.Expressions.Expression.Parameter(typeof(T), "o1");
                ParameterExpression o2 = System.Linq.Expressions.Expression.Parameter(typeof(T), "o2");

                // 3. Refleksja - pobranie publicznych, czytelnych właściwości
                var publicProperties = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead);

                // Lista wyrażeń porównujących
                List<System.Linq.Expressions.Expression> comparisonExpressions = new List<System.Linq.Expressions.Expression>();

                // 4. Budowanie logiki dla każdej właściwości
                foreach (var prop in publicProperties)
                {
                    // Tworzymy wyrażenia dostępu do właściwości: o1.PropName oraz o2.PropName
                    MemberExpression propO1 = System.Linq.Expressions.Expression.Property(o1, prop);
                    MemberExpression propO2 = System.Linq.Expressions.Expression.Property(o2, prop);
                    // Tworzymy wyrażenie porównujące: o1.PropName == o2.PropName
                    // Uwaga: używamy statycznej metody Equals, która jest bezpieczniejsza dla null
                    // i radzi sobie z typami wartościowymi i referencyjnymi.
                    MethodInfo equalsMethod = typeof(object).GetMethod("Equals", new[] { typeof(object), typeof(object) });

                    System.Linq.Expressions.Expression equalityCheck = System.Linq.Expressions.Expression.Call(
                        equalsMethod,
                        System.Linq.Expressions.Expression.Convert(propO1, typeof(object)), // boxing
                        System.Linq.Expressions.Expression.Convert(propO2, typeof(object))  // boxing
                    );
                    comparisonExpressions.Add(equalityCheck);
                }
                // Jeśli nie ma właściwości, obiekty są równe (lub można zwrócić wyjątek)
                if (comparisonExpressions.Count == 0)
                {
                    // Zakładamy, że obiekty bez publicznych właściwości są zawsze równe
                    Expression<Func<T, T, bool>> defaultLambda = (a, b) => true;
                    return defaultLambda.Compile();
                }
                // 5. Łączenie wszystkich wyrażeń 'AND'
                // Zaczynamy od pierwszego i iteracyjnie łączymy z pozostałymi
                System.Linq.Expressions.Expression finalBody = comparisonExpressions.First();
                foreach (var expression in comparisonExpressions.Skip(1))
                {
                    finalBody = System.Linq.Expressions.Expression.AndAlso(finalBody, expression); // operator &&
                }

                // 6. Utworzenie wyrażenia lambda (o1, o2) => [finalBody]
                var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, T, bool>>(
                    finalBody,
                    o1,
                    o2);

                // 7. Kompilacja i buforowanie
                var compiledComparer = lambda.Compile();
                _comparerCache.TryAdd(typeof(T), compiledComparer);

                return compiledComparer;
            }
        }

        private async Task<bool> SendToLLM(IEnumerable<IChatMessage> chatMessages, int llmNo, CancellationToken cancellationToken)
        {
            ChatMessage responseMessage = new ChatMessage { Content = "", Role = Role.assistant };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Messages.Add(responseMessage);
            });

            await foreach (var chunk in _promptTypeOrchiestratorService.SendStreamToLLM(Messages, WybranyTemat, 1, cancellationToken))
            {
                responseMessage.Content = responseMessage.Content + chunk;
                await Application.Current.Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);
            }

            return true; 
        }

        private async Task<bool> SaveCurrentTopic(CancellationToken cancellationToken)
        {
            if (!IsSaveToDbVec)
            {
                return false;      
            }

            var saveTopicResult = await _promptTypeOrchiestratorService.SaveCurrentTopic(Messages.Where(x => x.IsUser || x.IsAssistant), 1, cancellationToken);

            if (saveTopicResult != null && saveTopicResult.Content != string.Empty)
            {
                var toAddView = saveTopicResult;
                toAddView.Role = Role.system;
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Messages.Add(toAddView);
                });
            }

            return true;
        }


        private async Task<bool> SetBaseIdioms()
        {
            var result = await _promptTypeOrchiestratorService.GetBaseIdioms();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Messages.Add(result);
            });

            return true;
        }

        public void OnUserSend()
        {
            Messages.Add(new ChatMessage { Content = $"{InputText}", Role = Role.user });
            SelectedMessage = Messages.LastOrDefault();
            InputText = string.Empty;
        }

        private async Task SendMessageAsync()
        {
            _promptTypeOrchiestratorService.IsSaveToDbVector = IsSaveToDbVec;

            using var cts = new CancellationTokenSource();

            try
            {
                OnUserSend();

                await Task.Run(() => SetCategory(1, cts.Token));
                await Task.Run(async () => await LoadFromDbVec(1, new CancellationToken()));
                await Task.Run(async () => await LoadCurrentTopic(cts.Token));
                await Task.Run(() => SetBaseIdioms());
                await Task.Run(async () => await SendToLLM(Messages, 1, cts.Token));


                RemoveToolMessages();

                // Zapis poza głównym strumieniem
                _ = Task.Run(async () => await SaveFromDbVec(1, cts.Token));
                _ = Task.Run(async () => await SaveCurrentTopic(cts.Token));
            }
            catch (OperationCanceledException)
            {
                // Obsługa anulowania
            }

            SaveLastConversation();
            LoadConversationHistorian();
        }
    }
}
