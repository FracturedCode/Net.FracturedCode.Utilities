# Net.FracturedCode.Utilities

A repository of handy C# utilities.

[Nuget Package](https://www.nuget.org/packages/Net.FracturedCode.Utilities/)

## List of features

1. RangeEnumerator. Use ranges in your foreach loops like `foreach (int i in 0..9)`.
2. TimeSpan.GetAwaiter. `await` a `TimeSpan`.
3. A `GenericBenchmark` class for repurposing the test explorer GUI for a benchmark suite.
4. A `TypedFactory<T>` class which is effectively a macro of `serviceProvider.GetRequiredService<T>()`. 
My primary use case for this is to more easily consume [typed `HttpClient`s in Singleton services](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#avoid-typed-clients-in-singleton-services).
5. A `.WhenAll` extension method for `Task`s and `ValueTask`s. Now you can 
   further chain your async LINQ.

## Usage

Please reference the unit tests.