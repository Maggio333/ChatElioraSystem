using ChatElioraSystem.Core.Application_.Common.Factories;
using ChatElioraSystem.Core.Application_.Enums;
using ChatElioraSystem.Core.Application_.Services;
using ChatElioraSystem.Core.Infrastructure.Models;
using ChatElioraSystem.Core.Infrastructure.Services;
using ChatElioraSystemMobile.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace ChatElioraSystemMobile.ViewModels
{
    public class ChatViewModel : ObservableObject, IChatViewModel
    {
        private readonly IPromptTypeOrchiestratorService _promptTypeOrchiestratorService;
        private readonly IChatLogService _chatLogService;

        public ObservableCollection<IChatMessage> Messages { get; } = new();

        private IChatMessage _selectedMessage;
        public IChatMessage SelectedMessage
        {
            get => _selectedMessage;
            set => SetProperty(ref _selectedMessage, value);
        }

        public ObservableCollection<ChatSession> TopicsConversation { get; } = new();

        public event Action ScrollToEndRequested;
        private void OnScrollToEnd() => ScrollToEndRequested?.Invoke();

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                if (SetProperty(ref _inputText, value))
                {
                    SendCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private SesjaTematu _wybranyTemat = SesjaTematu.Ogólna;
        public SesjaTematu WybranyTemat
        {
            get => _wybranyTemat;
            set => SetProperty(ref _wybranyTemat, value);
        }

        private bool _canEditContent;
        public bool CanEditContent
        {
            get => _canEditContent;
            set => SetProperty(ref _canEditContent, value);
        }

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

        public IRelayCommand SendCommand { get; }
        public IRelayCommand CreateNewSessionCommand { get; }
        public IRelayCommand OpenSessionCommand { get; }


        public ChatViewModel(IPromptTypeOrchiestratorService promptTypeOrchiestratorService, IChatLogService chatLogService)
        {
            _promptTypeOrchiestratorService = promptTypeOrchiestratorService;
            _chatLogService = chatLogService;

            SendCommand = new AsyncRelayCommand(SendMessageAsync, CanSend);

            CreateNewSessionCommand = new RelayCommand(NewConversation);
            OpenSessionCommand = new RelayCommand<IEnumerable<IChatMessage>>(OpenSession);
        }

        private void OpenSession(IEnumerable<IChatMessage> messages)
        {
            SaveLastConversation();

            Messages.Clear();
            foreach (var msg in messages)
                Messages.Add(msg);
        }

        private void NewConversation()
        {
            SaveLastConversation();
            _chatLogService.GenerateNewFileName();
            Messages.Clear();
            InputText = string.Empty;
        }

        private void SaveLastConversation()
        {
            if (!Messages.Any())
                return;

            var fileName = Messages.First().FileName;
            _chatLogService.Save(Messages, fileName);
        }

        private bool CanSend() => !string.IsNullOrWhiteSpace(InputText);

        public async Task<string> OnUserSend()
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                var userMessage = new ChatMessage { Content = InputText, Role = Role.user };
                Messages.Add(userMessage);
                InputText = string.Empty;
            });

            return string.Empty;
        }

        public async Task<string> AskLLM(int llmNo, CancellationToken cancellationToken)
        {
            // ASSISTANT: LLM
            var assistantMessage = new ChatMessage { Content = "", Role = Role.assistant };
            Messages.Add(assistantMessage);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                OnScrollToEnd();
            });

            await foreach (var chunk in _promptTypeOrchiestratorService.SendStreamToLLM(Messages, WybranyTemat, llmNo, cancellationToken))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    assistantMessage.Content += chunk;
                });
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                OnScrollToEnd();
            });

            return assistantMessage.Content;
        }

        public async Task<string> LoadCurrentTopic(CancellationToken cancellationToken)
        {
            var topicFromDb = await _promptTypeOrchiestratorService.LoadCurrentTopic(cancellationToken);
            Messages.Add(ChatMessageFactory.System($"Odczyt z bazy wektorowej ostatnich dostępnych tematów -\n\n {topicFromDb}"));
            return topicFromDb;
        }

        public async Task<string> LoadFromDbVec(int llmNo, CancellationToken cancellationToken)
        {
            try
            {
                IChatMessage toAddView = null;

                // TOOL: Pobranie z wektora
                if (IsSaveToDbVec)
                {
                    var readDbVec = new ChatMessage { Content = "", Role = Role.tool };
                    Messages.Add(readDbVec);

                    await foreach (var chunk in _promptTypeOrchiestratorService.GetStreamDataFromVectorDb(Messages.Where(x => x.IsUser || x.IsAssistant), llmNo, cancellationToken))
                    {
                        if (chunk.TextChunk is { } txt)
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                readDbVec.Content += txt;
                            });
                        }

                        if (chunk.ChatMessages != null && chunk.ChatMessages.Last() != null && chunk.ChatMessages.Last().Content != string.Empty)
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                toAddView = chunk.ChatMessages.Last();
                                toAddView.Role = Role.system;
                                Messages.Add(toAddView);
                            });
                        }
                    }

                    return readDbVec.Content;
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage { Content = $"NIE DZIAŁA - {ex.Message}", Role = Role.user });
            }

            return string.Empty;
        }

        public async Task SaveTopicToDbVec(int llmNo, CancellationToken cancellationToken)
        {
            await _promptTypeOrchiestratorService.SaveCurrentTopic(Messages.Where(x => x.IsUser || x.IsAssistant), llmNo, cancellationToken);
        }


        public async Task<string> SaveToDbVec(int llmNo, CancellationToken cancellationToken)
        {
            IChatMessage toAddView = null;

            // TOOL: Zapis do wektora
            try
            {
                if (IsSaveToDbVec)
                {
                    var saveDbVec = new ChatMessage { Content = "", Role = Role.tool };
                    Messages.Add(saveDbVec);

                    List<IChatMessage> newMessage = [.. Messages.Where(x => x.IsUser || x.IsAssistant)];

                    await foreach (var chunk in _promptTypeOrchiestratorService.SaveStreamDataFromVectorDb(newMessage, llmNo, cancellationToken))
                    {
                        if (chunk.TextChunk is { } txt)
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                saveDbVec.Content += txt;

                            });
                        }

                        if (chunk.ChatMessages != null && chunk.ChatMessages.Last() != null && chunk.ChatMessages.Last().Content != string.Empty)
                        {
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
                                toAddView = chunk.ChatMessages.Last();
                                toAddView.Role = Role.system;
                                Messages.Add(toAddView);
                            });
                        }
                    }

                    RemoveToolMessages();
                    return saveDbVec.Content;
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatMessage { Content = $"NIE DZIAŁA - {ex.Message}", Role = Role.user });
            }

            return string.Empty;
        }

        private void RemoveToolMessages()
        {
            var messagesToRemove = new List<IChatMessage>();
            messagesToRemove.AddRange(Messages.Where(x => x.Role == Role.tool));

            foreach (var message in messagesToRemove)
            {
                Messages.Remove(message);
            }
        }

        private async Task<bool> SetBaseIdioms()
        {
            var result = await _promptTypeOrchiestratorService.GetBaseIdioms();
            Messages.Add(result);

            return true;
        }

        private async Task SendMessageAsync()
        {
            _promptTypeOrchiestratorService.IsSaveToDbVector = IsSaveToDbVec;

            using var cts = new CancellationTokenSource();

            try
            {
                await OnUserSend();
                await Task.Run(async () => await LoadFromDbVec(1, new CancellationToken()));
                await Task.Run(async () => await LoadCurrentTopic(cts.Token));
                await Task.Run(() => SetBaseIdioms());
                await Task.Run(async () => await AskLLM(1, cts.Token));


                RemoveToolMessages();

                // Zapis poza głównym strumieniem
                _ = Task.Run(async () => await SaveToDbVec(1, cts.Token));
                _ = Task.Run(async () => await SaveTopicToDbVec(1, new CancellationToken()));
            }
            catch (OperationCanceledException)
            {
                // Obsługa anulowania
            }

            SaveLastConversation();
            //LoadConversationHistorian();
        }

        //private async Task SendMessageAsync()
        //{
        //    _promptTypeOrchiestratorService.IsSaveToDbVector = IsSaveToDbVec;
        //    using var cts = new CancellationTokenSource();

        //    try
        //    {
        //        await OnUserSend();
        //        await LoadFromDbVec(1, cts.Token);
        //        await LoadCurrentTopic(1, cts.Token);
        //        await SetBaseIdioms();
        //        await AskLLM(1, cts.Token);

        //        RemoveToolMessages();
        //        OnScrollToEnd();

        //        // Zapis w tle
        //        _ = Task.Run(async () => await SaveToDbVec(1, cts.Token));
        //        _ = Task.Run(async () => await SaveTopicToDbVec(1, cts.Token));
        //    }
        //    catch (Exception ex)
        //    {
        //        Messages.Add(new ChatMessage { Content = $"Błąd: {ex.Message}", Role = Role.system });
        //    }
        //}
    }
}
