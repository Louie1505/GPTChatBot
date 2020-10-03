using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Xml;
using System.Linq;

namespace GPTChatBot
{
    public class CommandsModule : ModuleBase<SocketCommandContext>
    {
        [Command("fact")]
        [Summary("Sends a random fact from API")]
        public async Task Fact()
        {
            APIHelper.CallAPI(out object fact, HttpVerbs.Get, typeof(Fact), "Facts");
            if (fact == null)
                return;
            // Access the channel from the Command Context.
            await Context.Channel.SendMessageAsync(((Fact)fact).sFact);
        }
    }
    class Fact
    {
        public int ID { get; set; }
        public string sFact { get; set; }
        public Fact()
        {
        }
    }
}
