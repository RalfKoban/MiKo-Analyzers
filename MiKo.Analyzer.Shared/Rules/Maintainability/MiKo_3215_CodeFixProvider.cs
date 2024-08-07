using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3215_CodeFixProvider)), Shared]
    public sealed class MiKo_3215_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_3215";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ParameterSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is ParameterSyntax parameter && parameter.Type is GenericNameSyntax generic)
            {
                return parameter.WithType(SyntaxFactory.GenericName("Func")
                                                       .WithTypeArgumentList(generic.TypeArgumentList.AddArguments(PredefinedType(SyntaxKind.BoolKeyword))));
            }

            return syntax;
        }
    }
}