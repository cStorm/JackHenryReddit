using JackHenry.Reddit;

namespace JackHenry.Reddit.Tests;

public class UserDataTrackerTest
{
    UserDataTracker _userTracker = new();

    [Fact]
    public void Test1()
    {
        Assert.Empty(_userTracker.MostActive());
        _userTracker.AcknowledgePost(new("a"));
        Assert.Single(_userTracker.MostActive());
        _userTracker.AcknowledgePost(new("a"));
        Assert.Single(_userTracker.MostActive());
        _userTracker.AcknowledgePost(new("b"));
        Assert.Equal(2, _userTracker.MostActive().Count());
    }
}