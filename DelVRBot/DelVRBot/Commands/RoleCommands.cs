using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelVRBot.Commands
{
    class RoleCommands : BaseCommandModule
    {
        private InteractivityResult<MessageReactionAddEventArgs> reactionResults;

        static DiscordMember reactedUser;

        List<EmojiProperties> emojis;
        List<RoleProperties> roles;

        DiscordEmoji dungeonMasterEmoji;
        DiscordEmoji lookingForGroupEmoji;

        DiscordRole dungeonMasterRole;
        DiscordRole lookingForGroupRole;

        /// Once you make the edit function you won't need the role reacting part.

        [Command("CreateRollAssigner")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Alchemist")]
        public async Task RollAssigner(CommandContext ctx)
        {

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "React using the emoji to get a specific role",
                Color = DiscordColor.Orange,
                Description = "React with <:DungeonMaster:786729797438013451> to become a Dungeon Master\n" +
                              "React with <:LookingForGroup:785988073283649578> to get the Looking for Group role",
            };

            var reactionMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            if (Program.DebugMode)
            {
                dungeonMasterEmoji = DiscordEmoji.FromName(ctx.Client, ":ChrisThink:");
                lookingForGroupEmoji = DiscordEmoji.FromName(ctx.Client, ":ChrisGag:");

                dungeonMasterRole = ctx.Guild.GetRole(738025540593778700);
                lookingForGroupRole = ctx.Guild.GetRole(782768719796502549);
            }
            else
            {
                dungeonMasterEmoji = DiscordEmoji.FromName(ctx.Client, ":DungeonMaster:");
                lookingForGroupEmoji = DiscordEmoji.FromName(ctx.Client, ":LookingForGroup:");

                dungeonMasterRole = ctx.Guild.GetRole(664693406320164864);
                lookingForGroupRole = ctx.Guild.GetRole(785984414156324874);
            }



            List<DiscordEmoji> reactionEmojis = new List<DiscordEmoji>();
            List<ulong> roleIDs = new List<ulong>();

            AssignerEmojis discordEmojis = new AssignerEmojis();
            emojis = new List<EmojiProperties>();

            AssignerRoles discordRoles = new AssignerRoles();
            roles = new List<RoleProperties>();

            reactionEmojis.Add(dungeonMasterEmoji);
            reactionEmojis.Add(lookingForGroupEmoji);

            roleIDs.Add(dungeonMasterRole.Id);
            roleIDs.Add(lookingForGroupRole.Id);

            await reactionMessage.CreateReactionAsync(dungeonMasterEmoji).ConfigureAwait(false);
            await reactionMessage.CreateReactionAsync(lookingForGroupEmoji).ConfigureAwait(false);


            var jsonObj = JObject.Parse(Bot.json);
            var emojisArray = jsonObj.GetValue("discordEmojis") as JArray;

            for (int i = 0; i < reactionEmojis.Count; i++)
            {
                EmojiProperties emote = new EmojiProperties();
                RoleProperties role = new RoleProperties();
                string emojiName = reactionEmojis[i].GetDiscordName();
                emote.Name = emojiName;
                role.ID = roleIDs[i];
                role.Name = ctx.Guild.GetRole(roleIDs[i]).Name;

                emojis.Add(emote);
                roles.Add(role);
            }

            discordEmojis.Emojis = new List<List<EmojiProperties>>();
            discordEmojis.Emojis.Add(emojis);
            var discordEmojisToken = JObject.FromObject(discordEmojis);

            jsonObj["discordEmojis"][0] = discordEmojisToken;

            discordRoles.Roles = new List<List<RoleProperties>>();
            discordRoles.Roles.Add(roles);
            var discordRolesToken = JObject.FromObject(discordRoles);

            jsonObj["discordRoles"][0] = discordRolesToken;

            ulong channelID = ctx.Channel.Id;
            ulong reactionMessageID = reactionMessage.Id;

            jsonObj["reactionRolesChannel"] = channelID;
            jsonObj["reactionRolesMesage"] = reactionMessageID;

            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("config.json", output);

            await ctx.Channel.DeleteMessageAsync(ctx.Message).ConfigureAwait(false);

            await UpdateRoles(ctx).ConfigureAwait(false);

            Bot.Client.MessageReactionRemoved += Client_MessageReactionRemoved;

            while (true)
            {
                var interactivity = ctx.Client.GetInteractivity();

                //var reactionResults = await interactivity.CollectReactionsAsync(reactionMessage).ConfigureAwait(false);

                reactionResults = await interactivity.WaitForReactionAsync(
                    x => x.Message == reactionMessage &&
                    x.Emoji == dungeonMasterEmoji || x.Emoji == lookingForGroupEmoji).ConfigureAwait(false);

                if (reactionResults.Result.Emoji == dungeonMasterEmoji)
                {
                    if (Program.DebugMode)
                    {
                        var role = ctx.Guild.GetRole(roleIDs[0]);

                        reactedUser = (DiscordMember)reactionResults.Result.User;

                        await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
                    }
                    else
                    {
                        var role = ctx.Guild.GetRole(roleIDs[0]);

                        reactedUser = (DiscordMember)reactionResults.Result.User;

                        await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
                    }

                }
                else if (reactionResults.Result.Emoji == lookingForGroupEmoji)
                {
                    if (Program.DebugMode)
                    {
                        var role = ctx.Guild.GetRole(roleIDs[1]);

                        reactedUser = (DiscordMember)reactionResults.Result.User;

                        await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
                    }
                    else
                    {
                        var role = ctx.Guild.GetRole(roleIDs[1]);

                        reactedUser = (DiscordMember)reactionResults.Result.User;

                        await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);
                    }

                }
            }
        }
        private async Task Client_MessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            DiscordUser discordUser = await sender.GetUserAsync(e.User.Id);
            DiscordMember discordMemeber = await e.Guild.GetMemberAsync(discordUser.Id);

            for (int i = 0; i < emojis.Count; i++)
            {
                string emojiName = e.Emoji.GetDiscordName();
                if (emojiName == emojis[i].Name && discordMemeber.Roles.Contains(e.Guild.GetRole(roles[i].ID)))
                {
                    await discordMemeber.RevokeRoleAsync(e.Guild.GetRole(roles[i].ID)).ConfigureAwait(false);
                    return;
                }
            }
        }

        //Updates the roles in Bot.cs so that when the bot restarts it can still use the reaction roles.
        public async Task UpdateRoles(CommandContext ctx)
        {
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                Bot.json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var result = JObject.Parse(Bot.json);
            var discordEmojis = result["discordEmojis"].Children();
            var discordRoles = result["discordRoles"].Children();

            var emojis = JArray.FromObject(discordEmojis);
            var roles = JArray.FromObject(discordRoles);

            Bot.emojiNames.Clear();
            Bot.roleIDs.Clear();

            foreach (var result1 in emojis)
            {
                foreach (JArray JEmojis in result1["emojis"])
                {
                    for (int i = 0; i < JEmojis.Count; i++)
                    {
                        string name = JEmojis[i]["name"].ToString();

                        Bot.emojiNames.Add(name);
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

                        Bot.roleIDs.Add(id);
                    }
                }
            }

            for (int i = 0; i < Bot.emojiNames.Count; i++)
            {
                Bot.roleEmojis.Add(DiscordEmoji.FromName(Bot.Client, Bot.emojiNames[i]));
                Bot.discordRoles.Add(ctx.Guild.GetRole(Bot.roleIDs[i]));
            }
        }

        public static DiscordMember DiscordUser()
        {
            return reactedUser;
        }

        public struct AssignerEmojis
        {
            [JsonProperty("emojis")]
            public List<List<EmojiProperties>> Emojis { get; set; }
        }

        public struct EmojiProperties
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public struct AssignerRoles
        {
            [JsonProperty("roles")]
            public List<List<RoleProperties>> Roles { get; set; }
        }

        public struct RoleProperties
        {
            [JsonProperty("id")]
            public ulong ID { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        }

        public struct CommandValues
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Emoji { get; set; }
            public string Role { get; set; }
        }

    }
}
