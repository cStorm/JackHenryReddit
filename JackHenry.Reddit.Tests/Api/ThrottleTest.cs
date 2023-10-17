using JackHenry.Reddit.Api;

namespace JackHenry.Reddit.Tests.Api;

public class ThrottleTest : IDisposable
{
    private readonly Throttle _throttle = new();

    public void Dispose() => _throttle.Dispose();

    [Fact]
    public void KeepsMultipleWaiting()
    {
        Dictionary<int, DateTime> done = new();
        Action<Task> Done(int key) => t => done.Add(key, DateTime.UtcNow);
        void AssertDone(params int[] keys)
        {
            Assert.Equal(keys, done.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray());
        }

        _throttle.UpdateBandwidth(3, default, 1);
        _throttle.WaitAsync(default).ContinueWith(Done(1));
        _throttle.WaitAsync(default).ContinueWith(Done(2));
        _throttle.WaitAsync(default).ContinueWith(Done(3));
        _throttle.WaitAsync(default).ContinueWith(Done(4));
        AssertDone();
        Thread.Sleep(400);
        AssertDone(1);
        Thread.Sleep(400);
        AssertDone(1, 2);
        Thread.Sleep(400);
        AssertDone(1, 2, 3);
        Thread.Sleep(400);
        AssertDone(1, 2, 3, 4);
    }
}
