using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2046_CodeFixProvider)), Shared]
    public sealed class MiKo_2046_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer.Id;

        protected override string Title => Resources.MiKo_2046_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax != null)
            {
                var name = MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer.GetReferencedName(syntax);

                return TypeParamRef(name);
            }

            return null;
        }
    }
}