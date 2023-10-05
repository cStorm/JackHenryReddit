namespace JackHenry.Reddit;

public class UserDataTracker
{
    private Dictionary<string, int> _counts = new();

    public void AcknowledgePost(PostSummary post)
    {
        string key = post.Username;
        if (_counts.TryGetValue(key, out int count))
            _counts[key] = count + 1;
        else
            _counts.Add(key, 1);
    }

    public IEnumerable<string> MostActive() => _counts.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key);
}

public class ChangedEventArgs<T> : EventArgs { }
public interface IRedditMonitor : IDisposable
{
    event EventHandler<ChangedEventArgs<PostSummary>> PostChanged;
    void Start(string subredditName);
    void Stop(string subredditName);
}