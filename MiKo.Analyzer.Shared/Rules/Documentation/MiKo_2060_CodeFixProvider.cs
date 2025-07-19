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
        private static readonly Lazy<MapData> MappedData = new Lazy<MapData>();

#if !NCRUNCH // do not define a static ctor to speed up tests in NCrunch
        static MiKo_2060_CodeFixProvider() => LoadData(); // ensure that we have the object available
#endif

        public override string FixableDiagnosticId => "MiKo_2060";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

        internal static bool CanFix(in ReadOnlySpan<char> text)
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

                            if (returnType is GenericNameSyntax g && g.TypeArgumentList.Arguments.Count is 1)
                            {
                                template = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhraseTemplate;
                                returnType = g.TypeArgumentList.Arguments[0];
                            }

                            var parts = template.FormatWith("|").Split('|');

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
#pragma warning disable SA1401 // Fields should be private
            public readonly Pair[] TypeReplacementMapA;
            public readonly string[] TypeReplacementMapKeysA;
            public readonly Pair[] TypeReplacementMapCD;
            public readonly string[] TypeReplacementMapKeysCD;
            public readonly Pair[] TypeReplacementMapThe;
            public readonly string[] TypeReplacementMapKeysThe;
            public readonly Pair[] TypeReplacementMapThis;
            public readonly string[] TypeReplacementMapKeysThis;
            public readonly Pair[] TypeReplacementMapOthers;
            public readonly string[] TypeReplacementMapKeysOthers;
            public readonly Pair[] MethodReplacementMap;
            public readonly string[] MethodReplacementMapKeys;
            public readonly Pair[] InstancesReplacementMap;
            public readonly string[] InstancesReplacementMapKeys;
            public readonly Pair[] CleanupReplacementMap;
            public readonly string[] CleanupReplacementMapKeys;
#pragma warning restore SA1401 // Fields should be private

            public MapData()
            {
                var typeKeys = CreateTypeReplacementMapKeys();
                var typeKeysLength = typeKeys.Length;

                //// Array.Sort(typeKeys, AscendingStringComparer.Default);

                var typeKeysStartingWithA = new List<string>(57228); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithCD = new List<string>(30362); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithThe = new List<string>(54490); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithThis = new List<string>(16660); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysOther = new List<string>(47532); // TODO RKN: Adjust number as soon as there are other texts

                for (var index = 0; index < typeKeysLength; index++)
                {
                    var typeKey = typeKeys[index];

                    switch (typeKey[0])
                    {
                        case 'A':
                        case 'a':
                            typeKeysStartingWithA.Add(typeKey);

                            break;

                        case 'C':
                        case 'D':
                        case 'c':
                        case 'd':
                            typeKeysStartingWithCD.Add(typeKey);

                            break;

                        case 'T':
                        case 't':
                            var list = typeKey[2] is 'e' ? typeKeysStartingWithThe : typeKeysStartingWithThis;
                            list.Add(typeKey);

                            break;

                        default:
                            typeKeysOther.Add(typeKey);

                            break;
                    }
                }

                typeKeysStartingWithA.Sort(AscendingStringComparer.Default);
                typeKeysStartingWithCD.Sort(AscendingStringComparer.Default);
                typeKeysStartingWithThe.Sort(AscendingStringComparer.Default);
                typeKeysStartingWithThis.Sort(AscendingStringComparer.Default);
                typeKeysOther.Sort(AscendingStringComparer.Default);

                TypeReplacementMapA = ToArray(typeKeysStartingWithA);
                TypeReplacementMapKeysA = GetTermsForQuickLookup(typeKeysStartingWithA);

                TypeReplacementMapCD = ToArray(typeKeysStartingWithCD);
                TypeReplacementMapKeysCD = GetTermsForQuickLookup(typeKeysStartingWithCD);

                TypeReplacementMapThe = ToArray(typeKeysStartingWithThe);
                TypeReplacementMapKeysThe = GetTermsForQuickLookup(typeKeysStartingWithThe);

                TypeReplacementMapThis = ToArray(typeKeysStartingWithThis);
                TypeReplacementMapKeysThis = GetTermsForQuickLookup(typeKeysStartingWithThis);

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

            private static string[] CreateTypeReplacementMapKeys()
            {
                var allPhrases = CreateAllPhrases();
                var allContinuations = new HashSet<string>();
                FillWithAllContinuations(allContinuations);

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
                                           "ethods a", "ethods instance", "ethods new", "ethods the", "actory class method", "ethod that are", "ethod which are",
                                           "es that is capable", "es which is capable", "es that is able", "es which is able",
                                           "ss that are capable", "ss which are capable", "ss that are able", "ss which are able",
                                           "y that are capable", "y which are capable", "y that are able", "y which are able",
                                           "rn that are capable", "rn which are capable", "rn that are able", "rn which are able",
                                           "ace that are capable", "ace which are capable", "ace that are able", "ace which are able",
                                           "ies that provides", "ies which provides",
                                           "roviding provid", "rovides provid", "rovide provid",
                                       };

                results.RemoveWhere(_ => _.ContainsAny(strangeTexts, StringComparison.Ordinal));

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
                                            " and set up",
                                            " and sets up",
                                        };

                var startingWordsLength = startingWords.Length;
                var continuationsLength = continuations.Length;

                for (var wordIndex = 0; wordIndex < startingWordsLength; wordIndex++)
                {
                    var word = startingWords[wordIndex];

                    for (var continuationsIndex = 0; continuationsIndex < continuationsLength; continuationsIndex++)
                    {
                        var continuation = continuations[continuationsIndex];
                        var start = word + continuation; // TODO RKN: Change string creation

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

                return results.ToArray();
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

                for (int index = 0, phrasesLength = phrases.Length; index < phrasesLength; index++)
                {
                    results.Add(phrases[index].Replace("actory", "actory class"));
                }

                return results;
            }

            private static void FillWithAllContinuations(HashSet<string> set)
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

                for (int index = 0, continuationsLength = continuations.Length; index < continuationsLength; index++)
                {
                    var continuation = continuations[index];

                    set.Add(" that can build " + continuation);
                    set.Add(" that build " + continuation);
                    set.Add(" that builds " + continuation);
                    set.Add(" that can construct " + continuation);
                    set.Add(" that construct " + continuation);
                    set.Add(" that constructs " + continuation);
                    set.Add(" that can create " + continuation);
                    set.Add(" that create " + continuation);
                    set.Add(" that creates " + continuation);
                    set.Add(" that can provide " + continuation);
                    set.Add(" that provide " + continuation);
                    set.Add(" that provides " + continuation);
                    set.Add(" that " + continuation);

                    set.Add(" which can build " + continuation);
                    set.Add(" which build " + continuation);
                    set.Add(" which builds " + continuation);
                    set.Add(" which can construct " + continuation);
                    set.Add(" which construct " + continuation);
                    set.Add(" which constructs " + continuation);
                    set.Add(" which can create " + continuation);
                    set.Add(" which create " + continuation);
                    set.Add(" which creates " + continuation);
                    set.Add(" which can provide " + continuation);
                    set.Add(" which provide " + continuation);
                    set.Add(" which provides " + continuation);
                    set.Add(" which " + continuation);

                    set.Add(" for building of " + continuation);
                    set.Add(" for building " + continuation);
                    set.Add(" for the building of " + continuation);
                    set.Add(" for constructing " + continuation);
                    set.Add(" for construction of " + continuation);
                    set.Add(" for the construction of " + continuation);
                    set.Add(" for creating " + continuation);
                    set.Add(" for creation of " + continuation);
                    set.Add(" for the creation of " + continuation);
                    set.Add(" for providing of " + continuation);
                    set.Add(" for providing " + continuation);
                    set.Add(" for " + continuation);

                    set.Add(" building " + continuation);
                    set.Add(" builds " + continuation);
                    set.Add(" constructing " + continuation);
                    set.Add(" constructs " + continuation);
                    set.Add(" creating " + continuation);
                    set.Add(" creates " + continuation);
                    set.Add(" providing " + continuation);
                    set.Add(" provides " + continuation);

                    set.Add(" that is able to build " + continuation);
                    set.Add(" which is able to build " + continuation);
                    set.Add(" that is capable to build " + continuation);
                    set.Add(" which is capable to build " + continuation);
                    set.Add(" that is able to construct " + continuation);
                    set.Add(" which is able to construct " + continuation);
                    set.Add(" that is capable to construct " + continuation);
                    set.Add(" which is capable to construct " + continuation);
                    set.Add(" that is able to create " + continuation);
                    set.Add(" which is able to create " + continuation);
                    set.Add(" that is capable to create " + continuation);
                    set.Add(" which is capable to create " + continuation);
                    set.Add(" that is able to provide " + continuation);
                    set.Add(" which is able to provide " + continuation);
                    set.Add(" that is capable to provide " + continuation);
                    set.Add(" which is capable to provide " + continuation);

                    set.Add(" that are able to build " + continuation);
                    set.Add(" which are able to build " + continuation);
                    set.Add(" that are capable to build " + continuation);
                    set.Add(" which are capable to build " + continuation);
                    set.Add(" that are able to construct " + continuation);
                    set.Add(" which are able to construct " + continuation);
                    set.Add(" that are capable to construct " + continuation);
                    set.Add(" which are capable to construct " + continuation);
                    set.Add(" that are able to create " + continuation);
                    set.Add(" which are able to create " + continuation);
                    set.Add(" that are capable to create " + continuation);
                    set.Add(" which are capable to create " + continuation);
                    set.Add(" that are able to provide " + continuation);
                    set.Add(" which are able to provide " + continuation);
                    set.Add(" that are capable to provide " + continuation);
                    set.Add(" which are capable to provide " + continuation);

                    set.Add(" to build " + continuation);
                    set.Add(" to construct " + continuation);
                    set.Add(" to create " + continuation);
                    set.Add(" to provide factory methods to build " + continuation);
                    set.Add(" to provide factory methods to construct " + continuation);
                    set.Add(" to provide factory methods to create " + continuation);
                    set.Add(" to provide methods to build " + continuation);
                    set.Add(" to provide methods to construct " + continuation);
                    set.Add(" to provide methods to create " + continuation);
                    set.Add(" to provide " + continuation);
                    set.Add(" to " + continuation);

                    set.Add(" " + continuation);
                }
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}