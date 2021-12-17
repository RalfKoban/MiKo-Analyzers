using System;
using System.Collections.Generic;
using System.Composition;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2060_CodeFixProvider)), Shared]
    public sealed class MiKo_2060_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        private static readonly Dictionary<string, string> TypeReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "A factory that creates ", string.Empty },
                                                                                    { "A factory that ", string.Empty },
                                                                                    { "A factory which creates ", string.Empty },
                                                                                    { "A factory which ", string.Empty },
                                                                                    { "A factory for ", string.Empty },
                                                                                    { "A interface for factories that create ", string.Empty },
                                                                                    { "A interface for factories which create ", string.Empty },
                                                                                    { "A interface that is implemented by factories that create ", string.Empty },
                                                                                    { "A interface that is implemented by factories which create ", string.Empty },
                                                                                    { "A interface to create ", string.Empty },
                                                                                    { "A interface which is implemented by factories that create ", string.Empty },
                                                                                    { "A interface which is implemented by factories which create ", string.Empty },
                                                                                    { "An interface for factories that create ", string.Empty },
                                                                                    { "An interface for factories which create ", string.Empty },
                                                                                    { "An interface that is implemented by factories that create ", string.Empty },
                                                                                    { "An interface that is implemented by factories which create ", string.Empty },
                                                                                    { "An interface to create ", string.Empty },
                                                                                    { "An interface which is implemented by factories that create ", string.Empty },
                                                                                    { "An interface which is implemented by factories which create ", string.Empty },
                                                                                    { "Creates ", string.Empty },
                                                                                    { "Factory for creating ", string.Empty },
                                                                                    { "Factory for ", string.Empty },
                                                                                    { "Factory to create ", string.Empty },
                                                                                    { "Interface for factories that create ", string.Empty },
                                                                                    { "Interface for factories which create ", string.Empty },
                                                                                    { "Interface to create ", string.Empty },
                                                                                    { "Provides methods to create ", string.Empty },
                                                                                    { "Provides ", string.Empty },
                                                                                    { "Represents a factory that creates ", string.Empty },
                                                                                    { "Represents a factory that ", string.Empty },
                                                                                    { "Represents a factory which creates ", string.Empty },
                                                                                    { "Represents a factory which ", string.Empty },
                                                                                    { "Represents the factory that creates ", string.Empty },
                                                                                    { "Represents the factory that ", string.Empty },
                                                                                    { "Represents the factory which creates ", string.Empty },
                                                                                    { "Represents the factory which ", string.Empty },
                                                                                    { "The factory that creates ", string.Empty },
                                                                                    { "The factory that ", string.Empty },
                                                                                    { "The factory which creates ", string.Empty },
                                                                                    { "The factory which ", string.Empty },
                                                                                    { "The interface that is implemented by factories that create ", string.Empty },
                                                                                    { "The interface that is implemented by factories which create ", string.Empty },
                                                                                    { "The interface which is implemented by factories that create ", string.Empty },
                                                                                    { "The interface which is implemented by factories which create ", string.Empty },
                                                                                    { "This interface is implemented by factories that create ", string.Empty },
                                                                                    { "This interface is implemented by factories which create ", string.Empty },
                                                                                    { "Used for creating ", string.Empty },
                                                                                    { "Used to create ", string.Empty },
                                                                                };

        private static readonly Dictionary<string, string> MethodReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { "Creates an new instance of ", string.Empty },
                                                                                    { "Creates an instance of ", string.Empty },
                                                                                    { "Creates an ", string.Empty },
                                                                                    { "Creates a ", string.Empty },
                                                                                    { "Used to create ", string.Empty },
                                                                                    { "Used for creating ", string.Empty },
                                                                                    { "Factory method for creating ", string.Empty },
                                                                                    { "Factory method that creates ", string.Empty },
                                                                                    { "Factory method which creates ", string.Empty },
                                                                                    { "A factory method for creating ", string.Empty },
                                                                                    { "A factory method that creates ", string.Empty },
                                                                                    { "A factory method which creates ", string.Empty },
                                                                                    { "The factory method for creating ", string.Empty },
                                                                                    { "The factory method that creates ", string.Empty },
                                                                                    { "The factory method which creates ", string.Empty },
                                                                                };

        private static readonly Dictionary<string, string> CleanupReplacementMap = new Dictionary<string, string>
                                                                                {
                                                                                    { " with for ", " with " },
                                                                                    { " type with type.", " type with default values." },
                                                                                };

        public override string FixableDiagnosticId => MiKo_2060_FactoryAnalyzer.Id;

        protected override string Title => Resources.MiKo_2060_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic diagnostic)
        {
            var summary = (XmlElementSyntax)syntax;

            foreach (var ancestor in summary.AncestorsAndSelf())
            {
                switch (ancestor)
                {
                    case ClassDeclarationSyntax _:
                    case InterfaceDeclarationSyntax _:
                    {
                        var preparedComment = PrepareTypeComment(summary);

                        return CommentStartingWith(preparedComment, Constants.Comments.FactorySummaryPhrase);
                    }

                    case MethodDeclarationSyntax m:
                    {
                        var preparedComment = PrepareMethodComment(summary);

                        var template = Constants.Comments.FactoryCreateMethodSummaryStartingPhraseTemplate;
                        var returnType = m.ReturnType;

                        if (returnType is GenericNameSyntax g && g.TypeArgumentList.Arguments.Count == 1)
                        {
                            template = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhraseTemplate;
                            returnType = g.TypeArgumentList.Arguments[0];
                        }

                        var parts = string.Format(template, '|').Split('|');

                        var fixedComment = CommentStartingWith(preparedComment, parts[0], SeeCref(returnType), parts[1]);

                        var cleanedContent = CleanupMethodComment(fixedComment);

                        return cleanedContent;
                    }
                }
            }

            return summary;
        }

        private static XmlElementSyntax PrepareTypeComment(XmlElementSyntax comment)
        {
            return Comment(comment, TypeReplacementMap.Keys, TypeReplacementMap);
        }

        private static XmlElementSyntax PrepareMethodComment(XmlElementSyntax comment)
        {
            var preparedComment = Comment(comment, MethodReplacementMap.Keys, MethodReplacementMap);

            if (preparedComment.Content.Count > 2)
            {
                var content1 = preparedComment.Content[0];
                var content2 = preparedComment.Content[1];
                if (content2.IsKind(SyntaxKind.XmlEmptyElement) && content1.IsWhiteSpaceOnlyText())
                {
                    return preparedComment.Without(content1, content2);
                }
            }

            return preparedComment;
        }

        private static XmlElementSyntax CleanupMethodComment(XmlElementSyntax comment)
        {
            return Comment(comment, CleanupReplacementMap.Keys, CleanupReplacementMap);
        }
    }
}