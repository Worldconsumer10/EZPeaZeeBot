using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using EZPeaZeeBot;

namespace ExampleBot
{
    public class Program
    {
        #pragma warning disable CS8618
        public static InteractionService _interactions;
        public static CommandService _commands;
        public static DiscordSocketClient _client;
        #pragma warning restore CS8618
        public static void Main(string[] args) => Init();
        static async void Init()
        {
            var config = Config.Create("C:\\Users\\User\\Documents\\VSProjects\\config.config");
            _client = await Bot.Create(config, true);
            _client.Log += CustomConsole.WriteDiscordLog;
            _interactions = Interactions.Create();
            _commands = EZPeaZeeBot.Commands.Create();
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}