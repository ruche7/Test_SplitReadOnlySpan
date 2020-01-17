using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace TestApp
{
    /// <summary>
    /// <see cref="Span{T}"/>,
    /// <see cref="ReadOnlySpan{T}"/> 向けの拡張メソッドを提供する静的クラス。
    /// </summary>
    public static class SpanExtensions
    {
        #region FindIndex, FindLastIndex

        /// <summary>
        /// 条件に一致する最初の要素を検索し、そのインデックスを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="match">条件を定義する <see cref="Predicate{T}"/> 。</param>
        /// <returns>インデックス。見つからない場合は -1 。</returns>
        public static int FindIndex<T>(this ReadOnlySpan<T> source, Predicate<T> match)
        {
            for (int i = 0; i < source.Length; ++i)
            {
                if (match(source[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 条件に一致する最初の要素を検索し、そのインデックスを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="match">条件を定義する <see cref="Predicate{T}"/> 。</param>
        /// <returns>インデックス。見つからない場合は -1 。</returns>
        public static int FindIndex<T>(this Span<T> source, Predicate<T> match) =>
            FindIndex((ReadOnlySpan<T>)source, match);

        /// <summary>
        /// 条件に一致する最後の要素を検索し、そのインデックスを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="match">条件を定義する <see cref="Predicate{T}"/> 。</param>
        /// <returns>インデックス。見つからない場合は -1 。</returns>
        public static int FindLastIndex<T>(this ReadOnlySpan<T> source, Predicate<T> match)
        {
            for (int i = source.Length; i > 0;)
            {
                if (match(source[--i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 条件に一致する最後の要素を検索し、そのインデックスを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="match">条件を定義する <see cref="Predicate{T}"/> 。</param>
        /// <returns>インデックス。見つからない場合は -1 。</returns>
        public static int FindLastIndex<T>(this Span<T> source, Predicate<T> match) =>
            FindLastIndex((ReadOnlySpan<T>)source, match);

        #endregion

        #region SplitToRanges<T>

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
            this Span<T> source,
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
            SplitToRangesCore(
                source,
                MemoryMarshal.CreateReadOnlySpan(ref separator, 1),
                -1,
                removeEmptyEntries);

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
            this Span<T> source,
            T separator,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            SplitToRangesCore(
                source,
                MemoryMarshal.CreateReadOnlySpan(ref separator, 1),
                -1,
                removeEmptyEntries);

        /// <summary>
        /// スパンを条件に一致する値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separatorMatch">
        /// セパレータ値の条件を定義する <see cref="Predicate{T}"/> 。
        /// </param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this ReadOnlySpan<T> source,
            Predicate<T> separatorMatch,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            SplitToRangesCore(source, separatorMatch, -1, removeEmptyEntries);

        /// <summary>
        /// スパンを条件に一致する値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separatorMatch">
        /// セパレータ値の条件を定義する <see cref="Predicate{T}"/> 。
        /// </param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this Span<T> source,
            Predicate<T> separatorMatch,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            SplitToRangesCore(source, separatorMatch, -1, removeEmptyEntries);

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
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separator, count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

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
            this Span<T> source,
            ReadOnlySpan<T> separator,
            int count,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separator, count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

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
            (count >= 0) ?
                SplitToRangesCore(
                    source,
                    MemoryMarshal.CreateReadOnlySpan(ref separator, 1),
                    count,
                    removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

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
            this Span<T> source,
            T separator,
            int count,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            (count >= 0) ?
                SplitToRangesCore(
                    source,
                    MemoryMarshal.CreateReadOnlySpan(ref separator, 1),
                    count,
                    removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        /// <summary>
        /// スパンを条件に一致する値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separatorMatch">
        /// セパレータ値の条件を定義する <see cref="Predicate{T}"/> 。
        /// </param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this ReadOnlySpan<T> source,
            Predicate<T> separatorMatch,
            int count,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separatorMatch, count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        /// <summary>
        /// スパンを条件に一致する値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separatorMatch">
        /// セパレータ値の条件を定義する <see cref="Predicate{T}"/> 。
        /// </param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges<T>(
            this Span<T> source,
            Predicate<T> separatorMatch,
            int count,
            bool removeEmptyEntries = false)
            where T : IEquatable<T>
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separatorMatch, count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        #endregion

        #region SplitToRanges<char>

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列配列。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this ReadOnlySpan<char> source,
            string[] separators,
            bool removeEmptyEntries = false)
            =>
            SplitToRangesCore(source, separators, -1, removeEmptyEntries);

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列配列。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this Span<char> source,
            string[] separators,
            bool removeEmptyEntries = false)
            =>
            SplitToRangesCore(source, separators, -1, removeEmptyEntries);

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列列挙。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this ReadOnlySpan<char> source,
            IEnumerable<string> separators,
            bool removeEmptyEntries = false)
            =>
            SplitToRangesCore(source, separators.ToArray(), -1, removeEmptyEntries);

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列列挙。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this Span<char> source,
            IEnumerable<string> separators,
            bool removeEmptyEntries = false)
            =>
            SplitToRangesCore(source, separators.ToArray(), -1, removeEmptyEntries);

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列配列。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this ReadOnlySpan<char> source,
            string[] separators,
            int count,
            bool removeEmptyEntries = false)
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separators, count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列配列。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this Span<char> source,
            string[] separators,
            int count,
            bool removeEmptyEntries = false)
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separators, count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列列挙。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this ReadOnlySpan<char> source,
            IEnumerable<string> separators,
            int count,
            bool removeEmptyEntries = false)
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separators.ToArray(), count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列列挙。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="count">最大分割数。 0 以上でなければならない。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        public static List<Range> SplitToRanges(
            this Span<char> source,
            IEnumerable<string> separators,
            int count,
            bool removeEmptyEntries = false)
            =>
            (count >= 0) ?
                SplitToRangesCore(source, separators.ToArray(), count, removeEmptyEntries) :
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    count,
                    @"`count` is less than 0.");

        #endregion

        #region 内部実装

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
                throw new ArgumentException(@"`separator` is empty.", nameof(separator));
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

        /// <summary>
        /// スパンを条件に一致する値で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <typeparam name="T">スパン要素型。</typeparam>
        /// <param name="source">対象スパン。</param>
        /// <param name="separatorMatch">
        /// セパレータ値の条件を定義する <see cref="Predicate{T}"/> 。
        /// </param>
        /// <param name="count">最大分割数。負数ならば上限なし。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        private static List<Range> SplitToRangesCore<T>(
            ReadOnlySpan<T> source,
            Predicate<T> separatorMatch,
            int count,
            bool removeEmptyEntries)
            where T : IEquatable<T>
        {
            var ranges = new List<Range>();

            if (count != 0)
            {
                int index = 0;

                for (int c = 1, pos = 0; count < 0 || c < count;)
                {
                    pos = source[index..].FindIndex(separatorMatch);
                    if (pos < 0)
                    {
                        break;
                    }
                    if (pos > 0 || !removeEmptyEntries)
                    {
                        ++c;
                        ranges.Add(index..(index + pos));
                    }
                    index += pos + 1;
                }

                if (!removeEmptyEntries || index < source.Length)
                {
                    ranges.Add(index..);
                }
            }

            return ranges;
        }

        /// <summary>
        /// 文字列をセパレータ文字列群で分割した時の各分割範囲を表す範囲リストを返す。
        /// </summary>
        /// <param name="source">対象文字列。</param>
        /// <param name="separators">
        /// セパレータ文字列配列。
        /// 要素数が 1 以上かつ各要素の文字列長が 1 以上でなければならない。
        /// </param>
        /// <param name="count">最大分割数。負数ならば上限なし。</param>
        /// <param name="removeEmptyEntries">
        /// 長さが 0 の分割範囲を除外するならば true 。
        /// </param>
        /// <returns>範囲リスト。</returns>
        private static List<Range> SplitToRangesCore(
            ReadOnlySpan<char> source,
            string[] separators,
            int count,
            bool removeEmptyEntries)
        {
            if (separators.Length == 0)
            {
                throw new ArgumentException(@"`separators` is empty.", nameof(separators));
            }
            if (separators.Any(s => s.Length == 0))
            {
                throw new ArgumentException(
                    @"Some elements of `separators` is empty.",
                    nameof(separators));
            }

            var ranges = new List<Range>();

            if (count != 0)
            {
                int index = 0;

                for (int c = 1, pos = 0, sepLen = 0; count < 0 || c < count;)
                {
                    // 最も手前に現れるセパレータを検索
                    pos = int.MaxValue;
                    var src = source[index..];
                    foreach (var s in separators)
                    {
                        var p = src.IndexOf(s);
                        if (p >= 0 && p < pos)
                        {
                            pos = p;
                            sepLen = s.Length;
                            if (p == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (pos == int.MaxValue)
                    {
                        break;
                    }
                    if (pos > 0 || !removeEmptyEntries)
                    {
                        ++c;
                        ranges.Add(index..(index + pos));
                    }
                    index += pos + sepLen;
                }

                if (!removeEmptyEntries || index < source.Length)
                {
                    ranges.Add(index..);
                }
            }

            return ranges;
        }

        #endregion
    }
}
