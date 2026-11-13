namespace v2rayN.WinUI.Views;

internal static class DialogPage
{
    public static (Grid Root, StackPanel Form, Button Save) Create(string title)
    {
        var form = UiKit.Stack();
        var save = new Button { Content = ResUI.TbConfirm, MinWidth = 120, HorizontalAlignment = HorizontalAlignment.Left };
        var bar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Padding = new Thickness(12)
        };
        bar.Children.Add(save);
        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        var scroll = UiKit.Scroll(form);
        Grid.SetRow(scroll, 0);
        Grid.SetRow(bar, 1);
        grid.Children.Add(scroll);
        grid.Children.Add(bar);
        return (grid, form, save);
    }

    public static void Show(UIElement element, bool visible)
    {
        element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
    }
}
