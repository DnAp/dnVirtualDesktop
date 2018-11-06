using System.Windows.Forms;

namespace dnVirtualDesktop.Internal
{
    public class ListViewHotkeyItem : ListViewItem
    {
        public readonly HotkeyItem HotkeyItem;

        public ListViewHotkeyItem(HotkeyItem hotkeyItem)
        {
            HotkeyItem = hotkeyItem;
            SubItems.Add(hotkeyItem.GetLabel());
            SubItems.Add(hotkeyItem.Hotkey.HotKeyString());
        }
    }
}