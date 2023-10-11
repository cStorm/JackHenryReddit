namespace JackHenry.Reddit;

public interface IRedditObserver<T>
{
    void AcknowledgeItems(IEnumerable<T> items);
    void UpdateItems(IEnumerable<T> items);
}
