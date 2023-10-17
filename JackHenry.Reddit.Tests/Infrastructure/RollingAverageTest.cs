namespace JackHenry.Reddit.Tests.Infrastructure;

public class RollingAverageTest
{
    [Fact]
    public void WindowCloses()
    {
        RollingAverage rolling = new(3);
        rolling.Add(1);
        Assert.Equal(1, rolling.Average); // 1
        rolling.Add(2);
        Assert.Equal(1.5, rolling.Average); // 1 2
        rolling.Add(3);
        Assert.Equal(2, rolling.Average); // 1 2 3
        rolling.Add(4);
        Assert.Equal(3, rolling.Average); // 2 3 4
        rolling.Add(5);
        Assert.Equal(4, rolling.Average); // 3 4 5
    }

    [Fact]
    public void ReplacesLatest()
    {
        RollingAverage rolling = new(3);
        rolling.UpdateLast(1);
        Assert.Equal(1, rolling.Average); // 1
        rolling.UpdateLast(2);
        Assert.Equal(2, rolling.Average); // 2
        rolling.Add(3);
        Assert.Equal(2.5, rolling.Average); // 2 3
        rolling.Add(4);
        Assert.Equal(3, rolling.Average); // 2 3 4
        rolling.UpdateLast(7);
        Assert.Equal(4, rolling.Average); // 2 3 7
        rolling.Add(-1);
        Assert.Equal(3, rolling.Average); // 3 7 -1
        rolling.UpdateLast(5);
        Assert.Equal(5, rolling.Average); // 3 7 5
    }
}
