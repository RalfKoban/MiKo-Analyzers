using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace MiKoSolutions.Analyzers
{
    public static class SyntaxNodeCollector
    {
        public static IEnumerable<T> Collect<T>(SyntaxNode node, Predicate<SyntaxNode> predicate = null) where T : SyntaxNode
        {
            var collector = new SyntaxNodeCollector<T>(predicate);

            collector.Visit(node);

            return collector.Nodes;
        }
    }
}