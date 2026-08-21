using Godot;
using MTG.Core.Cards;
using MTG.Core.Enums;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MTG.Frontend;

public static class CardImageLoader
{
    private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
    private static readonly ConcurrentDictionary<string, Texture2D> _textureCache = new ConcurrentDictionary<string, Texture2D>();

    public static async Task<Texture2D?> LoadCardTextureAsync(ICard card)
    {
        if (card == null || card.ImageUris == null) return null;

        // Try Normal image size first, then Small, Png, or ArtCrop
        Uri? targetUri = null;
        if (card.ImageUris.TryGetValue(ImageSize.Normal, out var uriNormal)) targetUri = uriNormal;
        else if (card.ImageUris.TryGetValue(ImageSize.Small, out var uriSmall)) targetUri = uriSmall;
        else if (card.ImageUris.TryGetValue(ImageSize.Png, out var uriPng)) targetUri = uriPng;
        else if (card.ImageUris.TryGetValue(ImageSize.ArtCrop, out var uriArt)) targetUri = uriArt;

        if (targetUri == null) return null;

        string url = targetUri.ToString();
        if (_textureCache.TryGetValue(url, out var cachedTexture))
        {
            return cachedTexture;
        }

        try
        {
            byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);
            if (imageBytes == null || imageBytes.Length == 0) return null;

            var image = new Godot.Image();
            Error error = image.LoadJpgFromBuffer(imageBytes);
            if (error != Error.Ok)
            {
                error = image.LoadPngFromBuffer(imageBytes);
            }

            if (error == Error.Ok)
            {
                var texture = ImageTexture.CreateFromImage(image);
                _textureCache[url] = texture;
                return texture;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to download card texture from {url}: {ex.Message}");
        }

        return null;
    }
}
