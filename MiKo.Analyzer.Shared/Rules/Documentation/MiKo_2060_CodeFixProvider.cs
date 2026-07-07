using System;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

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

        public override string FixableDiagnosticId => "MiKo_2060";

        public static void LoadData() => GC.KeepAlive(MappedData.Value);

        internal static bool CanFix(in ReadOnlySpan<char> text)
        {
            var mappedData = MappedData.Value;

            return text.StartsWithAny(mappedData.TypeReplacementMapAc.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapAf.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapAi.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapAn.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapAx.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapC.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapD.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapF.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapI.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapP.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapR.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapTheC.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapTheF.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapTheI.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapTheX.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapThis.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.TypeReplacementMapOthers.Keys, StringComparison.OrdinalIgnoreCase)
                || text.StartsWithAny(mappedData.InstancesReplacementMap.Keys, StringComparison.OrdinalIgnoreCase);
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
                            var mappedData = MappedData.Value;

                            var preparedComment = PrepareTypeComment(summary, mappedData);

                            if (preparedComment == summary)
                            {
                                preparedComment = Comment(summary, mappedData.InstancesReplacementMap);
                            }

                            var fixedComment = CommentStartingWith(preparedComment, Constants.Comments.FactorySummaryPhrase);

                            return CleanupComment(fixedComment);
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

                            return CleanupComment(fixedComment);
                        }
                    }
                }
            }

            return syntax;
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxAsync(SyntaxNode syntax, Diagnostic issue, Document document, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntax(syntax);

            return Task.FromResult(updatedSyntax);
        }

        private static XmlElementSyntax PrepareTypeComment(XmlElementSyntax comment, MapData mappedData)
        {
            var textTokens = comment.GetXmlTextTokens();

            if (textTokens.Count is 0)
            {
                return comment;
            }

            XmlElementSyntax updated;

            var startText = textTokens[0].ValueText.AsSpan().TrimStart();

            var map = GetApplicableMap(startText, mappedData);

            if (map != null)
            {
                updated = Comment(comment, map);

                if (ReferenceEquals(updated, comment) is false)
                {
                    // has been replaced, so nothing more to do
                    return updated;
                }
            }

            updated = Comment(comment, mappedData.TypeReplacementMapOthers);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            return comment;
        }

        private static ReplacementMap GetApplicableMap(in ReadOnlySpan<char> startText, MapData mappedData)
        {
            switch (startText[0])
            {
                case 'A':
                case 'a':
                {
                    if (startText.Length > 2 && startText[1] is Constants.Space)
                    {
                        switch (startText[2])
                        {
                            case 'c':
                            case 'C':
                                return mappedData.TypeReplacementMapAc;
                            case 'f':
                            case 'F':
                                return mappedData.TypeReplacementMapAf;
                            case 'i':
                            case 'I':
                                return mappedData.TypeReplacementMapAi;
                            default:
                                return mappedData.TypeReplacementMapAx;
                        }
                    }

                    return mappedData.TypeReplacementMapAn;
                }

                case 'C':
                case 'c':
                    return mappedData.TypeReplacementMapC;

                case 'D':
                case 'd':
                    return mappedData.TypeReplacementMapD;

                case 'F':
                case 'f':
                    return mappedData.TypeReplacementMapF;

                case 'I':
                case 'i':
                    return mappedData.TypeReplacementMapI;

                case 'P':
                case 'p':
                    return mappedData.TypeReplacementMapP;

                case 'R':
                case 'r':
                    return mappedData.TypeReplacementMapR;

                case 'T':
                case 't':
                    if (startText.Length > 4 && (startText[2] is 'e' || startText[2] is 'E'))
                    {
                        switch (startText[4])
                        {
                            case 'c':
                            case 'C':
                                return mappedData.TypeReplacementMapTheC;
                            case 'f':
                            case 'F':
                                return mappedData.TypeReplacementMapTheF;
                            case 'i':
                            case 'I':
                                return mappedData.TypeReplacementMapTheI;
                            default:
                                return mappedData.TypeReplacementMapTheX;
                        }
                    }

                    return mappedData.TypeReplacementMapThis;

                default:
                    return null;
            }
        }

        private static XmlElementSyntax PrepareMethodComment(XmlElementSyntax comment)
        {
            var mappedData = MappedData.Value;

            var preparedComment = Comment(comment, mappedData.MethodReplacementMap);

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

        private static XmlElementSyntax CleanupComment(XmlElementSyntax comment)
        {
            var mappedData = MappedData.Value;

            return Comment(comment, mappedData.CleanupReplacementMap);
        }

//// ncrunch: rdi off
//// ncrunch: no coverage start

        private sealed class MapData
        {
#pragma warning disable SA1401 // Fields should be private
            public readonly ReplacementMap TypeReplacementMapAc;
            public readonly ReplacementMap TypeReplacementMapAf;
            public readonly ReplacementMap TypeReplacementMapAi;
            public readonly ReplacementMap TypeReplacementMapAx;
            public readonly ReplacementMap TypeReplacementMapAn;
            public readonly ReplacementMap TypeReplacementMapC;
            public readonly ReplacementMap TypeReplacementMapD;
            public readonly ReplacementMap TypeReplacementMapF;
            public readonly ReplacementMap TypeReplacementMapI;
            public readonly ReplacementMap TypeReplacementMapP;
            public readonly ReplacementMap TypeReplacementMapR;
            public readonly ReplacementMap TypeReplacementMapTheC;
            public readonly ReplacementMap TypeReplacementMapTheF;
            public readonly ReplacementMap TypeReplacementMapTheI;
            public readonly ReplacementMap TypeReplacementMapTheX;
            public readonly ReplacementMap TypeReplacementMapThis;
            public readonly ReplacementMap TypeReplacementMapOthers;
            public readonly ReplacementMap MethodReplacementMap;
            public readonly ReplacementMap InstancesReplacementMap;
            public readonly ReplacementMap CleanupReplacementMap;
#pragma warning restore SA1401 // Fields should be private

            public MapData()
            {
                var typeKeys = CreateTypeReplacementMapKeys();

                var typeKeysStartingWithAc = new List<string>(10968); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAf = new List<string>(10488); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAi = new List<string>(11212); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAx = new List<string>(0); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAn = new List<string>(11212); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithC = new List<string>(15558); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithD = new List<string>(7093); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithF = new List<string>(10488); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithI = new List<string>(9208); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithP = new List<string>(7273); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithR = new List<string>(8172); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheC = new List<string>(14624); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheF = new List<string>(13230); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheI = new List<string>(11212); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheX = new List<string>(0); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithThis = new List<string>(12192); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysOther = new List<string>(822); // TODO RKN: Adjust number as soon as there are other texts

                foreach (var typeKey in typeKeys)
                {
                    GetDestinationList(typeKey.AsSpan()).Add(typeKey);
                }

                Initialize("MiKo_2060_A_c", typeKeysStartingWithAc, out TypeReplacementMapAc);
                Initialize("MiKo_2060_A_f", typeKeysStartingWithAf, out TypeReplacementMapAf);
                Initialize("MiKo_2060_A_i", typeKeysStartingWithAi, out TypeReplacementMapAi);
                Initialize("MiKo_2060_A_", typeKeysStartingWithAx, out TypeReplacementMapAx);
                Initialize("MiKo_2060_An_", typeKeysStartingWithAn, out TypeReplacementMapAn);
                Initialize("MiKo_2060_C", typeKeysStartingWithC, out TypeReplacementMapC);
                Initialize("MiKo_2060_D", typeKeysStartingWithD, out TypeReplacementMapD);
                Initialize("MiKo_2060_F", typeKeysStartingWithF, out TypeReplacementMapF);
                Initialize("MiKo_2060_I", typeKeysStartingWithI, out TypeReplacementMapI);
                Initialize("MiKo_2060_P", typeKeysStartingWithP, out TypeReplacementMapP);
                Initialize("MiKo_2060_R", typeKeysStartingWithR, out TypeReplacementMapR);
                Initialize("MiKo_2060_The_c", typeKeysStartingWithTheC, out TypeReplacementMapTheC);
                Initialize("MiKo_2060_The_f", typeKeysStartingWithTheF, out TypeReplacementMapTheF);
                Initialize("MiKo_2060_The_i", typeKeysStartingWithTheI, out TypeReplacementMapTheI);
                Initialize("MiKo_2060_The_", typeKeysStartingWithTheX, out TypeReplacementMapTheX);
                Initialize("MiKo_2060_This_", typeKeysStartingWithThis, out TypeReplacementMapThis);
                Initialize("MiKo_2060_Other", typeKeysOther, out TypeReplacementMapOthers);

                var methodKeys = CreateMethodReplacementMapKeys();
                var methodKeysLength = methodKeys.Length;

                var methodReplacementMap = new Pair[methodKeysLength];
                var instancesReplacementMap = new Pair[methodKeysLength];

                for (var i = 0; i < methodKeysLength; i++)
                {
                    var key = methodKeys[i];

                    methodReplacementMap[i] = new Pair(key);
                    instancesReplacementMap[i] = new Pair(key, "instances of the ");
                }

                MethodReplacementMap = new ReplacementMap("MiKo_2060_methods", methodReplacementMap, _ => GetTermsForQuickLookup(_, quickLookupMode: QuickLookupMode.Contains));
                InstancesReplacementMap = new ReplacementMap("MiKo_2060_instances", instancesReplacementMap, _ => GetTermsForQuickLookup(_, quickLookupMode: QuickLookupMode.Contains));

                CleanupReplacementMap = new ReplacementMap(
                                                       "MiKo_2060_Cleanup",
                                                       new[]
                                                           {
                                                               new Pair("classes", "types"),
                                                               new Pair("class", "type"),
                                                               new Pair("typeifi", "classifi"), // fix typo when 'class' in 'classification' or 'classified' gets changed into 'type'
                                                               new Pair(" based on ", " default values for "),
                                                               new Pair(" with for ", " with "),
                                                               new Pair(" with with ", " with "),
                                                               new Pair(" type with type.", " type with default values."),
                                                               new Pair(" type with that ", " type with default values that "),
                                                               new Pair(" type with which ", " type with default values which "),
                                                               new Pair(" creating creates ", " creating "),
                                                               new Pair(" creating create ", " creating "),
                                                           },
                                                       _ => _.ToArray(__ => __.Key));

                return;

                List<string> GetDestinationList(in ReadOnlySpan<char> typeKey)
                {
                    switch (typeKey[0])
                    {
                        case 'A':
                        case 'a':
                        {
                            if (typeKey[1] is 'n')
                            {
                                return typeKeysStartingWithAn;
                            }

                            switch (typeKey[2])
                            {
                                case 'c': return typeKeysStartingWithAc;
                                case 'f': return typeKeysStartingWithAf;
                                case 'i': return typeKeysStartingWithAi;
                                default: return typeKeysStartingWithAx;
                            }
                        }

                        case 'C':
                        case 'c':
                            return typeKeysStartingWithC;

                        case 'D':
                        case 'd':
                            return typeKeysStartingWithD;

                        case 'F':
                        case 'f':
                            return typeKeysStartingWithF;

                        case 'I':
                        case 'i':
                            return typeKeysStartingWithI;

                        case 'P':
                        case 'p':
                            return typeKeysStartingWithP;

                        case 'R':
                        case 'r':
                            return typeKeysStartingWithR;

                        case 'T':
                        case 't':
                        {
                            if (typeKey[2] is 'i')
                            {
                                return typeKeysStartingWithThis;
                            }

                            switch (typeKey[4])
                            {
                                case 'c': return typeKeysStartingWithTheC;
                                case 'f': return typeKeysStartingWithTheF;
                                case 'i': return typeKeysStartingWithTheI;
                                default: return typeKeysStartingWithTheX;
                            }
                        }

                        default:
                            return typeKeysOther;
                    }
                }

                void Initialize(string id, List<string> keys, out ReplacementMap map)
                {
                    keys.Sort(AscendingStringComparer.Default);

                    var pairs = new Pair[keys.Count];

                    for (var i = keys.Count - 1; i >= 0; i--)
                    {
                        pairs[i] = new Pair(keys[i]);
                    }

                    map = new ReplacementMap(id, pairs, _ => GetTermsForQuickLookup(_, quickLookupMode: QuickLookupMode.Contains));
                }
            }

            private static HashSet<string> CreateTypeReplacementMapKeys()
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
                                           "ethods a", "ethods instance", "ethods new", "ethods the", "actory class method", "ethod that are", "ethod which are", "methods that is", "methods which is",
                                           "es that is capable", "es which is capable", "es that is able", "es which is able",
                                           "ss that are capable", "ss which are capable", "ss that are able", "ss which are able",
                                           "y that are capable", "y which are capable", "y that are able", "y which are able",
                                           "rn that are capable", "rn which are capable", "rn that are able", "rn which are able",
                                           "ace that are capable", "ace which are capable", "ace that are able", "ace which are able",
                                           "ies that provides", "ies which provides",
                                           "roviding provid", "rovides provid", "rovide provid", "rovides the factory provid", "rovides the factory class provid", "rovides which", "rovides that", "to provide to", "rovides to provid",
                                           "rovides builds", "rovides constructs", "rovides creates",
                                           "to provide builds", "to provide constructs", "to provide creates",
                                           "rovides to instance", "that provides to", "which provides to", "Provides to ",
                                           "methods to provide methods", "methods provides",
                                           "ass a ", "ass an ", "ass the ", "actory a ", "actory an ", "actory the ",
                                           "Used that ", "Used which ", "Used builds", "Used creates", "Used constructs", "Used provides",
                                           "Uses that ", "Uses which ", "Uses builds", "Uses creates", "Uses constructs", "Uses provides",
                                           "pattern a",
                                           "nterface new", "nterface to new", "s to new",
                                           "ethods to instance of", "ethods to provide factory methods",
                                           "actory builds", "actories builds", "ethods builds", "pattern builds", "nterface builds", "lass builds",
                                           "actory constructs", "actories constructs", "ethods constructs", "pattern constructs", "nterface constructs", "lass constructs",
                                           "actory creates", "actories creates", "ethods creates", "pattern creates", "nterface creates", "lass creates",
                                           "that instance", "which instance",
                                           "that a instance", "to a instance", "which a instance",
                                           "that an instance", "to an instance", "which an instance",
                                           "that the instance", "to the instance", "which the instance",
                                           "that new", "to new", "which new",
                                           "that a new", "to a new", "which a new",
                                           "that an new", "to an new", "which an new",
                                           "that the new", "to the new", "which the new",
                                           //// accept phrases such as "to provide that/which is/are" as they are unusual but valid texts
                                       };

                results.RemoveWhere(_ => _.ContainsAny(strangeTexts));

                return results;
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
                                            "Initializes",
                                            "Initialize",
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

                var result = results.ToArray();

                Array.Sort(result, AscendingStringComparer.Default);

                return result;
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

                    set.Add(Constants.SingleSpace + continuation);
                }
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}