namespace Ivtem.TSqlParsing.Extensions;


internal static class TaskExtensions
{
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource();
        var delay = Task.Delay(timeout, cts.Token);

        var winner = await Task.WhenAny(task, delay).ConfigureAwait(continueOnCapturedContext: false);

        if (winner == task)
        {
            cts.Cancel();
        }
        else
        {
            throw new TimeoutException();
        }

        return await task.ConfigureAwait(continueOnCapturedContext: false);
    }

    public static async Task WithTimeout(this Task task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource();
        var delay = Task.Delay(timeout, cts.Token);

        var winner = await Task.WhenAny(task, delay).ConfigureAwait(continueOnCapturedContext: false);

        if (winner == task)
        {
            cts.Cancel();
        }
        else
        {
            throw new TimeoutException();
        }
    }
}
