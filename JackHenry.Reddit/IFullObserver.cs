namespace JackHenry.Reddit;

public interface IFullObserver : IRedditObserver<PostSummary>
{
    void IRedditObserver<PostSummary>.AcknowledgeItems(IEnumerable<PostSummary> items) { }
    void IRedditObserver<PostSummary>.UpdateItems(IEnumerable<PostSummary> items) { }
}
