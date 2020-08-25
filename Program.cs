using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace TestApp
{
    /// <summary>
    /// ベンチマークメソッド群を提供するクラス。
    /// </summary>
    [GroupBenchmarksBy(
        BenchmarkLogicalGroupRule.ByCategory,
        BenchmarkLogicalGroupRule.ByParams)]
    public class BenchmarkMethods
    {
        /// <summary>
        /// '\n' による分割対象文字列。
        /// </summary>
        private string textWithNewLine = @"";

        /// <summary>
        /// 各種改行コード("\r\n", "\n", "\r")による分割対象文字列。
        /// </summary>
        private string textWithLineBreaks = @"";

        /// <summary>
        /// 分割対象文字列の行数。 BenchmarkDotNet によって初期化される。
        /// </summary>
        [Params(1, 10, 100, 1000, 10000)]
        public int Line { get; set; }

        /// <summary>
        /// ベンチマーク前処理を行う。
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            this.textWithNewLine = MakeString(this.Line, "\n");
            this.textWithLineBreaks = MakeString(this.Line, LineBreaks);
        }

        /// <summary>
        /// string.Split('\n') を行う。
        /// </summary>
        /// <returns>分割文字列配列。</returns>
        [Benchmark(Baseline = true), BenchmarkCategory(@"NewLine")]
        public string[] String_SplitNewLine() => this.textWithNewLine.Split('\n');

        /// <summary>
        /// ReadOnlySpan{char}.SplitToRanges('\n') を用いた文字列分割を行う。
        /// </summary>
        /// <returns>分割文字列配列。</returns>
        [Benchmark, BenchmarkCategory(@"NewLine")]
        public string[] Span_SplitNewLine()
        {
            var span = this.textWithNewLine.AsSpan();
            var ranges = span.SplitToRanges('\n');
            var lines = new string[ranges.Count];
            for (int li = 0; li < lines.Length; ++li)
            {
                lines[li] = span[ranges[li]].ToString();
            }
            return lines;
        }

        /// <summary>
        /// string.Split(LineBreaks) を行う。
        /// </summary>
        /// <returns>分割文字列配列。</returns>
        [Benchmark(Baseline = true), BenchmarkCategory(@"LineBreaks")]
        public string[] String_SplitLineBreaks() =>
            this.textWithLineBreaks.Split(LineBreaks, StringSplitOptions.None);

        /// <summary>
        /// ReadOnlySpan{char}.SplitToRanges(LineBreaks) を用いた文字列分割を行う。
        /// </summary>
        /// <returns>分割文字列配列。</returns>
        [Benchmark, BenchmarkCategory(@"LineBreaks")]
        public string[] Span_SplitLineBreaks()
        {
            var span = this.textWithLineBreaks.AsSpan();
            var ranges = span.SplitToRanges(LineBreaks);
            var lines = new string[ranges.Count];
            for (int li = 0; li < lines.Length; ++li)
            {
                lines[li] = span[ranges[li]].ToString();
            }
            return lines;
        }

        /// <summary>
        /// 改行文字列配列。
        /// </summary>
        private static readonly string[] LineBreaks = { "\r\n", "\n", "\r" };

        /// <summary>
        /// 指定した行数で行頭と行末に空白文字を持つ文字列を作成する。
        /// </summary>
        /// <param name="lineCount">行数。</param>
        /// <param name="splitters">行分割文字列配列。指定無しならば "\n" が使われる。</param>
        /// <returns>作成した文字列。</returns>
        private static string MakeString(int lineCount, params string[] splitters)
        {
            var sp = (splitters.Length == 0) ? new[] { "\n" } : splitters;

            var builder = new StringBuilder();

            for (int li = 0; li < lineCount; ++li)
            {
                if (li != 0)
                {
                    builder.Append(sp[li % sp.Length]);
                }
                builder.Append(
                    new string(' ', li % 10) +
                    new string('a', (li + 100) % 1000 + 1) +
                    new string('\t', (li + 5) % 26));
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// ReadOnlySpan{T}.SplitToRanges ベンチマークアプリクラス。
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// メインエントリポイント。
        /// </summary>
        private static int Main()
        {
            // ベンチマーク実施
            BenchmarkRunner.Run<BenchmarkMethods>();

            return 0;
        }
    }
}
