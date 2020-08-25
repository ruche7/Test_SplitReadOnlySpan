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
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
    public class BenchmarkMethods
    {
        /// <summary>
        /// 各種改行コード("\r\n", "\n", "\r")による分割対象文字列。
        /// </summary>
        private string text = @"";

        /// <summary>
        /// 分割対象文字列の行数。 BenchmarkDotNet によって初期化される。
        /// </summary>
        [Params(1, 10, 100, 1000, 10000)]
        public int Line { get; set; }

        /// <summary>
        /// ベンチマーク前処理を行う。 BenchmarkDotNet によって呼び出される。
        /// </summary>
        [GlobalSetup]
        public void Setup() => this.text = MakeString(this.Line);

        /// <summary>
        /// string.Split(LineBreaks) を行う。
        /// </summary>
        /// <returns>分割文字列配列。</returns>
        [Benchmark(Baseline = true)]
        public string[] String_Split() =>
            this.text.Split(LineBreaks, StringSplitOptions.None);

        /// <summary>
        /// ReadOnlySpan{char}.SplitToRanges(LineBreaks) を用いた文字列分割を行う。
        /// </summary>
        /// <returns>分割文字列配列。</returns>
        [Benchmark]
        public string[] Span_Split()
        {
            var span = this.text.AsSpan();
            var ranges = span.SplitToRanges(LineBreaks);
            var lines = new string[ranges.Count];
            for (int li = 0; li < lines.Length; ++li)
            {
                lines[li] = span[ranges[li]].ToString();
            }
            return lines;
        }

        /// <summary>
        /// 各種改行コード文字列配列。
        /// </summary>
        private static readonly string[] LineBreaks = { "\r\n", "\n", "\r" };

        /// <summary>
        /// 指定した行数で行頭と行末に空白文字を持つ文字列を作成する。
        /// </summary>
        /// <param name="lineCount">行数。</param>
        /// <returns>作成した文字列。</returns>
        private static string MakeString(int lineCount)
        {
            var builder = new StringBuilder();

            for (int li = 0; li < lineCount; ++li)
            {
                if (li != 0)
                {
                    builder.Append(LineBreaks[li % LineBreaks.Length]);
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
