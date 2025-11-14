//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Threading.Channels;
//using System.Threading.Tasks;
//using System.Windows.Threading;

//namespace ChatElioraSystem.Core.Infrastructure.Services
//{
//    public interface IBgTaskQueueService
//    {
//        void Enqueue(Func<Task> taskFunc);
//        void EnqueueWithUI(Func<Task> backgroundWork, Action uiWork);
//        void EnqueueWithUI(Func<Task> backgroundWork, Func<Task> uiWorkAsync);
//    }

//    public class BgTaskQueueService : IBgTaskQueueService
//    {
//        private readonly Channel<Func<Task>> _queue;
//        private readonly Dispatcher _dispatcher;

//        public BgTaskQueueService()
//        {
//            _queue = Channel.CreateUnbounded<Func<Task>>();
//            StartProcessor();
//        }

//        public void Enqueue(Func<Task> taskFunc)
//        {
//            _queue.Writer.TryWrite(taskFunc);
//        }

//        public void EnqueueWithUI(Func<Task> backgroundWork, Action uiWork)
//        {
//            Enqueue(async () =>
//            {
//                await backgroundWork();
//                await _dispatcher.InvokeAsync(uiWork);
//            });
//        }

//        public void EnqueueWithUI(Func<Task> backgroundWork, Func<Task> uiWorkAsync)
//        {
//            Enqueue(async () =>
//            {
//                await backgroundWork();
//                await _dispatcher.InvokeAsync(uiWorkAsync);
//            });
//        }

//        private void StartProcessor()
//        {
//            Task.Run(async () =>
//            {
//                await foreach (var taskFunc in _queue.Reader.ReadAllAsync())
//                {
//                    try
//                    {
//                        await taskFunc();
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.WriteLine($"[BgTaskQueueService] Błąd: {ex.Message}");
//                        // Możesz tu dorzucić logger, retry itd.
//                    }
//                }
//            });
//        }
//    }
//}
