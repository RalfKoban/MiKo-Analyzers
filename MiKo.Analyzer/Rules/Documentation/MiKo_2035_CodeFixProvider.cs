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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2035_CodeFixProvider)), Shared]
    public sealed class MiKo_2035_CodeFixProvider : ReturnTypeDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "An enumerable of ", string.Empty },
                                                                                    { "A list of ", string.Empty },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2035_EnumerableReturnTypeDefaultPhraseAnalyzer.Id;

        protected override string Title => "Fix return comment";

        protected override SyntaxNode GenericComment(XmlElementSyntax comment, GenericNameSyntax returnType)
        {
            var preparedComment = PrepareComment(comment);

            // it's either a task or a generic collection
            if (returnType.Identifier.ValueText == nameof(Task))
            {
                // it is a task, so inspect the typ argument to check if it is an array type
                var parts = string.Format(Constants.Comments.GenericTaskReturnTypeStartingPhraseTemplate, "task", '|').Split('|');

                var isArray = returnType.TypeArgumentList.Arguments[0].IsKind(SyntaxKind.ArrayType);
                var middlePart = isArray ? "an array of " : "a collection of ";

                return CommentStartingWith(preparedComment, parts[0], SeeCrefTaskResult(), parts[1] + middlePart);
            }

            return CommentStartingWith(preparedComment, Constants.Comments.EnumerableReturnTypeStartingPhrase[0]);
        }

        protected override XmlElementSyntax NonGenericComment(XmlElementSyntax comment, TypeSyntax returnType)
        {
            var phrases = returnType.IsKind(SyntaxKind.ArrayType)
                              ? Constants.Comments.ArrayReturnTypeStartingPhrase
                              : Constants.Comments.EnumerableReturnTypeStartingPhrase;

            var preparedComment = PrepareComment(comment);
            return CommentStartingWith(preparedComment, phrases[0]);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment) => Comment(comment, ReplacementMap.Select(_ => _.Key).ToList(), ReplacementMap);
    }
}