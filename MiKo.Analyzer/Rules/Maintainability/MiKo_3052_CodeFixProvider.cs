﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3052_CodeFixProvider)), Shared]
    public sealed class MiKo_3052_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3052_DependencyPropertyKeyNonPublicStaticReadOnlyFieldAnalyzer.Id;

        protected override string Title => "Make DependencyPropertyKey 'private static readonly'";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<FieldDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var field = (FieldDeclarationSyntax)syntax;
            var modifiers = SyntaxFactory.TokenList(
                                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
            return field.WithModifiers(modifiers);
        }
    }
}