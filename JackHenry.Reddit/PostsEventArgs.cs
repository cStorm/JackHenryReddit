namespace JackHenry.Reddit;

public class PostsEventArgs : ListEventArgs<PostSummary>
{
    public PostsEventArgs(IEnumerable<PostSummary> items) : base(items)
    {
    }
}
