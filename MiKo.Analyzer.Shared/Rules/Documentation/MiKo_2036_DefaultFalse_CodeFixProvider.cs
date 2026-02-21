using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2036_DefaultFalse_CodeFixProvider)), Shared]
    public sealed class MiKo_2036_DefaultFalse_CodeFixProvider : MiKo_2036_CodeFixProvider
    {
        protected override string Title => Resources.MiKo_2036_CodeFixTitle_DefaultFalse;

        protected override Task<XmlNodeSyntax[]> GetDefaultCommentAsync(TypeSyntax returnType, Document document, CancellationToken cancellationToken)
        {
            var defaultComment = GetDefaultComment();

            return Task.FromResult(defaultComment);
        }

        private static XmlNodeSyntax[] GetDefaultComment() => new XmlNodeSyntax[]
                                                                  {
                                                                      XmlText(Constants.Comments.DefaultStartingPhrase),
                                                                      SeeLangword_False(),
                                                                      XmlText("."),
                                                                  };
    }
}