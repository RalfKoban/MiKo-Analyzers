using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2313_CodeFixProvider)), Shared]
    public sealed class MiKo_2313_CodeFixProvider : DocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> CommentTagToXmlTagMapping = new Dictionary<string, string>
                                                                                           {
                                                                                               { "Summary:", Constants.XmlTag.Summary },
                                                                                               { "Remark:", Constants.XmlTag.Remarks },
                                                                                               { "Remarks:", Constants.XmlTag.Remarks },
                                                                                               { "Return:", Constants.XmlTag.Returns },
                                                                                               { "Returns:", Constants.XmlTag.Returns },
                                                                                               { "Return value:", Constants.XmlTag.Returns },
                                                                                               { "ReturnValue:", Constants.XmlTag.Returns },
                                                                                               { "Value:", Constants.XmlTag.Value },
                                                                                           };

        private static readonly Dictionary<string, string> CommentTagToMultipleXmlTagMapping = new Dictionary<string, string>
                                                                                                   {
                                                                                                       { "Exception:", Constants.XmlTag.Exception },
                                                                                                       { "Exceptions:", Constants.XmlTag.Exception },
                                                                                                       { "Param:", Constants.XmlTag.Param },
                                                                                                       { "Parameter:", Constants.XmlTag.Param },
                                                                                                       { "Parameters:", Constants.XmlTag.Param },
                                                                                                   };

        private static readonly string[] XmlTagOrder =
                                                       {
                                                           Constants.XmlTag.Overloads,
                                                           Constants.XmlTag.Summary,
                                                           Constants.XmlTag.Remarks,
                                                           Constants.XmlTag.Param,
                                                           Constants.XmlTag.Returns,
                                                           Constants.XmlTag.Value,
                                                           Constants.XmlTag.Exception,
                                                       };

        public override string FixableDiagnosticId => "MiKo_2313";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<MemberDeclarationSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            if (syntax is MemberDeclarationSyntax member)
            {
                var foundCommentTags = issue.Properties[Constants.AnalyzerCodeFixSharedData.CommentTags].Split('|');

                var spaces = member.GetPositionWithinStartLine();
                var comment = CreateDocumentationComment(member, spaces, foundCommentTags);

                var updatedMember = member.WithoutLeadingTrivia();

                if (member.HasLeadingEmptyLine())
                {
                    updatedMember = updatedMember.WithLeadingEmptyLine();
                }

                updatedMember = updatedMember.WithLeadingSpaces(spaces)
                                             .WithAdditionalLeadingTrivia(SyntaxFactory.Trivia(comment))
                                             .WithAdditionalLeadingSpacesAtEnd(spaces);

                return updatedMember;
            }

            return syntax;
        }

        private static DocumentationCommentTriviaSyntax CreateDocumentationComment(MemberDeclarationSyntax member, in int spaces, string[] foundCommentTags)
        {
            var leadingComments = member.GetLeadingComments();
            var comments = leadingComments.ToList();

            var elements = new List<XmlElementSyntax>();

            for (int i = 0, length = foundCommentTags.Length; i < length; i++)
            {
                var commentTag = foundCommentTags[i];

                if (CommentTagToXmlTagMapping.TryGetValue(commentTag, out var xmlTag))
                {
                    elements.Add(Create(commentTag, xmlTag, comments, spaces, foundCommentTags));
                }
                else if (CommentTagToMultipleXmlTagMapping.TryGetValue(commentTag, out xmlTag))
                {
                    elements.AddRange(CreateMultiple(commentTag, xmlTag, comments, spaces, foundCommentTags));
                }
            }

            var contents = elements.WhereNotNull()
                                   .OrderBy(_ => XmlTagOrder.IndexOf(_.StartTag.GetName()));

            return SyntaxFactory.DocumentationComment(contents.ToArray<XmlNodeSyntax>());
        }

        private static XmlElementSyntax Create(string commentTag, string xmlTag, List<string> comments, in int spaces, string[] otherFoundCommentTags)
        {
            if (TryFindComments(comments, commentTag, otherFoundCommentTags, out var index, out var count))
            {
                var phrase = comments.Skip(index + 1).Take(count - 1).ConcatenatedWith(" ");

                // remove the processed items as we do not need to re-process them
                comments.RemoveRange(index, count);

                var comment = Comment(XmlElement(xmlTag), phrase);

                return comment.WithLeadingXmlCommentExterior()
                              .WithLeadingSpaces(spaces)
                              .WithEndOfLine();
            }

            return null;
        }

        private static XmlElementSyntax CreateTag(string xmlTag, in Pair pair)
        {
            switch (xmlTag)
            {
                case Constants.XmlTag.Param: return CreateParamTag(pair.Key, pair.Value);
                case Constants.XmlTag.Exception: return CreateExceptionTag(pair.Key, pair.Value);

                default:
                    return null;
            }
        }

        private static XmlElementSyntax CreateExceptionTag(string name, string text) => Comment(SyntaxFactory.XmlExceptionElement(Cref(name)), text);

        private static XmlElementSyntax CreateParamTag(string name, string text) => Comment(SyntaxFactory.XmlParamElement(name), text);

        private static XmlElementSyntax[] CreateMultiple(string xmlTag, in int spaces, List<Pair> pairs)
        {
            var results = new XmlElementSyntax[pairs.Count];

            for (int i = 0, pairsCount = pairs.Count; i < pairsCount; i++)
            {
                var comment = CreateTag(xmlTag, pairs[i]);

                if (comment is null)
                {
                    continue;
                }

                results[i] = comment.WithLeadingXmlCommentExterior()
                                    .WithLeadingSpaces(spaces)
                                    .WithEndOfLine();
            }

            return results;
        }

        private static XmlElementSyntax[] CreateMultiple(string commentTag, string xmlTag, List<string> comments, in int spaces, string[] otherFoundCommentTags)
        {
            if (TryFindComments(comments, commentTag, otherFoundCommentTags, out var index, out var count))
            {
                var parts = comments.Skip(index + 1).Take(count - 1).ToList();

                // remove the processed items as we do not need to re-process them
                comments.RemoveRange(index, count);

                if (parts.Count > 0)
                {
                    var pairs = FindComments(parts.ToList());

                    return CreateMultiple(xmlTag, spaces, pairs);
                }
            }

            return Array.Empty<XmlElementSyntax>();
        }

        private static List<Pair> FindComments(List<string> comments)
        {
            int specificCount;

            var pairs = new List<Pair>();

            do
            {
                var pair = FindComments(comments, out specificCount);

                pairs.Add(pair);

                comments.RemoveRange(0, specificCount);
            }
            while (specificCount > 0 && comments.Count > 0);

            return pairs;
        }

        private static Pair FindComments(List<string> comments, out int count)
        {
            var index = comments.FindIndex(0, _ => _.Contains(':'));
            var nextIndex = comments.FindIndex(1, _ => _.Contains(':'));

            if (nextIndex is -1)
            {
                nextIndex = comments.Count;
            }

            count = nextIndex - index;

            var takes = count - 1;

            if (takes <= 0)
            {
                var parts = comments[index].Split(':');

                return new Pair(parts[0].Trim(), parts[1].Trim());
            }
            else
            {
                var parts = comments.Skip(index + 1).Take(takes).ConcatenatedWith(" ");

                return new Pair(comments[index].TrimEnd(':'), parts);
            }
        }

        private static bool TryFindComments(List<string> comments, string commentTag, string[] otherFoundCommentTags, out int index, out int count)
        {
            index = comments.IndexOf(commentTag);
            count = 0;

            if (index is -1)
            {
                return false;
            }

            var nextIndex = comments.FindIndex(index + 1, _ => _.ContainsAny(otherFoundCommentTags));

            if (nextIndex is -1)
            {
                nextIndex = comments.Count;
            }

            count = nextIndex - index;

            return true;
        }
    }
}