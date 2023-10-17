namespace JackHenry.Reddit.Api
{
    public interface IThrottle
    {
        void UpdateBandwidth(int remaining, int used, int reset);
        Task WaitAsync(CancellationToken cancellationToken);
    }
}
