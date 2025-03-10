﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3044_CodeFixProvider)), Shared]
    public sealed class MiKo_3044_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3044";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<LiteralExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var literal = (LiteralExpressionSyntax)syntax;
            var type = FindRelatedType(document, literal);

            return type is null
                   ? NameOf(literal)
                   : NameOf(type, literal);
        }

        private static ITypeSymbol FindRelatedType(Document document, LiteralExpressionSyntax syntax)
        {
            var identifierName = syntax.Token.ValueText;

            var ifStatement = syntax.GetEnclosing<IfStatementSyntax>();

            if (ifStatement != null)
            {
                // search if block
                return FindRelatedType(document, ifStatement.Statement, identifierName) ?? FindRelatedType(document, ifStatement.Else, identifierName);
            }

            var switchStatement = syntax.GetEnclosing<SwitchStatementSyntax>();

            if (switchStatement != null)
            {
                // search switch block
                return FindRelatedType(document, switchStatement, identifierName);
            }

            return null;
        }

        private static ITypeSymbol FindRelatedType(Document document, SyntaxNode syntax, string identifierName)
        {
            var identifier = syntax?.FirstDescendant<IdentifierNameSyntax>(_ => _.GetName() == identifierName);

            if (identifier?.Parent is MemberAccessExpressionSyntax maes)
            {
                return maes.GetTypeSymbol(document);
            }

            // try to find type
            return null;
        }
    }
}