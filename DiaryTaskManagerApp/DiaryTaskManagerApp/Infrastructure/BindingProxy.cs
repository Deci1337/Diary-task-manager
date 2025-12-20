namespace DiaryTaskManagerApp.Infrastructure;

public sealed class BindingProxy : BindableObject
{
    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(nameof(Data), typeof(object), typeof(BindingProxy));

    public object? Data
    {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }
}



