﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5013_EmptyArrayAnalyzer : ArrayCreationExpressionPerformanceAnalyzer
    {
        public const string Id = "MiKo_5013";

        public MiKo_5013_EmptyArrayAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeArrayCreation(ArrayCreationExpressionSyntax node, SemanticModel semanticModel) => HasIssue(node)
                                                                                                                                            ? new[] { Issue(node) }
                                                                                                                                            : Array.Empty<Diagnostic>();

        protected override IEnumerable<Diagnostic> AnalyzeArrayInitializer(InitializerExpressionSyntax node, SemanticModel semanticModel) => HasIssue(node)
                                                                                                                                             ? new[] { Issue(node) }
                                                                                                                                             : Array.Empty<Diagnostic>();

        private static bool HasIssue(ArrayCreationExpressionSyntax node)
        {
            var rankSpecifiers = node.Type.RankSpecifiers;

            if (rankSpecifiers.Count != 1)
            {
                // seems to be a multi-dimensional array
                return false;
            }

            var sizes = rankSpecifiers[0].Sizes;

            if (sizes.Count != 1)
            {
                // seems to have different sizes
                return false;
            }

            var size = sizes[0];

            switch (size)
            {
                case LiteralExpressionSyntax literal when literal.Token.ValueText is "0":
                case OmittedArraySizeExpressionSyntax _ when node.Initializer?.ChildNodes().Any() is false:
                {
                    // it's an empty array
                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }

        private static bool HasIssue(InitializerExpressionSyntax node) => node.Expressions.Count is 0; // seems we do not have any contents
    }
}