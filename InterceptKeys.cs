using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SIGame_Clicker
{
    //public class InterceptKeys
    //{
    //    private const int WH_KEYBOARD_LL = 13;
    //    private const int WM_KEYDOWN = 0x0100;

    //    private static HookProc _proc = HookCallback;
    //    private static IntPtr _hookID = IntPtr.Zero;

    //    public static event EventHandler<KeyInterceptedEventArgs> KeyIntercepted;

    //    public static void Start()
    //    {
    //        _hookID = SetHook(_proc);
    //    }

    //    public static void Stop()
    //    {
    //        UnhookWindowsHookEx(_hookID);
    //    }

    //    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    //    private static IntPtr SetHook(HookProc proc)
    //    {
    //        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
    //        using (var curModule = curProcess.MainModule)
    //        {
    //            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    //        }
    //    }

    //    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    //    {
    //        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
    //        {
    //            int vkCode = Marshal.ReadInt32(lParam);
    //            Key key = KeyInterop.KeyFromVirtualKey(vkCode);

    //            KeyIntercepted?.Invoke(null, new KeyInterceptedEventArgs(key));
    //        }
    //        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    //    }

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    [return: MarshalAs(UnmanagedType.Bool)]
    //    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    //    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    //    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    private static extern IntPtr GetModuleHandle(string lpModuleName);
    //}
    //public class KeyInterceptedEventArgs : EventArgs
    //{
    //    public KeyInterceptedEventArgs(Key key)
    //    {
    //        InterceptedKey = key;
    //    }

    //    public Key InterceptedKey { get; }
    //}

    public class InterceptKeys
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private static HookProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static event EventHandler<KeyInterceptedEventArgs> KeyIntercepted;

        public static bool isKeyPressed = false;
        public static Key targetKey = Key.F1; // Пример: Клавиша "A"

        public static void Start()
        {
            _hookID = SetHook(_proc);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr SetHook(HookProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (wParam == (IntPtr)WM_KEYDOWN)
                {
                    // Клавиша была нажата
                    Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                    if (key == targetKey)
                    {
                        isKeyPressed = true;
                        KeyIntercepted?.Invoke(null, new KeyInterceptedEventArgs(key));
                    }
                }
                else if (wParam == (IntPtr)WM_KEYUP)
                {
                    // Клавиша была отпущена
                    Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                    if (key == targetKey)
                    {
                        isKeyPressed = false;
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }

    public class KeyInterceptedEventArgs : EventArgs
    {
        public KeyInterceptedEventArgs(Key key)
        {
            InterceptedKey = key;
        }

        public Key InterceptedKey { get; }
    }
}
