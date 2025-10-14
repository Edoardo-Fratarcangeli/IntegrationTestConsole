using IntegrationTestManager.CommandLine;
using IntegrationTestManager.DataServices;
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

		if (Initialization().Succeeded == false)
		{
			SafeFailedExit(cancellationTokenSource);
		}

		ServiceProvider serviceProvider = null;
		try
		{
			ILogger<TestManager> logger = null;

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

			IContextService contextService = serviceProvider.GetRequiredService<IContextService>();

			if (SetUpContext(contextService, args).Succeeded == false)
			{
				SafeFailedExit(cancellationTokenSource);
			}

			logger = serviceProvider.GetRequiredService<ILogger<TestManager>>();
			
			services.AddSingleton(provider =>
			{
				return new TestManager(contextService, logger, cancellationTokenSource);
			});

			serviceProvider = services.BuildServiceProvider();

			TestManager testManager = serviceProvider.GetRequiredService<TestManager>();

			if (testManager.Execute().Succeeded == false)
			{
				SafeFailedExit(cancellationTokenSource);
			}
			
			shutdownManager.WaitForShutdown();
		}
		catch (Exception e)
		{
#if DEBUG
            throw new CatchedException(e);
#else
			Log?.LogError(e);
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
            throw new CatchedException(e);
#else
			Log?.LogError(e);
            return Result.Fail();
#endif
        }
	}
	
    private static Result SetUpContext(IContextService context, string[] args)
	{
		try
		{
			CommandLineParser parser = new();
			parser.Parse(args);

			if (parser.Options is CommandLineOptions options)
			{
				context.SetCacheFolderPath(options.CacheFolderPath)
					   .SetDegreeOfParallelism(options.DegreeOfParallelism)
					   .SetEnableLogger(options.EnableLogger?? false)
					   .SetEnableVerbose(options.EnableVerbose?? false)
					   .SetExePath(options.ExePath)
					   .SetTestMode(options.TestMode.ToTestMode())
					   .SetTests(options.Tests);

				return Result.Success();
			}
		}
		catch (Exception e)
		{
#if DEBUG
            throw new CatchedException(e);
#else
			Log?.LogError(e);
            return Result.Fail();
#endif
        }
		
		return Result.Fail();
    }

    private static void SafeFailedExit(CancellationTokenSource cancellationTokenSource)
    {
#if DEBUG
        throw new ResultFailException();
#else
		Log?.LogError($"{nameof(ResultFailException)}: {nameof(GetType())}");
		SafeExit(cancellationTokenSource);
#endif
	}

	private static void SafeExit(CancellationTokenSource cancellationTokenSource)
	{
		cancellationTokenSource?.Cancel();
		return;
	}
	
    #endregion
}
