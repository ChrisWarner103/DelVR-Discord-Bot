using DelVRBot.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using SimpleJSON;

namespace DelVRBot.Commands
{
    public class EventCommands : BaseCommandModule
    {

        [Command("CreateGlobalEvent")]
        public async Task CreateGlobalEvent(CommandContext ctx, [RemainingText] string config)
        {
            var httpClient = new HttpClient();
            string URL = "https://delvr.studiodelvr.com/CreateGlobalEvent.php";

            if (config.Contains(","))
            {
                int index = config.IndexOf(',');

                string eventName = config.Substring(0, index);
                string eventDateTime = config.Substring(index + 1);

                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();

                keyValues.Add(new KeyValuePair<string, string>("EventName", eventName));
                keyValues.Add(new KeyValuePair<string, string>("EventDateTime", eventDateTime));

                var content = new FormUrlEncodedContent(keyValues);

                //var parametersJson = new JSONObject();

                //parametersJson.Add("EventName", "Test Event");
                //parametersJson.Add("EventDateTime", "2020 - 02 - 28 19:30:00");

                ////string parametersJson = "{'EventName':'Test Event','EventDate':'2020-02-28 19:30:00'}";

                //var stringContent = new StringContent("'EventName', 'Test Event'");

                var response = await httpClient.PostAsync(URL, content);

                response.EnsureSuccessStatusCode();

                string content1 = await response.Content.ReadAsStringAsync();

                await ctx.Channel.SendMessageAsync(content1 + " | Event Created");
            }else
            {
                await ctx.Channel.SendMessageAsync("Invalid Arguments, please try again!");
            }
            
            //return await Task.Run(() => JSONObject.Parse(content));
            
        }
    }
}
