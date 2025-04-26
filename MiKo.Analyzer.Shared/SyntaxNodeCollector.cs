﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers
{
    public static class SyntaxNodeCollector
    {
        public static IReadOnlyList<T> Collect<T>(SyntaxNode node, in SyntaxKind syntaxKindToIgnore = SyntaxKind.None) where T : SyntaxNode
        {
            var collector = new SyntaxNodeCollector<T>(syntaxKindToIgnore);

            collector.Visit(node);

            return collector.Nodes;
        }
    }
}