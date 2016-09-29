﻿using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MessageModel = Discord.API.Message;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketTextChannel : SocketGuildChannel, ITextChannel
    {
        private readonly MessageCache _messages;

        public string Topic { get; private set; }

        public string Mention => MentionUtils.MentionChannel(Id);

        internal SocketTextChannel(DiscordSocketClient discord, ulong id, ulong guildId)
            : base(discord, id, guildId)
        {
            if (Discord.MessageCacheSize > 0)
                _messages = new MessageCache(Discord, this);
        }
        internal new static SocketTextChannel Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketTextChannel(discord, model.Id, model.GuildId.Value);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Topic = model.Topic.Value;
        }

        public Task ModifyAsync(Action<ModifyTextChannelParams> func)
            => ChannelHelper.ModifyAsync(this, Discord, func);

        public Task<RestGuildUser> GetUserAsync(ulong id)
            => ChannelHelper.GetUserAsync(this, Discord, id);
        public IAsyncEnumerable<IReadOnlyCollection<RestGuildUser>> GetUsersAsync()
            => ChannelHelper.GetUsersAsync(this, Discord);

        public Task<RestMessage> GetMessageAsync(ulong id)
            => ChannelHelper.GetMessageAsync(this, Discord, id);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, limit: limit);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessageId, dir, limit);
        public IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch)
            => ChannelHelper.GetMessagesAsync(this, Discord, fromMessage.Id, dir, limit);
        public Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync()
            => ChannelHelper.GetPinnedMessagesAsync(this, Discord);

        public Task<RestUserMessage> SendMessageAsync(string text, bool isTTS)
            => ChannelHelper.SendMessageAsync(this, Discord, text, isTTS);
        public Task<RestUserMessage> SendFileAsync(string filePath, string text, bool isTTS)
            => ChannelHelper.SendFileAsync(this, Discord, filePath, text, isTTS);
        public Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text, bool isTTS)
            => ChannelHelper.SendFileAsync(this, Discord, stream, filename, text, isTTS);

        public Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => ChannelHelper.DeleteMessagesAsync(this, Discord, messages);

        public IDisposable EnterTypingState()
            => ChannelHelper.EnterTypingState(this, Discord);

        internal SocketMessage AddMessage(SocketUser author, MessageModel model)
        {
            var msg = SocketMessage.Create(Discord, author, model);
            _messages.Add(msg);
            return msg;
        }
        internal SocketMessage RemoveMessage(ulong id)
        {
            return _messages.Remove(id);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"@{Name} ({Id}, Text)";

        //IGuildChannel
        async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id)
            => await GetUserAsync(id);
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync()
            => GetUsersAsync();

        //IMessageChannel
        IReadOnlyCollection<IMessage> IMessageChannel.CachedMessages => ImmutableArray.Create<IMessage>();
        IMessage IMessageChannel.GetCachedMessage(ulong id) => null;

        async Task<IMessage> IMessageChannel.GetMessageAsync(ulong id)
            => await GetMessageAsync(id);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(int limit)
            => GetMessagesAsync(limit);
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> IMessageChannel.GetMessagesAsync(ulong fromMessageId, Direction dir, int limit)
            => GetMessagesAsync(fromMessageId, dir, limit);
        async Task<IReadOnlyCollection<IMessage>> IMessageChannel.GetPinnedMessagesAsync()
            => await GetPinnedMessagesAsync().ConfigureAwait(false);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(string filePath, string text, bool isTTS)
            => await SendFileAsync(filePath, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendFileAsync(Stream stream, string filename, string text, bool isTTS)
            => await SendFileAsync(stream, filename, text, isTTS);
        async Task<IUserMessage> IMessageChannel.SendMessageAsync(string text, bool isTTS)
            => await SendMessageAsync(text, isTTS);
        IDisposable IMessageChannel.EnterTypingState()
            => EnterTypingState();
    }
}