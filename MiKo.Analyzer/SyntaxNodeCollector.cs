using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public sealed class SyntaxNodeCollector<T> : CSharpSyntaxWalker where T : SyntaxNode
    {
        private readonly List<T> m_nodes = new List<T>();

        public static IEnumerable<T> Collect(SyntaxNode node)
        {
            var collector = new SyntaxNodeCollector<T>();
            collector.Visit(node);
            return collector.m_nodes;
        }

        public override void Visit(SyntaxNode node)
        {
            if (node is T t) m_nodes.Add(t);
            base.Visit(node);
        }
    }
}