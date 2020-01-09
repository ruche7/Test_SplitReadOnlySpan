using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TestApp
{
    /// <summary>
    /// <see cref="ReadOnlySpan{T}"/> 向けの拡張メソッドを提供する静的クラス。
    /// </summary>
    public static class ReadOnlySpanExtensions
    {
        /// <summary>
        /// スパンをセパレータスパンで分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separator">セパレータスパン。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this ReadOnlySpan<T> source,
            ReadOnlySpan<T> separator,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            SplitToRangesCore(source, separator, -1, removeEmptyEntries);

        /// <summary>
        /// スパンをセパレータ値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separator">セパレータ値。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this ReadOnlySpan<T> source,
            T separator,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            SplitToRanges(
                source,
                MemoryMarshal.CreateReadOnlySpan(ref separator, 1),
                removeEmptyEntries);

        /// <summary>
        /// スパンをセパレータスパンで分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separator">セパレータスパン。</param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this ReadOnlySpan<T> source,
            ReadOnlySpan<T> separator,
            int count,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");
            }

            return SplitToRangesCore(source, separator, count, removeEmptyEntries);
        }

        /// <summary>
        /// スパンをセパレータ値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separator">セパレータ値。</param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this ReadOnlySpan<T> source,
            T separator,
            int count,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            SplitToRanges(
                source,
                MemoryMarshal.CreateReadOnlySpan(ref separator, 1),
                count,
                removeEmptyEntries);

        /// <summary>
        /// スパンをセパレータスパンで分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separator">セパレータスパン。</param>
        /// <param name="count">最大分割数。負数ならば上限なし。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        private static List<Range> SplitToRangesCore<T>(
            ReadOnlySpan<T> source,
            ReadOnlySpan<T> separator,
            int count,
            bool removeEmptyEntries)
            where T : IEquatable<T>
        {
            if (separator.Length == 0)
            {
                throw new ArgumentException(@"The span is empty.", nameof(separator));
            }

            var ranges = new List<Range>();

            if (count != 0)
            {
                int index = 0;

                for (int c = 1, pos = 0; count < 0 || c < count;)
                {
                    pos = source[index..].IndexOf(separator);
                    if (pos < 0)
                    {
                        break;
                    }
                    if (pos > 0 || !removeEmptyEntries)
                    {
                        ++c;
                        ranges.Add(index..(index + pos));
                    }
                    index += pos + separator.Length;
                }

                if (!removeEmptyEntries || index < source.Length)
                {
                    ranges.Add(index..);
                }
            }

            return ranges;
        }
    }
}
