namespace JackHenry.Reddit.Tests;

public class UserDataTrackerTest
{
    UserDataTracker _userTracker = new();

    [Fact]
    public void Test1()
    {
        Assert.Empty(_userTracker.MostActive());
        _userTracker.AcknowledgePost(new("a", "1"));
        Assert.Single(_userTracker.MostActive());
        _userTracker.AcknowledgePost(new("a", "2"));
        Assert.Single(_userTracker.MostActive());
        _userTracker.AcknowledgePost(new("b", "3"));
        Assert.Equal(2, _userTracker.MostActive().Count());
    }
}