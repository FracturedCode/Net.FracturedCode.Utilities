using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Net.FracturedCode.Utilities.Benchmarks;

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