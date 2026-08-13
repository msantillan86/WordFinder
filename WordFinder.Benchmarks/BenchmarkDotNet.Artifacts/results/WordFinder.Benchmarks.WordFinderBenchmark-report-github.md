```

BenchmarkDotNet v0.15.8, Windows 10 (10.0.19045.6466/22H2/2022Update)
Intel Core i5-7200U CPU 2.50GHz (Max: 2.70GHz) (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), X64 RyuJIT x86-64-v3


```
| Method              | Mean     | Error   | StdDev  | Gen0    | Gen1    | Gen2    | Allocated |
|-------------------- |---------:|--------:|--------:|--------:|--------:|--------:|----------:|
| TestFindPerformance | 308.8 μs | 5.14 μs | 4.56 μs | 49.8047 | 49.8047 | 49.8047 | 198.86 KB |
