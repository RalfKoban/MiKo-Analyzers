﻿using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1108_CodeFixProvider)), Shared]
    public sealed class MiKo_1108_CodeFixProvider : LocalVariableNamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1108";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes)
        {
            var syntax = base.GetSyntax(syntaxNodes);

            if (syntax is null)
            {
                foreach (var syntaxNode in syntaxNodes)
                {
                    switch (syntaxNode.Kind())
                    {
                        case SyntaxKind.Parameter:
                        case SyntaxKind.PropertyDeclaration:
                        {
                            return syntaxNode;
                        }
                    }
                }
            }

            return syntax;
        }
    }
}