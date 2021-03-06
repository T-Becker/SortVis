﻿using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortVis
{
    public abstract partial class SorterBase : ISorter
    {
        private class TenE0Comparer : Comparer<int>
        {
            public override int Compare(int x, int y)
            {
                x %= 10;
                y %= 10;

                return Comparer<int>.Default.Compare(x, y);
            }
        }

        private class TenE1Comparer : TenE0Comparer
        {
            public override int Compare(int x, int y)
            {
                x = (x / 10) % 10;
                y = (y / 10) % 10;

                return Comparer<int>.Default.Compare(x, y);
            }
        }

        private readonly TenE0Comparer _ten0 = new TenE0Comparer();
        private readonly TenE1Comparer _ten1 = new TenE1Comparer();

        /// <summary>
        /// Sort all permutations of tuples by first and last item and see if equal
        /// elements keep their order.
        /// </summary>
        /// <returns><c>true</c> if stable, <c>false</c> otherwise.</returns>
        protected virtual bool CheckIfStable()
        {
            if (ConsideredBig > 8)
            {
                var soe = new StackOverflowException("The length of the stable sort check array will become too long, "
                + "as test run time increases with the factorial.");
                throw new NotImplementedException("Stable check will not work here. Devise your own test or decrease "
                + "what is considered a length worthy of running the full-blown algorithm.", soe);
            }

            try
            {
                var three = Task.Run(() => { return CheckIfStable(3); });
                var four = Task.Run(() => { return CheckIfStable(4); });
                return three.Result && four.Result;
            }
            catch (AggregateException ae)
            {
                var worthy = (
                    from ie in ae.InnerExceptions
                    where ie is NotSortedException
                    select ie).FirstOrDefault();

                if (worthy != null)
                {
                    // Of several exceptions I'd like to see this first.
                    throw worthy;
                }
                throw ae.InnerException;
            }
        }

        private bool CheckIfStable(int maxNum)
        {
            var nums = DoubleNums(maxNum).Take(Math.Max(7, ConsideredBig)).ToArray();

            // Do this in another instance, otherwise we get nasty race conditions.
            var thisSorter = (SorterBase)Activator.CreateInstance(GetType());
            thisSorter.Numbers = nums;
            return thisSorter.RecursiveCheckIfStable(0);
        }

        private IEnumerable<int> DoubleNums(int maxNum)
        {
            if (maxNum < 2 || maxNum > 9)
            {
                throw new ArgumentOutOfRangeException("maxNum", "Must be larger than 1 and less than 10.");
            }
            while (true)
            {
                foreach (int ten in Enumerable.Range(1, maxNum))
                {
                    foreach (int one in Enumerable.Range(1, maxNum))
                    {
                        yield return ten * 10 + one;
                    }
                }
            }
        }

        private bool RecursiveCheckIfStable(int i)
        {
            int n = Numbers.Length;
            if (i >= n - 1)
            {
                return CheckIfStable(Numbers);
            }

            if (!RecursiveCheckIfStable(i + 1))
            {
                return false;
            }
            for (int j = i + 1; j < n; ++j)
            {
                int temp = Numbers[i];
                Numbers[i] = Numbers[j];
                Numbers[j] = temp;
                if (!RecursiveCheckIfStable(i + 1))
                {
                    return false;
                }
                Numbers[j] = Numbers[i];
                Numbers[i] = temp;
            }

            return CheckIfStable(Numbers);
        }

        private bool CheckIfStable(int[] nums)
        {
            return CheckIfStable(nums, _ten0, _ten1) && CheckIfStable(nums, _ten1, _ten0);
        }

        private bool CheckIfStable(int[] nums, Comparer<int> first, Comparer<int> second)
        {
            Comparer = first;
            Numbers = nums;
            Sort();
            Comparer = second;
            Sort();

            for (int i = 1; i < nums.Length; ++i)
            {
                int left = Numbers[i - 1];
                int right = Numbers[i];
                if (second.Compare(left, right) > 0)
                {
                    throw new NotSortedException(Name, i);
                }
                if (second.Compare(left, right) == 0 && first.Compare(left, right) > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
