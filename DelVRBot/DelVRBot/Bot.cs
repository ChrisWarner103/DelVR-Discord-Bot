using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using DelVRBot.Commands;
using DSharpPlus.VoiceNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Attributes;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DelVRBot
{
    class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public VoiceNextExtension Voice { get; set; }
        public InteractivityExtension interactivity { get; private set; }

        public DiscordMessage reactionMessage { get; private set; }

        public DiscordGuild Guild { get; private set; }

        public DiscordUser currentUser { get; private set; }

        public DiscordEmoji dungeonMasterEmoji { get; set; }
        public DiscordEmoji lookingForGroupEmoji { get; set; }

        public List<string> emojiNames { get; private set; }
        public List<ulong> roleIDs { get; private set; }

        public List<DiscordEmoji> roleEmojis { get; private set; }
        public List<DiscordRole> discordRoles { get; private set; }


        public async Task RunAsyn()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.All,
            };

            Client = new DiscordClient(config);
            Guild = await Client.GetGuildAsync(695835309270761472, true).ConfigureAwait(false);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {

            });

            var commandConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = false,
                DmHelp = true,
                
            };

            var result = JObject.Parse(json);
            var discordEmojis = result["discordEmojis"].Children();
            var discordRoles = result["discordRoles"].Children();

            var emojis = JArray.FromObject(discordEmojis);
            var roles = JArray.FromObject(discordRoles);

            //var test = JToken.Parse(":ChrisThink:");

            //emojis.Add(test);

            emojiNames = new List<string>();
            roleIDs = new List<ulong>();

            foreach (var result1 in emojis)
            {
                foreach (JObject JEmojis in result1["emojis"])
                {
                    string name = JEmojis["name"].ToString();

                    emojiNames.Add(name);
                }
            }

            foreach (var result2 in roles)
            {
                foreach (JObject JRoles in result2["roles"])
                {
                    string idString = JRoles["id"].ToString();

                    ulong id = UInt64.Parse(idString);

                    roleIDs.Add(id);
                }
            }

            Commands = Client.UseCommandsNext(commandConfig);

            Commands.RegisterCommands<DiceCommands>();
            Commands.RegisterCommands<RoleCommands>();

            Commands.SetHelpFormatter<CustomHelpFormatter>();

            await Client.ConnectAsync();

            Voice = Client.UseVoiceNext();


            DiscordChannel roleReactChannel = await Client.GetChannelAsync(782812708990877736).ConfigureAwait(false);
            reactionMessage = await roleReactChannel.GetMessageAsync(782812845539852298).ConfigureAwait(false);

            await RoleReactionMessage();


            //Anything added to this function needs to be before this
            await Task.Delay(-1);
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        public async Task RoleReactionMessage()
        {
            roleEmojis = new List<DiscordEmoji>();
            discordRoles = new List<DiscordRole>();

            for (int i = 0; i < emojiNames.Count; i++)
            {
                roleEmojis.Add(DiscordEmoji.FromName(Client, emojiNames[i]));
                discordRoles.Add(Guild.GetRole(roleIDs[i]));
            }

            var interactivity = Client.GetInteractivity();

            while (true)
            {
                var reactionResults = await interactivity.WaitForReactionAsync(
                    x => x.Message == reactionMessage).ConfigureAwait(false);

                Client.MessageReactionRemoved += Client_MessageReactionRemoved;

                if (reactionResults.Result != null)
                {
                    for (int i = 0; i < roleEmojis.Count; i++)
                    {
                        if(reactionResults.Result.Emoji == roleEmojis[i])
                        {
                            DiscordMember reactedUser = (DiscordMember)reactionResults.Result.User;

                            await reactedUser.GrantRoleAsync(discordRoles[i]).ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        private async Task Client_MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            DiscordUser discordUser = await sender.GetUserAsync(e.User.Id);
            DiscordMember discordMemeber = await e.Guild.GetMemberAsync(discordUser.Id);

            for (int i = 0; i < roleEmojis.Count; i++)
            {
                if (e.Emoji == roleEmojis[i] && discordMemeber.Roles.Contains(discordRoles[i]))
                {
                    await discordMemeber.RevokeRoleAsync(discordRoles[i]).ConfigureAwait(false);
                }
            }
        }

        public struct DiscordInfo
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
    }
}
