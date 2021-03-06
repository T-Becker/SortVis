﻿using SortLib;
using SortVis;
using System;
using System.ComponentModel.Composition;

namespace n_log_n
{
    /// <summary>
    /// Implementation of merge sort.
    /// </summary>
    [Export(typeof(ISorter))]
    [ExportMetadata("Name", "Merge sort")]
    public class Mergesort : SorterBase
    {
        /// <summary>
        /// Do the actual sorting work.
        /// </summary>
        protected override void SortIt()
        {
            int n = Numbers.Length;
            var store = new int[n];

            for (int len = 1; len < n; len <<= 1)
            {
                int len2 = len << 1;
                for (int i = 0; i < n - len; i += len2)
                {
                    Abort.ThrowIfCancellationRequested();
                    Merge(store, i, i + len, Math.Min(i + len2, n));
                }
            }
        }

        private void Merge(int[] store, int lo, int mi, int hi)
        {
            int cursor = lo;

            // Copy always the smaller element.
            int l = lo;
            int h = mi;
            while (l < mi && h < hi)
            {
                Abort.ThrowIfCancellationRequested();
                if (CompareInArray(l, h) <= 0)
                {
                    store[cursor++] = Numbers[l++];
                }
                else
                {
                    store[cursor++] = Numbers[h++];
                }
            }

            if (l == h)
            {
                // We wrote the whole left half to the swap space, i.e. our whole range is sorted.
                Writes += l - lo;
                return;
            }

            // Copy the rest.
            Array.Copy(Numbers, l, store, cursor, mi - l);
            cursor += mi - l;
            Array.Copy(Numbers, h, store, cursor, hi - h);
            // This costs that many write to swap space.
            Writes += hi - lo;

            // Write back.
            for (mi = lo; mi < hi; ++mi)
            {
                Abort.ThrowIfCancellationRequested();
                Write(store[mi], mi);
                if (lo == 0)
                {
                    SortedTo = mi + 1;
                }
            }
        }
    }
}
