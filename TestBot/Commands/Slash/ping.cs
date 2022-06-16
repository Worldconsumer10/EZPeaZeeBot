using Discord.Interactions;
using ExampleBot;

namespace EZPeaZeeBot //the namespace needs to be this or else the bot will not find the command
{
    public class PingSlash : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping","a simple ping command")]
        public async Task Run()
        {
            await RespondAsync($"Pong! {Program._client.Latency}ms");
        }
    }
}
