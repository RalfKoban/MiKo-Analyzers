using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public sealed class SyntaxNodeCollector<T> : CSharpSyntaxWalker where T : SyntaxNode
    {
        private readonly SyntaxKind m_syntaxKindToIgnore;
        private List<T> m_nodes = null;

        public SyntaxNodeCollector(in SyntaxKind syntaxKindToIgnore) => m_syntaxKindToIgnore = syntaxKindToIgnore;

        public IReadOnlyList<T> Nodes
        {
            get
            {
                if (m_nodes != null)
                {
                    return m_nodes;
                }

                return Array.Empty<T>();
            }
        }

        public override void Visit(SyntaxNode node)
        {
            if (node.IsKind(m_syntaxKindToIgnore))
            {
                return;
            }

            if (node is T t)
            {
                if (m_nodes is null)
                {
                    m_nodes = new List<T>();
                }

                m_nodes.Add(t);
            }

            base.Visit(node);
        }
    }
}