using CommandLine;

namespace IntegrationTestManager.Configuration;

public sealed class CommandLineOptions
{

	[Option('f', "folder", Required = false, HelpText = "Cache folder path for base tests.")]
	public string CacheFolderPath { get; set; } = null;

	[Option('p', "parallelism", Required = false, HelpText = "Number of parallel logic CPUs to use")]
	public int? DegreeOfParallelism { get; set; } = null;

	[Option('l', "logger", Required = false, HelpText = "Enable logger")]
	public bool? EnableLogger { get; set; } = null;

	[Option('v', "verbose", Required = false, HelpText = "Enable verbose mode")]
	public bool? EnableVerbose { get; set; } = null;
	
	[Option('x', "exepath", Required = false, HelpText = "Full path to actor of a test (usually .exe).")]
	public string ExePath { get; set; } = null;

	[Option('g', "gpu", Required = false, HelpText = "Use GPU for computations")]
	public bool? UseGPUComputation { get; set; } = null;

	[Option('m', "mode", Required = false, HelpText = "Modality of execution: (1) parallel (2) sequential ")]
	public ushort? TestMode { get; set; } = null;

	[Option('t', "tests", Required = false, HelpText = "Specific test to execute")]
	public IEnumerable<string> Tests { get; set; } = [];
}
