using System.Diagnostics;

namespace MatrixMultiplication.Timing;

public class TimeRecorder
{
    public long TimeElapsed { get; private set; }

    public T RecordSpentTime<T>(Func<T> action, int millisecondsTimeout = -1)
    {
        var stopwatch = new Stopwatch();
        T result = default!;

        stopwatch.Start();
        var thread = new Thread(() => result = action.Invoke());
        thread.Start();
        if (millisecondsTimeout == -1) thread.Join();
        else thread.Join(millisecondsTimeout);
        stopwatch.Stop();
        
        TimeElapsed = stopwatch.ElapsedMilliseconds;
        return result;
    }
}