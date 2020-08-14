using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2031_CodeFixProvider)), Shared]
    public sealed class MiKo_2031_CodeFixProvider : DocumentationCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_2031_TaskReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix return comment";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(Constants.XmlTag.Returns, syntaxNodes).FirstOrDefault() // method
                                                                                             ?? GetXmlSyntax(Constants.XmlTag.Value, syntaxNodes).FirstOrDefault(); // property

        protected override SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            var comment = (XmlElementSyntax)syntax;
            foreach (var ancestor in comment.Ancestors())
            {
                switch (ancestor)
                {
                    case MethodDeclarationSyntax m:
                        return Comment(comment, m.ReturnType);

                    case PropertyDeclarationSyntax p:
                        return Comment(comment, p.Type);

                    default:
                        continue;
                }
            }

            return comment;
        }

        private static SyntaxNode Comment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            if (returnType.IsKind(SyntaxKind.GenericName))
            {
                var parts = string.Format(Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate, "task", "|").Split('|');
                var type = SyntaxFactory.ParseTypeName("Task<TResult>");
                var member = SyntaxFactory.ParseName(nameof(Task<object>.Result));
                return CommentWithSeeCRef(comment, parts[0], type, member, parts[1] + comment.Content);
            }

            return Comment(comment, Constants.Comments.NonGenericTaskReturnTypePhrase);
        }
    }
}