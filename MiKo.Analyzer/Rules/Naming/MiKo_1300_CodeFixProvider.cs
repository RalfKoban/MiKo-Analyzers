using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1300_CodeFixProvider)), Shared]
    public sealed class MiKo_1300_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_1300_SimpleLambdaExpressionIdentifierAnalyzer.Id;

        protected override string Title => "Name it '" + Constants.LambdaIdentifiers.Default + "'";

        protected override string GetNewName(ISymbol symbol)
        {
            var p = (IParameterSymbol)symbol;

            // find argument candidates to see how long the default identifier shall become (note that the own parent is included)
            var repeat = p.GetSyntax().Ancestors().OfType<ArgumentSyntax>().Count(_ => _.ChildNodes().OfType<SimpleLambdaExpressionSyntax>().Any());
            if (repeat == 0)
            {
                return Constants.LambdaIdentifiers.Default;
            }

            return string.Concat(Enumerable.Repeat(Constants.LambdaIdentifiers.Default, repeat));
        }

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<SimpleLambdaExpressionSyntax>().First().Parameter;
    }
}