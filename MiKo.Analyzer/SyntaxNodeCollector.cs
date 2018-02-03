using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public class SyntaxNodeCollector<T> : CSharpSyntaxWalker where T : SyntaxNode
    {
        public List<T> Nodes { get; } = new List<T>();

        public override void Visit(SyntaxNode node)
        {
            if (node is T t) Nodes.Add(t);

            base.Visit(node);
        }
    }
}