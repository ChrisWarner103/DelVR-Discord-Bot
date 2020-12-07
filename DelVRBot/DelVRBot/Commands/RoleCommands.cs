using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DelVRBot.Commands
{
    class RoleCommands : BaseCommandModule
    {
        private InteractivityResult<MessageReactionAddEventArgs> reactionResults;

        static DiscordMember reactedUser;

        /// Once you make the edit function you won't need the role reacting part.

        [Command("CreateRollAssigner")]
        [Hidden]
        [RequireRoles(RoleCheckMode.Any, "Admin", "Alchemist")]
        public async Task Join(CommandContext ctx)
        {

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "React using the emoji to get a specific role",
                Color = DiscordColor.Orange,
                Description = "React with <:ChrisThink:706721730281865298> to become a Dungeon Master\n" +
                              "React with <a:ChrisGag:701176932171251767> to become a Valorant player",
            };

            var reactionMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            var dungeonMasterEmoji = DiscordEmoji.FromName(ctx.Client, ":ChrisThink:");
            var lookingForGroupEmoji = DiscordEmoji.FromName(ctx.Client, ":ChrisGag:");

            await reactionMessage.CreateReactionAsync(dungeonMasterEmoji).ConfigureAwait(false);
            await reactionMessage.CreateReactionAsync(lookingForGroupEmoji).ConfigureAwait(false);

            while (true)
            {
                var interactivity = ctx.Client.GetInteractivity();

                //var reactionResults = await interactivity.CollectReactionsAsync(reactionMessage).ConfigureAwait(false);

                reactionResults = await interactivity.WaitForReactionAsync(
                    x => x.Message == reactionMessage &&
                    x.Emoji == dungeonMasterEmoji || x.Emoji == lookingForGroupEmoji).ConfigureAwait(false);

                if (reactionResults.Result.Emoji == dungeonMasterEmoji)
                {
                    var role = ctx.Guild.GetRole(782768719796502549);

                    reactedUser = (DiscordMember)reactionResults.Result.User;

                    await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);

                }
                else if (reactionResults.Result.Emoji == lookingForGroupEmoji)
                {
                    var role = ctx.Guild.GetRole(738025540593778700);

                    reactedUser = (DiscordMember)reactionResults.Result.User;

                    await ctx.Member.GrantRoleAsync(role).ConfigureAwait(false);

                }
            }
        }

        public static DiscordMember DiscordUser()
        {
            return reactedUser;
        }

    }
}
