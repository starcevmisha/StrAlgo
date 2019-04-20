using System;
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
        public delegate void ReportAction(int endPosition, int id);

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

            Console.WriteLine("OK");
        }

        public AhoCorasick(IEnumerable<IList<TChar>> strings, out List<int> stringIds)
        {
            var set = new HashSet<string>();
            var index = 1;
            stringIds = new List<int>();
            foreach (var item in strings)
            {
                if(!set.Contains(String.Concat(item)))
                {
                    AddString(item, index);
                    set.Add(String.Concat(item));
                    index++;
                }
            }

            BuildLinks();


            //stringIds будет содержать id для строк strings, равным строкам равные id
//            throw new NotImplementedException();
        }


        private Vertex<TChar> root = new Vertex<TChar> {next = new Dictionary<TChar, Vertex<TChar>>()};

        public IEnumerable<int> Find(IEnumerable<TChar> text)
        {
            var res = new List<int>();
            var node = root;
            var i = 0;
            foreach (TChar c in text)
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
                        res.Add(i - u.len);
                }
            }

            Console.WriteLine("OK");
            return res;
        }

        private void AddString(IList<TChar> s, int strId)
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
            v.mark = true;
            v.strId = strId;
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