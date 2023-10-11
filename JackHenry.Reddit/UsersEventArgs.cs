namespace JackHenry.Reddit;

public class UsersEventArgs : ListEventArgs<UserHandle>
{
    public UsersEventArgs(IEnumerable<UserHandle> items) : base(items)
    {
    }
}
