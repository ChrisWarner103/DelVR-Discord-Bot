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
using Tweetinvi;
using DSharpPlus.CommandsNext.Exceptions;

namespace DelVRBot
{
    class Bot
    {
        public static DiscordClient Client { get; set; }
        public CommandsNextExtension Commands { get; private set; }
        public VoiceNextExtension Voice { get; set; }
        public InteractivityExtension interactivity { get; private set; }

        public DiscordMessage reactionMessage { get; private set; }

        public DiscordGuild Guild { get; private set; }

        public DiscordUser currentUser { get; private set; }

        public DiscordEmoji dungeonMasterEmoji { get; set; }
        public DiscordEmoji lookingForGroupEmoji { get; set; }

        public static List<string> emojiNames { get; private set; }
        public static List<ulong> roleIDs { get; private set; }

        public static List<DiscordEmoji> roleEmojis { get; private set; }
        public static List<DiscordRole> discordRoles { get; private set; }

        public static string json { get; set; }

        public readonly EventId BotEventId = new EventId(42, "Bot-Ex02");


        public async Task RunAsyn()
        {
            json = string.Empty;

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

            Guild = await Client.GetGuildAsync(configJson.Guild, true).ConfigureAwait(false);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {

            });

            var commandConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                DmHelp = true,
            };

            LoadReactionEmojisFromConfig();

            Commands = Client.UseCommandsNext(commandConfig);

            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            Client.MessageReactionAdded += Client_MessageReactionAdded;
            Client.MessageReactionRemoved += Client_MessageReactionRemoved;
            Client.GuildMemberAdded += Client_GuildMemeberJoined;
            Client.Resumed += Client_Resumed;

            Commands.SetHelpFormatter<CustomHelpFormatter>();

            Commands.RegisterCommands<DiceCommands>();
            Commands.RegisterCommands<RoleCommands>();

            await Client.ConnectAsync();

            Voice = Client.UseVoiceNext();

            DiscordChannel roleReactChannel;

            //This is only called when there are values inside the config file. Otherwise it is ignored
            if (configJson.ReactionChannel != 0)
            {
                roleReactChannel = await Client.GetChannelAsync(configJson.ReactionChannel).ConfigureAwait(false);

                if (configJson.ReactionMessage != 0)
                {
                    reactionMessage = await roleReactChannel.GetMessageAsync(configJson.ReactionMessage).ConfigureAwait(false);
                }
            }
            //Anything added to this function needs to be before this
            await Task.Delay(-1);
        }

        //This is called when a user joins the discord sever
        private async Task Client_GuildMemeberJoined(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            ulong DelverRole = 664986265438912512;
            var defaultRole = Guild.GetRole(DelverRole);
            await e.Member.GrantRoleAsync(defaultRole, "Joined the server!");
        }

        private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            // let's log the error details
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = new DiscordColor(0xFF0000) // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }

        private Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} used the command '{e.Command}'", DateTime.Now);
            return Task.CompletedTask;
        }

        private void LoadReactionEmojisFromConfig()
        {
            var result = JObject.Parse(json);
            var discordEmojis = result["discordEmojis"].Children();
            var discordRoles = result["discordRoles"].Children();

            var emojis = JArray.FromObject(discordEmojis);
            var roles = JArray.FromObject(discordRoles);

            emojiNames = new List<string>();
            roleIDs = new List<ulong>();

            foreach (var result1 in emojis)
            {
                foreach (JArray JEmojis in result1["emojis"])
                {
                    for (int i = 0; i < JEmojis.Count; i++)
                    {
                        string name = JEmojis[i]["name"].ToString();

                        emojiNames.Add(name);
                    }

                }
            }

            foreach (var result2 in roles)
            {
                foreach (JArray JRoles in result2["roles"])
                {
                    for (int i = 0; i < JRoles.Count; i++)
                    {
                        string idString = JRoles[i]["id"].ToString();

                        ulong id = UInt64.Parse(idString);

                        roleIDs.Add(id);
                    }

                }
            }
        }

        private async Task Client_MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            roleEmojis = new List<DiscordEmoji>();
            discordRoles = new List<DiscordRole>();

            for (int i = 0; i < emojiNames.Count; i++)
            {
                roleEmojis.Add(DiscordEmoji.FromUnicode(Client, emojiNames[i]));
                discordRoles.Add(Guild.GetRole(roleIDs[i]));
            }

            DiscordUser discordUser = await sender.GetUserAsync(e.User.Id);
            DiscordMember discordMemeber = await e.Guild.GetMemberAsync(discordUser.Id);

            for (int i = 0; i < roleEmojis.Count; i++)
            {
                string emojiName = e.Emoji.GetDiscordName();
                if (emojiName == roleEmojis[i] && discordMemeber.Roles.Contains(discordRoles[i]))
                {
                    await discordMemeber.RevokeRoleAsync(discordRoles[i]).ConfigureAwait(false);
                    return;
                }
            }
        }

        private async Task Client_MessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            roleEmojis = new List<DiscordEmoji>();
            discordRoles = new List<DiscordRole>();

            for (int i = 0; i < emojiNames.Count; i++)
            {
                roleEmojis.Add(DiscordEmoji.FromUnicode(Client, emojiNames[i]));
                discordRoles.Add(Guild.GetRole(roleIDs[i]));
            }

            DiscordUser discordUser = await sender.GetUserAsync(e.User.Id);
            DiscordMember discordMemeber = await e.Guild.GetMemberAsync(discordUser.Id);

            for (int i = 0; i < roleEmojis.Count; i++)
            {

                string emojiName = e.Emoji.GetDiscordName();

                if (emojiName == roleEmojis[i])
                {
                    await discordMemeber.GrantRoleAsync(Guild.GetRole(roleIDs[i])).ConfigureAwait(false);
                    return;
                }
            }
        }

        private async Task Client_Resumed(DiscordClient sender, ReadyEventArgs e)
        {
            Client.Ready += OnClientReady;

            Commands.RegisterCommands<DiceCommands>();
            Commands.RegisterCommands<RoleCommands>();

            Commands.SetHelpFormatter<CustomHelpFormatter>();

            Client.MessageReactionAdded += Client_MessageReactionAdded;
            Client.MessageReactionRemoved += Client_MessageReactionRemoved;
            Client.Resumed += Client_Resumed;

            await Client.ConnectAsync();
        }

        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
