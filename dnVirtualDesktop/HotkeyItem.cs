using dnVirtualDesktop.Internal;

namespace dnVirtualDesktop
{
    public class HotkeyItem
    {
        public readonly Hotkey Hotkey;

        public HotkeyItem(string type, Hotkey hotkey)
        {
            Type = type;
            Hotkey = hotkey;
        }

        public string Type { get; }

        public string GetLabel()
        {
            var desktopNumber = DesktopNumber();

            switch (Type)
            {
                case "Pin/Unpin Window":
                case "Pin/Unpin Application":
                    return Type;
                case "Move Window to Desktop":
                    switch (desktopNumber)
                    {
                        case "Next":
                            return "Move to Next Desktop";
                        case "Previous":
                            return "Move to Previous Desktop";
                    }
                    break;
                case "Move Window to Desktop & Follow":

                    switch (desktopNumber)
                    {
                        case "Next":
                            return "Move Window to Next Desktop & Follow";
                        case "Previous":
                            return "Move Window to Previous Desktop & Follow";
                        default:
                            return "Move Window to Desktop #" + desktopNumber + " & Follow";
                    }
                //case "Navigate to Desktop":
            }

            return Type + " #" + desktopNumber;
        }


        public bool ALT()
        {
            return Hotkey.modifierALT;
        }

        public bool CTRL()
        {
            return Hotkey.modifierCTRL;
        }

        public bool SHIFT()
        {
            return Hotkey.modifierSHIFT;
        }

        public bool WIN()
        {
            return Hotkey.modifierWIN;
        }

        public string KEY()
        {
            return Hotkey.Key;
        }

        public string DesktopNumber()
        {
            return Hotkey.DesktopNumber();
        }
    }
}