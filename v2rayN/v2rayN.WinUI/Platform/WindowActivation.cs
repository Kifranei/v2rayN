namespace v2rayN.WinUI.Platform;

/// <summary>
/// WinUI's built-in fetcher only watches FrameworkElement. These windows are
/// shown with ShowWindow(SW_SHOWNOACTIVATE), so activation follows that call.
/// </summary>
internal sealed class WindowActivationFetcher : IActivationForViewFetcher
{
    public static event Action<Window, bool>? Changed;

    public static void Notify(Window window, bool active) => Changed?.Invoke(window, active);

    public int GetAffinityForView(Type view) =>
        typeof(Window).IsAssignableFrom(view) ? 20 : 0;

    public IObservable<bool> GetActivationForView(IActivatableView view) => new Stream((Window)view);

    private sealed class Stream(Window window) : IObservable<bool>
    {
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            var last = false;
            var seen = false;

            void Emit(bool active)
            {
                if (seen && last == active)
                {
                    return;
                }

                seen = true;
                last = active;
                observer.OnNext(active);
            }

            void OnChanged(Window source, bool active)
            {
                if (ReferenceEquals(source, window))
                {
                    Emit(active);
                }
            }

            void OnActivated(object sender, WindowActivatedEventArgs e) =>
                Emit(e.WindowActivationState != WindowActivationState.Deactivated);

            void OnVisibility(object sender, WindowVisibilityChangedEventArgs e) => Emit(e.Visible);

            void OnClosed(object sender, WindowEventArgs e) => Emit(false);

            Changed += OnChanged;
            window.Activated += OnActivated;
            window.VisibilityChanged += OnVisibility;
            window.Closed += OnClosed;
            Emit(Native.IsWindowVisible(WinRT.Interop.WindowNative.GetWindowHandle(window)));
            return new ActionDisposable(() =>
            {
                Changed -= OnChanged;
                window.Activated -= OnActivated;
                window.VisibilityChanged -= OnVisibility;
                window.Closed -= OnClosed;
            });
        }
    }
}
