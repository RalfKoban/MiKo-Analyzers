using System;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2056_CodeFixProvider)), Shared]
    public sealed class MiKo_2056_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer.Id;

        protected override string Title => Resources.MiKo_2056_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(Document document, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            foreach (var ancestor in syntax.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case ConstructorDeclarationSyntax _:
                    case PropertyDeclarationSyntax _:
                    case MethodDeclarationSyntax _:
                    {
                        return FixComment(document, ancestor, syntax);
                    }
                }
            }

            return null;
        }

        private static DocumentationCommentTriviaSyntax FixComment(Document document, SyntaxNode syntax, DocumentationCommentTriviaSyntax comment)
        {
            foreach (var part in comment.Content)
            {
                if (part is XmlElementSyntax e && e.GetName() == Constants.XmlTag.Exception)
                {
                    foreach (var attribute in e.GetAttributes<XmlCrefAttributeSyntax>())
                    {
                        switch (attribute.Cref)
                        {
                            case QualifiedCrefSyntax q when IsObjectDisposedException(q.ToString()):
                            case NameMemberCrefSyntax m when IsObjectDisposedException(m.ToString()):
                            {
                                var symbol = GetSymbol(document, syntax);
                                var phrase = MiKo_2056_ObjectDisposedExceptionPhraseAnalyzer.GetEndingPhrase(symbol);

                                var exceptionComment = CommentEndingWith(e, phrase);

                                return comment.ReplaceNode(part, exceptionComment);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsObjectDisposedException(string syntax)
        {
            switch (syntax)
            {
                case nameof(ObjectDisposedException):
                case nameof(System) + "." + nameof(ObjectDisposedException):
                    return true;

                default:
                    return false;
            }
        }
    }
}