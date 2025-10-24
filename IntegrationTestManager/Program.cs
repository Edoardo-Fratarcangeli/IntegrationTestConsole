using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IntegrationTestManager;

internal class Program
{

	#region MAIN
	
	static void Main(string[] args)
	{
		CancellationTokenSource cancellationTokenSource = null;

		if (Result.IsFailed(Initialization))
		{
			SafeFailedExit(nameof(Initialization));
		}

		ServiceProvider serviceProvider = null;
		try
		{
			ILogger<TestManager> logger = null;

			// --- inject services ---
			var services = new ServiceCollection()
									.AddLogging(builder =>
									{
										builder.ClearProviders();
										builder.AddSerilog();
									})
									.AddSingleton<ShutdownManager>()
									.AddTransient<IContextService, ContextService>();
			serviceProvider = services.BuildServiceProvider();
			
			ShutdownManager shutdownManager = serviceProvider.GetRequiredService<ShutdownManager>();

			// --- build context ---
			IContextService contextService = serviceProvider.GetRequiredService<IContextService>()
															.SetArgs(args);
			if (Result.IsFailed(contextService.SetProperties))
			{
				SafeFailedExit(nameof(IContextService.SetProperties));
			}

			logger = serviceProvider.GetRequiredService<ILogger<TestManager>>();
			services.AddSingleton(provider =>
			{
				return new TestManager(contextService, logger, cancellationTokenSource);
			});
			serviceProvider = services.BuildServiceProvider();

			// --- test execution ---
			TestManager testManager = serviceProvider.GetRequiredService<TestManager>();
			if (Result.IsFailed(testManager.Execute))
			{
				SafeFailedExit(cancellationTokenSource, nameof(TestManager.Execute));
			}
			
			shutdownManager.WaitForShutdown();
		}
		catch (Exception e)
		{
#if DEBUG
        	throw new CatchedException(message: $"Collected in {nameof(Main)}", innerException: e);
#else
			Console.WriteLine($"{nameof(CatchedException)} [{nameof(Main)}] : {e}");
#endif
		}
        finally
		{			
			serviceProvider?.Dispose();
			SafeExit(cancellationTokenSource);
        }
	}

    #endregion

    #region Private Methods

    private static Result Initialization()
	{
		try
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Information()
				.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
				.CreateLogger();

			return Result.Success();
		}
		catch (Exception e)
		{
#if DEBUG
        	throw new CatchedException(message: $"Collected in {nameof(Initialization)}", innerException: e);
#else
			Console.WriteLine($"{nameof(CatchedException)} [{nameof(Initialization)}] : {e}");
            return Result.Fail();
#endif
        }
	}
	
	private static void SafeFailedExit(string nameOfMethod)
	{
#if DEBUG
		throw new ResultFailException(message: $"Collected in {nameOfMethod}");
#else
		Console.WriteLine($"{nameof(ResultFailException)}: {nameOfMethod}");
#endif
	}
	
    private static void SafeFailedExit(CancellationTokenSource cancellationTokenSource, string nameOfMethod)
	{
		SafeFailedExit(nameOfMethod);
		SafeExit(cancellationTokenSource);
	}

	private static void SafeExit(CancellationTokenSource cancellationTokenSource)
	{
		cancellationTokenSource?.Cancel();
		return;
	}
	
    #endregion
}
