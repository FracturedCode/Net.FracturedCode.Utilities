using System.Runtime.CompilerServices;

namespace Net.FracturedCode.Utilities;

// https://youtu.be/ileC_qyLdD4
public static class TimeSpanAwaiter
{
	public static TaskAwaiter GetAwaiter(this TimeSpan timeSpan) =>
		Task.Delay(timeSpan).GetAwaiter();
}