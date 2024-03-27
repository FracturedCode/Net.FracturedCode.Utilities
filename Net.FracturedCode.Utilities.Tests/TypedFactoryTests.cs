using System.Collections.Immutable;
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
		var factory = serviceProvider.GetRequiredService<TypedFactory<T>>();
		return key is null ? (factory.Create(), factory.Create()) : (factory.Create(key), factory.Create(key));
	}

	private interface ITestService;

	private class TestService : ITestService;

	[Test]
	public void ExpandedExampleUsage()
	{
		// Analogous to ConfigureServices
		ServiceCollection services = [];
		
		services.AddKeyedTransient<ITestService, TestService>("test")
			.AddFactory<ITestService>();
		
		services.AddHttpClient<MyHttpClient>();
		services.AddFactory<MyHttpClient>();
		
		// ...
		// Analogous to builder.Build()
		IServiceProvider serviceProvider = services.BuildServiceProvider(); 
		
		// ...
		// Analogous to public class SomeSingletonService(TypedFactory<MyHttpClient> clientFactory)...
		var clientFactory = serviceProvider.GetRequiredService<TypedFactory<MyHttpClient>>();
		var testServiceFactory = serviceProvider.GetRequiredService<TypedFactory<ITestService>>(); 
		
		// ...
		// In SomeSingletonService.DoWork()
		clientFactory.Create().PretendMakeRequest();
		testServiceFactory.Create("test");
	}

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
	public void TransientInterfaceT_ProducesTransientInstances()
	{
		var instances = GetInstances<ITestService>(s => 
			s.AddTransient<ITestService, TestService>()
				.AddFactory<ITestService>()
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

	[Test]
	public void KeyedTransientT_ProducesTransientInstances()
	{
		var instances = GetInstances<TestService>(s =>
		{
			s.AddKeyedTransient<TestService>("key")
				.AddFactory<TestService>();
		}, "key");
		Assert.That(instances.Item1, Is.Not.EqualTo(instances.Item2));
	}

	[Test]
	public void KeyedTransientInterfaceT_ProducesTransientInstance()
	{
		var instances = GetInstances<ITestService>(s =>
		{
			s.AddKeyedTransient<ITestService, TestService>("key")
				.AddFactory<ITestService>();
		}, "key");
		Assert.That(instances.Item1, Is.Not.EqualTo(instances.Item2));
	}

	private class MyHttpClient(HttpClient client)
	{
		public readonly HttpClient Client = client;

		public void PretendMakeRequest() { }
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