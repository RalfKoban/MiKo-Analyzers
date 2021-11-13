using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2036_DefaultFalse_CodeFixProvider)), Shared]
    public sealed class MiKo_2036_DefaultFalse_CodeFixProvider : MiKo_2036_CodeFixProvider
    {
        protected override string Title => Resources.MiKo_2036_CodeFixTitle_DefaultFalse;

        protected override IEnumerable<XmlNodeSyntax> GetDefaultComment()
        {
            yield return SyntaxFactory.XmlText(Constants.Comments.DefaultStartingPhrase);
        }
    }
}