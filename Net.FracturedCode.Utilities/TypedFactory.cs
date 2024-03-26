using Microsoft.Extensions.DependencyInjection;

namespace Net.FracturedCode.Utilities;

/// <summary>
/// Effectively a macro for serviceProvider.GetRequiredService&lt;T&gt;(). Only use with Transient lifetime services.
/// </summary>
/// <typeparam name="T">Any transient lifetime service already registered in your DI container.</typeparam>
public class TypedFactory<T> where T : notnull
{
	private readonly IServiceProvider _serviceProvider;
	public TypedFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public T Create() => _serviceProvider.GetRequiredService<T>();

	public T Create(string key) => _serviceProvider.GetRequiredKeyedService<T>(key);
}

public static class TypedFactoryServiceCollectionExtensions
{
	public static IServiceCollection AddFactory<T>(this IServiceCollection serviceCollection) where T : notnull
	{
		serviceCollection.AddTransient<TypedFactory<T>>();
		return serviceCollection;
	}
}