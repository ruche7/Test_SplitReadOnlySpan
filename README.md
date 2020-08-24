# Test_SplitReadOnlySpan

## Summary

* Visual Studio 2019
* .NET Core 3.1
* Enable nullable option
* Use [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)

Benchmarking `SpanExtensions.SplitToRanges<T>` extension methods.

These methods splits `ReadOnlySpan<T>` or `Span<T>` by a separator(s), and returns `List<Range>` that means each split range.

## Benchmark methods

All following objects and methods are implemented on `BenchmarkMethods` static class.

```csharp
private static readonly string[] LineBreaks = { "\r\n", "\n", "\r" };

// Multi-line string. Lines are separated using LineBreaks elements.
private string text = @"";

// Number of lines of text for Setup() method. (Assigned by BenchmarkDotNet)
[Params(1, 10, 100, 1000, 10000)]
public int Line;

// Make N-line text. (N = Line)
[GlobalSetup]
public void Setup() => this.text = MakeString(this.Line);
```

* **<code>String_SplitNewLine</code>**
    * Split <code>text</code> to `string[]` using `string.Split('\n')`.
* **<code>Span_SplitNewLine</code>**
    1. Split <code>text.AsSpan()</code> to `List<Range>` using `ReadOnlySpan<char>.SplitToRanges('\n')`.
    2. Create `string[]` using `List<Range>`.
* **<code>String_SplitLineBreaks</code>**
    * Split <code>text</code> to `string[]` using `string.Split(LineBreaks, StringSplitOptions.None)`.
* **<code>Span_SplitLineBreaks</code>**
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
|                 Method |  Line |             Mean |          Error |         StdDev |           Median |
|----------------------- |------:|-----------------:|---------------:|---------------:|-----------------:|
|    String_SplitNewLine |     1 |         93.89 ns |       1.891 ns |       2.391 ns |         93.73 ns |
|      Span_SplitNewLine |     1 |         89.65 ns |       1.886 ns |       2.453 ns |         89.05 ns |
| String_SplitLineBreaks |     1 |        534.85 ns |      10.564 ns |      16.756 ns |        525.68 ns |
|   Span_SplitLineBreaks |     1 |        168.95 ns |       3.395 ns |       4.869 ns |        169.24 ns |
|    String_SplitNewLine |    10 |      1,075.52 ns |      13.211 ns |      11.032 ns |      1,069.83 ns |
|      Span_SplitNewLine |    10 |        521.44 ns |      10.509 ns |      19.995 ns |        516.58 ns |
| String_SplitLineBreaks |    10 |      5,813.50 ns |     115.262 ns |     107.816 ns |      5,773.06 ns |
|   Span_SplitLineBreaks |    10 |      1,225.32 ns |      24.501 ns |      39.564 ns |      1,210.19 ns |
|    String_SplitNewLine |   100 |     15,152.31 ns |     299.246 ns |     604.491 ns |     14,985.60 ns |
|      Span_SplitNewLine |   100 |      6,098.98 ns |     120.936 ns |     161.446 ns |      6,106.55 ns |
| String_SplitLineBreaks |   100 |     82,048.37 ns |   1,579.709 ns |   2,214.531 ns |     82,003.34 ns |
|   Span_SplitLineBreaks |   100 |     15,717.83 ns |     313.031 ns |     667.094 ns |     15,635.70 ns |
|    String_SplitNewLine |  1000 |    524,568.81 ns |  10,397.713 ns |  23,890.476 ns |    521,822.27 ns |
|      Span_SplitNewLine |  1000 |    249,858.21 ns |   4,959.699 ns |   4,396.644 ns |    248,217.16 ns |
| String_SplitLineBreaks |  1000 |  2,581,948.07 ns |  51,306.684 ns |  81,377.837 ns |  2,562,842.19 ns |
|   Span_SplitLineBreaks |  1000 |    460,632.10 ns |   9,181.251 ns |  10,204.936 ns |    455,389.89 ns |
|    String_SplitNewLine | 10000 | 15,434,441.42 ns | 302,882.208 ns | 645,466.679 ns | 15,423,582.81 ns |
|      Span_SplitNewLine | 10000 | 11,791,850.29 ns | 233,637.468 ns | 433,062.028 ns | 11,768,551.56 ns |
| String_SplitLineBreaks | 10000 | 35,465,864.29 ns | 231,007.433 ns | 204,782.072 ns | 35,427,995.83 ns |
|   Span_SplitLineBreaks | 10000 | 14,520,633.89 ns | 286,800.958 ns | 524,431.770 ns | 14,517,304.69 ns |
