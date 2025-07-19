using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_5019_CodeFixProvider)), Shared]
    public sealed class MiKo_5019_CodeFixProvider : PerformanceCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_5019";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ParameterSyntax parameter)
            {
                return parameter.WithModifiers(parameter.Modifiers.Add(SyntaxKind.InKeyword.AsToken()));
            }

            return syntax;
        }
    }
}