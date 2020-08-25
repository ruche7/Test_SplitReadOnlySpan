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

// Multi-line string. Lines are separated using '\n'.
private string textWithNewLine = @"";

// Multi-line string. Lines are separated using LineBreaks elements.
private string textWithLineBreaks = @"";

// Number of lines of texts for Setup() method. (Assigned by BenchmarkDotNet)
[Params(1, 10, 100, 1000, 10000)]
public int Line { get; set; }

// Make N-line texts. (Called by BenchmarkDotNet before benchmarking)
[GlobalSetup]
public void Setup()
{
    this.textWithNewLine = MakeString(this.Line, "\n");
    this.textWithLineBreaks = MakeString(this.Line, LineBreaks);
}
```

* **<code>String_SplitNewLine</code>**
    * Split <code>textWithNewLine</code> to `string[]` using `string.Split('\n')`.
* **<code>Span_SplitNewLine</code>**
    1. Split <code>textWithNewLine.AsSpan()</code> to `List<Range>` using `ReadOnlySpan<char>.SplitToRanges('\n')`.
    2. Create `string[]` using `List<Range>`.
* **<code>String_SplitLineBreaks</code>**
    * Split <code>textWithLineBreaks</code> to `string[]` using `string.Split(LineBreaks, StringSplitOptions.None)`.
* **<code>Span_SplitLineBreaks</code>**
    1. Split <code>textWithLineBreaks.AsSpan()</code> to `List<Range>` using `ReadOnlySpan<char>.SplitToRanges(LineBreaks)`.
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
|                 Method |  Line |             Mean |          Error |         StdDev |           Median | Ratio | RatioSD |
|----------------------- |------:|-----------------:|---------------:|---------------:|-----------------:|------:|--------:|
|    String_SplitNewLine |     1 |         95.16 ns |       1.981 ns |       2.432 ns |         95.29 ns |  1.00 |    0.00 |
|      Span_SplitNewLine |     1 |         88.02 ns |       1.823 ns |       3.241 ns |         87.28 ns |  0.93 |    0.04 |
|                        |       |                  |                |                |                  |       |         |
| String_SplitLineBreaks |     1 |        535.48 ns |      10.543 ns |      17.322 ns |        524.74 ns |  1.00 |    0.00 |
|   Span_SplitLineBreaks |     1 |        164.18 ns |       3.299 ns |       5.690 ns |        163.77 ns |  0.31 |    0.01 |
|                        |       |                  |                |                |                  |       |         |
|    String_SplitNewLine |    10 |      1,168.94 ns |      22.938 ns |      36.382 ns |      1,159.28 ns |  1.00 |    0.00 |
|      Span_SplitNewLine |    10 |        641.60 ns |      12.820 ns |      21.063 ns |        635.01 ns |  0.55 |    0.03 |
|                        |       |                  |                |                |                  |       |         |
| String_SplitLineBreaks |    10 |      5,866.13 ns |     116.753 ns |     143.383 ns |      5,796.83 ns |  1.00 |    0.00 |
|   Span_SplitLineBreaks |    10 |      1,291.84 ns |      25.517 ns |      37.402 ns |      1,286.95 ns |  0.22 |    0.01 |
|                        |       |                  |                |                |                  |       |         |
|    String_SplitNewLine |   100 |     15,516.05 ns |     307.663 ns |     592.763 ns |     15,222.39 ns |  1.00 |    0.00 |
|      Span_SplitNewLine |   100 |      7,050.35 ns |     139.022 ns |     203.777 ns |      7,031.02 ns |  0.45 |    0.02 |
|                        |       |                  |                |                |                  |       |         |
| String_SplitLineBreaks |   100 |     80,980.63 ns |   1,602.805 ns |   2,139.698 ns |     80,115.99 ns |  1.00 |    0.00 |
|   Span_SplitLineBreaks |   100 |     15,043.74 ns |     299.639 ns |     618.806 ns |     14,745.88 ns |  0.19 |    0.01 |
|                        |       |                  |                |                |                  |       |         |
|    String_SplitNewLine |  1000 |    531,209.00 ns |  10,428.672 ns |  20,340.324 ns |    525,057.23 ns |  1.00 |    0.00 |
|      Span_SplitNewLine |  1000 |    277,541.18 ns |   5,548.385 ns |  12,859.226 ns |    272,262.67 ns |  0.52 |    0.03 |
|                        |       |                  |                |                |                  |       |         |
| String_SplitLineBreaks |  1000 |  2,568,626.85 ns |  51,089.372 ns |  83,941.303 ns |  2,563,442.97 ns |  1.00 |    0.00 |
|   Span_SplitLineBreaks |  1000 |    466,290.67 ns |   8,680.937 ns |   8,120.153 ns |    468,105.57 ns |  0.18 |    0.01 |
|                        |       |                  |                |                |                  |       |         |
|    String_SplitNewLine | 10000 | 14,966,129.04 ns | 298,504.084 ns | 482,028.454 ns | 14,967,072.66 ns |  1.00 |    0.00 |
|      Span_SplitNewLine | 10000 | 12,513,163.83 ns | 229,669.429 ns | 408,237.177 ns | 12,419,979.69 ns |  0.84 |    0.04 |
|                        |       |                  |                |                |                  |       |         |
| String_SplitLineBreaks | 10000 | 35,700,258.00 ns | 670,069.094 ns | 894,522.994 ns | 35,222,568.75 ns |  1.00 |    0.00 |
|   Span_SplitLineBreaks | 10000 | 14,255,644.85 ns | 277,051.889 ns | 447,387.157 ns | 14,194,607.81 ns |  0.40 |    0.01 |
