using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public sealed class SyntaxNodeCollector<T> : CSharpSyntaxWalker where T : SyntaxNode
    {
        private readonly List<T> m_nodes;
        private readonly SyntaxKind m_syntaxKindToIgnore;

        public SyntaxNodeCollector(SyntaxKind syntaxKindToIgnore)
        {
            m_nodes = new List<T>();
            m_syntaxKindToIgnore = syntaxKindToIgnore;
        }

        public IReadOnlyList<T> Nodes => m_nodes;

        public override void Visit(SyntaxNode node)
        {
            // duplicate negative, we accept all except the one to ignore
            var acceptNode = node.IsKind(m_syntaxKindToIgnore) is false;

            if (acceptNode)
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