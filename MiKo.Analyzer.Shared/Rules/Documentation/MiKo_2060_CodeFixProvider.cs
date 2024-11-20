﻿using System;
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
        private static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

#if !NCRUNCH // do not define a static ctor to speed up tests in NCrunch
        static MiKo_2060_CodeFixProvider() => LoadData(); // ensure that we have the object available
#endif

        public override string FixableDiagnosticId => "MiKo_2060";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

        internal static bool CanFix(ReadOnlySpan<char> text)
        {
            var mappedData = MappedData.Value;

            return text.StartsWithAny(mappedData.InstancesReplacementMapKeys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapKeysA, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapKeysCD, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapKeysThe, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapKeysThis, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapKeysOthers, StringComparison.OrdinalIgnoreCase);
        }

        internal static SyntaxNode GetUpdatedSyntax(SyntaxNode syntax)
        {
            if (syntax is XmlElementSyntax summary)
            {
                foreach (var ancestor in summary.AncestorsAndSelf())
                {
                    switch (ancestor.Kind())
                    {
                        case SyntaxKind.ClassDeclaration:
                        case SyntaxKind.InterfaceDeclaration:
                        {
                            var preparedComment = PrepareTypeComment(summary);

                            if (preparedComment == summary)
                            {
                                var mappedData = MappedData.Value;

                                preparedComment = Comment(summary, mappedData.InstancesReplacementMapKeys, mappedData.InstancesReplacementMap);
                            }

                            return CommentStartingWith(preparedComment, Constants.Comments.FactorySummaryPhrase);
                        }

                        case SyntaxKind.MethodDeclaration:
                        {
                            var preparedComment = PrepareMethodComment(summary);

                            var template = Constants.Comments.FactoryCreateMethodSummaryStartingPhraseTemplate;
                            var returnType = ((MethodDeclarationSyntax)ancestor).ReturnType;

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

            return syntax;
        }

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue) => GetUpdatedSyntax(syntax);

        private static XmlElementSyntax PrepareTypeComment(XmlElementSyntax comment)
        {
            var mappedData = MappedData.Value;

            var updated = Comment(comment, mappedData.TypeReplacementMapKeysA, mappedData.TypeReplacementMapA);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            updated = Comment(comment, mappedData.TypeReplacementMapKeysThe, mappedData.TypeReplacementMapThe);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            updated = Comment(comment, mappedData.TypeReplacementMapKeysThis, mappedData.TypeReplacementMapThis);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            updated = Comment(comment, mappedData.TypeReplacementMapKeysOthers, mappedData.TypeReplacementMapOthers);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            updated = Comment(comment, mappedData.TypeReplacementMapKeysCD, mappedData.TypeReplacementMapCD);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            return comment;
        }

        private static XmlElementSyntax PrepareMethodComment(XmlElementSyntax comment)
        {
            var mappedData = MappedData.Value;

            var preparedComment = Comment(comment, mappedData.MethodReplacementMapKeys, mappedData.MethodReplacementMap);

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

        private static XmlElementSyntax CleanupMethodComment(XmlElementSyntax comment)
        {
            var mappedData = MappedData.Value;

            return Comment(comment, mappedData.CleanupReplacementMapKeys, mappedData.CleanupReplacementMap);
        }

//// ncrunch: rdi off
//// ncrunch: no coverage start

        private sealed class MapData
        {
            public MapData()
            {
                var typeKeys = CreateTypeReplacementMapKeys();
                var typeKeysLength = typeKeys.Length;

                //// Array.Sort(typeKeys, AscendingStringComparer.Default);

                var typeKeysWithA = new List<string>(57228); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysWithCD = new List<string>(30362); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysWithThe = new List<string>(54490); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysWithThis = new List<string>(16660); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysOther = new List<string>(47532); // TODO RKN: Adjust number as soon as there are other texts

                for (var index = 0; index < typeKeysLength; index++)
                {
                    var typeKey = typeKeys[index];

                    switch (typeKey[0])
                    {
                        case 'A':
                        case 'a':
                            typeKeysWithA.Add(typeKey);

                            break;

                        case 'C':
                        case 'D':
                        case 'c':
                        case 'd':
                            typeKeysWithCD.Add(typeKey);

                            break;

                        case 'T':
                        case 't':
                            var list = typeKey[2] == 'e' ? typeKeysWithThe : typeKeysWithThis;
                            list.Add(typeKey);

                            break;

                        default:
                            typeKeysOther.Add(typeKey);

                            break;
                    }
                }

                typeKeysWithA.Sort(AscendingStringComparer.Default);
                typeKeysWithCD.Sort(AscendingStringComparer.Default);
                typeKeysWithThe.Sort(AscendingStringComparer.Default);
                typeKeysWithThis.Sort(AscendingStringComparer.Default);
                typeKeysOther.Sort(AscendingStringComparer.Default);

                TypeReplacementMapA = ToArray(typeKeysWithA);
                TypeReplacementMapKeysA = GetTermsForQuickLookup(typeKeysWithA);

                TypeReplacementMapCD = ToArray(typeKeysWithCD);
                TypeReplacementMapKeysCD = GetTermsForQuickLookup(typeKeysWithCD);

                TypeReplacementMapThe = ToArray(typeKeysWithThe);
                TypeReplacementMapKeysThe = GetTermsForQuickLookup(typeKeysWithThe);

                TypeReplacementMapThis = ToArray(typeKeysWithThis);
                TypeReplacementMapKeysThis = GetTermsForQuickLookup(typeKeysWithThis);

                TypeReplacementMapOthers = ToArray(typeKeysOther);
                TypeReplacementMapKeysOthers = GetTermsForQuickLookup(typeKeysOther);

                var methodKeys = CreateMethodReplacementMapKeys();
                var methodKeysLength = methodKeys.Length;

                Array.Sort(methodKeys, AscendingStringComparer.Default);

                var methodReplacementMap = new Pair[methodKeysLength];
                var instancesReplacementMap = new Pair[methodKeysLength];

                for (var i = 0; i < methodKeysLength; i++)
                {
                    var key = methodKeys[i];

                    methodReplacementMap[i] = new Pair(key);
                    instancesReplacementMap[i] = new Pair(key, "instances of the ");
                }

                MethodReplacementMap = methodReplacementMap;
                MethodReplacementMapKeys = GetTermsForQuickLookup(methodKeys);

                InstancesReplacementMap = instancesReplacementMap;
                InstancesReplacementMapKeys = GetTermsForQuickLookup(methodKeys);

                CleanupReplacementMap = new[]
                                            {
                                                new Pair(" based on ", " default values for "),
                                                new Pair(" with for ", " with "),
                                                new Pair(" with with ", " with "),
                                                new Pair(" type with type.", " type with default values."),
                                                new Pair(" type with that ", " type with default values that "),
                                                new Pair(" type with which ", " type with default values which "),
                                            };
                CleanupReplacementMapKeys = CleanupReplacementMap.ToArray(_ => _.Key);

                Pair[] ToArray(IReadOnlyList<string> keys)
                {
                    var length = keys.Count;
                    var pairs = new Pair[length];

                    for (var i = 0; i < length; i++)
                    {
                        pairs[i] = new Pair(keys[i]);
                    }

                    return pairs;
                }
            }

            public Pair[] TypeReplacementMapA { get; }

            public string[] TypeReplacementMapKeysA { get; }

            public Pair[] TypeReplacementMapCD { get; }

            public string[] TypeReplacementMapKeysCD { get; }

            public Pair[] TypeReplacementMapThe { get; }

            public string[] TypeReplacementMapKeysThe { get; }

            public Pair[] TypeReplacementMapThis { get; }

            public string[] TypeReplacementMapKeysThis { get; }

            public Pair[] TypeReplacementMapOthers { get; }

            public string[] TypeReplacementMapKeysOthers { get; }

            public Pair[] MethodReplacementMap { get; }

            public string[] MethodReplacementMapKeys { get; }

            public Pair[] InstancesReplacementMap { get; }

            public string[] InstancesReplacementMapKeys { get; }

            public Pair[] CleanupReplacementMap { get; }

            public string[] CleanupReplacementMapKeys { get; }

            private static string[] CreateTypeReplacementMapKeys()
            {
                var allPhrases = CreateAllPhrases();
                var allContinuations = new HashSet<string>(CreateAllContinuations());

                var results = new HashSet<string> // avoid duplicates
                                  {
                                      "Implementations construct ",
                                      "Implementations create ",
                                      "Implementations build ",
                                      "Implementations provide ",
                                  };

                foreach (var phrase in allPhrases)
                {
                    foreach (var continuation in allContinuations)
                    {
                        results.Add(phrase + continuation);
                    }
                }

                var strangeTexts = new[]
                                       {
                                           "methods a", "methods instance", "methods new", "methods the", "factory class method", "method that are", "method which are",
                                           "es that is capable", "es which is capable", "es that is able", "es which is able",
                                           "ss that are capable", "ss which are capable", "ss that are able", "ss which are able",
                                           "y that are capable", "y which are capable", "y that are able", "y which are able",
                                           "rn that are capable", "rn which are capable", "rn that are able", "rn which are able",
                                           "ace that are capable", "ace which are capable", "ace that are able", "ace which are able",
                                           "ies that provides", "ies which provides",
                                           "providing provid", "provides provid", "provide provid",
                                       };

                results.RemoveWhere(_ => _.ContainsAny(strangeTexts));

                return results.ToArray();
            }

            private static string[] CreateMethodReplacementMapKeys()
            {
                var results = new HashSet<string> // avoid duplicates
                                  {
                                      "A factory method for building ",
                                      "A factory method for constructing ",
                                      "A factory method for creating ",
                                      "A factory method that builds ",
                                      "A factory method that constructs ",
                                      "A factory method that creates ",
                                      "A factory method which builds ",
                                      "A factory method which constructs ",
                                      "A factory method which creates ",
                                      "Factory method for building ",
                                      "Factory method for constructing ",
                                      "Factory method for creating ",
                                      "Factory method that builds ",
                                      "Factory method that constructs ",
                                      "Factory method that creates ",
                                      "Factory method which builds ",
                                      "Factory method which constructs ",
                                      "Factory method which creates ",
                                      "The factory method for building ",
                                      "The factory method for constructing ",
                                      "The factory method for creating ",
                                      "The factory method that builds ",
                                      "The factory method that constructs ",
                                      "The factory method that creates ",
                                      "The factory method which builds ",
                                      "The factory method which constructs ",
                                      "The factory method which creates ",
                                      "This factory method builds ",
                                      "This factory method constructs ",
                                      "This factory method creates ",
                                      "This method builds ",
                                      "This method constructs ",
                                      "This method creates ",
                                      "Used for building ",
                                      "Used for constructing ",
                                      "Used for creating ",
                                      "Used to build ",
                                      "Used to construct ",
                                      "Used to create ",
                                  };

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

                var startingWordsLength = startingWords.Length;
                var continuationsLength = continuations.Length;

                for (var wordIndex = 0; wordIndex < startingWordsLength; wordIndex++)
                {
                    var word = startingWords[wordIndex];

                    for (var continuationsIndex = 0; continuationsIndex < continuationsLength; continuationsIndex++)
                    {
                        var continuation = continuations[continuationsIndex];
                        var start = word + continuation;

                        results.Add(start + " an new instances of the ");
                        results.Add(start + " an new instances of an ");
                        results.Add(start + " an new instances of a ");
                        results.Add(start + " an new instances of ");
                        results.Add(start + " an new instance of the ");
                        results.Add(start + " an new instance of an ");
                        results.Add(start + " an new instance of a ");
                        results.Add(start + " an new instance of ");
                        results.Add(start + " an instances of the ");
                        results.Add(start + " an instances of an ");
                        results.Add(start + " an instances of a ");
                        results.Add(start + " an instances of ");
                        results.Add(start + " an instance of the ");
                        results.Add(start + " an instance of an ");
                        results.Add(start + " an instance of a ");
                        results.Add(start + " an instance of ");
                        results.Add(start + " an ");
                        results.Add(start + " a factory ");
                        results.Add(start + " a new instances of the ");
                        results.Add(start + " a new instances of an ");
                        results.Add(start + " a new instances of a ");
                        results.Add(start + " a new instances of ");
                        results.Add(start + " a new instance of the ");
                        results.Add(start + " a new instance of an ");
                        results.Add(start + " a new instance of a ");
                        results.Add(start + " a new instance of ");
                        results.Add(start + " a instances of the ");
                        results.Add(start + " a instances of an ");
                        results.Add(start + " a instances of a ");
                        results.Add(start + " a instances of ");
                        results.Add(start + " a instance of the ");
                        results.Add(start + " a instance of an ");
                        results.Add(start + " a instance of a ");
                        results.Add(start + " a instance of ");
                        results.Add(start + " a new ");
                        results.Add(start + " a ");
                        results.Add(start + " instances of the ");
                        results.Add(start + " instances of an ");
                        results.Add(start + " instances of a ");
                        results.Add(start + " instances of ");
                        results.Add(start + " new instances of the ");
                        results.Add(start + " new instances of an ");
                        results.Add(start + " new instances of a ");
                        results.Add(start + " new instances of ");
                    }
                }

                var resultsArray = new string[results.Count];
                results.CopyTo(resultsArray);

                return resultsArray;
            }

            private static HashSet<string> CreateAllPhrases()
            {
                var phrases = new[]
                                  {
                                      "A class containing factory methods",
                                      "A class containing methods",
                                      "A class providing factory methods",
                                      "A class providing methods",
                                      "A class that contains factory methods",
                                      "A class that contains methods",
                                      "A class that provides factory methods",
                                      "A class that provides methods",
                                      "A class which contains factory methods",
                                      "A class which contains methods",
                                      "A class which provides factory methods",
                                      "A class which provides methods",
                                      "A factory that provides methods",
                                      "A factory that provides",
                                      "A factory to provide methods",
                                      "A factory to provide",
                                      "A factory which provides methods",
                                      "A factory which provides",
                                      "A factory",
                                      "A implementation of the abstract factory pattern",
                                      "A implementation of the factory pattern",
                                      "A interface for factories",
                                      "A interface implemented by factories",
                                      "A interface of a factory",
                                      "A interface that is implemented by factories",
                                      "A interface which is implemented by factories",
                                      "A interface",
                                      "An implementation of the abstract factory pattern",
                                      "An implementation of the factory pattern",
                                      "An interface for factories",
                                      "An interface implemented by factories",
                                      "An interface of a factory",
                                      "An interface that is implemented by factories",
                                      "An interface which is implemented by factories",
                                      "An interface",
                                      "Class for factory methods",
                                      "Class for methods",
                                      "Class containing factory methods",
                                      "Class containing methods",
                                      "Class providing factory methods",
                                      "Class providing methods",
                                      "Class that contains factory methods",
                                      "Class that contains methods",
                                      "Class that provides factory methods",
                                      "Class that provides methods",
                                      "Class which contains factory methods",
                                      "Class which contains methods",
                                      "Class which provides factory methods",
                                      "Class which provides methods",
                                      "Class to provide factory methods",
                                      "Class to provide methods",
                                      "Class",
                                      "Defines a factory",
                                      "Defines a method",
                                      "Defines methods",
                                      "Defines the factory",
                                      "Defines factories",
                                      "Factory that provides methods",
                                      "Factory that provides",
                                      "Factory to provide methods",
                                      "Factory to provide",
                                      "Factory which provides methods",
                                      "Factory which provides",
                                      "Factory",
                                      "Implementation of the abstract factory pattern",
                                      "Implementation of the factory pattern",
                                      "Interface for factories",
                                      "Interface of a factory",
                                      "Interface of factories",
                                      "Interface",
                                      "Provides a factory",
                                      "Provides a method",
                                      "Provides methods",
                                      "Provides the factory",
                                      "Provides factories",
                                      "Provides",
                                      "Represents a factory",
                                      "Represents the factory",
                                      "Represents factories",
                                      "Represents a method",
                                      "Represents the method",
                                      "Represents methods",
                                      "The class containing factory methods",
                                      "The class containing methods",
                                      "The class contains factory methods",
                                      "The class contains methods",
                                      "The class provides factory methods",
                                      "The class provides methods",
                                      "The class providing factory methods",
                                      "The class providing methods",
                                      "The class that contains factory methods",
                                      "The class that contains methods",
                                      "The class which contains factory methods",
                                      "The class which contains methods",
                                      "The class that provides factory methods",
                                      "The class that provides methods",
                                      "The class which provides factory methods",
                                      "The class which provides methods",
                                      "The factory that provides methods",
                                      "The factory that provides",
                                      "The factory to provide methods",
                                      "The factory to provide",
                                      "The factory which provides methods",
                                      "The factory which provides",
                                      "The factory providing factory methods",
                                      "The factory providing methods",
                                      "The factory",
                                      "The implementation of the abstract factory pattern",
                                      "The implementation of the factory pattern",
                                      "The interface for factories",
                                      "The interface implemented by factories",
                                      "The interface of a factory",
                                      "The interface that is implemented by factories",
                                      "The interface which is implemented by factories",
                                      "The interface",
                                      "This class containing factory methods",
                                      "This class containing methods",
                                      "This class contains factory methods",
                                      "This class contains methods",
                                      "This class provides factory methods",
                                      "This class provides methods",
                                      "This class providing factory methods",
                                      "This class providing methods",
                                      "This factory provides methods",
                                      "This factory",
                                      "This interface is implemented by factories",
                                      "Used",
                                      "Uses", // typo in 'Used'
                                  };

                var results = new HashSet<string>(phrases);

                foreach (var phrase in phrases)
                {
                    results.Add(phrase.Replace("actory", "actory class"));
                }

                return results;
            }

            private static IEnumerable<string> CreateAllContinuations()
            {
                var continuations = new[]
                                        {
                                            string.Empty,
                                            "a ",
                                            "a instance of a ",
                                            //// "a instances of a ", // currently ignored as this contains typos which we did not see in the wild
                                            "a new instance of a ",
                                            //// "a new instances of a ", // currently ignored as this contains typos which we did not see in the wild
                                            "an ",
                                            "an instance of an ",
                                            //// "an instances of an ", // currently ignored as this contains typos which we did not see in the wild
                                            //// "an new instance of an ", // currently ignored as this contains typos which we did not see in the wild
                                            //// "an new instances of an ", // currently ignored as this contains typos which we did not see in the wild
                                            "instance of ",
                                            "instances of ",
                                            "new instance of ",
                                            "new instances of ",
                                            "the ",
                                            "the instance of the ",
                                            "the instances of the ",
                                            "the new instance of the ",
                                            "the new instances of the ",
                                        };

                foreach (var continuation in continuations)
                {
                    yield return " that can build " + continuation;
                    yield return " that build " + continuation;
                    yield return " that builds " + continuation;
                    yield return " that can construct " + continuation;
                    yield return " that construct " + continuation;
                    yield return " that constructs " + continuation;
                    yield return " that can create " + continuation;
                    yield return " that create " + continuation;
                    yield return " that creates " + continuation;
                    yield return " that can provide " + continuation;
                    yield return " that provide " + continuation;
                    yield return " that provides " + continuation;
                    yield return " that " + continuation;

                    yield return " which can build " + continuation;
                    yield return " which build " + continuation;
                    yield return " which builds " + continuation;
                    yield return " which can construct " + continuation;
                    yield return " which construct " + continuation;
                    yield return " which constructs " + continuation;
                    yield return " which can create " + continuation;
                    yield return " which create " + continuation;
                    yield return " which creates " + continuation;
                    yield return " which can provide " + continuation;
                    yield return " which provide " + continuation;
                    yield return " which provides " + continuation;
                    yield return " which " + continuation;

                    yield return " for building of " + continuation;
                    yield return " for building " + continuation;
                    yield return " for the building of " + continuation;
                    yield return " for constructing " + continuation;
                    yield return " for construction of " + continuation;
                    yield return " for the construction of " + continuation;
                    yield return " for creating " + continuation;
                    yield return " for creation of " + continuation;
                    yield return " for the creation of " + continuation;
                    yield return " for providing of " + continuation;
                    yield return " for providing " + continuation;
                    yield return " for " + continuation;

                    yield return " building " + continuation;
                    yield return " builds " + continuation;
                    yield return " constructing " + continuation;
                    yield return " constructs " + continuation;
                    yield return " creating " + continuation;
                    yield return " creates " + continuation;
                    yield return " providing " + continuation;
                    yield return " provides " + continuation;

                    yield return " that is able to build " + continuation;
                    yield return " which is able to build " + continuation;
                    yield return " that is capable to build " + continuation;
                    yield return " which is capable to build " + continuation;
                    yield return " that is able to construct " + continuation;
                    yield return " which is able to construct " + continuation;
                    yield return " that is capable to construct " + continuation;
                    yield return " which is capable to construct " + continuation;
                    yield return " that is able to create " + continuation;
                    yield return " which is able to create " + continuation;
                    yield return " that is capable to create " + continuation;
                    yield return " which is capable to create " + continuation;
                    yield return " that is able to provide " + continuation;
                    yield return " which is able to provide " + continuation;
                    yield return " that is capable to provide " + continuation;
                    yield return " which is capable to provide " + continuation;

                    yield return " that are able to build " + continuation;
                    yield return " which are able to build " + continuation;
                    yield return " that are capable to build " + continuation;
                    yield return " which are capable to build " + continuation;
                    yield return " that are able to construct " + continuation;
                    yield return " which are able to construct " + continuation;
                    yield return " that are capable to construct " + continuation;
                    yield return " which are capable to construct " + continuation;
                    yield return " that are able to create " + continuation;
                    yield return " which are able to create " + continuation;
                    yield return " that are capable to create " + continuation;
                    yield return " which are capable to create " + continuation;
                    yield return " that are able to provide " + continuation;
                    yield return " which are able to provide " + continuation;
                    yield return " that are capable to provide " + continuation;
                    yield return " which are capable to provide " + continuation;

                    yield return " to build " + continuation;
                    yield return " to construct " + continuation;
                    yield return " to create " + continuation;
                    yield return " to provide factory methods to build " + continuation;
                    yield return " to provide factory methods to construct " + continuation;
                    yield return " to provide factory methods to create " + continuation;
                    yield return " to provide methods to build " + continuation;
                    yield return " to provide methods to construct " + continuation;
                    yield return " to provide methods to create " + continuation;
                    yield return " to provide " + continuation;
                    yield return " to " + continuation;

                    yield return " " + continuation;
                }
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}