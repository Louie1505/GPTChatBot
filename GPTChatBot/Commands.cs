using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GPTChatBot
{
    public class CommandsModule : ModuleBase<SocketCommandContext>
    {

        [Command("debug")]
        [Summary("Toggle debug mode, enables commands")]
        public async Task Debug()
        {
            Program.debug = !Program.debug;
            await Context.Channel.SendMessageAsync((Program.debug ? "Entering debug mode" : "Exiting debug mode"));
        }

        [Command("input")]
        [Summary("Dumps out the input that it would send to the AI")]
        public async Task Input()
        {
            var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, int.Parse(ConfigMan.Get("NumMessages"))).FlattenAsync();
            await Context.Channel.SendMessageAsync(BLL.input(Context, messages));
        }
        [Command("config")]
        [Summary("Dumps out the config that isn't secret")]
        public async Task Config()
        {
            StringBuilder bob = new StringBuilder();
            bob.Append($"Generating text from last {ConfigMan.Get("numMessages")} messages (numMessages)\r\n");
            bob.Append($"1/{ConfigMan.Get("respChance")} chance of responding (respChance)\r\n");
            bob.Append($"Conversation weighted by {ConfigMan.Get("weightFactor")} (weightFactor)\r\n");
            await Context.Channel.SendMessageAsync(bob.ToString());
        }
        [Command("setconfig")]
        [Summary("Sets a given config on the fly")]
        public async Task SetConfig(string key, string value)
        {
            if (ConfigMan.Config.ContainsKey(key))
            {
                ConfigMan.Update(key, value);
                await Context.Channel.SendMessageAsync($"Updated {key} to {value}");
            }
        }
        [Command("configjson")]
        [Summary("Dumps out the config as JSON because I'm too lazy to write it")]
        public async Task ConfigJSON()
        {
            await Context.Channel.SendMessageAsync(JsonConvert.SerializeObject(ConfigMan.Config, Formatting.Indented));
        }
        [Command("respond")]
        [Summary("Set flag to always respond for debugging")]
        public async Task Respond()
        {
            Program.talkative = !Program.talkative;
            await Context.Channel.SendMessageAsync($"Always respond {(Program.talkative ? "enabled" : "disabled")}");
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
