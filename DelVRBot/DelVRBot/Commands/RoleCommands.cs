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
        static DiscordMember reactedUser;

        List<EmojiProperties> emojis;
        List<RoleProperties> roles;

        /// Once you make the edit function you won't need the role reacting part.

        [Command("CreateRoleAssigner")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Alchemist")]
        public async Task RollAssigner(CommandContext ctx)
        {

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "React using the emoji to get a specific role",
                Color = DiscordColor.Orange,
            };

            var reactionMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            //Get config arrays.
            var configJson = JObject.Parse(Bot.json);

            ulong channelID = ctx.Channel.Id;
            ulong reactionMessageID = reactionMessage.Id;

            configJson["reactionRolesChannel"] = channelID;
            configJson["reactionRolesMesage"] = reactionMessageID;

            string output = JsonConvert.SerializeObject(configJson, Formatting.Indented);
            File.WriteAllText("config.json", output);

            await ctx.Channel.DeleteMessageAsync(ctx.Message).ConfigureAwait(false);
        }

        [Command("ReactEdit")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Alchemist")]
        public async Task RollEdit(CommandContext ctx, [Description("Message ID")] ulong messageID, [Description("Role ID")] ulong roleID, [Description("Assotiated Emoji")] string emojiID, [Description("Info Text")] [RemainingText] string reactMessage)
        {
            //Variables
            string modifiedEmojiID = emojiID;

            //Get the message and the embed that is attached to it.
            DiscordMessage message = await ctx.Channel.GetMessageAsync(messageID);
            DiscordEmbed embed = message.Embeds[0];
            DiscordEmoji emoji;

            //Pharsing and creating the emoji
            if (emojiID.Contains(":"))
            {
                int firstIndex = emojiID.IndexOf(':');
                int lastIndex = emojiID.LastIndexOf(':');
                modifiedEmojiID = emojiID.Substring(firstIndex, lastIndex);

                emoji = DiscordEmoji.FromName(ctx.Client, modifiedEmojiID);
            }
            else
            {
                emoji = DiscordEmoji.FromUnicode(ctx.Client, modifiedEmojiID);
            }


            string currentDescription = embed.Description;
            currentDescription += "\nReact with " + emojiID + " " + reactMessage;

            DiscordEmbed newEmbed = new DiscordEmbedBuilder { Title = embed.Title, Color = DiscordColor.Orange, Description = currentDescription };

            await message.ModifyAsync(embed: newEmbed).ConfigureAwait(false);
            await message.CreateReactionAsync(emoji).ConfigureAwait(false);

            var configJson = JObject.Parse(Bot.json);
            var emojisArray = (JArray)configJson["discordEmojis"];
            var roleArray = (JArray)configJson["discordRoles"];

            //Adding new emoji to Json
            JObject newEmoji = new JObject();
            newEmoji["name"] = modifiedEmojiID;
            emojisArray.Add(newEmoji);

            //Adding new role to Json
            JObject newRole = new JObject();
            newRole["id"] = roleID;
            newRole["name"] = ctx.Guild.GetRole(roleID).Name;
            roleArray.Add(newRole);

            await ctx.Channel.DeleteMessageAsync(ctx.Message).ConfigureAwait(false);

            string output = JsonConvert.SerializeObject(configJson, Formatting.Indented);
            File.WriteAllText("config.json", output);

            Bot.LoadReactionEmojisFromConfig();
            Bot.Client.MessageReactionRemoved += Client_MessageReactionRemoved;

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
                    ;
                    await discordMemeber.RevokeRoleAsync(e.Guild.GetRole(roles[i].ID)).ConfigureAwait(false);
                    return;
                }
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
