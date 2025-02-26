﻿using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Mini.Engine.DirectX;
using Mini.Engine.DirectX.Resources;
using Vortice.Direct3D11;
using Vortice.DXGI;

namespace Mini.Engine.Content.Textures;

internal sealed record TextureData(ContentId Id, int Width, int Height, int Pitch, Format Format, byte[] Data)
    : IContentData;

internal sealed class Texture2DContent : ITexture2D, IContent
{
    private readonly IContentDataLoader<TextureData> Loader;
    private readonly ILoaderSettings Settings;
    private ITexture2D texture;

    public Texture2DContent(ContentId id, Device device, IContentDataLoader<TextureData> loader, ILoaderSettings settings)
    {
        this.Id = id;
        this.Loader = loader;
        this.Settings = settings;
        this.Reload(device);        
    }

    public ContentId Id { get; }
    public int Width => this.texture.Width;
    public int Height => this.texture.Height;
    public Format Format => this.texture.Format;

    ID3D11ShaderResourceView ITexture2D.ShaderResourceView => this.texture.ShaderResourceView;
    ID3D11Texture2D ITexture2D.Texture => this.texture.Texture;

    [MemberNotNull(nameof(texture))]
    public void Reload(Device device)
    {
        this.texture?.Dispose();

        var data = this.Loader.Load(device, this.Id, this.Settings);
        this.texture = new Texture2D(device, data.Data, data.Width, data.Height, data.Format, true, data.Id.ToString());
    }

    public void Dispose()
    {
        this.texture.Dispose();
    }

    public override string ToString()
    {
        return $"Texture2D: {this.Id}";
    }
}
