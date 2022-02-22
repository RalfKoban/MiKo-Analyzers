using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public sealed class SyntaxNodeCollector<T> : CSharpSyntaxWalker where T : SyntaxNode
    {
        private readonly List<T> m_nodes;
        private readonly Predicate<SyntaxNode> m_predicate;

        public SyntaxNodeCollector(Predicate<SyntaxNode> predicate)
        {
            m_nodes = new List<T>();
            m_predicate = predicate;
        }

        public IEnumerable<T> Nodes => m_nodes;

        public override void Visit(SyntaxNode node)
        {
            if (m_predicate is null || m_predicate(node))
            {
                if (node is T t)
                {
                    m_nodes.Add(t);
                }

                base.Visit(node);
            }
        }
    }
}