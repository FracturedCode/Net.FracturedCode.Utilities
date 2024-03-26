namespace Net.FracturedCode.Utilities.Tests;

public class RangeEnumeratorTests
{

	[Test]
	public void TestNoLower()
	{
		int i = 0;
		foreach (int j in ..9)
		{
			i++;
		}
        Assert.That(i, Is.EqualTo(10));
	}

	[Test]
	public void TestTwoBounds()
	{
		int i = 0;
		foreach (int j in 1..10)
		{
			i++;
		}

		Assert.That(i, Is.EqualTo(10));
	}

	[Test]
	public void TestNoUpperBound()
	{
		Assert.Throws<NotSupportedException>(() =>
		{
			foreach (int _ in 0..)
			{

			}
		});

		Assert.Throws<NotSupportedException>(() =>
		{
			foreach (int _ in ..)
			{

			}
		});
	}

	[Test]
	public void TestNoFromEnd()
	{
		Assert.Throws<NotSupportedException>(() =>
		{
			foreach (int _ in ^0..2)
			{

			}
		});

		Assert.Throws<NotSupportedException>(() =>
		{
			foreach (int _ in ..^2)
			{

			}
		});
	}
}