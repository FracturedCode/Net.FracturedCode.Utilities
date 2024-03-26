using BenchmarkDotNet.Attributes;
using NUnit.Framework;

namespace Net.FracturedCode.Utilities.Benchmarks;

/*
|          Method |  Size |         Mean |     Error |    StdDev | Allocated |
|---------------- |------ |-------------:|----------:|----------:|----------:|
|   NormalForLoop |    10 |     2.112 ns | 0.0196 ns | 0.0174 ns |         - |
| ExtendedForLoop |    10 |     7.499 ns | 0.0140 ns | 0.0124 ns |         - |
|   NormalForLoop |  1000 |   230.362 ns | 1.2501 ns | 1.1693 ns |         - |
| ExtendedForLoop |  1000 |   240.736 ns | 2.2977 ns | 2.1493 ns |         - |
|   NormalForLoop | 10000 | 2,272.061 ns | 6.3941 ns | 4.9921 ns |         - |
| ExtendedForLoop | 10000 | 2,283.120 ns | 7.9026 ns | 6.5990 ns |         - |
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