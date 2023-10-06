namespace JackHenry.Reddit;

public class UsersEventArgs : ListEventArgs<string>
{
    public UsersEventArgs(IEnumerable<string> items) : base(items)
    {
    }
}
