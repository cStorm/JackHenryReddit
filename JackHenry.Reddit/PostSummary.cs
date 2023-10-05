namespace JackHenry.Reddit;

public record PostSummary(string Username);


public interface IRedditAggregationService
{
    void AcknowledgePost(PostSummary post);
    public IEnumerable<string> MostActive();
}