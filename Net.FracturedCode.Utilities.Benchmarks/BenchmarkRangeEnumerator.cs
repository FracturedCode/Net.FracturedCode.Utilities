using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Net.FracturedCode.Utilities;

BenchmarkRunner.Run<BenchmarkRangeEnumerator>();
/*
|          Method |  Size |         Mean |      Error |     StdDev | Allocated |
|---------------- |------ |-------------:|-----------:|-----------:|----------:|
|   NormalForLoop |    10 |     3.140 ns |  0.0397 ns |  0.0372 ns |         - |
| ExtendedForLoop |    10 |    10.133 ns |  0.0417 ns |  0.0390 ns |         - |
|   NormalForLoop |  1000 |   253.020 ns |  2.7288 ns |  2.5525 ns |         - |
| ExtendedForLoop |  1000 |   259.726 ns |  0.7270 ns |  0.5676 ns |         - |
|   NormalForLoop | 10000 | 2,476.260 ns | 18.2096 ns | 16.1423 ns |         - |
| ExtendedForLoop | 10000 | 2,490.464 ns | 12.9125 ns | 12.0784 ns |         - |
 */

[MemoryDiagnoser(false)]
public class BenchmarkRangeEnumerator
{
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