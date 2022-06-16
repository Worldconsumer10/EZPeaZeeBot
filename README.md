# EZPeaZeeBot
Create a discord bot in 2 lines! <br></br>
<a href="https://discord.gg/CM57nS8Kc4">
  <img src="https://discord.com/api/guilds/877249283726905475/widget.png" alt="Help Discord">
</a>
<br></br>
<br></br>
# Creating A Bot
After you reference the bot with nuget you can create a bot by adding these two lines
```cs
Config _config = Config.Create("path\to\your\config\file"); //config file must end with .config or it will throw an error
DiscordSocketClient client = await Bot.Create(_config);
```
This will start the bot completely. If you want custom DiscordSocketConfig use this instead:
```cs
Config _config = Config.Create("path\to\your\config\file"); //config file must end with .config or it will throw an error
DiscordSocketClient client = await Bot.CreateAdvanced(_config, new DiscordSocketConfig());
```
For writing to a Console Application add this line
```cs
client.OnLog += CustomConsole.WriteDiscordLog;
```
this will color the text related to its severity:
```
Cyan -> Debug
Green -> Info
Dark Yellow -> Warning
Red -> Error
Dark Red -> Critical
```
For registering commands add these 2 lines
```cs
InteractionService _interaction = EZPeaZeeBot.Interactions.Create();
CommandService _commands = EZPeaZeeBot.Commands.Create();
```
The bot comes with a help command in-built. If you want to use your own help command change thoes lines to
```cs
InteractionService _interaction = EZPeaZeeBot.Interactions.Create(false);
CommandService _commands = EZPeaZeeBot.Commands.Create(false);
```
<br></br>
Finally for creating your own commands you can do this
<br></br>
For Prefix Commands
```cs
using Discord.Commands;

namespace EZPeaZeeBot
{
    public class ExampleCmd : ModuleBase<SocketCommandContext>
    {
        [Command("example")] //adding spaces within the command name will cause the bot to ignore this command
        [Summary("an example command")]
        public async Task Run()
        {
            await ReplyAsync("Command Run!");
        }
    }
}
```
For Slash Commands:
```cs
using Discord.Interactions;

namespace EZPeaZeeBot
{
    public class ExampleSlash : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("example","an example command")] //adding spaces within the command name will cause the bot to ignore this command
        public async Task Run()
        {
            await RespondAsync("Command Run!");
        }
    }
}
```
By the way, you must set the namespace to be `EZPeaZeeBot` or the program will not be able to find the command. (This may be fixed at a later stage)
