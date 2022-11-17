using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace EZPeaZeeBot
{
    /// <summary>
    /// Custom Console.. Allows color within the console
    /// <br><b><i>Cannot Be Inherited!</i></b></br>
    /// </summary>
    sealed public class CustomConsole
    {
        /// <summary>
        /// Append text content to console with custom foreground or background colors
        /// </summary>
        /// <param name="content">Content to be written</param>
        /// <param name="foregroundColor">ConsoleColor Enumerator for foreground (text) color</param>
        /// <param name="backgroundColor">ConsoleColor Enumerator for background color</param>
        public static void WriteLine(string content,ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(content);
            Console.ResetColor();
        }
        /// <summary>
        /// Append text character to console with custom foreground or background colors
        /// </summary>
        /// <param name="content">Content to be written</param>
        /// <param name="foregroundColor">ConsoleColor Enumerator for foreground (text) color</param>
        /// <param name="backgroundColor">ConsoleColor Enumerator for background color</param>
        public static void WriteLine(char content,ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(content);
            Console.ResetColor();
        }
        /// <summary>
        /// Append text content to console without inserting a new line. With custom foreground or background colors
        /// </summary>
        /// <param name="content">Content to be written</param>
        /// <param name="foregroundColor">ConsoleColor Enumerator for foreground (text) color</param>
        /// <param name="backgroundColor">ConsoleColor Enumerator for background color</param>
        public static void Write(string content, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(content);
            Console.ResetColor();
        }
        /// <summary>
        /// Append text character to console without inserting a new line. With custom foreground or background colors
        /// </summary>
        /// <param name="content">Content to be written</param>
        /// <param name="foregroundColor">ConsoleColor Enumerator for foreground (text) color</param>
        /// <param name="backgroundColor">ConsoleColor Enumerator for background color</param>
        public static void Write(char content, bool colored = false, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(content);
            Console.ResetColor();
        }
        static List<Tuple<LogSeverity, ConsoleColor>> SeverityColor = new List<Tuple<LogSeverity, ConsoleColor>>();
        private static List<Tuple<LogSeverity, ConsoleColor>> CreateSeverityColors()
        {
            List<Tuple<LogSeverity, ConsoleColor>> colors = new List<Tuple<LogSeverity, ConsoleColor>>();
            colors.Add(Tuple.Create(LogSeverity.Debug, ConsoleColor.DarkYellow));
            colors.Add(Tuple.Create(LogSeverity.Verbose, ConsoleColor.Cyan));
            colors.Add(Tuple.Create(LogSeverity.Info, ConsoleColor.Green));
            colors.Add(Tuple.Create(LogSeverity.Warning, ConsoleColor.DarkYellow));
            colors.Add(Tuple.Create(LogSeverity.Error, ConsoleColor.Red));
            colors.Add(Tuple.Create(LogSeverity.Critical, ConsoleColor.DarkRed));
            return colors;
        }
        public static Task WriteDiscordLog(Discord.LogMessage log)
        {
            if (SeverityColor == null || SeverityColor.Count <= 0) { SeverityColor = CreateSeverityColors(); }
            var color = SeverityColor.Find(c => c.Item1 == log.Severity) ?? SeverityColor[0];
            Console.ForegroundColor = color.Item2;
            Console.WriteLine($"[{log.Severity}] {log}");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
    /// <summary>
    /// Used in creating the bot.
    /// <br><b><i>Cannot Be Inherited!</i></b></br>
    /// </summary>
    sealed public class Bot
    {
        #pragma warning disable CS8618
        internal static DiscordSocketClient _client { get; private set; }
        internal static Config _config { get; private set; }
        #pragma warning restore CS8618
        /// <summary>
        /// Creates a bot and logs it in with the provided config.
        /// </summary>
        /// <param name="config">
        /// A Config Object Created Through Config.Create()
        /// </param>
        /// <param name="hasPrivilage">
        /// Set this to true if you have enabled both privilage's through the developer portal
        /// </param>
        /// <returns>
        /// A DiscordSocketClient to be used for event registry and other related actions.
        /// </returns>
        public static async Task<DiscordSocketClient> Create(Config config,bool hasPrivilage = false)
        {
            _config = config;
            var client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                ConnectionTimeout = 30000,
                LogGatewayIntentWarnings = false,
                LogLevel = LogSeverity.Info,
                GatewayIntents = !hasPrivilage ? Discord.GatewayIntents.AllUnprivileged : Discord.GatewayIntents.All
            });
            var ourdate = new TimeSpan(DateTime.Now.Ticks);
            CustomConsole.WriteLine($"[Info] {ourdate.Hours}:{ourdate.Minutes}:{ourdate.Seconds} Gateway     Logging In",ConsoleColor.Green);
            await client.LoginAsync(Discord.TokenType.Bot,config.token);
            await client.StartAsync();
            _client = client;
            _client.Disconnected += _client_Disconnected;
            return client;
        }

        private static async Task _client_Disconnected(Exception arg)
        {
            await Task.Delay(10000);
            if (_client == null) return;
            if (_client.ConnectionState == ConnectionState.Connected) return;
            CustomConsole.WriteLine("[EZPZBot] Auto Reconnecting",ConsoleColor.DarkYellow);
            await _client.LogoutAsync();
            await _client.StopAsync();
            await _client.LoginAsync(TokenType.Bot,_config.token);
            await _client.StartAsync();
        }

        /// <summary>
        /// Creates a bot and logs it in with the provided config.
        /// <br>This is the advanced version that allows you to make your own DiscordSocketConfig. This config is different from your login config</br>
        /// </summary>
        /// <param name="config">
        /// A Config Object Created Through Config.Create()
        /// </param>
        /// <param name="socketConfig">
        /// A DiscordSocketConfig Create through new DiscordSocketConfig(){}
        /// </param>
        /// <param name="hasPrivilage">
        /// Set to true if both privilages within the developer portal are set to enabled
        /// </param>
        /// <returns></returns>
        public static async Task<DiscordSocketClient> CreateAdvanced(Config config,DiscordSocketConfig socketConfig, bool hasPrivilage = false)
        {
            _config = config;
            var client = new DiscordSocketClient(socketConfig);
            var ourdate = new TimeSpan(DateTime.Now.Ticks);
            CustomConsole.WriteLine($"[Info] {ourdate.Hours}:{ourdate.Minutes}:{ourdate.Seconds} Gateway     Logging In", ConsoleColor.Green);
            await client.LoginAsync(Discord.TokenType.Bot, config.token);
            await client.StartAsync();
            _client = client;
            return client;
        }
    }
    /// <summary>
    /// Used in creating the bot.
    /// <br><i>Can Be Inherited!</i></br>
    /// </summary>
    public class Config
    {
        public string token { get; private set; } = string.Empty;
        public string prefix { get; private set; } = "ez!";
        /// <summary>
        /// Creates a Config Object from a absolute file path
        /// </summary>
        /// <param name="path_to_config">
        /// An Absolute file path to a config file. Config file must be a .config file.
        /// </param>
        /// <exception cref="Exception">File Path Invalid</exception>
        /// <returns>
        /// A Config Object with the token and prefix
        /// </returns>
        public static Config Create(string path_to_config)
        {
            if (!path_to_config.Contains('\\')) throw new Exception("File Path Invalid");
            if (!path_to_config.Split("\\").Last().EndsWith(".config")) throw new Exception("Target File Is Not A .config File");
            try
            {
                var file = File.ReadAllLines(path_to_config);
                var token = string.Empty;
                var prefix = string.Empty;
                foreach (var line in file)
                {
                    var linecontent = string.Join("", line.Split(" "));
                    var index = linecontent.Split(":")[0];
                    var value = linecontent.Split(":")[1];
                    if (index.ToLower().Contains("token") && token == string.Empty)
                    {
                        token = Regex.Matches(value, "\"([^\"]*)\"").ElementAt(0).ToString().TrimStart('"').TrimEnd('"');
                    }
                    else if (index.ToLower().Contains("prefix") && prefix == string.Empty)
                    {
                        prefix = Regex.Matches(value, "\"([^\"]*)\"").ElementAt(0).ToString().TrimStart('"').TrimEnd('"');
                    }
                }
                return new Config() { prefix=prefix,token=token};
            }
            catch(Exception ex) { throw new Exception($"Unexpected Error While Created Config.\n{ex.Source}\n{ex.Message}\n\n{ex.StackTrace}"); }
        }
    }
    /// <summary>
    /// Interaction Handler And Creator.
    /// <br><b><i>Cannot Be Inherited!</i></b></br>
    /// </summary>
    sealed public class Interactions
    {
#pragma warning disable CS8618
        internal static InteractionService _interactions { get; set; }
        internal static bool help_create { get; set; } = true;
        #pragma warning restore CS8618
        /// <summary>
        /// Automatically creates and handles commands for you.
        /// <br>Includes a help command by default</br>
        /// <para><b>CAUTION: </b>Will throw an exception if the bot hasnt been created first.</para>
        /// </summary>
        /// <param name="create_help">Should the help command be automatically created or not</param>
        /// <exception cref="Exception"></exception>
        /// <returns>
        /// An InteractionService Object. Allowing you to access the Interactions
        /// </returns>
        public static InteractionService Create(bool create_help = true)
        {
            if (Bot._client == null) throw new Exception("Bot Hasnt been Initalized Yet.");
            help_create = create_help;
            var service = new InteractionService(Bot._client, new InteractionServiceConfig()
            {
                EnableAutocompleteHandlers = true,
                LogLevel = LogSeverity.Warning,
                DefaultRunMode = Discord.Interactions.RunMode.Async
            });
            _interactions = service;
            RegisterCommands(service);
            return service;
        }
        private static async Task UploadSlashCommands(InteractionService service)
        {
            foreach (var guild in Bot._client.Guilds)
            {
                if (guild == null) continue;
                try
                {
                    await service.RegisterCommandsToGuildAsync(guild.Id, true);
                    CustomConsole.WriteLine($"[EZPeaZeePackage] Slash Commands For: {guild.Name} [Registered]",ConsoleColor.Green);
                }catch(Exception)
                {
                    CustomConsole.WriteLine($"[EZPeaZeePackage] Slash Commands For: {guild.Name} [Failed To Register]", ConsoleColor.Red);
                }
            }
        }
        private static async void RegisterCommands(InteractionService service)
        {
            await service.AddModulesAsync(assembly: System.Reflection.Assembly.GetEntryAssembly(), services: null);
            if (!help_create && service.Modules.Any(m=>m.Name=="HelpBaseInteraction"))
            {
                await service.RemoveModuleAsync(service.Modules.First(i => i.Name == "HelpBaseInteraction"));
            }
            foreach (var cmd in service.SlashCommands)
            {
                if (!cmd.Name.Contains(' ')) continue;
                await service.RemoveModuleAsync(cmd.Module);
            }
            Bot._client.Ready += Readied;
            Bot._client.SlashCommandExecuted += SlashCommandExecuted;
        }
        private static async Task Readied() { await UploadSlashCommands(_interactions); Bot._client.Ready -= Readied; }

        private static async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            try
            {
                if (!_interactions.SlashCommands.Any(s=>s.Name == arg.CommandName)) { 
                    await arg.RespondAsync($"Slash Command: {arg.CommandName} has no valid endpoint.");
                    var ourdate = new TimeSpan(DateTime.Now.Ticks);
                    CustomConsole.WriteLine($"[Critical] {ourdate.Hours}:{ourdate.Minutes}:{ourdate.Seconds} Discord     Slash Command: {arg.CommandName} was run but the bot doesnt have a command registered from that.", ConsoleColor.DarkRed);
                    return;
                }
                var ctx = new SocketInteractionContext(Bot._client, arg);
                var res = await _interactions.ExecuteCommandAsync(ctx, services: null);
            }
            catch (Exception) { }
        }
    }
    /// <summary>
    /// Command Handler And Creator
    /// <br><b><i>Cannot Be Inherited!</i></b></br>
    /// </summary>
    sealed public class Commands
    {
        #pragma warning disable CS8618
        internal static CommandService _commands { get; set; }
        internal static bool help_create { get; set; } = true;
        #pragma warning restore CS8618
        /// <summary>
        /// Automatically creates and handles commands for you.
        /// <br>Includes a help command by default</br>
        /// <para><b>CAUTION: </b>Will throw an exception if the bot hasnt been created first.</para>
        /// </summary>
        /// <param name="create_help">Should the help command be automatically created or not</param>
        /// <exception cref="Exception"></exception>
        /// <returns>
        /// An InteractionService Object. Allowing you to access the Interactions
        /// </returns>
        public static CommandService Create(bool create_help = true)
        {
            if (Bot._client == null) throw new Exception("Bot Hasnt been Initalized Yet.");
            help_create = create_help;
            var service = new CommandService(new CommandServiceConfig()
            {
                DefaultRunMode = Discord.Commands.RunMode.Async,
                IgnoreExtraArgs = true,
                CaseSensitiveCommands=false,
                LogLevel = LogSeverity.Warning
            });
            _commands = service;
            RegisterCommands(service);
            return service;
        }
        private static async void RegisterCommands(CommandService service)
        {
            await service.AddModulesAsync(assembly: System.Reflection.Assembly.GetEntryAssembly(), services: null);
            if (!help_create && service.Modules.Any(m => m.Name == "HelpBaseCommand"))
            {
                await service.RemoveModuleAsync(service.Modules.First(i => i.Name == "HelpBaseCommand"));
            }
            foreach (var cmd in service.Commands)
            {
                if (!cmd.Name.Contains(' ')) continue;
                await service.RemoveModuleAsync(cmd.Module);
            }
            Bot._client.MessageReceived += CommandMessage;
        }
        public static async Task<IUserMessage> InlineReply(SocketCommandContext context,string? text = null, bool isTTS = false, Embed? embed = null,AllowedMentions? allowedMentions=null, RequestOptions? requestoptions=null, MessageComponent? messageComponents=null, ISticker[]? stickers = null, Embed[]? embedarray=null)
        {
            return await context.Message.ReplyAsync(text, isTTS, embed, allowedMentions, requestoptions, messageComponents, stickers, embedarray);
        }

        private static async Task CommandMessage(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;

            int ArgPos = 0;
            if (!message.HasStringPrefix(Bot._config.prefix, ref ArgPos) || message.Author.IsBot) return;

            var context = new SocketCommandContext(Bot._client, message);

            var command = message.Content.Split(" ")[0].Split(Bot._config?.prefix)[1];

            if (!_commands.Commands.Where(c => c.Name == command).Any()) return;

            await message.DeleteAsync();

            await _commands.ExecuteAsync(
                context: context,
                argPos: ArgPos,
                services: null
                );
        }
    }
    sealed public class HelpBaseCommand : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Discord.Commands.Summary("help command")]
        public async Task Run()
        {
            var commands = Commands._commands.Commands.ToList();
            List<Embed> embed_list = new List<Embed>();
            var embed = new EmbedBuilder().WithAuthor(Context.User).WithCurrentTimestamp().WithTitle("Help Command");
            for (int i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                if (command == null) continue;
                if (embed.Fields.Count >= 25)
                {
                    embed_list.Add(embed.Build());
                    embed.Fields.Clear();
                }
                embed.AddField($"{i+1}", $"Name: **{command.Name}**\nDescription: **{command.Summary}**\nRun With: **{Bot._config.prefix}{command.Name}**");
            }
            embed_list.Add(embed.Build());
            await ReplyAsync(embeds: embed_list.ToArray());
        }
    }
    sealed public class HelpBaseInteraction : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("help","help command")]
        public async Task Run()
        {
            var commands = Commands._commands.Commands.ToList();
            List<Embed> embed_list = new List<Embed>();
            var embed = new EmbedBuilder().WithAuthor(Context.User).WithCurrentTimestamp().WithTitle("Help Command");
            for (int i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                if (command == null) continue;
                if (embed.Fields.Count >= 25)
                {
                    embed_list.Add(embed.Build());
                    embed.Fields.Clear();
                }
                embed.AddField($"{i + 1}", $"Name: **{command.Name}**\nDescription: **{command.Summary}**\nRun With: **{Bot._config.prefix}{command.Name}**");
            }
            embed_list.Add(embed.Build());
            await RespondAsync(embeds: embed_list.ToArray());
        }
    }
}