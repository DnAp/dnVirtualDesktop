using System.Windows.Forms;

namespace dnVirtualDesktop.Internal
{
    public class ListViewHotkeyItem : ListViewItem
    {
        public readonly HotkeyItem HotkeyItem;

        public ListViewHotkeyItem(HotkeyItem hotkeyItem) : base(new[] {"", ""})
        {
            HotkeyItem = hotkeyItem;

            SubItems[0].Text = hotkeyItem.Hotkey.HotKeyString();
            SubItems[1].Text = hotkeyItem.GetLabel();
        }
    }
}