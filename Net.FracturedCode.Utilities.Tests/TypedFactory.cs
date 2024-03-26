using Microsoft.Extensions.DependencyInjection;

namespace Net.FracturedCode.Utilities.Tests;

public class TypedFactoryTests
{
	private class MyHttpClient
	{
		public readonly HttpClient Client;
		public MyHttpClient(HttpClient client)
		{
			Client = client;
		}
	}
	[Test]
	public void TypedFactory_HttpClient_ProducesTransientInstances()
	{
		ServiceCollection services = [];
		services.AddHttpClient<MyHttpClient>();
		services.AddSingleton<TypedFactory<MyHttpClient>>();
		IServiceProvider serviceProvider = services.BuildServiceProvider();
		var factory = serviceProvider.GetRequiredService<TypedFactory<MyHttpClient>>();
		var client1 = factory.Create();
		var client2 = factory.Create();
		Assert.Multiple(() =>
		{
			Assert.That(client1, Is.Not.EqualTo(client2));
			Assert.That(client1.Client, Is.Not.EqualTo(client2.Client));
		});
	}

	private class ATestService();
	
	// How NOT to use the TypedFactory
	[Test]
	public void TypedFactory_Scoped_ProducesTheSameThing()
	{
		ServiceCollection services = [];
		services.AddScoped<ATestService>();
		services.AddSingleton<TypedFactory<ATestService>>();
		IServiceProvider serviceProvider = services.BuildServiceProvider();
		var factory = serviceProvider.GetRequiredService<TypedFactory<ATestService>>();
		var instance2 = factory.Create();
		Assert.That(factory.Create(), Is.EqualTo(instance2));
	}
}