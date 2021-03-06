﻿using SortLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unittest
{
    internal class RedBlackSetTester<T> : RedBlackSet<T> where T : IComparable<T>
    {
        public RedBlackSetTester()
        {
        }

        public RedBlackSetTester(IEnumerable<T> list)
            : base(list)
        {
        }

        public LinkedList<T> Cache
        {
            get
            {
                return FlatList;
            }
        }

        public int MaxDepth()
        {
            return MaxDepth(_root);
        }

        public bool AreNodesOrdered()
        {
            return AreNodesOrdered(_root);
        }

        private bool AreNodesOrdered(RedBlackTree<T>.Node node)
        {
            if (node == null)
            {
                return true;
            }

            if (node.Left != null && Comparer.Compare(node.Left.Item, node.Item) > 0)
            {
                return false;
            }
            if (node.Right != null && Comparer.Compare(node.Right.Item, node.Item) < 0)
            {
                return false;
            }

            return true;
        }

        private static int MaxDepth(RedBlackTree<T>.Node node)
        {
            return node == null ? 0 : 1 + Math.Max(MaxDepth(node.Left), MaxDepth(node.Right));
        }
    }

    internal class RedBlackMapTester<K, V> : RedBlackMap<K, V> where K : IComparable<K> where V : new()
    {
        public RedBlackMapTester(IEnumerable<K> keys, IEnumerable<V> values)
            : base(keys, values)
        {
        }

        public RedBlackMapTester(IEnumerable<KeyValuePair<K, V>> pairs)
            : base(pairs)
        {
        }

        public RedBlackMapTester(IEnumerable<Tuple<K, V>> tuples)
            : base(tuples)
        {
        }
    }
}
