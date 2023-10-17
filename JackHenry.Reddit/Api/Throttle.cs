using System.Diagnostics;

namespace JackHenry.Reddit.Api
{
    public class Throttle : IThrottle, IDisposable
    {
        private int _remaining;
        private int _reset;

        private DateTime _lastRequest = DateTime.UtcNow;
        private RollingAverage _secondsBetweenRequests = new(20);

        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0, 1);
        private Task _releaser = Task.CompletedTask;
        private int _waiting;


        protected double RemainingEta => _remaining * _secondsBetweenRequests.Average;


        public void UpdateBandwidth(int remaining, int used, int reset)
        {
            _remaining = remaining;
            _reset = reset;
            Debug.WriteLine($"ratelimit: {remaining}/{used + remaining} remain for {reset}s");
        }

        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            AcceptWaitRequest();
            if (_waiting != 0 || RemainingEta < _reset)
            {
                if (_releaser.IsCompleted)
                    _releaser = Task.Run(ReleaseWaitsAsync);

                _waiting++;
                await _signal.WaitAsync(cancellationToken);
            }
        }

        private void AcceptWaitRequest()
        {
            DateTime now = DateTime.UtcNow;
            (TimeSpan elapsed, _lastRequest) = (now - _lastRequest, now);
            _secondsBetweenRequests.Add(elapsed.TotalSeconds);
            Debug.WriteLine($"Request rate: {_secondsBetweenRequests.Average:0.000}s, Remaining ETA: {RemainingEta:0}s");
        }

        private async Task ReleaseWaitsAsync()
        {
            do
            {
                double seconds = (double)_reset / _remaining;
                Debug.WriteLine($"Waiting requests: {_waiting}, next: {seconds:0.000}s");
                await Task.Delay(TimeSpan.FromSeconds(seconds));

                _signal.Release();
                _waiting--;
            } while (_waiting > 0);
        }


        public void Dispose()
        {
            if (!_releaser.IsCompleted)
            {
                _waiting = 0;
                _releaser.Wait();
            }
            _releaser.Dispose();
            _signal.Dispose();
        }
    }
}
