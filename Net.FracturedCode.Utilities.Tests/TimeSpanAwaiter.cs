namespace Net.FracturedCode.Utilities.Tests;

public class TimeSpanAwaiter
{
	[Test]
	public async Task CompareTimeSpanAwaiterAndTaskDelay()
	{
		// In this situation maybe Task.Delay is more succinct,
		// but with a little imagination you could conceive where this could be useful
		// at the very least it's pretty cool
		await Task.Delay(TimeSpan.FromMilliseconds(10));
		await Task.Delay(10);
		await TimeSpan.FromMilliseconds(10);
	}
}