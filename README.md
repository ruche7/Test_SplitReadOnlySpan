# Test_SplitReadOnlySpan

## Summary

* Visual Studio 2019
* .NET Core 3.1
* Enable nullable option
* Use [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)

Benchmarking `SpanExtensions.SplitToRanges<T>` extension methods.

These methods splits `ReadOnlySpan<T>` or `Span<T>` by a separator(s), and returns `List<Range>` that means each split range.

## Benchmark methods

All following objects and methods are implemented on `BenchmarkMethods` class.

```csharp
private static readonly string[] LineBreaks = { "\r\n", "\n", "\r" };

// Multi-line string. Lines are separated using LineBreaks elements.
private string text = @"";

// Number of lines of texts for Setup() method. (Assigned by BenchmarkDotNet)
[Params(1, 10, 100, 1000, 10000)]
public int Line { get; set; }

// Make N-line texts. (Called by BenchmarkDotNet before benchmarking)
[GlobalSetup]
public void Setup() => this.text = MakeString(this.Line);
```

* **<code>String_Split</code>**
    * Split <code>text</code> to `string[]` using `string.Split(LineBreaks, StringSplitOptions.None)`.
* **<code>Span_Split</code>**
    1. Split <code>text.AsSpan()</code> to `List<Range>` using `ReadOnlySpan<char>.SplitToRanges(LineBreaks)`.
    2. Create `string[]` using `List<Range>`.

## Benchmark results (on ruche7's PC)

`ReadOnlySpan<char>.SplitToRanges` is faster than `string.Split` in most cases.

```
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1049 (1909/November2018Update/19H2)
Intel Core i7-5820K CPU 3.30GHz (Broadwell), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.401
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
```
|       Method |  Line |            Mean |         Error |        StdDev |          Median | Ratio | RatioSD |
|------------- |------:|----------------:|--------------:|--------------:|----------------:|------:|--------:|
| String_Split |     1 |        508.4 ns |      10.28 ns |      19.57 ns |        506.6 ns |  1.00 |    0.00 |
|   Span_Split |     1 |        168.6 ns |       3.41 ns |       5.10 ns |        166.1 ns |  0.33 |    0.02 |
|              |       |                 |               |               |                 |       |         |
| String_Split |    10 |      5,859.7 ns |     115.27 ns |     165.32 ns |      5,766.9 ns |  1.00 |    0.00 |
|   Span_Split |    10 |      1,231.2 ns |      24.62 ns |      41.14 ns |      1,220.2 ns |  0.21 |    0.01 |
|              |       |                 |               |               |                 |       |         |
| String_Split |   100 |     81,594.4 ns |   1,627.17 ns |   2,385.08 ns |     81,128.1 ns |  1.00 |    0.00 |
|   Span_Split |   100 |     15,421.4 ns |     307.08 ns |     537.83 ns |     15,533.0 ns |  0.19 |    0.01 |
|              |       |                 |               |               |                 |       |         |
| String_Split |  1000 |  2,511,940.7 ns |  49,692.28 ns |  53,170.18 ns |  2,487,233.2 ns |  1.00 |    0.00 |
|   Span_Split |  1000 |    466,776.6 ns |   9,109.31 ns |  11,844.68 ns |    471,881.8 ns |  0.19 |    0.01 |
|              |       |                 |               |               |                 |       |         |
| String_Split | 10000 | 33,730,480.7 ns | 114,755.19 ns |  89,593.31 ns | 33,720,993.8 ns |  1.00 |    0.00 |
|   Span_Split | 10000 | 14,362,424.3 ns | 286,109.48 ns | 419,375.57 ns | 14,449,090.6 ns |  0.42 |    0.01 |
