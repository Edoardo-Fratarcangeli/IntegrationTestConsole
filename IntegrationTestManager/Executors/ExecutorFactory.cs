using System.Reflection;
using IntegrationTestManager.Configuration;
using IntegrationTestManager.Utility;
using Microsoft.Extensions.Logging;

namespace IntegrationTestManager.Executors;

/// <summary>
/// Factory that creates the specific tester
/// </summary>
public class ExecutorFactory : LogEntity<TestManager>
{
    private static ExecutorFactory Istance { get; set; }
    private readonly List<Type> Registry;
    private bool IsInitialized { get; set; }
    CancellationTokenSource CancellationTokenSource { get; init; }
    IContextService Context { get; init; }

    #region Constructor

    private ExecutorFactory(IContextService context,
                            ILogger<TestManager> logger,
                            CancellationTokenSource cancellationTokenSource)
                : base(logger, context.EnableLogger)
    {
        Registry = [.. Assembly.GetExecutingAssembly()
                                .GetTypes()
                                .Where(t => !t.IsAbstract)
                                .Where(t => t.IsSubclassOf(typeof(ATester)))];
        Context = context;
        CancellationTokenSource = cancellationTokenSource;
        IsInitialized = true;
    }

    #endregion

    #region Initialize
    public static void Initialize(IContextService context,
                                  ILogger<TestManager> logger,
                                  CancellationTokenSource cancellationTokenSource)
    {
        Istance ??= new(context, logger, cancellationTokenSource);
    }
    #endregion

    #region Create

    public static Result<ATester> Create(ExecutorType executorType)
    {
        if (Istance.IsInitialized)
        {
            foreach (var type in Istance.GetRegistry())
            {
                (object[] parameters, Type[] parametersTypes) = ExtractParams(type);

                if (type.GetConstructor(parametersTypes) is ConstructorInfo constructorInfo)
                {
                    if (constructorInfo.Invoke(parameters) is IStrategy strategy)
                    {
                        if (strategy.Match(executorType))
                        {
                            return Result<ATester>.Success((ATester)strategy);
                        }
                    }
                }
            }
        }

        return Result<ATester>.Fail();
    }


    #endregion

    #region Private Methods

    private static (object[] parameters, Type[] parametersTypes) ExtractParams(Type type)
    {
        if (type.IsSubclassOf(typeof(AParallelTester)))
        {
            return (new object[] { Istance.Context, Istance.GetLogger(), Istance.CancellationTokenSource },
                    new Type[] { typeof(ContextService), typeof(ILogger<TestManager>),typeof(CancellationTokenSource) });
        }
        else
        {
            return (new object[] { Istance.Context, Istance.GetLogger()},
                    new Type[] { typeof(ContextService), typeof(ILogger<TestManager>)});
        }
    }

    private List<Type> GetRegistry()
    {
        return Registry;
    }
    
    #endregion
}