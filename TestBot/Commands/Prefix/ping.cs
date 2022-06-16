using Discord.Commands;
using ExampleBot;

namespace EZPeaZeeBot //the namespace needs to be this or else the bot will not find the command
{
    public class PingCmd : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("a simple ping command")]
        public async Task Run()
        {
            await ReplyAsync($"Pong! {Program._client.Latency}ms"); //you might ask wont the client be null? No. It wont. Commands are registered AFTER the bot is created but before its ready so the latency function will still work.
        }
    }
}
