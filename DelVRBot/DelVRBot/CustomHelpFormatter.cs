using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DelVRBot.Attributes;
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

        private string EmbedTitle;
        private string helpDescription;

        public CustomHelpFormatter(CommandContext ctx) : base(ctx)
        {
            this.MessageBuilder = new StringBuilder();
        }

        // this method is called first, it sets the command
        public override BaseHelpFormatter WithCommand(Command command)
        {

            if (command.Name.ToLower() == "roll" || command.Name.ToLower() == "rollt")
            {
                EmbedTitle = Formatter.Bold("![roll|r]");
                helpDescription = File.ReadAllText("DiceHelpDescription.txt");
                //EmbedImage.Url = "../Images/D20_20_Green.png";
            }


            this.MessageBuilder.Append(helpDescription)
                .AppendLine();

            return this;
        }

        // this method is called second, it sets the current group's subcommands
        // if no group is being processed or current command is not a group, it 
        // won't be called
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            //subcommands = subcommands.Where(c => c.Name != "help").ToList();

            //var list = subcommands.Select(g => g.CustomAttributes.ty).ToList();

            //for (int i = 0; i < list.Count; i++)
            //{
            //    list[i].
            //}

            //var yes = subcommands.Select(g => g.CustomAttributes.Where(t => t.TypeId)).ToList();

            //var test = yes.Select(n => n).ToList();

            EmbedTitle = string.Empty;
            //EmbedImage.Url = "../Images/DelVR Logo.png";
            helpDescription = File.ReadAllText("HelpDescription.txt");


            this.MessageBuilder.Append(helpDescription)
                .AppendLine();

            return this;
        }

        public BaseHelpFormatter Groups(IEnumerable<CustomGroupAttribute> groups)
        {
            this.MessageBuilder.Append(groups.Select(xc => xc.GroupName));
            return this;
        }

        // this is called as the last method, this should produce the final 
        // message, and return it
        public override CommandHelpMessage Build()
        {
            var helpEmbed = new DiscordEmbedBuilder
            {
                Title = EmbedTitle,
                //Thumbnail = EmbedImage,
                Color = DiscordColor.Orange,
                Description = this.MessageBuilder.ToString(),
            };
            return new CommandHelpMessage(embed: helpEmbed);
        }
    }
}
