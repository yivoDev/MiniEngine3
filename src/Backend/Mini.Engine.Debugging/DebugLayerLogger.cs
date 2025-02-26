﻿using Mini.Engine.DirectX;
using Serilog;
using Serilog.Events;
using Vortice.DXGI;
using Vortice.DXGI.Debug;
using Vortice.Direct3D11;
using Vortice.Direct3D11.Debug;
using Mini.Engine.Configuration;
using System;

namespace Mini.Engine.Debugging;

[Service]
public sealed class DebugLayerLogger
{
#if DEBUG
    private readonly ID3D11InfoQueue DebugInfoQueue;
    private readonly ILogger Logger;
#endif
    public DebugLayerLogger(Device device, ILogger logger)
    {
#if DEBUG
        this.DebugInfoQueue = device.ID3D11Debug.QueryInterface<ID3D11InfoQueue>();
        this.DebugInfoQueue.PushEmptyStorageFilter();

        this.DebugInfoQueue.SetBreakOnSeverity(MessageSeverity.Error, true);
        this.DebugInfoQueue.SetBreakOnSeverity(MessageSeverity.Corruption, true);

        var dxgiInfoQueue = DXGI.DXGIGetDebugInterface1<IDXGIInfoQueue>();
        dxgiInfoQueue.SetBreakOnSeverity(DXGI.DebugAll, InfoQueueMessageSeverity.Error, true);
        dxgiInfoQueue.SetBreakOnSeverity(DXGI.DebugAll, InfoQueueMessageSeverity.Corruption, true);


        this.Logger = logger.ForContext<DebugLayerLogger>();

        var annotations = device.ID3D11DeviceContext.QueryInterface<ID3DUserDefinedAnnotation>();
        if (annotations.Status)
        {
            this.Logger.Warning("A Microsoft Direct3D profiling tool (RenderDoc, Visual Studio Profiler, ...) is attached to the current context. Log messages from the DirectX Debug Layer might not be available");
        }
#endif
    }

    public void LogMessages()
    {
#if DEBUG
        var stored = this.DebugInfoQueue.NumStoredMessages;

        for (ulong i = 0; i < stored; i++)
        {
            var message = this.DebugInfoQueue.GetMessage(i);
            var level = SeverityToLevel(message.Severity);            
            this.Logger.Write(
                level,
                "[{@category}:{@id}] {@description}",
                message.Id.ToString(), message.Category.ToString(), UnterminateString(message.Description));

           if (level != LogEventLevel.Information)
            {
                throw new Exception($"[{message.Id}:{message.Category}] {UnterminateString(message.Description)}");
            }
        }

        this.DebugInfoQueue.ClearStoredMessages();
#endif
    }

    private static string UnterminateString(string message)
    {
        if (message.EndsWith('\0'))
        {
            return message[0..^1];
        }
        return message;
    }

    private static LogEventLevel SeverityToLevel(MessageSeverity severity)
    {
        return severity switch
        {
            MessageSeverity.Corruption => LogEventLevel.Fatal,
            MessageSeverity.Error => LogEventLevel.Error,
            MessageSeverity.Warning => LogEventLevel.Warning,
            _ => LogEventLevel.Information,
        };
    }
}
