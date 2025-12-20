using System.Windows.Input;

namespace DiaryTaskManagerApp.Features.Tasks.Behaviors;

public sealed class LongPressDeleteBehavior : Behavior<Border>
{
    private Border? _border;
    private CancellationTokenSource? _cts;

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(LongPressDeleteBehavior));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(LongPressDeleteBehavior));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnAttachedTo(Border bindable)
    {
        base.OnAttachedTo(bindable);
        _border = bindable;

        var pointerGesture = new PointerGestureRecognizer();
        pointerGesture.PointerPressed += OnPointerPressed;
        pointerGesture.PointerReleased += OnPointerReleased;
        pointerGesture.PointerExited += OnPointerExited;

        _border.GestureRecognizers.Add(pointerGesture);
    }

    protected override void OnDetachingFrom(Border bindable)
    {
        if (_border is not null)
        {
            var pointerGesture = _border.GestureRecognizers.OfType<PointerGestureRecognizer>().FirstOrDefault();
            if (pointerGesture is not null)
            {
                pointerGesture.PointerPressed -= OnPointerPressed;
                pointerGesture.PointerReleased -= OnPointerReleased;
                pointerGesture.PointerExited -= OnPointerExited;
                _border.GestureRecognizers.Remove(pointerGesture);
            }
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _border = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        AnimateDeleteAsync(_cts.Token);
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e)
    {
        ResetAnimation();
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        ResetAnimation();
    }

    private async void AnimateDeleteAsync(CancellationToken ct)
    {
        if (_border is null) return;

        try
        {
            var originalColor = _border.BackgroundColor ?? Colors.Transparent;
            var targetColor = Color.FromRgb(176, 0, 32);
            const int durationMs = 1000;
            const int steps = 30;
            const int delayMs = durationMs / steps;

            for (int i = 0; i <= steps; i++)
            {
                if (ct.IsCancellationRequested) return;

                var t = (float)i / steps;
                var r = (byte)((1 - t) * originalColor.Red * 255 + t * targetColor.Red * 255);
                var g = (byte)((1 - t) * originalColor.Green * 255 + t * targetColor.Green * 255);
                var b = (byte)((1 - t) * originalColor.Blue * 255 + t * targetColor.Blue * 255);

                _border.BackgroundColor = Color.FromRgb(r, g, b);

                await Task.Delay(delayMs, ct);
            }

            if (!ct.IsCancellationRequested && Command?.CanExecute(CommandParameter) == true)
            {
                Command.Execute(CommandParameter);
            }

            _border.BackgroundColor = originalColor;
        }
        catch (TaskCanceledException)
        {
        }
    }

    private void ResetAnimation()
    {
        _cts?.Cancel();
        _cts = null;

        if (_border is not null)
        {
            _border.BackgroundColor = Color.FromRgb(21, 25, 35);
        }
    }
}


