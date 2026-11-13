namespace v2rayN.WinUI.Views;

// WinUI's window factory fail-fasts when a generic type sits between Window and the concrete window.
public class WinUIWindow : Window;

public class ReactiveWindow<TViewModel> : WinUIWindow, IViewFor<TViewModel>
    where TViewModel : class
{
    public TViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }
}
