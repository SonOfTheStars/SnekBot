using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SnekBot.Data;

namespace SnekBot.Modules.Protection
{
    public class ProtectionHandler
    {
        private const string CommandPrefix = "-p";
        private const string LinkProtectionRegex = @"(?:(?:https?|ftp):\/\/)?[\w/\-?=%.]+\.[\w/\-?=%.]+";
        private CommandsNextModule protectionCommands;

        public ProtectionHandler(DiscordClient discord)
        {
            protectionCommands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = CommandPrefix,
                CaseSensitive = false
            });

            protectionCommands.RegisterCommands<ProtectionCommands>();

            discord.MessageCreated += OnMessageCreated;
            discord.GuildMemberAdded += OnUserJoin;
        }

        /**
         * Protect server against accounts that are too newly made.
         */
        public async Task OnUserJoin(GuildMemberAddEventArgs e)
        {
            var createDate = e.Member.CreationTimestamp.DateTime;
            var now = DateTime.Now;
            if(createDate <= now && createDate > now.AddHours(-24))
            {
                MuteMember(e.Member, e.Guild,$"Account made too early!\n{createDate}");
            }
        }

        /**
         * Protect Protected channels according to their protection Flags
         */
        public async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if(DataService.Data.ProtectedChannels.Any(c => c.ChannelId == e.Channel.Id))
            {
                var flags = DataService.GetChannelProtectionFlags(e.Channel.Id);

                if(flags.Contains(ProtectionFlags.NoLink))
                {
                    if(Regex.Matches(e.Message.Content, LinkProtectionRegex).Any())
                        MuteMember(e.Guild.Members.First(m => m.Id == e.Author.Id), e.Guild,$"User {e.Author.Username} posted a link/links in a NoLink protected Channel!");
                }
                if(flags.Contains(ProtectionFlags.LinkBL))
                {
                    var links = Regex.Matches(e.Message.Content, LinkProtectionRegex);
                    if(links.Any())
                    {
                        if(DataService.Data.LinkBlacklist.Any(l => e.Message.Content.Contains(l, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            BanMember(e.Guild.Members.First(m => m.Id == e.Author.Id), e.Guild,$"User {e.Author.Username} posted a blacklisted link/links in a  Channel with url blacklisting!");
                            return;
                        }
                    }
                }
                if(flags.Contains(ProtectionFlags.NoLink))
                {
                    if(DataService.Data.WordBlacklist.Any(w => e.Message.Content.Contains(w, StringComparison.InvariantCultureIgnoreCase)))
                        MuteMember(e.Guild.Members.First(m => m.Id == e.Author.Id), e.Guild,$"User {e.Author.Username} posted a message with a blacklisted word in a Channel with word blacklisting!");
                }
            }
        }

        private async void MuteMember(DiscordMember member, DiscordGuild guild, string reason)
        {
            Role muted = DataService.GetMutedRole();
            if(muted != null)
                await member.GrantRoleAsync(guild.Roles.First(r => r.Id == DataService.GetMutedRole().RoleId), reason).ConfigureAwait(false);
        }

        private async void BanMember(DiscordMember member, DiscordGuild guild, string reason)
        {
            await guild.BanMemberAsync(member, 5, reason).ConfigureAwait(false);
        }
    }
}
