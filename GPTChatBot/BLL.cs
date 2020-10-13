using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPTChatBot
{
    static class BLL
    {
        public static string input(SocketCommandContext context, IEnumerable<IMessage> messages) 
        { 
            StringBuilder input = new StringBuilder();
            string formatter = ConfigMan.Get("messageFormat");
            foreach (var message in messages.ToArray().Reverse())
            {
                if(!string.IsNullOrEmpty(formatter))
                    input.Append((formatter + "\r\n").Replace("[author]", message.Author.Username).Replace("[content]", message.Content);
                else
                    input.Append($"{message.Author.Username}: {message.Content}\r\n");
            }
            return input.ToString();
        }
        public static bool ShouldRandomlySend(SocketCommandContext context, IEnumerable<IMessage> messages, SocketSelfUser selfUser)
        {
            //Fire once every x messages on average, configurable
            Random randy = new Random();
            int i = randy.Next(0, int.Parse(ConfigMan.Get("respChance")));
            if (!int.TryParse(ConfigMan.Get("weightFactor"), out int w))
                w = 0;
            //If we have a message in the set from us then make it likelier to respond, simulate a conversation. Kinda. Needs work.
            if (messages.Any(m => m.Author == selfUser))
                i -= w;

            if (i > 0)
                return false;

            return true;
        }
        /*This makes me sad*/
        public static bool CheckIsOtherBotPrefix(char c)
        {
            if (c == '~' || c == '!' || c == '#')
                return true;
            return false;
        }
    }
}
