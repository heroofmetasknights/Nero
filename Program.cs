using System;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Microsoft.Extensions.DependencyInjection;

namespace Nero
{
    class Program
    {
        
        static void Main(string[] args) {
            new Program().StartAsync().GetAwaiter().GetResult();
        }


        private DiscordSocketClient _client;

        public async Task StartAsync() {
            // Ensures the configuration file has been created.
            Configuration.EnsureExists();
           
            // Creates a new client
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 1000
            });

            await _client.SetGameAsync("!n help");

            _client.Log += Log;
            _client.JoinedGuild += Joined;

            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token);
            await _client.StartAsync();

            var services = ConfigureServices(); 
            await new CommandHandler().Install(services);

            await Task.Delay(-1); // Prevents Console Window from closing.
        }

        public IServiceProvider ConfigureServices(){
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<InteractiveService>();
                //.AddPaginator(_client, Log);
            return services.BuildServiceProvider();
        }

        private Task Log(LogMessage msg){
            var cc = Console.ForegroundColor;
            string logPath = Path.Combine(AppContext.BaseDirectory, @"log.txt");

            switch (msg.Severity) {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            

            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}");
            
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }

        private async Task Joined(SocketGuild guild) {
            var owner = guild.Owner;
            
            await Nero.ServerIntro.JoinedServer(guild, owner);
            
            

            return;
        }

    }
}
