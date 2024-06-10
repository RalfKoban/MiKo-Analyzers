using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2060_CodeFixProvider)), Shared]
    public sealed class MiKo_2060_CodeFixProvider : SummaryDocumentationCodeFixProvider
    {
        public static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

        public override string FixableDiagnosticId => "MiKo_2060";

        protected override string Title => Resources.MiKo_2060_CodeFixTitle;

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
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

                        if (preparedComment == summary)
                        {
                            preparedComment = Comment(summary, MappedData.Value.InstancesReplacementMapKeys, MappedData.Value.InstancesReplacementMap);
                        }

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

                        var parts = template.FormatWith('|').Split('|');

                        var fixedComment = CommentStartingWith(preparedComment, parts[0], SeeCref(returnType), parts[1]);

                        var cleanedContent = CleanupMethodComment(fixedComment);

                        return cleanedContent;
                    }
                }
            }

            return summary;
        }

        private static XmlElementSyntax PrepareTypeComment(XmlElementSyntax comment) => Comment(comment, MappedData.Value.TypeReplacementMapKeys, MappedData.Value.TypeReplacementMap);

        private static XmlElementSyntax PrepareMethodComment(XmlElementSyntax comment)
        {
            var preparedComment = Comment(comment, MappedData.Value.MethodReplacementMapKeys, MappedData.Value.MethodReplacementMap);

            var content = preparedComment.Content;

            if (content.Count > 2)
            {
                var content1 = content[0];
                var content2 = content[1];

                if (content2.IsKind(SyntaxKind.XmlEmptyElement) && content1.IsWhiteSpaceOnlyText())
                {
                    return preparedComment.Without(content1, content2);
                }
            }

            return preparedComment;
        }

        private static XmlElementSyntax CleanupMethodComment(XmlElementSyntax comment) => Comment(comment, MappedData.Value.CleanupReplacementMap.Keys, MappedData.Value.CleanupReplacementMap);

//// ncrunch: rdi off
//// ncrunch: no coverage start

        public sealed class MapData
        {
            public MapData()
            {
                TypeReplacementMap = CreateTypeReplacementMapKeys().ToHashSet() // avoid duplicates
                                                                   .Select(_ => new KeyValuePair<string, string>(_, string.Empty))
                                                                   .ToArray(_ => _.Key, AscendingStringComparer.Default);

                TypeReplacementMapKeys = GetTermsForQuickLookup(TypeReplacementMap.Select(_ => _.Key));

                MethodReplacementMap = CreateMethodReplacementMapKeys().ToHashSet() // avoid duplicates
                                                                       .Select(_ => new KeyValuePair<string, string>(_, string.Empty))
                                                                       .ToArray(_ => _.Key, AscendingStringComparer.Default);

                MethodReplacementMapKeys = GetTermsForQuickLookup(MethodReplacementMap.Select(_ => _.Key));

                InstancesReplacementMap = MethodReplacementMap.Select(_ => _.Key)
                                                              .Select(_ => new KeyValuePair<string, string>(_, "instances of the "))
                                                              .ToArray(_ => _.Key, AscendingStringComparer.Default);

                InstancesReplacementMapKeys = GetTermsForQuickLookup(InstancesReplacementMap.Select(_ => _.Key));

                CleanupReplacementMap = new Dictionary<string, string>
                                            {
                                                { " based on ", " default values for " },
                                                { " with for ", " with " },
                                                { " with with ", " with " },
                                                { " type with type.", " type with default values." },
                                                { " type with that ", " type with default values that " },
                                                { " type with which ", " type with default values which " },
                                            };
            }

            public IReadOnlyCollection<KeyValuePair<string, string>> TypeReplacementMap { get; }

            public string[] TypeReplacementMapKeys { get; }

            public IReadOnlyCollection<KeyValuePair<string, string>> MethodReplacementMap { get; }

            public string[] MethodReplacementMapKeys { get; }

            public IReadOnlyCollection<KeyValuePair<string, string>> InstancesReplacementMap { get; }

            public string[] InstancesReplacementMapKeys { get; }

            public Dictionary<string, string> CleanupReplacementMap { get; }

            private static IEnumerable<string> CreateTypeReplacementMapKeys()
            {
                var phrases = new[]
                                  {
                                      "A factory",
                                      "A factory to provide methods",
                                      "A factory that provides methods",
                                      "A factory which provides methods",
                                      "A factory to provide",
                                      "A factory that provides",
                                      "A factory which provides",
                                      "The factory",
                                      "The factory to provide methods",
                                      "The factory that provides methods",
                                      "The factory which provides methods",
                                      "The factory to provide",
                                      "The factory that provides",
                                      "The factory which provides",
                                      "This factory",
                                      "This factory provides methods",
                                      "A interface for factories",
                                      "An interface for factories",
                                      "The interface for factories",
                                      "A interface that is implemented by factories",
                                      "An interface that is implemented by factories",
                                      "The interface that is implemented by factories",
                                      "A interface which is implemented by factories",
                                      "An interface which is implemented by factories",
                                      "The interface which is implemented by factories",
                                      "A interface implemented by factories",
                                      "An interface implemented by factories",
                                      "The interface implemented by factories",
                                      "A interface",
                                      "An interface",
                                      "The interface",
                                      "Factory",
                                      "Factory to provide methods",
                                      "Factory that provides methods",
                                      "Factory which provides methods",
                                      "Factory to provide",
                                      "Factory that provides",
                                      "Factory which provides",
                                      "Interface for factories",
                                      "Interface",
                                      "Provides methods",
                                      "Provides a method",
                                      "Provides a factory",
                                      "Provides the factory",
                                      "Provides",
                                      "Defines a factory",
                                      "Defines the factory",
                                      "Defines methods",
                                      "Defines a method",
                                      "Represents a factory",
                                      "Represents the factory",
                                      "This interface is implemented by factories",
                                      "Used",
                                      "Uses", // typo in 'Used'
                                      "A implementation of the factory pattern",
                                      "An implementation of the factory pattern",
                                      "The implementation of the factory pattern",
                                      "Implementation of the factory pattern",
                                      "A implementation of the abstract factory pattern",
                                      "An implementation of the abstract factory pattern",
                                      "The implementation of the abstract factory pattern",
                                      "Implementation of the abstract factory pattern",
                                  };

                var articles = new[]
                                   {
                                       "a ",
                                       "an ",
                                       "the ",
                                       string.Empty,
                                   };

                var instances = new[]
                                    {
                                        "instance of ",
                                        "instances of ",
                                        "new instance of ",
                                        "new instances of ",
                                    };

                var continuations = new List<string>();

                foreach (var article in articles)
                {
                    continuations.AddRange(instances.Select(_ => article + _ + article));
                    continuations.Add(article);
                }

                foreach (var phrase in phrases)
                {
                    foreach (var continuation in continuations)
                    {
                        yield return phrase + " that can create " + continuation;
                        yield return phrase + " that create " + continuation;
                        yield return phrase + " that creates " + continuation;
                        yield return phrase + " that provides " + continuation;
                        yield return phrase + " that " + continuation;

                        yield return phrase + " which can create " + continuation;
                        yield return phrase + " which create " + continuation;
                        yield return phrase + " which creates " + continuation;
                        yield return phrase + " which provides " + continuation;
                        yield return phrase + " which " + continuation;

                        yield return phrase + " for creating " + continuation;
                        yield return phrase + " for creation of " + continuation;
                        yield return phrase + " for the creation of " + continuation;
                        yield return phrase + " for providing " + continuation;
                        yield return phrase + " for " + continuation;

                        yield return phrase + " creating " + continuation;
                        yield return phrase + " creates " + continuation;

                        yield return phrase + " that is able to create " + continuation;
                        yield return phrase + " which is able to create " + continuation;
                        yield return phrase + " that is capable to create " + continuation;
                        yield return phrase + " which is capable to create " + continuation;
                        yield return phrase + " to create " + continuation;
                        yield return phrase + " to " + continuation;

                        yield return phrase + " " + continuation;
                    }
                }

                yield return "Implementations create ";
            }

            private static IEnumerable<string> CreateMethodReplacementMapKeys()
            {
                var startingWords = new[]
                                        {
                                            "Create",
                                            "Creates",
                                            "Construct",
                                            "Constructs",
                                            "Return",
                                            "Returns",
                                            "Get",
                                            "Gets",
                                        };

                var continuations = new[]
                                        {
                                            string.Empty,
                                            " and initialize",
                                            " and initializes",
                                            " and provide",
                                            " and provides",
                                            " and return",
                                            " and returns",
                                        };

                foreach (var word in startingWords)
                {
                    foreach (var continuation in continuations)
                    {
                        var start = word + continuation;

                        yield return start + " an new instances of the ";
                        yield return start + " an new instances of an ";
                        yield return start + " an new instances of a ";
                        yield return start + " an new instances of ";
                        yield return start + " an new instance of the ";
                        yield return start + " an new instance of an ";
                        yield return start + " an new instance of a ";
                        yield return start + " an new instance of ";
                        yield return start + " an instances of the ";
                        yield return start + " an instances of an ";
                        yield return start + " an instances of a ";
                        yield return start + " an instances of ";
                        yield return start + " an instance of the ";
                        yield return start + " an instance of an ";
                        yield return start + " an instance of a ";
                        yield return start + " an instance of ";
                        yield return start + " an ";
                        yield return start + " a factory ";
                        yield return start + " a new instances of the ";
                        yield return start + " a new instances of an ";
                        yield return start + " a new instances of a ";
                        yield return start + " a new instances of ";
                        yield return start + " a new instance of the ";
                        yield return start + " a new instance of an ";
                        yield return start + " a new instance of a ";
                        yield return start + " a new instance of ";
                        yield return start + " a instances of the ";
                        yield return start + " a instances of an ";
                        yield return start + " a instances of a ";
                        yield return start + " a instances of ";
                        yield return start + " a instance of the ";
                        yield return start + " a instance of an ";
                        yield return start + " a instance of a ";
                        yield return start + " a instance of ";
                        yield return start + " a new ";
                        yield return start + " a ";
                        yield return start + " instances of the ";
                        yield return start + " instances of an ";
                        yield return start + " instances of a ";
                        yield return start + " instances of ";
                        yield return start + " new instances of the ";
                        yield return start + " new instances of an ";
                        yield return start + " new instances of a ";
                        yield return start + " new instances of ";
                    }
                }

                yield return "Used to create ";
                yield return "Used for creating ";
                yield return "Factory method for creating ";
                yield return "Factory method that creates ";
                yield return "Factory method which creates ";
                yield return "A factory method for creating ";
                yield return "A factory method that creates ";
                yield return "A factory method which creates ";
                yield return "The factory method for creating ";
                yield return "The factory method that creates ";
                yield return "The factory method which creates ";
                yield return "This factory method creates ";
                yield return "This method creates ";
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}