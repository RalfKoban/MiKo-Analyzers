using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3014_CodeFixProvider)), Shared]
    public sealed class MiKo_3014_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3014";

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var argument = syntax.IsInsideTestClass()
                           ? Argument(StringLiteral("It's a mock."))
                           : ToDo();

            return syntax.ArgumentList.AddArguments(argument);
        }
    }
}