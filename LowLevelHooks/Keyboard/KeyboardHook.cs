using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LowLevelHooks.Keyboard
{
    public sealed class KeyboardHook : IDisposable, IHook
    {
        private uint _modCode;

        #region Custom Events

        public delegate bool KeyDownDelegate(KeyboardHookEventArgs e);

        private readonly KeyDownDelegate _keyDown;

        private bool OnKeyDown(KeyboardHookEventArgs e)
        {
            return _keyDown == null || _keyDown(e);
        }

        public event EventHandler<KeyboardHookEventArgs> KeyUp;

        private void OnKeyUp(KeyboardHookEventArgs e)
        {
            KeyUp?.Invoke(this, e);
            OnKeyEvent(e);
        }

        public event EventHandler<KeyboardHookEventArgs> KeyEvent;

        private void OnKeyEvent(KeyboardHookEventArgs e)
        {
            KeyEvent?.Invoke(this, e);
        }

        #endregion

        /// <summary>
        /// The hook Id we create. This is stored so we can unhook later.
        /// </summary>
        private IntPtr _hookId;

        private readonly LowLevelProc _callback;

        public bool Hooked { get; private set; }

        public KeyboardHook(KeyDownDelegate keyDown)
        {
            _keyDown = keyDown;
            _callback = KeyboardHookCallback;
        }

        public void Hook()
        {
            if (Hooked) return;

            _hookId = Win32.SetWindowsHook(Win32.Hooks.WH_KEYBOARD_LL, _callback);
            Hooked = true;
        }

        public void Unhook()
        {
            if (!Hooked) return;

            NativeMethods.UnhookWindowsHookEx(_hookId);
            Hooked = false;
        }

        /// <summary>
        /// This is the callback method that is called whenever a low level keyboard event is triggered.
        /// We use it to call our individual custom events.
        /// </summary>
        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);

            var lParamStruct = (KBDLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            var e = new KeyboardHookEventArgs(lParamStruct, (KeyboardMessages) wParam, _modCode);
            _modCode = e.ModCode;
            switch (e.KeyboardEventName)
            {
                case KeyboardEventNames.KeyDown:
                case KeyboardEventNames.SystemKeyDown:
                    if (!OnKeyDown(e))
                        return new IntPtr(1);
                    break;
                case KeyboardEventNames.KeyUp:
                case KeyboardEventNames.SystemKeyUp:
                    OnKeyUp(e);
                    break;
            }

            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        #region IDisposable Members / Finalizer

        /// <summary>
        /// Call this method to unhook the Keyboard Hook, and to release resources allocated to it.
        /// </summary>
        public void Dispose()
        {
            Unhook();
            GC.SuppressFinalize(this);
        }

        ~KeyboardHook()
        {
            Unhook();
        }

        #endregion
    }
}