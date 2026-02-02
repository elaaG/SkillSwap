public class BindConverter<T>
{
    private readonly Func<T> _get;
    private readonly Func<T, Task> _set;

    public BindConverter(Func<T> get, Func<T, Task> set)
    {
        _get = get; _set = set;
    }

    public T Value { get => _get(); set { _set(value).GetAwaiter().GetResult(); } }
}