namespace JackHenry.Reddit;

public class RollingAverage
{
    private readonly int _window;
    private readonly Queue<double> _values;
    private double? _last;
    private double _total;

    public RollingAverage(int window)
    {
        if (window <= 0) throw new ArgumentException();
        _window = window;
        _values = new Queue<double>(_window);
    }

    public double Average => _total / (_values.Count + 1);

    public void Add(double value)
    {
        if (_last.HasValue)
        {
            _values.Enqueue(_last.Value);
            while (_values.Count >= _window)
                _total -= _values.Dequeue();
        }
        _total += (_last = value).Value;
    }

    public void UpdateLast(double value)
    {
        _total += -(_last ?? 0) + (_last = value).Value;
    }
}
