using CommandLine;
using System.Collections;

namespace IntegrationTestManager.CommandLine;

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

	[Option('m', "mode", Required = false, HelpText = "Modality of execution: (1) sequential (2) parallel")]
	public ushort? TestMode { get; set; } = null;

	[Option('t', "tests", Required = false, HelpText = "Specific test to execute")]
	public IEnumerable<string> Tests { get; set; } = new List<string>();
}

/// <summary/>
internal class CommandLineParser
{
	/// <summary/>
	public CommandLineOptions Options { get; private set; }

	public void Parse(string[] args)
	{
		Parser parser = new Parser(ConfigurationParser);
		var p = parser.ParseArguments<CommandLineOptions>(args.ToArray())
						.WithParsed(opts => ReadOptions(opts))
						.WithNotParsed((errs) => HandleParseError(errs));
	}

	private void ConfigurationParser(ParserSettings settings)
	{ }

	private void ReadOptions(CommandLineOptions opts)
	{
		Options = opts;
	}

	private static void HandleParseError(IEnumerable errs)
	{
		Console.WriteLine($"Parse Error: {errs.ToString()}");
	}
}
