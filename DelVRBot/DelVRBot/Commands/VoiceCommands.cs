using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DelVRBot.Commands
{
    class VoiceCommands
    {
        [Command("j")]
        public async Task JoinVoice(CommandContext ctx, DiscordChannel chnl = null)
        {
            var vnext = ctx.Client.GetVoiceNext();

            if (vnext == null)
            {
                await ctx.RespondAsync("xNext is not enabled or configured");
                return;
            }

            var vnc = vnext.GetConnection(ctx.Guild);
            if (vnc != null)
            {
                await ctx.RespondAsync("Already connected in this guild");
                return;
            }

            var vstat = ctx.Member?.VoiceState;

            if (vstat?.Channel == null && chnl == null)
            {
                await ctx.RespondAsync("You are not in a voice channel");
                return;
            }

            if (chnl == null)
                chnl = vstat.Channel;

            vnc = await vnext.ConnectAsync(chnl).ConfigureAwait(false);
        }
    }
}
