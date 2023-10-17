namespace JackHenry.Reddit.Api
{
    public class NullThrottle : IThrottle
    {
        public void UpdateBandwidth(int remaining, int used, int reset)
        {
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
