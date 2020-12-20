using DelVRBot.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DelVRBot.Commands
{
    //[RequireCatagories(ChannelCheckMode.Any, "Playing Area")]
    [Description("These commands are used to roll dice")]
    public class DiceCommands : BaseCommandModule
    {

        [CustomGroup("Dice")]
        [Command("roll"), Aliases("r")]
        [Description("Roll is used to roll any combination of dice in the `XdY` format. (1d6, 2d8, etc)\nIn emote format.")]
        public async Task RollDice(CommandContext ctx, [RemainingText] string command)
        {
            int diceAmount = 0;
            int diceType = 0;
            string diceString = string.Empty;
            int diceRoll = 0;
            List<int> diceRolls = new List<int>();
            List<string> diceRollEmotes = new List<string>();
            float diceRolledTotal = 0;
            var random = new Random();
            string eachRoll = string.Empty;
            string modifierString = string.Empty;
            int modifier = 0;

            char[] commandArray;
            char dice = 'd';
            char plusModifier = '+';
            char minusModifier = '-';
            char divideModifier = '/';
            char multiplyModifier = '*';
            string adv = "adv";
            string disadv = "dis";

            if (command == null)
            {
                command = "1d20";
            }

            command = command.ToLower();

            commandArray = command.ToCharArray();

            int index = Array.IndexOf(commandArray, dice);

            int plusModIndex = Array.IndexOf(commandArray, plusModifier);
            int minusModIndex = Array.IndexOf(commandArray, minusModifier);
            int divideModIndex = Array.IndexOf(commandArray, divideModifier);
            int multiplyModIndex = Array.IndexOf(commandArray, multiplyModifier);
            bool advRoll = command.Contains(adv);
            bool disadvRoll = command.Contains(disadv);

            bool regularDiceRoll = true;
            if (disadvRoll || advRoll)
            {
                command = command.Replace("dis", "");
                command = command.Replace("adv", "");
                commandArray = command.ToCharArray();

                if (command == string.Empty)
                {
                    diceType = 20;
                    diceAmount = 2;
                    regularDiceRoll = false;
                }
            }

            if (plusModIndex != -1)
            {
                for (int i = plusModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(plusModIndex, 1);
                }
                modifier = Convert.ToInt32(new string(modifierString));
            }
            else if (minusModIndex != -1)
            {
                for (int i = minusModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(minusModIndex, 1);
                }
                modifier = Convert.ToInt32(new string(modifierString));
            }
            else if (divideModIndex != -1)
            {
                for (int i = divideModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(divideModIndex, 1);
                }
                modifierString = modifierString.Remove(0, 1);
                modifier = Convert.ToInt32(new string(modifierString));
            }
            else if (multiplyModIndex != -1)
            {
                for (int i = multiplyModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(multiplyModIndex, 1);
                }
                modifierString = modifierString.Remove(0, 1);
                modifier = Convert.ToInt32(new string(modifierString));
            }


            if (regularDiceRoll)
            {
                if (index != -1)
                {
                    for (int i = 0; i < index; i++)
                    {
                        diceString += string.Join("", commandArray[i]);
                        command = command.Remove(0, 1);
                    }

                    diceAmount += Convert.ToInt32(new string(diceString));

                    command = command.Remove(0, 1);
                }

                diceType = Convert.ToInt32(command);
            }

            if (diceType == 0)
            {
                await ctx.Channel.SendMessageAsync("Dice type entered isn't a dice").ConfigureAwait(false);
            }
            else if (diceType == 4)
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);

                    switch (diceRoll)
                    {
                        case 1:
                            diceRollEmotes.Add("<:D4_1:782725547347673088>");
                            break;
                        case 2:
                            diceRollEmotes.Add("<:D4_2:782725552443490345>");
                            break;
                        case 3:
                            diceRollEmotes.Add("<:D4_3:782725557573124116>");
                            break;
                        case 4:
                            diceRollEmotes.Add("<:D4_4:782725563260338197> ");
                            break;
                    }

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }
            else if (diceType == 6)
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);

                    switch (diceRoll)
                    {
                        case 1:
                            diceRollEmotes.Add("<:D6_1:782725507488940133>");
                            break;
                        case 2:
                            diceRollEmotes.Add("<:D6_2:782725513432530975>");
                            break;
                        case 3:
                            diceRollEmotes.Add("<:D6_3:782725518851571742>");
                            break;
                        case 4:
                            diceRollEmotes.Add("<:D6_4:782725525038825472>");
                            break;
                        case 5:
                            diceRollEmotes.Add("<:D6_5:782725531027111936>");
                            break;
                        case 6:
                            diceRollEmotes.Add("<:D6_6:782725535411208233>");
                            break;
                    }

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }
            else if (diceType == 8)
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);


                    switch (diceRoll)
                    {
                        case 1:
                            diceRollEmotes.Add("<:D8_1:782743521882144768>");
                            break;
                        case 2:
                            diceRollEmotes.Add("<:D8_2:782743531676368936>");
                            break;
                        case 3:
                            diceRollEmotes.Add("<:D8_3:782743539896549430>");
                            break;
                        case 4:
                            diceRollEmotes.Add("<:D8_4:782743544490098691>");
                            break;
                        case 5:
                            diceRollEmotes.Add("<:D8_5:782743550105485372>");
                            break;
                        case 6:
                            diceRollEmotes.Add("<:D8_6:782743555255828490>");
                            break;
                        case 7:
                            diceRollEmotes.Add("<:D8_7:782743561728426016>");
                            break;
                        case 8:
                            diceRollEmotes.Add("<:D8_8:782743567394406440>");
                            break;
                    }

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }
            else if (diceType == 10)
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);

                    switch (diceRoll)
                    {
                        case 1:
                            diceRollEmotes.Add("<:D10_1:782743451983806487>");
                            break;
                        case 2:
                            diceRollEmotes.Add("<:D10_2:782743459412049921> ");
                            break;
                        case 3:
                            diceRollEmotes.Add("<:D10_3:782743464852455434>");
                            break;
                        case 4:
                            diceRollEmotes.Add("<:D10_4:782743469847609414>");
                            break;
                        case 5:
                            diceRollEmotes.Add("<:D10_5:782743474574983199>");
                            break;
                        case 6:
                            diceRollEmotes.Add("<:D10_6:782743482572734484>");
                            break;
                        case 7:
                            diceRollEmotes.Add("<:D10_7:782743488826703882>");
                            break;
                        case 8:
                            diceRollEmotes.Add("<:D10_8:782743495995686912>");
                            break;
                        case 9:
                            diceRollEmotes.Add("<:D10_9:782743501653147669>");
                            break;
                        case 10:
                            diceRollEmotes.Add("<:D10_10:782743508359315486>");
                            break;
                    }

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }
            else if (diceType == 12)
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);

                    switch (diceRoll)
                    {
                        case 1:
                            diceRollEmotes.Add("<:D12_1:782743377531633664>");
                            break;
                        case 2:
                            diceRollEmotes.Add("<:D12_2:782743382912925727>");
                            break;
                        case 3:
                            diceRollEmotes.Add("<:D12_3:782743388176252939>");
                            break;
                        case 4:
                            diceRollEmotes.Add("<:D12_4:782743393963868201>");
                            break;
                        case 5:
                            diceRollEmotes.Add("<:D12_5:782743399383957545>");
                            break;
                        case 6:
                            diceRollEmotes.Add("<:D12_6:782743405675282464>");
                            break;
                        case 7:
                            diceRollEmotes.Add("<:D12_7:782743410867437598>");
                            break;
                        case 8:
                            diceRollEmotes.Add("<:D12_8:782743416952848405>");
                            break;
                        case 9:
                            diceRollEmotes.Add("<:D12_9:782743422695505980>");
                            break;
                        case 10:
                            diceRollEmotes.Add("<:D12_10:782743428085448714>");
                            break;
                        case 11:
                            diceRollEmotes.Add("<:D12_11:782743433201975327>");
                            break;
                        case 12:
                            diceRollEmotes.Add("<:D12_12:782743438083751946>");
                            break;
                    }

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }
            else if (diceType == 20)
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);

                    switch (diceRoll)
                    {
                        case 1:
                            diceRollEmotes.Add("<:D20_1_Red:782743236426465290>");
                            break;
                        case 2:
                            diceRollEmotes.Add("<:D20_2:782743253472378911>");
                            break;
                        case 3:
                            diceRollEmotes.Add("<:D20_3:782743260732719115>");
                            break;
                        case 4:
                            diceRollEmotes.Add("<:D20_4:782743267158654996>");
                            break;
                        case 5:
                            diceRollEmotes.Add("<:D20_5:782743273268838400>");
                            break;
                        case 6:
                            diceRollEmotes.Add("<:D20_6:782743279162621952>");
                            break;
                        case 7:
                            diceRollEmotes.Add("<:D20_7:782743284027621387>");
                            break;
                        case 8:
                            diceRollEmotes.Add("<:D20_8:782743290415022131>");
                            break;
                        case 9:
                            diceRollEmotes.Add("<:D20_9:782743297206517790>");
                            break;
                        case 10:
                            diceRollEmotes.Add("<:D20_10:782743302800670770>");
                            break;
                        case 11:
                            diceRollEmotes.Add("<:D20_11:782743308353929216>");
                            break;
                        case 12:
                            diceRollEmotes.Add("<:D20_12:782743313777557535>");
                            break;
                        case 13:
                            diceRollEmotes.Add("<:D20_13:782743319116906536>");
                            break;
                        case 14:
                            diceRollEmotes.Add("<:D20_14:782743324352053258>");
                            break;
                        case 15:
                            diceRollEmotes.Add("<:D20_15:782743330282143794>");
                            break;
                        case 16:
                            diceRollEmotes.Add("<:D20_16:782743336452227094>");
                            break;
                        case 17:
                            diceRollEmotes.Add("<:D20_17:782743343364309042>");
                            break;
                        case 18:
                            diceRollEmotes.Add("<:D20_18:782743347680247830>");
                            break;
                        case 19:
                            diceRollEmotes.Add("<:D20_19:782743356471115779>");
                            break;
                        case 20:
                            diceRollEmotes.Add("<:D20_20_Green:782743364877287444>");
                            break;
                    }

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }
            else
            {
                for (int i = 0; i < diceAmount; i++)
                {
                    diceRoll = random.Next(1, diceType + 1);

                    diceRolls.Add(diceRoll);
                    diceRollEmotes.Add(diceRoll.ToString());

                    if (i == diceAmount - 1)
                    {
                        if (divideModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal / modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else if (multiplyModIndex != -1)
                        {
                            diceRolledTotal += diceRoll;
                            diceRolledTotal = diceRolledTotal * modifier;
                            diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                        }
                        else
                        {
                            diceRolledTotal += diceRoll + modifier;
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                    }
                    else
                    {
                        if (disadvRoll || advRoll)
                        {
                            diceRolls[i] += modifier;
                            if (diceRolls[i] <= 0)
                                diceRolls[i] = 1;
                        }
                        else
                            diceRolledTotal += diceRoll;
                    }
                }
            }


            eachRoll = string.Join(" ", diceRollEmotes.ToArray());

            if (eachRoll.Length >= 2000)
            {
                await ctx.Channel.SendMessageAsync("Looks like you've tried to roll a large amount of dice!\n" +
                                                   "Use " + ctx.Prefix + "rt to roll that many dice");

            }
            else
            {
                if (modifier == 0 && !disadvRoll && !advRoll)
                {
                    await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                    await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                        "\n" + "**Total** : " + diceRolledTotal.ToString()).ConfigureAwait(true);
                }
                else if (disadvRoll)
                {
                    await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                    int rollTotal = diceRolls.Min();
                    await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() + modifierString + "dis" +
                                                        "\n" + "**Total** : " + rollTotal.ToString()).ConfigureAwait(true);
                }
                else if (advRoll)
                {
                    await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                    int rollTotal = diceRolls.Max();
                    await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() + modifierString + "adv" +
                                                        "\n" + "**Total** : " + rollTotal.ToString()).ConfigureAwait(true);
                }
                else if (divideModIndex != -1)
                {
                    await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                    int rollTotal = diceRolls.GetRange(0, diceAmount).Sum();
                    await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                        "\n" + "**Total** : " + rollTotal.ToString() + "/" + modifierString.ToString() + " = " + diceRolledTotal.ToString()).ConfigureAwait(true);
                }
                else if (multiplyModIndex != -1)
                {
                    await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                    int rollTotal = diceRolls.GetRange(0, diceAmount).Sum();
                    await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                        "\n" + "**Total** : " + rollTotal.ToString() + "*" + modifierString.ToString() + " = " + diceRolledTotal.ToString()).ConfigureAwait(true);
                }
                else
                {
                    await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                    int rollTotal = diceRolls.GetRange(0, diceAmount).Sum();
                    await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                        "\n" + "**Total** : " + (rollTotal - modifier).ToString() + modifierString.ToString() + " = " + diceRolledTotal.ToString()).ConfigureAwait(true);
                }
            }
        }

        [Command("rollt"), Aliases("rt")]
        [Description("Rollt is used to roll any combination of dice in the `XdY` format. (1d6, 2d8, etc)\nIn text format.")]
        public async Task RollDiceText(CommandContext ctx, [RemainingText] string command)
        {
            int diceAmount = 0;
            int diceType = 0;
            string diceString = string.Empty;
            int diceRoll = 0;
            List<int> diceRolls = new List<int>();
            List<string> diceRollEmotes = new List<string>();
            float diceRolledTotal = 0;
            var random = new Random();
            string eachRoll = string.Empty;
            string modifierString = string.Empty;
            int modifier = 0;

            char[] commandArray;
            char dice = 'd';
            char plusModifier = '+';
            char minusModifier = '-';
            char divideModifier = '/';
            char multiplyModifier = '*';
            string adv = "adv";
            string disadv = "dis";

            if (command == null)
            {
                command = "1d20";
            }

            command = command.ToLower();

            commandArray = command.ToCharArray();

            int index = Array.IndexOf(commandArray, dice);

            int plusModIndex = Array.IndexOf(commandArray, plusModifier);
            int minusModIndex = Array.IndexOf(commandArray, minusModifier);
            int divideModIndex = Array.IndexOf(commandArray, divideModifier);
            int multiplyModIndex = Array.IndexOf(commandArray, multiplyModifier);
            bool advRoll = command.Contains(adv);
            bool disadvRoll = command.Contains(disadv);

            bool regularDiceRoll = true;
            if (disadvRoll || advRoll)
            {
                command = command.Replace("dis", "");
                command = command.Replace("adv", "");
                commandArray = command.ToCharArray();

                if (command == string.Empty)
                {
                    diceType = 20;
                    diceAmount = 2;
                    regularDiceRoll = false;
                }
            }

            if (plusModIndex != -1)
            {
                for (int i = plusModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(plusModIndex, 1);
                }
                modifier = Convert.ToInt32(new string(modifierString));
            }
            else if (minusModIndex != -1)
            {
                for (int i = minusModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(minusModIndex, 1);
                }
                modifier = Convert.ToInt32(new string(modifierString));
            }
            else if (divideModIndex != -1)
            {
                for (int i = divideModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(divideModIndex, 1);
                }
                modifierString = modifierString.Remove(0, 1);
                modifier = Convert.ToInt32(new string(modifierString));
            }
            else if (multiplyModIndex != -1)
            {
                for (int i = multiplyModIndex; i < commandArray.Length; i++)
                {
                    modifierString += string.Join("", commandArray[i]);
                    command = command.Remove(multiplyModIndex, 1);
                }
                modifierString = modifierString.Remove(0, 1);
                modifier = Convert.ToInt32(new string(modifierString));
            }


            if (regularDiceRoll)
            {
                if (index != -1)
                {
                    for (int i = 0; i < index; i++)
                    {
                        diceString += string.Join("", commandArray[i]);
                        command = command.Remove(0, 1);
                    }

                    diceAmount += Convert.ToInt32(new string(diceString));

                    command = command.Remove(0, 1);
                }
            }

            diceType = Convert.ToInt32(command);

            if (diceType == 0)
                await ctx.Channel.SendMessageAsync("Dice type entered isn't a dice").ConfigureAwait(false);

            for (int i = 0; i < diceAmount; i++)
            {
                diceRoll = random.Next(1, diceType + 1);

                diceRolls.Add(diceRoll);
                diceRollEmotes.Add(diceRoll.ToString());

                if (i == diceAmount - 1)
                {
                    if (divideModIndex != -1)
                    {
                        diceRolledTotal += diceRoll;
                        diceRolledTotal = diceRolledTotal / modifier;
                        diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                    }
                    else if (multiplyModIndex != -1)
                    {
                        diceRolledTotal += diceRoll;
                        diceRolledTotal = diceRolledTotal * modifier;
                        diceRolledTotal = Convert.ToInt32(Math.Floor(diceRolledTotal));
                    }
                    else
                    {
                        diceRolledTotal += diceRoll + modifier;
                        diceRolls[i] += modifier;
                        if (diceRolls[i] <= 0)
                            diceRolls[i] = 1;
                    }
                }
                else
                {
                    if (disadvRoll || advRoll)
                    {
                        diceRolls[i] += modifier;
                        if (diceRolls[i] <= 0)
                            diceRolls[i] = 1;
                    }
                    else
                        diceRolledTotal += diceRoll;
                }

            }


            eachRoll = string.Join(" ", diceRolls.ToArray());

            if (modifier == 0 && !disadvRoll && !advRoll)
            {
                await ctx.Channel.SendMessageAsync(eachRoll).ConfigureAwait(false);
                await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                    "\n" + "**Total** : " + diceRolledTotal.ToString()).ConfigureAwait(true);
            }
            else if (disadvRoll)
            {
                int rollTotal = diceRolls.Min();
                await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() + modifierString + "dis" +
                                                    "\n" + "**Total** : " + rollTotal.ToString()).ConfigureAwait(true);
            }
            else if (advRoll)
            {
                int rollTotal = diceRolls.Max();
                await ctx.Channel.SendMessageAsync("**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() + modifierString + "adv" +
                                                    "\n" + "**Total** : " + rollTotal.ToString()).ConfigureAwait(true);
            }
            else if (divideModIndex != -1)
            {
                await ctx.Channel.SendMessageAsync("(" + eachRoll + ")" + $"/{modifierString}" + "\n**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                   "\n" + "**Total** : " + diceRolledTotal.ToString()).ConfigureAwait(true);
            }
            else if (multiplyModIndex != -1)
            {
                await ctx.Channel.SendMessageAsync("(" + eachRoll + ")" + $"*{modifierString}" + "\n**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                    "\n" + "**Total** : " + diceRolledTotal.ToString()).ConfigureAwait(true);
            }
            else
            {
                await ctx.Channel.SendMessageAsync("(" + eachRoll + ")" + $"{modifierString}" + "\n**Result**: " + diceAmount.ToString() + "D" + diceType.ToString() +
                                                    "\n" + "**Total** : " + diceRolledTotal.ToString()).ConfigureAwait(true);


            }


        }
    }
}