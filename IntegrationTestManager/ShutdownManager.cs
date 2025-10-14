using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Unix;
using Mono.Unix.Native;

namespace IntegrationTestManager;

/// <summary>
/// Safe closure manager
/// </summary>
public class ShutdownManager : IDisposable
{
    #region Fields & Handlers
    private readonly CancellationTokenSource _cts = new();
    private readonly Thread _unixSignalThread;
    private UnixSignal[] _unixSignals;

    private delegate bool ConsoleEventDelegate(int eventType);
    private readonly ConsoleEventDelegate _handler;

    [DllImport("Kernel32", SetLastError = true)]
    private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handler, bool add);
    #endregion

    #region Constructor
    public ShutdownManager()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _handler = new ConsoleEventDelegate(OnWindowsConsoleEvent);
            SetConsoleCtrlHandler(_handler, true);
        }
        else
        {
            AppDomain.CurrentDomain.ProcessExit += (_, __) => TriggerShutdown("ProcessExit");
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                TriggerShutdown("SIGINT / CancelKeyPress");
            };

            _unixSignals = 
            [
                new UnixSignal(Signum.SIGTERM),
                new UnixSignal(Signum.SIGINT)
            ];

            _unixSignalThread = new Thread(() =>
            {
                UnixSignal.WaitAny(_unixSignals);
                TriggerShutdown("SIGTERM / SIGINT (Unix)");
            })
            {
                IsBackground = true
            };
            _unixSignalThread.Start();
        }
    }
    #endregion

    #region Private Methods
    private bool OnWindowsConsoleEvent(int eventType)
    {
        TriggerShutdown($"Windows console event {eventType}");
        return false;
    }

    private void TriggerShutdown(string reason)
    {
        if (!_cts.IsCancellationRequested)
        {
            Debug.WriteLine("Closing: {Reason}", reason);
            _cts.Cancel();
        }
    }

    public void WaitForShutdown()
    {
        _cts.Token.WaitHandle.WaitOne();
    }

    public void Dispose()
    {
        _unixSignals?.ToList().ForEach(s => s.Close());
        _cts.Dispose();
    }
    #endregion
}
