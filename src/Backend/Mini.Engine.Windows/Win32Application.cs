﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mini.Engine.Windows.Events;
using Vortice.Win32;
using static Vortice.Win32.User32;
using static Windows.Win32.PInvoke;

namespace Mini.Engine.Windows;

public static class Win32Application
{
    public static readonly WindowEvents WindowEvents = new WindowEvents();
    public static readonly RawEvents RawEvents = new RawEvents();

    public static Win32Window Initialize(string title, int width, int height)
    {
#nullable disable
        var moduleHandle = GetModuleHandle((string)null);
#nullable restore
        var wndClass = new WNDCLASSEX
        {
            Size = Unsafe.SizeOf<WNDCLASSEX>(),
            Styles = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW | WindowClassStyles.CS_OWNDC,
            WindowProc = &WndProc,
            InstanceHandle = moduleHandle.DangerousGetHandle(),
            CursorHandle = LoadCursor(IntPtr.Zero, SystemCursor.IDC_ARROW),
            BackgroundBrushHandle = IntPtr.Zero,
            IconHandle = IntPtr.Zero,
            ClassName = "WndClass"
        };

        RegisterClassEx(ref wndClass);
        return new Win32Window(title, width, height, WindowEvents);
    }

    public static void RegisterMessageListener(WindowMessage message, Action<UIntPtr, IntPtr> handler)
    {
        RawEvents.OnEvent += (o, e) =>
        {
            if (e.Msg == message)
            {
                handler(e.WParam, e.LParam);
            }
        };
    }

    public static bool PumpMessages()
    {
        var @continue = true;
        while (PeekMessage(out var msg, IntPtr.Zero, 0, 0, PM_REMOVE))
        {
            TranslateMessage(ref msg);
            DispatchMessage(ref msg);

            @continue = @continue && (msg.Value != (uint)WindowMessage.Quit);
        }

        return @continue;
    }

    [UnmanagedCallersOnly]
    public static IntPtr WndProc(IntPtr hWnd, WindowMessage msg, UIntPtr wParam, IntPtr lParam)
    {
        // TODO: ideally we never want to expose these events, right now its necessary for input
        // but we should replace it with input system similar to what ImGui uses

        // TODO: maybe we can move the input classes here and make ImGui use RawInputController?
        RawEvents.FireWindowEvents(hWnd, msg, wParam, lParam);

        switch (msg)
        {
            case WindowMessage.Destroy:
                PostQuitMessage(0);
                break;

            default:
                WindowEvents.FireWindowEvents(hWnd, msg, wParam, lParam);
                break;
        }

        return DefWindowProc(hWnd, msg, wParam, lParam);
    }
}
