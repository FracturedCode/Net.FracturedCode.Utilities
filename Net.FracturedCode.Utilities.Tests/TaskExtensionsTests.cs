using BenchmarkDotNet.Engines;

namespace Net.FracturedCode.Utilities.Tests;

public class TaskExtensionsTests
{
	[Test]
	public async Task FromResult()
	{
		await 1.TaskFromResult();
		
		object obj = new();
		object res = await obj.TaskFromResult();
		Assert.That(obj, Is.EqualTo(res));
	}

	[Test]
	public async Task WhenAllTask()
	{
		await (new List<Task> { Task.CompletedTask }).WhenAll();
		var results = await new List<Task<int>> { Task.FromResult(100) }.WhenAll();
		Assert.That(results.ToList() is [ 100 ]);
		var vtResults = await new List<ValueTask<int>> { ValueTask.FromResult(99) }.WhenAll();
		Assert.That(vtResults.ToList() is [ 99 ]);
		await new List<ValueTask> { ValueTask.CompletedTask }.WhenAll();
	}
}