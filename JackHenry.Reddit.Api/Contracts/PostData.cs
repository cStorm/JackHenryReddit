namespace JackHenry.Reddit.Api.Contracts;

public record PostData(string name, string id, string author, string title, int ups, double created_utc)
{
    public DateTime GetCreated() => DateTimeOffset.FromUnixTimeSeconds((long)created_utc).UtcDateTime;

    public PostSummary CreatePostSummary()
    {
        return new(author, title, id, ups);
    }
}
