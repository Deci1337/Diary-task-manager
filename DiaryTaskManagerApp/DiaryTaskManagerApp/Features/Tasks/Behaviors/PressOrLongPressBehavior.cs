using System.Windows.Input;

namespace DiaryTaskManagerApp.Features.Tasks.Behaviors;

public sealed class PressOrLongPressBehavior : Behavior<Border>
{
    private Border? _border;
    private CancellationTokenSource? _cts;
    private DateTime _pressStart;

    public static readonly BindableProperty LongPressCommandProperty =
        BindableProperty.Create(nameof(LongPressCommand), typeof(ICommand), typeof(PressOrLongPressBehavior));

    public static readonly BindableProperty LongPressCommandParameterProperty =
        BindableProperty.Create(nameof(LongPressCommandParameter), typeof(object), typeof(PressOrLongPressBehavior));

    public static readonly BindableProperty LongPressDurationProperty =
        BindableProperty.Create(nameof(LongPressDuration), typeof(int), typeof(PressOrLongPressBehavior), 700);

    public ICommand? LongPressCommand
    {
        get => (ICommand?)GetValue(LongPressCommandProperty);
        set => SetValue(LongPressCommandProperty, value);
    }

    public object? LongPressCommandParameter
    {
        get => GetValue(LongPressCommandParameterProperty);
        set => SetValue(LongPressCommandParameterProperty, value);
    }

    public int LongPressDuration
    {
        get => (int)GetValue(LongPressDurationProperty);
        set => SetValue(LongPressDurationProperty, value);
    }

    protected override void OnAttachedTo(Border bindable)
    {
        base.OnAttachedTo(bindable);
        _border = bindable;

        var pointer = new PointerGestureRecognizer();
        pointer.PointerPressed += OnPressed;
        pointer.PointerReleased += OnReleased;
        pointer.PointerExited += OnExited;

        _border.GestureRecognizers.Add(pointer);
    }

    protected override void OnDetachingFrom(Border bindable)
    {
        if (_border is not null)
        {
            var pointer = _border.GestureRecognizers.OfType<PointerGestureRecognizer>().FirstOrDefault();
            if (pointer is not null)
            {
                pointer.PointerPressed -= OnPressed;
                pointer.PointerReleased -= OnReleased;
                pointer.PointerExited -= OnExited;
                _border.GestureRecognizers.Remove(pointer);
            }
        }

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _border = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnPressed(object? sender, PointerEventArgs e)
    {
        _pressStart = DateTime.Now;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        _ = RunLongPressAsync(_cts.Token);
    }

    private void OnReleased(object? sender, PointerEventArgs e)
    {
        _cts?.Cancel();
    }

    private void OnExited(object? sender, PointerEventArgs e)
    {
        _cts?.Cancel();
    }

    private async Task RunLongPressAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(LongPressDuration, ct);
            if (ct.IsCancellationRequested) return;

            if (LongPressCommand?.CanExecute(LongPressCommandParameter) == true)
            {
                LongPressCommand.Execute(LongPressCommandParameter);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}


