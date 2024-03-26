using Microsoft.Diagnostics.Tracing.Parsers.FrameworkEventSource;
using Microsoft.Extensions.DependencyInjection;

namespace Net.FracturedCode.Utilities.Tests;

public class TypedFactoryTests
{
	private static (T, T) GetInstances<T>(Action<IServiceCollection> serviceSetup, string? key = null) where T : notnull
	{
		ServiceCollection services = [];
		serviceSetup(services);
		IServiceProvider serviceProvider = services.BuildServiceProvider();
		var factory = key is not null ?
			serviceProvider.GetRequiredKeyedService<TypedFactory<T>>(key)
			: serviceProvider.GetRequiredService<TypedFactory<T>>();
		return (factory.Create(), factory.Create());
	}

	private class TestService;

	[Test]
	public void TransientT_ProducesTransientInstances()
	{
		var instances = GetInstances<TestService>(s =>
			s.AddTransient<TestService>()
				.AddFactory<TestService>()
		);
		Assert.That(instances.Item1, Is.Not.EqualTo(instances.Item2));
	}

	[Test]
	public void ScopedT_ProducesOneInstance()
	{
		// How NOT to use TypedFactory<T>
		var instances = GetInstances<TestService>(s =>
			s.AddScoped<TestService>()
				.AddFactory<TestService>()
		);
		Assert.That(instances.Item1, Is.EqualTo(instances.Item2));
	}

	public void KeyedTransientT_ProducesTransientInstances()
	{
		var instances = GetInstances<TestService>(s =>
			s.AddKeyedTransient<TestService>("key")
				.AddFactory<TestService>()
		);
		Assert.That(instances.Item1, Is.Not.EqualTo(instances.Item2));
	}

	private class MyHttpClient(HttpClient client)
	{
		public readonly HttpClient Client = client;
	}

	[Test]
	public void HttpClient_ProducesTransientInstances()
	{
		ServiceCollection services = [];
		services.AddHttpClient<MyHttpClient>();
		services.AddFactory<MyHttpClient>();
		var instances = GetInstances<MyHttpClient>(s =>
		{
			s.AddHttpClient<MyHttpClient>();
			s.AddFactory<MyHttpClient>();
		});
		Assert.Multiple(() =>
		{
			Assert.That(instances.Item1, Is.Not.EqualTo(instances.Item2));
			Assert.That(instances.Item1.Client, Is.Not.EqualTo(instances.Item2.Client));
		});
	}
}