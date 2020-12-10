using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelVRBot.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CustomGroupAttribute : Attribute
    {
        public string GroupName { get; }
        public CustomGroupAttribute(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentNullException(nameof(groupName), "Group names cannot be null, empty, or all-whitespace.");

            if (groupName.Any(xc => char.IsWhiteSpace(xc)))
                throw new ArgumentException("Group names cannot contain whitespace characters.", nameof(groupName));

            GroupName = groupName;
        }

        //public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        //{
        //    if (ctx.Guild == null | ctx.Member == null)
        //        return Task.FromResult(string.Empty);

        //    return Task.FromResult(GroupName);
        //}
    }
}
