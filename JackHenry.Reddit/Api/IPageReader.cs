using System.Diagnostics.CodeAnalysis;

namespace JackHenry.Reddit.Api;

public interface IPageReader<T>
{
    bool ReadNext([MaybeNullWhen(false)] out IEnumerable<T> page);
}
