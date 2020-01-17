# Test_SplitReadOnlySpan

## Summary

* Visual Studio 2019
* .NET Core 3.1
* Enable nullable option
* Use [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)

Benchmarking `SpanExtensions.SplitToRanges<T>` extension methods.

These methods splits `ReadOnlySpan<T>` or `Span<T>` by a separator(s), and returns `List<Range>` that means each split range.

Japanese article : https://www.ruche-home.net/boyaki/2020-01-18/CReadOnl

## Benchmark methods

All following objects and methods are implemented on `BenchmarkMethods` static class.

```csharp
private static readonly string[] LineBreaks = { "\r\n", "\n", "\r" };

// Multi-line strings. Lines are separated using LineBreaks elements.
private static readonly string Line1 = MakeString(1);
private static readonly string Line10 = MakeString(10);
private static readonly string Line100 = MakeString(100);
private static readonly string Line1000 = MakeString(1000);
private static readonly string Line10000 = MakeString(10000);
```

* **<code>String_SplitNewLine_Line*N*</code>**
    * Split <code>Line*N*</code> to `string[]` using `string.Split('\n')`.
* **<code>Span_SplitNewLine_Line*N*</code>**
    1. Split <code>Line*N*.AsSpan()</code> to `List<Range>` using `ReadOnlySpan<char>.SplitToRanges('\n')`.
    2. Create `string[]` using `List<Range>`.
* **<code>String_SplitLineBreaks_Line*N*</code>**
    * Split <code>Line*N*</code> to `string[]` using `string.Split(LineBreaks, StringSplitOptions.None)`.
* **<code>Span_SplitLineBreaks_Line*N*</code>**
    1. Split <code>Line*N*.AsSpan()</code> to `List<Range>` using `ReadOnlySpan<char>.SplitToRanges(LineBreaks)`.
    2. Create `string[]` using `List<Range>`.

## Benchmark results (on ruche7's PC)

`ReadOnlySpan<char>.SplitToRanges` is faster than `string.Split` in most cases.

```
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.17763.973 (1809/October2018Update/Redstone5)
Intel Core i7-5820K CPU 3.30GHz (Broadwell), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.101
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  DefaultJob : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
```

|                           Method |             Mean |          Error |           StdDev |           Median |
|--------------------------------- |-----------------:|---------------:|-----------------:|-----------------:|
|        String_SplitNewLine_Line1 |         89.12 ns |       0.274 ns |         0.229 ns |         89.01 ns |
|          Span_SplitNewLine_Line1 |         89.52 ns |       1.788 ns |         4.248 ns |         86.65 ns |
|       String_SplitNewLine_Line10 |      1,041.42 ns |       1.587 ns |         1.485 ns |      1,041.15 ns |
|         Span_SplitNewLine_Line10 |        492.08 ns |       9.951 ns |        15.197 ns |        485.98 ns |
|      String_SplitNewLine_Line100 |     13,826.88 ns |      33.435 ns |        26.104 ns |     13,824.87 ns |
|        Span_SplitNewLine_Line100 |      5,921.85 ns |     117.426 ns |       255.274 ns |      5,708.20 ns |
|     String_SplitNewLine_Line1000 |    480,190.60 ns |   1,008.693 ns |       894.180 ns |    480,153.91 ns |
|       Span_SplitNewLine_Line1000 |    259,307.68 ns |   8,304.329 ns |    24,355.159 ns |    258,941.54 ns |
|    String_SplitNewLine_Line10000 | 20,784,475.25 ns | 675,790.161 ns | 1,992,582.536 ns | 20,827,651.56 ns |
|      Span_SplitNewLine_Line10000 | 17,842,851.41 ns | 449,327.838 ns | 1,324,853.267 ns | 17,734,568.75 ns |
|     String_SplitLineBreaks_Line1 |        491.91 ns |       9.825 ns |        25.361 ns |        476.68 ns |
|       Span_SplitLineBreaks_Line1 |        180.32 ns |       3.675 ns |         5.501 ns |        181.08 ns |
|    String_SplitLineBreaks_Line10 |      6,206.53 ns |     122.705 ns |       131.293 ns |      6,180.42 ns |
|      Span_SplitLineBreaks_Line10 |      1,328.56 ns |      30.957 ns |        28.957 ns |      1,329.20 ns |
|   String_SplitLineBreaks_Line100 |     83,692.28 ns |   1,033.005 ns |       862.606 ns |     83,491.14 ns |
|     Span_SplitLineBreaks_Line100 |     16,415.50 ns |     324.911 ns |       551.723 ns |     16,300.75 ns |
|  String_SplitLineBreaks_Line1000 |  2,632,953.10 ns |  52,607.952 ns |    49,209.511 ns |  2,611,248.83 ns |
|    Span_SplitLineBreaks_Line1000 |    468,851.55 ns |   4,621.762 ns |     4,323.199 ns |    468,512.60 ns |
| String_SplitLineBreaks_Line10000 | 35,813,810.42 ns | 694,449.965 ns | 1,081,174.932 ns | 35,669,300.00 ns |
|   Span_SplitLineBreaks_Line10000 | 15,293,042.45 ns | 294,792.642 ns |   315,424.825 ns | 15,256,049.22 ns |
