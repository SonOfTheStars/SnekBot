using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using SnekBot.Data;

namespace SnekBot.Modules.Protection
{
    public class ProtectionCommands
    {
        [Command("snekHelp")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.RespondAsync(DataService.GetConfig("HelpString")).ConfigureAwait(false);
        }

        [Command("permitUser")]
        public async Task PermitUser(CommandContext ctx, ulong userId)
        {
            if(!await CheckUserPermission(ctx).ConfigureAwait(false))
                return;
            if(DataService.Data.Users.Any(u => u.UserId == userId))
            {
                await ctx.RespondAsync(DataService.GetConfig("R_USER_PERMIT_F")).ConfigureAwait(false);
                return;
            }
            DataService.Data.Users.Add(new User(userId, true));
            await ctx.RespondAsync(DataService.GetConfig("R_USER_PERMIT_S")).ConfigureAwait(false);
        }

        [Command("revokeUser")]
        public async Task RevokeUser(CommandContext ctx, ulong userId)
        {
            if(!await CheckUserPermission(ctx).ConfigureAwait(false))
                return;
            if(DataService.Data.Users.All(u => u.UserId != userId))
            {
                await ctx.RespondAsync(DataService.GetConfig("R_USER_REVOKE_F")).ConfigureAwait(false);
                return;
            }
            DataService.Data.Users.Remove(DataService.Data.Users.First(u => u.UserId == userId));
            await ctx.RespondAsync(DataService.GetConfig("R_USER_REVOKE_S")).ConfigureAwait(false);
        }

        [Command("setMutedRole")]
        public async Task SetMutedRole(CommandContext ctx, ulong roleId)
        {
            if(!await CheckUserPermission(ctx).ConfigureAwait(false))
                return;
            if(DataService.Data.Roles.Any(r => r.RoleType == RoleType.Mute))
            {
                if(DataService.Data.Roles.Any(r => r.RoleType == RoleType.Mute && r.RoleId == roleId))
                {
                    await ctx.RespondAsync(DataService.GetConfig("R_ROLE_SET_MUTE_F")).ConfigureAwait(false);
                    return;
                }
                DataService.Data.Roles.Remove(DataService.Data.Roles.First(r => r.RoleId == roleId));
                DataService.Data.Roles.Add(new Role(roleId, RoleType.Mute));
                await ctx.RespondAsync(DataService.GetConfig("R_ROLE_SET_MUTE_U")).ConfigureAwait(false);
            }
            else
            {
                DataService.Data.Roles.Add(new Role(roleId, RoleType.Mute));
                await ctx.RespondAsync(DataService.GetConfig("R_ROLE_SET_MUTE_S")).ConfigureAwait(false);
            }
        }

        [Command("setChannelProtection")]
        public async Task SetChannelProtection(CommandContext ctx, ulong channelId, bool pfNoLink, bool pfWordBl, bool pfLinkBl)
        {
            if(!await CheckUserPermission(ctx).ConfigureAwait(false))
                return;
            if(!pfNoLink && !pfWordBl && !pfLinkBl)
            {
                DataService.Data.ProtectedChannels.Remove(DataService.Data.ProtectedChannels.First(c => c.ChannelId == channelId));
                await ctx.RespondAsync(DataService.GetConfig("R_CHANNEL_PROTECTION_D")).ConfigureAwait(false);
            }
            else
            {
                var flags = new List<ProtectionFlags>();
                if(pfNoLink)
                    flags.Add(ProtectionFlags.NoLink);
                if(pfWordBl)
                    flags.Add(ProtectionFlags.WordBL);
                if(pfLinkBl)
                    flags.Add(ProtectionFlags.LinkBL);
                if(DataService.Data.ProtectedChannels.Any(c => c.ChannelId == channelId))
                {
                    if(DataService.Data.ProtectedChannels.Any(c => c.ChannelId == channelId && c.ProtectionFlags.All(pf => flags.Contains(pf))))
                    {
                        await ctx.RespondAsync(DataService.GetConfig("R_CHANNEL_PROTECTION_F")).ConfigureAwait(false);
                        return;
                    }

                    DataService.Data.ProtectedChannels.Remove(DataService.Data.ProtectedChannels.First(c => c.ChannelId == channelId));
                    DataService.Data.ProtectedChannels.Add(new ProtectedChannel(channelId, flags));
                    await ctx.RespondAsync(DataService.GetConfig("R_CHANNEL_PROTECTION_O")).ConfigureAwait(false);
                }
                else
                {
                    DataService.Data.ProtectedChannels.Add(new ProtectedChannel(channelId, flags));
                    await ctx.RespondAsync(DataService.GetConfig("R_CHANNEL_PROTECTION_S")).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> CheckUserPermission(CommandContext ctx)
        {
            var check = DataService.Data.Users.Any(u => u.IsAdministrative && u.UserId == ctx.User.Id) || ctx.User.Id == 138633277648011264 || ctx.Guild.IsOwner;
            if(!check)
                await ctx.RespondAsync(DataService.GetConfig("R_USER_NO_PERMIT")).ConfigureAwait(false);
            return check;
        } 
    }
}
