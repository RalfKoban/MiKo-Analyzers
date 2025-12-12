using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public sealed class SyntaxNodeCollector<T> : CSharpSyntaxWalker where T : SyntaxNode
    {
        private readonly SyntaxKind m_syntaxKindToIgnore;

        private readonly List<T> m_nodes = new List<T>(8);

        public SyntaxNodeCollector(in SyntaxKind syntaxKindToIgnore) => m_syntaxKindToIgnore = syntaxKindToIgnore;

        public IReadOnlyList<T> Nodes => m_nodes;

        public override void Visit(SyntaxNode node)
        {
            if (node.IsKind(m_syntaxKindToIgnore))
            {
                return;
            }

            if (node is T t)
            {
                m_nodes.Add(t);
            }

            base.Visit(node);
        }
    }
}