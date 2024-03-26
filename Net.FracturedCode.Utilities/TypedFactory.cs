using Microsoft.Extensions.DependencyInjection;

namespace Net.FracturedCode.Utilities;

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