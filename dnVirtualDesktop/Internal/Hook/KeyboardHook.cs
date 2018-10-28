using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LowLevelHooks.Keyboard;

namespace dnVirtualDesktop.Internal.Hook
{
    public class KeyboardHook : IDisposable
    {
        private readonly LowLevelHooks.Keyboard.KeyboardHook _lowLevelHook;
        private readonly List<Hotkey> _hotKeys = new List<Hotkey>();

        public KeyboardHook()
        {
            _lowLevelHook = new LowLevelHooks.Keyboard.KeyboardHook(KeyDownDelegate);
        }

        public bool AddHotKey(Hotkey hotKey)
        {
            if (!_lowLevelHook.Hooked)
                _lowLevelHook.Hook();

            var hasDuplicate = _hotKeys.Exists(h => hotKey.ModCode == h.ModCode && hotKey.KeyCode == h.KeyCode);
            if (hasDuplicate)
                return false;

            _hotKeys.Add(hotKey);
            return true;
        }

        public void RemoveHotKey(Hotkey hotKey)
        {
            _hotKeys.Remove(hotKey);
            if (_hotKeys.Count != 0)
                return;

            _lowLevelHook.Unhook();
        }


        private bool KeyDownDelegate(KeyboardHookEventArgs e)
        {
            foreach (var hotKey in _hotKeys)
            {
                if (hotKey.ModCode != e.ModCode || hotKey.KeyCode != e.Key)
                    continue;

                // 
                new Thread(() => { hotKey.HotKeyMessage(); }).Start();

                return false;
            }

            return true;
        }

        public void Dispose() => _lowLevelHook.Dispose();
    }
}