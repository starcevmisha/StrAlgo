using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Interfaces;

namespace DocsReport
{
    class DocumentOccurrences
    {
        public string DocumentKey { get; set; }
        public List<int> OccurrencePositions { get; set; }
    }

    class DocumentInfo
    {
        public string name { get; set; }
        public int start { get; set; }
    }

    class DocumentIndex
    {
        private SuffixTree<int> tree;
        private List<int> input;
        private List<DocumentInfo> documentsInfo;

        public DocumentIndex(IReadOnlyList<KeyValuePair<string, byte[]>> documents)
        {
            documentsInfo = new List<DocumentInfo>();
            var argstart = 0;
            foreach (var document in documents)
            {
                documentsInfo.Add(new DocumentInfo() {name = document.Key, start = argstart});
                argstart += document.Value.Length + 1;
            }

            var intCast = documents.Select(d => d.Value.Select(x => (int) x)).ToList();
            var inputAlphabet = Enumerable.Range(0, 256 + intCast.Count);
            input = intCast.SelectMany((d, i) => d.Concat(new[] {256 + i})).ToList();
            tree = new SuffixTree<int>(input, inputAlphabet, 255, documents.Count);
        }

        public List<DocumentOccurrences> ReportOccurrences(byte[] pattern)
        {
            var result = new List<DocumentOccurrences>();
            var node = tree.Root;

            int viewed = 0; //Будем хранить количество символов, которе прочитали на последнем ребре
            //Идём по ребрам
            for (int i = 0; i < pattern.Length; i += node.Len)
            {
                if (!node.Next.TryGetValue(pattern[i], out node))
                    return result;
                
                viewed = 0;
                for (int j = 0; j < node.Len && i + j < pattern.Length && pattern[i+j]<256; j++)
                {
                    viewed++;
                    if (pattern[i + j] != input[node.Pos - node.Len + j])
                        return result;
                }

            }

            var occurrences = new List<(int, int)>();//(docId, endOfPattern)
            TraverseChild(node, node.Len - viewed, occurrences);
            

            var occurrencesByDoc = new SortedDictionary<int, List<int>>();
            foreach (var (docId, occurrence) in occurrences)
            {
                if(!occurrencesByDoc.ContainsKey(docId))
                    occurrencesByDoc[docId] = new List<int>();
                occurrencesByDoc[docId].Add(occurrence-pattern.Length);
            }


            foreach (var kvp in occurrencesByDoc)
            {
                result.Add(new DocumentOccurrences{DocumentKey = documentsInfo[kvp.Key].name, OccurrencePositions = kvp.Value});
            }
            
            
            return result;
        }

        private void TraverseChild(SuffixTree<int>.Node node, int accumulator, List<(int, int)> occurience)
		{
            if (node.Mark)
                occurience.Add((node.DocNumber, node.Pos - documentsInfo[node.DocNumber].start - accumulator));
               
            foreach (var child in node.Next)
                TraverseChild(child.Value, accumulator + child.Value.Len, occurience);
		}
    }
}