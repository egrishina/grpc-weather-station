namespace Route256.WeatherSensorClient.Helpers;

public static class RetryHelper
{
    public const int DefaultRetryCount = 3;
    public const int DefaultRetryDelay = 1000;
    
    public static async Task<T> DoWithRetryAsync<T>(
        Func<Task<T>> func,
        ILogger logger,
        int retryCount,
        Predicate<Exception> retryCondition,
        int retryDelay)
    {
        for (int i = 0; i < retryCount + 1; ++i)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                if (retryCondition(e))
                {
                    logger.LogError(e, "Connection to the server failed.");
                }
                else
                {
                    logger.LogError(e, "Unexpected error occurred while connecting to the server.");
                    break;
                }
            }

            await Task.Delay(retryDelay);
        }
        
        return default (T);
    }
}