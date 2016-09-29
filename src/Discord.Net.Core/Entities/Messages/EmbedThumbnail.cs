﻿using Model = Discord.API.EmbedThumbnail;

namespace Discord
{
    public struct EmbedThumbnail
    {
        public string Url { get; }
        public string ProxyUrl { get; }
        public int? Height { get; }
        public int? Width { get; }

        private EmbedThumbnail(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }
        internal static EmbedThumbnail Create(Model model)
        {
            return new EmbedThumbnail(model.Url, model.ProxyUrl,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
    }
}