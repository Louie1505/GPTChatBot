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
        [Command("fact")]
        [Summary("Sends a specific fact")]
        public async Task Fact(int id)
        {
            APIHelper.CallAPI(out object fact, HttpVerbs.Get, typeof(Fact), "Facts", id.ToString());
            if (fact == null)
                return;
            // Access the channel from the Command Context.
            await Context.Channel.SendMessageAsync(((Fact)fact).sFact);
        }
        [Command("dump")]
        [Summary("Dumps all facts from API")]
        public async Task Dump()
        {
            APIHelper.CallAPI(out object facts, HttpVerbs.Get, typeof(List<Fact>), "Facts", null, "Dump");
            if (facts == null)
                return;
            StringBuilder s = new StringBuilder();
            foreach (Fact f in ((List<Fact>)facts))
            {
                s.Append($"\r\n{f.sFact}");
            }
            // Access the channel from the Command Context.
            await Context.Channel.SendMessageAsync(s.ToString());
        }
        [Command("DidYouKnow")]
        [Summary("Informs the bot of a new fact, which will be added to the library")]
        public async Task DidYouKnow([Remainder]string sFact)
        {
            Fact fact = new Fact { sFact = sFact };
            APIHelper.CallAPI(out _, HttpVerbs.Post, typeof(object), "Facts", null, null, fact);
            await Context.Channel.SendMessageAsync("I did not - That was very interesting, thank you 😉");
        }
        [Command("kys")]
        [Summary("idk man I'm a sick fuck")]
        public async Task kys()
        {
            await Context.Channel.SendMessageAsync("https://thumbs.gfycat.com/PaleApprehensiveIaerismetalmark-mobile.mp4");
        }

        [Command("art")]
        [Summary("REEEEEEEEEEEEEE")]
        public async Task art([Remainder]string search)
        {
            if(string.IsNullOrEmpty(search))
                return;
            search = search.Trim();
            string url = $"https://rule34.xxx/index.php?page=dapi&s=post&q=index&tags={search}";
            HttpClient client = new HttpClient();
            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get,
                };
                var resp = await client.SendAsync(request);
                if (!resp.IsSuccessStatusCode)
                    throw new WebException();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(resp.Content.ReadAsStringAsync().Result);
                var nodes = xmlDoc.SelectNodes("posts")[0]?.ChildNodes;
                if (nodes == null || nodes.Count < 1)
                    throw new Exception();
                Random randy = new Random();
                var post = nodes[randy.Next(0, nodes.Count - 1)];
                await Context.Channel.SendMessageAsync(post.Attributes["file_url"].Value);
            }
            catch (Exception)
            {
                await Context.Channel.SendMessageAsync("I was unable to find anything in my gallery of beautiful art 😢");
            }
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
