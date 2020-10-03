using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GPTChatBot
{
    class Program
    {
        public static IConfigurationRoot configuration; 
        public static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(
              options => options.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>()
                  .Configure<EventLogSettings>(config =>
                  {
                      config.LogName = "Sample Service";
                      config.SourceName = "Sample Service Source";
                  });
                // Add access to generic IConfigurationRoot
                services.AddSingleton<IConfigurationRoot>(configuration);
            }).UseWindowsService();
        } 
    }
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                new Bot().StartAsync().GetAwaiter().GetResult();
            }
        }
    }
    public class Bot
    {
        private DiscordSocketClient _client;
        public async Task StartAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            var token = Program.configuration["Token"];

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            CommandHandler handler = new CommandHandler(_client, new Discord.Commands.CommandService());
            await handler.InstallCommandsAsync();
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}