namespace v2rayN.WinUI.Platform;

/// <summary>
/// v2rayN stores hotkeys as WPF <c>System.Windows.Input.Key</c> values.
/// These maps keep configs written by the WPF build working.
/// </summary>
internal static class WpfKeys
{
    public static int? ToWpf(int virtualKey)
    {
        return virtualKey switch
        {
            0x08 => 2,
            0x09 => 3,
            0x0D => 6,
            0x13 => 7,
            0x14 => 8,
            0x1B => 13,
            0x20 => 18,
            0x21 => 19,
            0x22 => 20,
            0x23 => 21,
            0x24 => 22,
            0x25 => 23,
            0x26 => 24,
            0x27 => 25,
            0x28 => 26,
            0x2C => 30,
            0x2D => 31,
            0x2E => 32,
            >= 0x30 and <= 0x39 => virtualKey - 14,
            >= 0x41 and <= 0x5A => virtualKey - 21,
            0x5B => 70,
            0x5C => 71,
            0x5D => 72,
            >= 0x60 and <= 0x69 => virtualKey - 22,
            0x6A => 84,
            0x6B => 85,
            0x6C => 86,
            0x6D => 87,
            0x6E => 88,
            0x6F => 89,
            >= 0x70 and <= 0x87 => virtualKey - 22,
            0x90 => 114,
            0x91 => 115,
            0xBA => 140,
            0xBB => 141,
            0xBC => 142,
            0xBD => 143,
            0xBE => 144,
            0xBF => 145,
            0xC0 => 146,
            0xDB => 149,
            0xDC => 150,
            0xDD => 151,
            0xDE => 152,
            _ => null
        };
    }

    public static int? ToVirtual(int? wpfKey)
    {
        if (wpfKey is null or 0)
        {
            return null;
        }

        var key = wpfKey.Value;
        return key switch
        {
            2 => 0x08,
            3 => 0x09,
            6 => 0x0D,
            7 => 0x13,
            8 => 0x14,
            13 => 0x1B,
            18 => 0x20,
            19 => 0x21,
            20 => 0x22,
            21 => 0x23,
            22 => 0x24,
            23 => 0x25,
            24 => 0x26,
            25 => 0x27,
            26 => 0x28,
            30 => 0x2C,
            31 => 0x2D,
            32 => 0x2E,
            >= 34 and <= 43 => key + 14,
            >= 44 and <= 69 => key + 21,
            70 => 0x5B,
            71 => 0x5C,
            72 => 0x5D,
            >= 74 and <= 83 => key + 22,
            84 => 0x6A,
            85 => 0x6B,
            86 => 0x6C,
            87 => 0x6D,
            88 => 0x6E,
            89 => 0x6F,
            >= 90 and <= 113 => key + 22,
            114 => 0x90,
            115 => 0x91,
            140 => 0xBA,
            141 => 0xBB,
            142 => 0xBC,
            143 => 0xBD,
            144 => 0xBE,
            145 => 0xBF,
            146 => 0xC0,
            149 => 0xDB,
            150 => 0xDC,
            151 => 0xDD,
            152 => 0xDE,
            _ => null
        };
    }

    public static string Format(KeyEventItem? item)
    {
        if (item is null)
        {
            return string.Empty;
        }

        var parts = new List<string>();
        if (item.Control)
        {
            parts.Add("Ctrl");
        }

        if (item.Shift)
        {
            parts.Add("Shift");
        }

        if (item.Alt)
        {
            parts.Add("Alt");
        }

        if (ToVirtual(item.KeyCode) is int vk)
        {
            parts.Add(Name(vk));
        }

        return string.Join(" + ", parts);
    }

    public static string Name(int virtualKey)
    {
        return virtualKey switch
        {
            >= 0x41 and <= 0x5A => ((char)virtualKey).ToString(),
            >= 0x30 and <= 0x39 => ((char)virtualKey).ToString(),
            >= 0x70 and <= 0x87 => "F" + (virtualKey - 0x6F),
            0x08 => "Back",
            0x09 => "Tab",
            0x0D => "Enter",
            0x1B => "Esc",
            0x20 => "Space",
            0x21 => "PageUp",
            0x22 => "PageDown",
            0x23 => "End",
            0x24 => "Home",
            0x25 => "Left",
            0x26 => "Up",
            0x27 => "Right",
            0x28 => "Down",
            0x2D => "Insert",
            0x2E => "Delete",
            >= 0x60 and <= 0x69 => "NumPad" + (virtualKey - 0x60),
            _ => "0x" + virtualKey.ToString("X")
        };
    }
}
