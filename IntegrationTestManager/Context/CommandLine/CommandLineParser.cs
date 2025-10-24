using CommandLine;
using System.Collections;

namespace IntegrationTestManager.Configuration;

/// <summary/>
public class CommandLineParser
{
	/// <summary/>
	public CommandLineOptions Options { get; private set; }

	public void Parse(string[] args)
	{
		Parser parser = new (ConfigurationParser);
		var p = parser.ParseArguments<CommandLineOptions>([.. args])
						.WithParsed(ReadOptions)
						.WithNotParsed(HandleParseError);
	}

	private void ConfigurationParser(ParserSettings settings)
	{ }

	private void ReadOptions(CommandLineOptions opts)
	{
		Options = opts;
	}

	private static void HandleParseError(IEnumerable errs)
	{
		Console.WriteLine($"Parse Error: {errs}");
	}
}
