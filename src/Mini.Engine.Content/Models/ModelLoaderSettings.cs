﻿using Mini.Engine.Content.Materials;

namespace Mini.Engine.Content.Models;

internal sealed record ModelLoaderSettings(MaterialLoaderSettings MaterialSettings) : ILoaderSettings
{
    public static ModelLoaderSettings Default = new(MaterialLoaderSettings.Default);
}
