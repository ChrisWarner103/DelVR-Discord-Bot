using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace DelVRBot
{
    // help formatters can alter the look of default help command,
    // this particular one replaces the embed with a simple text message.
    public class CustomHelpFormatter : BaseHelpFormatter
    {
        private StringBuilder MessageBuilder { get; }

        public CustomHelpFormatter(CommandContext ctx) : base(ctx)
        {
            this.MessageBuilder = new StringBuilder();
        }

        // this method is called first, it sets the command
        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.MessageBuilder.Append("Command: ")
               .AppendLine(Formatter.Bold(command.Name))
               .AppendLine();


            this.MessageBuilder.Append("Description: ")
                .AppendLine(command.Description)
                .AppendLine();

            if (command is CommandGroup)
                this.MessageBuilder.AppendLine("This group has a standalone command.").AppendLine();

            this.MessageBuilder.Append("Aliases: ")
                .AppendLine(string.Join(", ", command.Aliases))
                .AppendLine();

            return this;
        }

        // this method is called second, it sets the current group's subcommands
        // if no group is being processed or current command is not a group, it 
        // won't be called
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            this.MessageBuilder.Append("Subcommands: ")
                .AppendLine(string.Join(", ", subcommands.Select(xc => xc.Name)) + subcommands.Select(xc => xc.Description))
                .AppendLine();

            return this;
        }

        // this is called as the last method, this should produce the final 
        // message, and return it
        public override CommandHelpMessage Build()
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Commands",
                Color = DiscordColor.Orange,
                Description = "These are all the current commands \n" + this.MessageBuilder.ToString(),
            };
            return new CommandHelpMessage(embed: joinEmbed);
        }
    }
}
