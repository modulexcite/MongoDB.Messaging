﻿using System;
using MongoDB.Messaging;
using MongoDB.Messaging.Service;
using Sleep.Messages;

namespace Sleep.Service
{
    public class Program
    {
        private static MessageService _messageService;

        public static void Main(string[] args)
        {
            ShowVersion();
            Initialize();
            DebugRun(args);
        }

        private static void Initialize()
        {
            MessageQueue.Default.Configure(c => c
                .Connection("MongoMessaging")
                .Queue(q => q
                    .Name(SleepMessage.QueueName)
                    .Retry(5)
                )
                .Subscribe(s => s
                    .Queue(SleepMessage.QueueName)
                    .Handler<SleepHandler>()
                    .Workers(4)
                )
            );

            _messageService = new MessageService();
        }

        private static void ShowVersion()
        {
            Console.WriteLine("{0} {1}", ThisAssembly.AssemblyProduct, ThisAssembly.AssemblyFileVersion);
            Console.WriteLine(ThisAssembly.AssemblyCopyright);
            Console.WriteLine();
        }

        private static void DebugRun(string[] args)
        {
            _messageService.Start();

            DebugHelp();

            while (true)
            {
                string line = Console.In.ReadLine() ?? string.Empty;
                if (string.Equals(line, "P", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Pausing....");
                    _messageService.Stop();
                }

                if (string.Equals(line, "C", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Continuing....");
                    _messageService.Start();
                }

                if (string.Equals(line, "R", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Restarting...");
                    _messageService.Stop();
                    _messageService.Start();
                }

                if (string.Equals(line, "Q", StringComparison.OrdinalIgnoreCase))
                {
                    _messageService.Stop();
                    break;
                }

                DebugHelp();
            }

            _messageService.Stop();
        }

        private static void DebugHelp()
        {
            Console.WriteLine("Service running in debug mode.");
            Console.WriteLine("  C.  Continue service");
            Console.WriteLine("  P.  Pause service");
            Console.WriteLine("  R.  Restart service");
            Console.WriteLine("  Q.  Quit");
        }
    }
}
