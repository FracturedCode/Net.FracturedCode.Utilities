namespace Net.FracturedCode.Utilities;

public static class TaskExtensions
{
	public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks) => await Task.WhenAll(tasks);

	public static async Task WhenAll(this IEnumerable<Task> tasks) => await Task.WhenAll(tasks);

	public static async Task WhenAll(this IEnumerable<ValueTask> tasks) => await Task.WhenAll(tasks.Select(async t => await t.AsTask()));

	public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<ValueTask<T>> tasks) =>
		await Task.WhenAll(tasks.Select(async t => await t.AsTask()));

	public static async ValueTask<T> TaskFromResult<T>(this T result) => await ValueTask.FromResult(result);
}