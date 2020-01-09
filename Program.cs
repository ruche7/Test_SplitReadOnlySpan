using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TestApp
{
    /// <summary>
    /// ReadOnlySpan{T}.SplitToRanges ベンチマークアプリクラス。
    /// </summary>
    internal static class Program
    {
        #region ベンチマーク対象メソッド群

        /// <summary>
        /// 文字列の各行について行頭と行末の空白文字を取り除く。
        /// </summary>
        /// <param name="text">文字列。</param>
        /// <returns>加工した文字列。</returns>
        /// <remarks>
        /// string.Split + string.Join
        /// </remarks>
        private static string TrimLines_StringSplitJoin(string text) =>
            string.Join('\n', text.Split('\n').Select(s => s.Trim()));

        /// <summary>
        /// 文字列の各行について行頭と行末の空白文字を取り除く。
        /// </summary>
        /// <param name="text">文字列。</param>
        /// <returns>加工した文字列。</returns>
        /// <remarks>
        /// string.Split + StringBuilder
        /// </remarks>
        private static string TrimLines_StringSplitBuild(string text)
        {
            var result = new StringBuilder(text.Length);

            var lines = text.Split('\n');
            for (int i = 0; i < lines.Length;)
            {
                result.Append(lines[i].AsSpan().Trim());
                if (++i < lines.Length)
                {
                    result.Append('\n');
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 文字列の各行について行頭と行末の空白文字を取り除く。
        /// </summary>
        /// <param name="text">文字列。</param>
        /// <returns>加工した文字列。</returns>
        /// <remarks>
        /// ReadOnlySpan{T}.SplitToRanges + string.Join
        /// </remarks>
        private static string TrimLines_SpanRangesJoin(string text)
        {
            var result = new StringBuilder(text.Length);

            var span = text.AsSpan();
            var ranges = span.SplitToRanges('\n');
            var lines = new string[ranges.Count];

            for (int i = 0; i < ranges.Count; ++i)
            {
                lines[i] = span[ranges[i]].Trim().ToString();
            }

            return string.Join('\n', lines);
        }

        /// <summary>
        /// 文字列の各行について行頭と行末の空白文字を取り除く。
        /// </summary>
        /// <param name="text">文字列。</param>
        /// <returns>加工した文字列。</returns>
        /// <remarks>
        /// ReadOnlySpan{T}.SplitToRanges + StringBuilder
        /// </remarks>
        private static string TrimLines_SpanRangesBuild(string text)
        {
            var result = new StringBuilder(text.Length);

            var span = text.AsSpan();
            var ranges = span.SplitToRanges('\n');

            for (int i = 0; i < ranges.Count;)
            {
                result.Append(span[ranges[i]].Trim());
                if (++i < ranges.Count)
                {
                    result.Append('\n');
                }
            }

            return result.ToString();
        }

        #endregion

        #region ベンチマーク処理

        /// <summary>
        /// ベンチマークを実施する。
        /// </summary>
        /// <param name="lineCount">行数。</param>
        /// <param name="loopCount">ループ回数。</param>
        private static void DoBenchmark(int lineCount, int loopCount)
        {
            var text = MakeString(lineCount);

#if DEBUG
            // 結果が等しいか確認しておく
            {
                var resultSJ = TrimLines_StringSplitJoin(text);
                var resultSB = TrimLines_StringSplitBuild(text);
                var resultRJ = TrimLines_SpanRangesJoin(text);
                var resultRB = TrimLines_SpanRangesBuild(text);
                Debug.Assert(resultSJ == resultSB);
                Debug.Assert(resultSJ == resultRJ);
                Debug.Assert(resultSJ == resultRB);
            }
#endif // DEBUG

            DoMeasure(
                nameof(TrimLines_StringSplitJoin),
                TrimLines_StringSplitJoin,
                text,
                lineCount,
                loopCount);
            DoMeasure(
                nameof(TrimLines_StringSplitBuild),
                TrimLines_StringSplitBuild,
                text,
                lineCount,
                loopCount);
            DoMeasure(
                nameof(TrimLines_SpanRangesJoin),
                TrimLines_SpanRangesJoin,
                text,
                lineCount,
                loopCount);
            DoMeasure(
                nameof(TrimLines_SpanRangesBuild),
                TrimLines_SpanRangesBuild,
                text,
                lineCount,
                loopCount);
        }

        /// <summary>
        /// 指定した行数で行頭と行末に空白文字を持つ文字列を作成する。
        /// </summary>
        /// <param name="lineCount">行数。</param>
        /// <returns>作成した文字列。</returns>
        private static string MakeString(int lineCount) =>
            string.Join(
                '\n',
                Enumerable.Range(0, lineCount)
                    .Select(i =>
                        new string(' ', i % 10) +
                        new string('a', (i + 100) % 1000 + 1) +
                        new string('\t', (i + 5) % 26)));

        /// <summary>
        /// メソッド呼び出しの時間計測を開始する。
        /// </summary>
        /// <param name="funcName">メソッド名。コンソール出力用。</param>
        /// <param name="func">メソッド。</param>
        /// <param name="text">メソッドの引数に渡す文字列。</param>
        /// <param name="lineCount">行数。コンソール出力用。</param>
        /// <param name="loopCount">ループ回数。</param>
        private static void DoMeasure(
            string funcName,
            Func<string, string> func,
            string text,
            int lineCount,
            int loopCount)
        {
            Console.WriteLine($@"{funcName,-28}: line = {lineCount}, loop = {loopCount}");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < loopCount; ++i)
            {
                func(text);
            }
            var elapsed = sw.Elapsed.TotalMilliseconds;

            Console.WriteLine($@"  -> {elapsed:F3} ms");
        }

        #endregion

        /// <summary>
        /// メインエントリポイント。
        /// </summary>
        private static int Main()
        {
            const int baseLoopCount = 2000000;

            DoBenchmark(1, baseLoopCount);
            Console.WriteLine();
            DoBenchmark(10, baseLoopCount / 10);
            Console.WriteLine();
            DoBenchmark(100, baseLoopCount / 100);
            Console.WriteLine();
            DoBenchmark(1000, baseLoopCount / 2000);

            return 0;
        }
    }
}
