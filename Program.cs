using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace TestApp
{
    /// <summary>
    /// ベンチマークメソッド群を提供するクラス。
    /// </summary>
    public class BenchmarkMethods
    {
        #region ベンチマークメソッド群

        [Benchmark]
        public string[] String_SplitNewLine_Line1() => String_SplitNewLine(Line1);
        [Benchmark]
        public string[] Span_SplitNewLine_Line1() => Span_SplitNewLine(Line1);

        [Benchmark]
        public string[] String_SplitNewLine_Line10() => String_SplitNewLine(Line10);
        [Benchmark]
        public string[] Span_SplitNewLine_Line10() => Span_SplitNewLine(Line10);

        [Benchmark]
        public string[] String_SplitNewLine_Line100() => String_SplitNewLine(Line100);
        [Benchmark]
        public string[] Span_SplitNewLine_Line100() => Span_SplitNewLine(Line100);

        [Benchmark]
        public string[] String_SplitNewLine_Line1000() => String_SplitNewLine(Line1000);
        [Benchmark]
        public string[] Span_SplitNewLine_Line1000() => Span_SplitNewLine(Line1000);

        [Benchmark]
        public string[] String_SplitNewLine_Line10000() => String_SplitNewLine(Line10000);
        [Benchmark]
        public string[] Span_SplitNewLine_Line10000() => Span_SplitNewLine(Line10000);

        [Benchmark]
        public string[] String_SplitLineBreaks_Line1() => String_SplitLineBreaks(Line1);
        [Benchmark]
        public string[] Span_SplitLineBreaks_Line1() => Span_SplitLineBreaks(Line1);

        [Benchmark]
        public string[] String_SplitLineBreaks_Line10() => String_SplitLineBreaks(Line10);
        [Benchmark]
        public string[] Span_SplitLineBreaks_Line10() => Span_SplitLineBreaks(Line10);

        [Benchmark]
        public string[] String_SplitLineBreaks_Line100() => String_SplitLineBreaks(Line100);
        [Benchmark]
        public string[] Span_SplitLineBreaks_Line100() => Span_SplitLineBreaks(Line100);

        [Benchmark]
        public string[] String_SplitLineBreaks_Line1000() => String_SplitLineBreaks(Line1000);
        [Benchmark]
        public string[] Span_SplitLineBreaks_Line1000() => Span_SplitLineBreaks(Line1000);

        [Benchmark]
        public string[] String_SplitLineBreaks_Line10000() => String_SplitLineBreaks(Line10000);
        [Benchmark]
        public string[] Span_SplitLineBreaks_Line10000() => Span_SplitLineBreaks(Line10000);

        #endregion

        /// <summary>
        /// string.Split('\n') を行う。
        /// </summary>
        /// <param name="text">対象文字列。</param>
        /// <returns>分割文字列配列。</returns>
        private static string[] String_SplitNewLine(string text) => text.Split('\n');

        /// <summary>
        /// ReadOnlySpan{char}.SplitToRanges('\n') を用いた文字列分割を行う。
        /// </summary>
        /// <param name="text">対象文字列。</param>
        /// <returns>分割文字列配列。</returns>
        private static string[] Span_SplitNewLine(string text)
        {
            var span = text.AsSpan();
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
        /// <param name="text">対象文字列。</param>
        /// <returns>分割文字列配列。</returns>
        private static string[] String_SplitLineBreaks(string text) =>
            text.Split(LineBreaks, StringSplitOptions.None);

        /// <summary>
        /// ReadOnlySpan{char}.SplitToRanges(LineBreaks) を用いた文字列分割を行う。
        /// </summary>
        /// <param name="text">対象文字列。</param>
        /// <returns>分割文字列配列。</returns>
        private static string[] Span_SplitLineBreaks(string text)
        {
            var span = text.AsSpan();
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

        #region ソース文字列群

        private static readonly string Line1 = MakeString(1);
        private static readonly string Line10 = MakeString(10);
        private static readonly string Line100 = MakeString(100);
        private static readonly string Line1000 = MakeString(1000);
        private static readonly string Line10000 = MakeString(10000);

        #endregion

        /// <summary>
        /// 指定した行数で行頭と行末に空白文字を持つ文字列を作成する。
        /// </summary>
        /// <param name="lineCount">行数。</param>
        /// <returns>作成した文字列。</returns>
        /// <remarks>
        /// 改行は "\r\n", "\n", "\r" の混合となる。
        /// </remarks>
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
            Validate();

            // ベンチマーク実施
            BenchmarkRunner.Run<BenchmarkMethods>();

            return 0;
        }

        /// <summary>
        /// Debug ビルドにおいてベンチマークメソッド群の正当性を評価する。
        /// </summary>
        [Conditional(@"DEBUG")]
        private static void Validate()
        {
            var methods = new BenchmarkMethods();

            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitNewLine_Line1(),
                    methods.Span_SplitNewLine_Line1()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitNewLine_Line10(),
                    methods.Span_SplitNewLine_Line10()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitNewLine_Line100(),
                    methods.Span_SplitNewLine_Line100()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitNewLine_Line1000(),
                    methods.Span_SplitNewLine_Line1000()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitNewLine_Line10000(),
                    methods.Span_SplitNewLine_Line10000()));

            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitLineBreaks_Line1(),
                    methods.Span_SplitLineBreaks_Line1()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitLineBreaks_Line10(),
                    methods.Span_SplitLineBreaks_Line10()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitLineBreaks_Line100(),
                    methods.Span_SplitLineBreaks_Line100()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitLineBreaks_Line1000(),
                    methods.Span_SplitLineBreaks_Line1000()));
            Debug.Assert(
                Enumerable.SequenceEqual(
                    methods.String_SplitLineBreaks_Line10000(),
                    methods.Span_SplitLineBreaks_Line10000()));
        }
    }
}
