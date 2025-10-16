namespace IntegrationTestManager.Utility;

/// <summary>
/// Enum that describes the executor type
/// </summary>
public enum ExecutorType
{
	/// <summary>
	/// None
	/// </summary>
	None,
	/// <summary>
	/// Parallel tests
	/// </summary>
	Parallel,
	/// <summary>
	/// Sequential tests
	/// </summary>
	Sequential,
	/// <summary>
	/// Parallel tests on GPU
	/// </summary>
	GPUParallel,
	/// <summary>
	/// Sequential tests on GPU
	/// </summary>
	GPUSequential
}

