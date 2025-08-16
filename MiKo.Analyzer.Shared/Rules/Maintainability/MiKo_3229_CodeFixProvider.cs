using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3229_CodeFixProvider)), Shared]
    public sealed class MiKo_3229_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3229";

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ObjectCreationExpressionSyntax objectCreation && objectCreation.ArgumentList is ArgumentListSyntax list)
            {
                return Invocation("KeyValuePair", "Create", list.Arguments.ToArray()).WithTriviaFrom(objectCreation);
            }

            return syntax;
        }
    }
}