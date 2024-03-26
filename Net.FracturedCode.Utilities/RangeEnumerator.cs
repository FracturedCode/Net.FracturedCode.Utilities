namespace Net.FracturedCode.Utilities;

// https://youtu.be/jmmz1cInNow
public static class RangeExtensions
{
	public static CustomIntEnumerator GetEnumerator(this Range range) => new(range);
}

public ref struct CustomIntEnumerator
{
	public int Current { get; private set; }
	private readonly int _end;
	public CustomIntEnumerator(Range range)
	{
		if (range.End.IsFromEnd || range.Start.IsFromEnd)
		{
			throw new NotSupportedException();
		}
		Current = range.Start.Value - 1;
		_end = range.End.Value;
	}

	public bool MoveNext()
	{
		Current++;
		return Current <= _end;
	}
}