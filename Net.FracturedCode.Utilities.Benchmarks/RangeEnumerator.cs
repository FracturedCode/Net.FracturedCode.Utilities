using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Net.FracturedCode.Utilities.Benchmarks;

/*
|          Method |  Size |         Mean |      Error |     StdDev | Allocated |
|---------------- |------ |-------------:|-----------:|-----------:|----------:|
|   NormalForLoop |    10 |     2.882 ns |  0.0505 ns |  0.0448 ns |         - |
| ExtendedForLoop |    10 |     9.610 ns |  0.0292 ns |  0.0244 ns |         - |
|   NormalForLoop |  1000 |   250.845 ns |  1.1304 ns |  0.9439 ns |         - |
| ExtendedForLoop |  1000 |   258.087 ns |  2.6768 ns |  2.2352 ns |         - |
|   NormalForLoop | 10000 | 2,488.214 ns | 38.8218 ns | 36.3139 ns |         - |
| ExtendedForLoop | 10000 | 2,485.906 ns | 24.8535 ns | 23.2480 ns |         - |
 */

[MemoryDiagnoser(false)]
public class BenchmarkRangeEnumerator : GenericBenchmark
{
	[Test]
	public override void Benchmark() => DefaultBench();

	[Params(10, 1000, 10000)]
	public int Size { get; set; }

	[Benchmark]
	public void NormalForLoop()
	{
		for (int i = 0; i < Size; i++)
		{
			noop(i);
		}
	}

	[Benchmark]
	public void ExtendedForLoop()
	{
		foreach (int i in ..Size)
		{
			noop(i);
		}
	}

	private static void noop(int x) {}
}