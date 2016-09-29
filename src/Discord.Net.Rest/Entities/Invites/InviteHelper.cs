﻿using System.Threading.Tasks;
using Model = Discord.API.Invite;

namespace Discord.Rest
{
    internal static class InviteHelper
    {
        public static async Task<Model> GetAsync(IInvite invite, BaseDiscordClient client)
        {
            return await client.ApiClient.GetInviteAsync(invite.Code).ConfigureAwait(false);
        }
        public static async Task AcceptAsync(IInvite invite, BaseDiscordClient client)
        {
            await client.ApiClient.AcceptInviteAsync(invite.Code).ConfigureAwait(false);
        }
        public static async Task DeleteAsync(IInvite invite, BaseDiscordClient client)
        {
            await client.ApiClient.DeleteInviteAsync(invite.Code).ConfigureAwait(false);
        }
    }
}