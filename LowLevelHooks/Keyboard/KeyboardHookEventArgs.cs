using System;
using System.Globalization;
using System.Windows.Forms;

namespace LowLevelHooks.Keyboard
{
    public class KeyboardHookEventArgs : HookEventArgs
    {
        public KeyboardHookEventArgs(KBDLLHOOKSTRUCT lParam, KeyboardMessages wParam, uint modCode)
        {
            ModCode = modCode;
            EventType = HookEventType.Keyboard;
            LParam = lParam;

            switch (wParam)
            {
                case KeyboardMessages.WmKeydown:
                    KeyboardEventName = KeyboardEventNames.KeyDown;
                    break;
                case KeyboardMessages.WmSyskeydown:
                    KeyboardEventName = KeyboardEventNames.SystemKeyDown;
                    break;
                case KeyboardMessages.WmKeyup:
                    KeyboardEventName = KeyboardEventNames.KeyUp;
                    break;
                case KeyboardMessages.WmSyskeyup:
                    KeyboardEventName = KeyboardEventNames.SystemKeyUp;
                    break;
            }

            UpdateModCode();
        }

        private KBDLLHOOKSTRUCT lParam;

        private KBDLLHOOKSTRUCT LParam
        {
            get { return lParam; }
            set
            {
                lParam = value;
                var nonVirtual = NativeMethods.MapVirtualKey((uint) VirtualKeyCode, 2);
                Char = Convert.ToChar(nonVirtual);
            }
        }

        public int VirtualKeyCode => LParam.VkCode;

        public Keys Key => (Keys) VirtualKeyCode;

        public char Char { get; private set; }

        public uint ModCode { get; private set; }

        public string KeyString
        {
            get
            {
                if (Char == '\0')
                {
                    return Key == Keys.Return ? "[Enter]" : string.Format("[{0}]", Key);
                }

                if (Char == '\r')
                {
                    Char = '\0';
                    return "[Enter]";
                }

                if (Char == '\b')
                {
                    Char = '\0';
                    return "[Backspace]";
                }

                return Char.ToString(CultureInfo.InvariantCulture);
            }
        }

        public KeyboardEventNames KeyboardEventName { get; internal set; }

        private void UpdateModCode()
        {
            uint mod = 0;
            switch (Key)
            {
                case Keys.Alt:
                    mod = NativeMethods.MOD_ALT;
                    break;
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    mod = NativeMethods.MOD_CONTROL;
                    break;
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    mod = NativeMethods.MOD_SHIFT;
                    break;
                case Keys.LWin:
                case Keys.RWin:
                    mod = NativeMethods.MOD_WIN;
                    break;
            }

            if (mod == 0)
                return;

            switch (KeyboardEventName)
            {
                case KeyboardEventNames.KeyUp:
                case KeyboardEventNames.SystemKeyUp:
                    ModCode &= ~mod;
                    /*
                     * 1001
                     * 1000
                     *~
                     * 0111
                     *|
                     * 0001
                     */
                    
                    
                    break;
                case KeyboardEventNames.KeyDown:
                case KeyboardEventNames.SystemKeyDown:
                    ModCode |= mod;

                    
                    break;
            }
        }
    }
}