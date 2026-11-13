using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace v2rayN.WinUI.Platform;

internal static class UiKit
{
    public static StackPanel Stack() => new()
    {
        Spacing = 8,
        Padding = new Thickness(12)
    };

    public static ScrollViewer Scroll(UIElement content) => new()
    {
        Content = content,
        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto
    };

    public static TextBlock Header(Panel panel, string text)
    {
        var block = new TextBlock
        {
            Text = text,
            FontSize = 16,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            Margin = new Thickness(0, 8, 0, 0)
        };
        panel.Children.Add(block);
        return block;
    }

    public static TextBox Text(Panel panel, string label, bool multi = false)
    {
        panel.Children.Add(new TextBlock { Text = label });
        var box = new TextBox
        {
            MinWidth = 280,
            AcceptsReturn = multi,
            TextWrapping = multi ? TextWrapping.Wrap : TextWrapping.NoWrap,
            MinHeight = multi ? 96 : 0
        };
        panel.Children.Add(box);
        return box;
    }

    public static PasswordBox Password(Panel panel, string label)
    {
        panel.Children.Add(new TextBlock { Text = label });
        var box = new PasswordBox { MinWidth = 280 };
        panel.Children.Add(box);
        return box;
    }

    public static ToggleSwitch Toggle(Panel panel, string label)
    {
        var toggle = new ToggleSwitch { Header = label, OnContent = "", OffContent = "" };
        panel.Children.Add(toggle);
        return toggle;
    }

    public static AutoSuggestBox Suggest(Panel panel, string label, IEnumerable<string>? items = null)
    {
        panel.Children.Add(new TextBlock { Text = label });
        var box = new AutoSuggestBox { MinWidth = 280, PlaceholderText = label };
        if (items is not null)
        {
            var list = items.ToList();
            box.ItemsSource = list;
            box.TextChanged += (_, e) =>
            {
                if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                {
                    var query = box.Text ?? string.Empty;
                    box.ItemsSource = string.IsNullOrEmpty(query)
                        ? list
                        : list.Where(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            };
        }

        panel.Children.Add(box);
        return box;
    }

    public static ComboBox Combo(Panel panel, string label, IEnumerable? items = null, string? display = null)
    {
        panel.Children.Add(new TextBlock { Text = label });
        var box = new ComboBox { MinWidth = 280, HorizontalAlignment = HorizontalAlignment.Left };
        if (display is not null)
        {
            box.DisplayMemberPath = display;
        }

        if (items is not null)
        {
            box.ItemsSource = items;
        }

        panel.Children.Add(box);
        return box;
    }

    public static Button Button(Panel panel, string label)
    {
        var button = new Button { Content = label, MinWidth = 120, HorizontalAlignment = HorizontalAlignment.Left };
        panel.Children.Add(button);
        return button;
    }

    public static ListView List(Panel? panel, string? display = null, double height = 280)
    {
        var list = new ListView
        {
            Height = height,
            SelectionMode = ListViewSelectionMode.Single
        };
        if (display is not null)
        {
            list.DisplayMemberPath = display;
        }

        panel?.Children.Add(list);
        return list;
    }

    public static MenuFlyoutItem Item(string text)
    {
        return new MenuFlyoutItem { Text = text };
    }

    public static async Task ShowQrAsync(string url)
    {
        byte[]? bytes = null;
        try
        {
            bytes = QRCodeUtils.GenQRCode(url);
        }
        catch (Exception ex)
        {
            Logging.SaveLog("ShowQr", ex);
        }

        var image = new Image { Width = 240, Height = 240, Stretch = Stretch.Uniform };
        if (bytes is { Length: > 0 })
        {
            var bitmap = new BitmapImage();
            using var stream = new InMemoryRandomAccessStream();
            var writer = new DataWriter(stream);
            writer.WriteBytes(bytes);
            await writer.StoreAsync();
            await writer.FlushAsync();
            writer.DetachStream();
            stream.Seek(0);
            await bitmap.SetSourceAsync(stream);
            image.Source = bitmap;
        }

        var panel = new StackPanel { Spacing = 8 };
        panel.Children.Add(image);
        panel.Children.Add(new TextBox { Text = url, IsReadOnly = true, TextWrapping = TextWrapping.Wrap });
        var copy = new Button { Content = ResUI.menuEditCopy };
        copy.Click += (_, _) => Native.SetClipboardText(url);
        panel.Children.Add(copy);

        var dialog = new ContentDialog
        {
            Title = Global.AppName,
            Content = panel,
            CloseButtonText = "OK",
            XamlRoot = AppHost.Root
        };
        await dialog.ShowAsync();
    }

    public static async Task<string?> PasswordAsync(string prompt)
    {
        var box = new PasswordBox();
        var dialog = new ContentDialog
        {
            Title = prompt,
            Content = box,
            PrimaryButtonText = "OK",
            CloseButtonText = ResUI.menuClose,
            XamlRoot = AppHost.Root
        };
        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary ? box.Password : null;
    }

    public static IDisposable Defer(Action dispose) => new ActionDisposable(dispose);

    private sealed class ActionDisposable(Action action) : IDisposable
    {
        public void Dispose() => action();
    }

    public static Task<bool> YesNo(string message)
    {
        var done = new TaskCompletionSource<bool>();
        var text = new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap };
        var yes = new Button { Content = ResUI.TbConfirm, MinWidth = 88 };
        var no = new Button { Content = ResUI.menuClose, MinWidth = 88 };
        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Right };
        buttons.Children.Add(yes);
        buttons.Children.Add(no);
        var panel = new StackPanel { Spacing = 16, Padding = new Thickness(16) };
        panel.Children.Add(text);
        panel.Children.Add(buttons);
        var window = new Window { Title = Global.AppName, Content = panel };
        void Close(bool accepted)
        {
            done.TrySetResult(accepted);
            window.Close();
        }

        yes.Click += (_, _) => Close(true);
        no.Click += (_, _) => Close(false);
        window.Closed += (_, _) => done.TrySetResult(false);
        AppHost.Place(window, "YesNoWindow", 440, 200);
        AppHost.Present(window, show: true, stealFocus: false);
        return done.Task;
    }
}
