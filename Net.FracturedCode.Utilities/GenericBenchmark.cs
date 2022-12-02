using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Net.FracturedCode.Utilities;

public abstract class GenericBenchmark
{
	/// <summary>
	/// Use [Test] in your implementation so dotnet and the text explorer pick it up.
	/// Don't forget to use Release when you run the tests.
	/// </summary>
	public abstract void Benchmark();
	
	protected void DefaultBench() =>
		BenchmarkRunner.Run(
			GetType(),
			ManualConfig
				.Create(DefaultConfig.Instance)
				.WithOptions(ConfigOptions.DisableLogFile | ConfigOptions.DisableOptimizationsValidator));
}