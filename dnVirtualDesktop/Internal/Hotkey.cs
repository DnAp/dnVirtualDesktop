using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace dnVirtualDesktop.Internal
{
    public class Hotkey : IDisposable
    {
        private HotkeyWindow _Window;

        private int _ID;
        public string ID;

        public Hotkey(string ID)
        {
            this.ID = ID;
            System.Threading.Interlocked.Increment(ref _ID);
        }

        public string DesktopNumber()
        {
            return ID;
        }


        public event HotkeyActivatedEventHandler HotkeyActivated;

        public delegate void HotkeyActivatedEventHandler(object sender, EventArgs e);

        public Keys KeyCode { get; private set; }

        public bool modifierALT { get; private set; }
        public bool modifierCTRL { get; private set; }
        public bool modifierSHIFT { get; private set; }
        public bool modifierWIN { get; private set; }

        public string Key { get; private set; } = "";

        public string HotKeyString()
        {
            var keys = new List<string>();

            if (modifierALT)
                keys.Add("ALT");
            if (modifierCTRL)
                keys.Add("CTRL");
            if (modifierSHIFT)
                keys.Add("SHIFT");
            if (modifierWIN)
                keys.Add("WIN");
            keys.Add(Key);

            return string.Join("+", keys);
        }

        public uint ModCode { get; private set; }

        public bool Register(Keys keyCode, bool alt, bool ctrl, bool shift, bool win)
        {
            if (IsRegistered)
            {
                Unregister();
            }

            modifierALT = alt;
            modifierCTRL = ctrl;
            modifierSHIFT = shift;
            modifierWIN = win;
            Key = keyCode.ToString();
            KeyCode = keyCode;

            //Dim keyAlt As Keys = (key And Keys.Alt)
            //Dim keyControl As Keys = (key And Keys.Control)
            //Dim keyShift As Keys = (key And Keys.Shift)

            ModCode = 0;
            if (alt)
                ModCode += NativeMethods.MOD_ALT;
            if (ctrl)
                ModCode += NativeMethods.MOD_CONTROL;
            if (shift)
                ModCode += NativeMethods.MOD_SHIFT;
            if (win)
                ModCode += NativeMethods.MOD_WIN;

            var keyValue = Convert.ToUInt32(keyCode);

            _Window = new HotkeyWindow();
            _Window.CreateHandle(new CreateParams());
            _Window.HotkeyMessage += Window_HotkeyMessage;

            if (NativeMethods.RegisterHotKey(_Window.Handle, _ID, ModCode, keyValue) == 0)
            {
                if (!AddToHook())
                {
                    MessageBox.Show(HotKeyString() + Convert.ToString(" hotkey is already registered."));
                    return false;
                }
            }

            IsRegistered = true;
            return true;

            //Me._IsRegistered = Not (NativeMethods.RegisterHotKey(Me._Window.Handle, _ID, modValue, keyValue) = 0)
        }

        public void Unregister()
        {
            if (!IsRegistered)
                return;

            RemoveFromHook();
            IsRegistered = NativeMethods.UnregisterHotKey(_Window.Handle, _ID) == 0;
            if (IsRegistered)
                return;

            _Window.DestroyHandle();
            _Window = null;
        }

        private bool IsRegistered { get; set; }

        private void Window_HotkeyMessage(object sender, EventArgs e)
        {
            HotkeyActivated?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Hack for invoke event
        /// </summary>
        public void HotKeyMessage()
        {
            HotkeyActivated?.Invoke(this, new EventArgs());
        }

        private bool AddToHook()
        {
            return Program.MainForm.KeyboardHook.AddHotKey(this);
        }

        private void RemoveFromHook()
        {
            Program.MainForm.KeyboardHook.RemoveHotKey(this);
        }


        #region " IDisposable Support "

        // To detect redundant calls

        private bool _disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue && disposing && IsRegistered)
            {
                Unregister();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void IDisposable_Dispose()
        {
            Dispose(true);
        }

        void IDisposable.Dispose()
        {
            IDisposable_Dispose();
        }

        #endregion


        private class HotkeyWindow : NativeWindow
        {
            internal event HotkeyMessageEventHandler HotkeyMessage;

            internal delegate void HotkeyMessageEventHandler(object sender, EventArgs e);

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case NativeMethods.WM_HOTKEY:
                        HotkeyMessage?.Invoke(this, new EventArgs());
                        break;
                }

                base.WndProc(ref m);
            }
        }


        public static class NativeMethods
        {
            internal const uint MOD_ALT = 0x1;
            internal const uint MOD_CONTROL = 0x2;
            internal const uint MOD_SHIFT = 0x4;

            internal const uint MOD_WIN = 0x8;

            internal const int WM_HOTKEY = 0x312;

            [DllImport("kernel32", EntryPoint = "GlobalAddAtom", SetLastError = true, ExactSpelling = false)]
            public static extern int GlobalAddAtom([MarshalAs(UnmanagedType.LPTStr)] string lpString);


            [DllImport("user32", SetLastError = true)]
            public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);


            [DllImport("user32", SetLastError = true)]
            public static extern int UnregisterHotKey(IntPtr hWnd, int id);
        }
    }
}