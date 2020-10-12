using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPTChatBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private int messageNum;
        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            //generic add modules
            //await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
            //                                services: null);

            //add my module manually
            await _commands.AddModuleAsync(typeof(CommandsModule), null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            //Ignore bots
            if (message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);
            messageNum = int.Parse(ConfigMan.Get("NumMessages"));
            var messages = await context.Channel.GetMessagesAsync(context.Message, Direction.Before, int.Parse(ConfigMan.Get("NumMessages"))).FlattenAsync();
            if (message.HasCharPrefix(ConfigMan.Get("Prefix")[0], ref argPos))
            {
                if (Program.debug || Regex.IsMatch(message.Content.Trim(), @"\+debug", RegexOptions.IgnoreCase))
                {
                    // Execute the command with the command context we just
                    // created, along with the service provider for precondition checks.
                    await _commands.ExecuteAsync(
                        context: context,
                        argPos: argPos,
                        services: null);
                }
                else
                {
                    await context.Channel.SendMessageAsync("This command requires debug mode, send +debug to toggle debug mode.");
                }
            }
            else if((BLL.ShouldRandomlySend(context, messages, _client.CurrentUser) || Program.talkative) || message.HasMentionPrefix(_client.CurrentUser, ref argPos) && !BLL.CheckIsOtherBotPrefix(message.Content[0]))
            {
                /*@ people properly*/
                await context.Channel.SendMessageAsync();
            }
        }
    }
}
