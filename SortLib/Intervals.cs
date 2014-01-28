﻿using System;

namespace SortLib
{
    /// <summary>
    /// Implements a class to keep track of intervals that can merge when overlapped.
    /// </summary>
    /// <typeparam name="T">The numeric type to describe bound with.</typeparam>
    public class Intervals<T>
        where T : IComparable<T>, new()
    {
        private readonly RedBlackSameMap<T> _map = new RedBlackSameMap<T>();

        /// <summary>
        /// Adds an interval.
        /// </summary>
        /// <param name="lo">Inclusive lower bound.</param>
        /// <param name="hi">Exclusive upper bound.</param>
        public void Add(T lo, T hi)
        {
            var predNull = _map.PredecessorOrNode(lo);

            if (predNull == null)
            {
                _map.Add(lo, hi);
            }
            else
            {
                var pred = predNull.Value;

                if (pred.Key.CompareTo(lo) == 0)
                {
                    if (pred.Value.CompareTo(hi) < 0)
                    {
                        _map[pred.Key] = hi;
                    }
                }
                else
                {
                    var succNull = _map.Successor(pred.Value);
                    if (succNull == null)
                    {
                        if (pred.Value.CompareTo(lo) > 0)
                        {
                            _map[pred.Key] = hi;
                        }
                        else
                        {
                            _map.Add(lo, hi);
                        }
                    }
                    else
                    {
                        var succ = succNull.Value;
                        if (hi.CompareTo(succ.Key) > 0)
                        {
                            _map.Remove(succ.Key);
                            if (hi.CompareTo(succ.Value) >= 0)
                            {
                                _map.Add(lo, hi);
                            }
                            else
                            {
                                _map.Add(lo, succ.Value);
                            }
                        }
                        else
                        {
                            _map.Add(lo, hi);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests whether a value falls within one of the saved intervals.
        /// </summary>
        /// <param name="x">The number to check.</param>
        /// <returns><c>true</c> when in one of the intervals, <c>false</c>otherwise.</returns>
        public bool Contains(T x)
        {
            var pre = _map.PredecessorOrNode(x);

            if (pre == null)
            {
                return false;
            }

            var found = pre.Value;

            if (x.CompareTo(found.Key) == 0)
            {
                return true;
            }

            return found.Key.CompareTo(x) <= 0 && x.CompareTo(found.Value) < 0;
        }
    }
}
