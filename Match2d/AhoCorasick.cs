﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace task_Match2d
{
    class Vertex<TChar> where TChar : IComparable<TChar>
    {
        public Dictionary<TChar, Vertex<TChar>> next;

        public bool mark;
        public int len;

        public Vertex<TChar> link;
        public Vertex<TChar> report;

        public Vertex<TChar> parent;
        public TChar parentChar;
        public int strId;
    }

    class AhoCorasick<TChar> where TChar : IComparable<TChar>
    {

        public void ReportOccurrencesIds(IEnumerable<TChar> input, Action<int, int> reportAction)
        {
            var node = root;
            var i = 0;
            foreach (TChar c in input)
            {
                i++;
                while (node != root && !node.next.ContainsKey(c))
                {
                    node = node.link;
                }

                if (!node.next.TryGetValue(c, out node))
                    node = root;


                for (var u = node; u != root; u = u.report)
                {
                    if (u.mark)
                        reportAction(i - u.len, u.strId);
                }
            }

        }

        public AhoCorasick(IEnumerable<IList<TChar>> strings, out List<int> stringIds)
        {
            stringIds = new List<int>();
            foreach (var item in strings)
            {
                AddString(item, out var index);
                stringIds.Add(index);
            }

            BuildLinks();
        }


        private Vertex<TChar> root = new Vertex<TChar> {next = new Dictionary<TChar, Vertex<TChar>>()};
        private int nextIndex = 1;

        private void AddString(IList<TChar> s, out int strId)
        {
            var v = root;
            foreach (var c in s)
            {
                if (!v.next.ContainsKey(c))
                {
                    var w = new Vertex<TChar>
                    {
                        next = new Dictionary<TChar, Vertex<TChar>>(),
                        len = v.len + 1,
                        parent = v,
                        parentChar = c
                    };
                    v.next.Add(c, w);
                }

                v = v.next[c];
            }

            if (v.mark)
            {
                strId = v.strId;
            }
            else
            {
                v.mark = true;
                v.strId = nextIndex;
                strId = v.strId;
                nextIndex++;
            }
        }

        private void BuildLinks()
        {
            root.link = root;
            root.report = root;

            var queue = new Queue<Vertex<TChar>>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                // Посетим детей
                foreach (var children in node.next)
                    queue.Enqueue(children.Value);

                if (node == root)
                    root.link = root;

                var parent = node.parent;
                while (parent != root && node.link == null)
                {
                    if (parent.link.next.ContainsKey(node.parentChar))
                        node.link = parent.link.next[node.parentChar];
                    parent = parent.link;
                }

                if (node.link == null)
                    node.link = root;

                node.report = node.link;
                if (!node.report.mark)
                    node.report = node.link.report;
            }
        }
    }
}