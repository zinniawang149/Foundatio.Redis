﻿using System;
using System.Threading.Tasks;
using Foundatio.Jobs;
using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Foundatio.SampleJob
{
    public class Program
    {
        private static ILogger _logger;

        public static int Main()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger("MessageBus");

            var serviceProvider = SampleServiceProvider.Create(loggerFactory);
            var jobOptions = JobOptions.GetDefaults<PingQueueJob>(() => serviceProvider.GetRequiredService<PingQueueJob>());
            var messageBus = serviceProvider.GetRequiredService<IMessageBus>();
            messageBus.SubscribeAsync<EchoMessage>(m => HandleEchoMessage(m)).GetAwaiter().GetResult();
            return new JobRunner(jobOptions).RunInConsoleAsync().GetAwaiter().GetResult();
        }

        private static void HandleEchoMessage(EchoMessage m)
        {
            _logger.LogInformation($"Got message: {m.Message}");
        }
    }

    public class EchoMessage
    {
        public string Message { get; set; }
    }
}
