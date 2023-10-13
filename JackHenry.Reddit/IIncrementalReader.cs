namespace JackHenry.Reddit;

public interface IIncrementalReader<T>
{
    bool Done { get; }
    IEnumerable<T> Enumerate();
}
