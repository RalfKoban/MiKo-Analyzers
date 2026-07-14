using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
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

            var keys = Enumerable.Empty<string>()
                                 .Concat(mappedData.TypeReplacementMapAc.Keys)
                                 .Concat(mappedData.TypeReplacementMapAf.Keys)
                                 .Concat(mappedData.TypeReplacementMapAi.Keys)
                                 .Concat(mappedData.TypeReplacementMapAn.Keys)
                                 .Concat(mappedData.TypeReplacementMapAx.Keys)
                                 .Concat(mappedData.TypeReplacementMapC.Keys)
                                 .Concat(mappedData.TypeReplacementMapD.Keys)
                                 .Concat(mappedData.TypeReplacementMapF.Keys)
                                 .Concat(mappedData.TypeReplacementMapI.Keys)
                                 .Concat(mappedData.TypeReplacementMapP.Keys)
                                 .Concat(mappedData.TypeReplacementMapR.Keys)
                                 .Concat(mappedData.TypeReplacementMapTheC.Keys)
                                 .Concat(mappedData.TypeReplacementMapTheF.Keys)
                                 .Concat(mappedData.TypeReplacementMapTheI.Keys)
                                 .Concat(mappedData.TypeReplacementMapTheX.Keys)
                                 .Concat(mappedData.TypeReplacementMapThis.Keys)
                                 .Concat(mappedData.TypeReplacementMapOthers.Keys)
                                 .Concat(mappedData.MethodReplacementMapA.Keys)
                                 .Concat(mappedData.MethodReplacementMapThe.Keys)
                                 .Concat(mappedData.MethodReplacementMapThis.Keys)
                                 .Concat(mappedData.MethodReplacementMapOthers.Keys)
                                 .Concat(mappedData.FunctionReplacementMapA.Keys)
                                 .Concat(mappedData.FunctionReplacementMapThe.Keys)
                                 .Concat(mappedData.FunctionReplacementMapThis.Keys)
                                 .Concat(mappedData.FunctionReplacementMapOthers.Keys)
                                 .Concat(mappedData.InstancesReplacementMapA.Keys)
                                 .Concat(mappedData.InstancesReplacementMapThe.Keys)
                                 .Concat(mappedData.InstancesReplacementMapThis.Keys)
                                 .Concat(mappedData.InstancesReplacementMapOthers.Keys)
                                 .Concat(mappedData.InstancesFunctionReplacementMapA.Keys)
                                 .Concat(mappedData.InstancesFunctionReplacementMapThe.Keys)
                                 .Concat(mappedData.InstancesFunctionReplacementMapThis.Keys)
                                 .Concat(mappedData.InstancesFunctionReplacementMapOthers.Keys);

            return text.StartsWithAny(keys, StringComparison.OrdinalIgnoreCase);
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
                            return GetUpdatedTypeComment(summary);

                        case SyntaxKind.MethodDeclaration:
                            return GetUpdatedMethodComment(summary, (MethodDeclarationSyntax)ancestor);
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

        private static XmlElementSyntax GetUpdatedTypeComment(XmlElementSyntax summary)
        {
            var mappedData = MappedData.Value;

            var preparedComment = PrepareTypeComment(summary, mappedData);

            if (ReferenceEquals(preparedComment, summary))
            {
                preparedComment = PrepareInstancesComment(summary, mappedData);
            }

            var fixedComment = CommentStartingWith(preparedComment, Constants.Comments.FactorySummaryPhrase);

            return CleanupComment(fixedComment);
        }

        private static XmlElementSyntax GetUpdatedMethodComment(XmlElementSyntax summary, MethodDeclarationSyntax method)
        {
            var preparedComment = PrepareMethodComment(summary);

            var template = Constants.Comments.FactoryCreateMethodSummaryStartingPhraseTemplate;
            var returnType = method.ReturnType;

            if (returnType is GenericNameSyntax g && g.TypeArgumentList.Arguments.Count is 1)
            {
                template = Constants.Comments.FactoryCreateCollectionMethodSummaryStartingPhraseTemplate;
                returnType = g.TypeArgumentList.Arguments[0];
            }

            var parts = template.FormatWith("|").Split('|');

            var fixedComment = CommentStartingWith(preparedComment, parts[0], SeeCref(returnType), parts[1]);

            return CleanupComment(fixedComment);
        }

        private static XmlElementSyntax PrepareTypeComment(XmlElementSyntax comment, MapData data)
        {
            var textTokens = comment.GetXmlTextTokens();

            if (textTokens.Count is 0)
            {
                return comment;
            }

            XmlElementSyntax updated;

            var startText = textTokens[0].ValueText.AsSpan().TrimStart();

            var map = GetApplicableTypeMap(startText, data);

            if (map != null)
            {
                updated = Comment(comment, map);

                if (ReferenceEquals(updated, comment) is false)
                {
                    // has been replaced, so nothing more to do
                    return updated;
                }
            }

            updated = Comment(comment, data.TypeReplacementMapOthers);

            if (ReferenceEquals(updated, comment) is false)
            {
                // has been replaced, so nothing more to do
                return updated;
            }

            return comment;
        }

        private static ReplacementMap GetApplicableTypeMap(in ReadOnlySpan<char> startText, MapData data)
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
                                return data.TypeReplacementMapAc;
                            case 'f':
                            case 'F':
                                return data.TypeReplacementMapAf;
                            case 'i':
                            case 'I':
                                return data.TypeReplacementMapAi;
                            default:
                                return data.TypeReplacementMapAx;
                        }
                    }

                    return data.TypeReplacementMapAn;
                }

                case 'C':
                case 'c':
                    return data.TypeReplacementMapC;

                case 'D':
                case 'd':
                    return data.TypeReplacementMapD;

                case 'F':
                case 'f':
                    return data.TypeReplacementMapF;

                case 'I':
                case 'i':
                    return data.TypeReplacementMapI;

                case 'P':
                case 'p':
                    return data.TypeReplacementMapP;

                case 'R':
                case 'r':
                    return data.TypeReplacementMapR;

                case 'T':
                case 't':
                    if (startText.Length > 4 && (startText[2] is 'e' || startText[2] is 'E'))
                    {
                        switch (startText[4])
                        {
                            case 'c':
                            case 'C':
                                return data.TypeReplacementMapTheC;
                            case 'f':
                            case 'F':
                                return data.TypeReplacementMapTheF;
                            case 'i':
                            case 'I':
                                return data.TypeReplacementMapTheI;
                            default:
                                return data.TypeReplacementMapTheX;
                        }
                    }

                    return data.TypeReplacementMapThis;

                default:
                    return null;
            }
        }

        private static XmlElementSyntax PrepareInstancesComment(XmlElementSyntax summary, MapData data)
        {
            var textTokens = summary.GetXmlTextTokens();

            if (textTokens.Count > 0)
            {
                var startText = textTokens[0].ValueText.AsSpan().TrimStart();
                var map = GetApplicableInstancesMap(startText, data);

                if (map != null)
                {
                    return Comment(summary, map);
                }
            }

            return summary;
        }

        private static ReplacementMap GetApplicableInstancesMap(in ReadOnlySpan<char> startText, MapData data)
        {
            var isFunction = startText.Contains("unction".AsSpan());

            switch (startText[0])
            {
                case 'A':
                case 'a':
                {
                    return isFunction ? data.InstancesFunctionReplacementMapA : data.InstancesReplacementMapA;
                }

                case 'T':
                case 't':
                    if (startText.Length > 4 && (startText[2] is 'e' || startText[2] is 'E'))
                    {
                        return isFunction ? data.InstancesFunctionReplacementMapThe : data.InstancesReplacementMapThe;
                    }

                    return isFunction ? data.InstancesFunctionReplacementMapThis : data.InstancesReplacementMapThis;

                default:
                    return isFunction ? data.InstancesFunctionReplacementMapOthers : data.InstancesReplacementMapOthers;
            }
        }

        private static XmlElementSyntax PrepareMethodComment(XmlElementSyntax comment)
        {
            var textTokens = comment.GetXmlTextTokens();

            if (textTokens.Count > 0)
            {
                var startText = textTokens[0].ValueText.AsSpan().TrimStart();
                var map = GetApplicableMethodMap(startText, MappedData.Value);

                if (map != null)
                {
                    var preparedComment = Comment(comment, map);

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
            }

            return comment;
        }

        private static ReplacementMap GetApplicableMethodMap(in ReadOnlySpan<char> startText, MapData data)
        {
            var isFunction = startText.Contains("unction".AsSpan());

            switch (startText[0])
            {
                case 'A':
                case 'a':
                {
                    return isFunction ? data.FunctionReplacementMapA : data.MethodReplacementMapA;
                }

                case 'T':
                case 't':
                    if (startText.Length > 4 && (startText[2] is 'e' || startText[2] is 'E'))
                    {
                        return isFunction ? data.FunctionReplacementMapThe : data.MethodReplacementMapThe;
                    }

                    return isFunction ? data.FunctionReplacementMapThis : data.MethodReplacementMapThis;

                default:
                    return isFunction ? data.FunctionReplacementMapOthers : data.MethodReplacementMapOthers;
            }
        }

        private static XmlElementSyntax CleanupComment(XmlElementSyntax comment) => Comment(comment, MappedData.Value.CleanupReplacementMap);

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
            public readonly ReplacementMap MethodReplacementMapA;
            public readonly ReplacementMap MethodReplacementMapThe;
            public readonly ReplacementMap MethodReplacementMapThis;
            public readonly ReplacementMap MethodReplacementMapOthers;
            public readonly ReplacementMap FunctionReplacementMapA;
            public readonly ReplacementMap FunctionReplacementMapThe;
            public readonly ReplacementMap FunctionReplacementMapThis;
            public readonly ReplacementMap FunctionReplacementMapOthers;
            public readonly ReplacementMap InstancesReplacementMapA;
            public readonly ReplacementMap InstancesReplacementMapThe;
            public readonly ReplacementMap InstancesReplacementMapThis;
            public readonly ReplacementMap InstancesReplacementMapOthers;
            public readonly ReplacementMap InstancesFunctionReplacementMapA;
            public readonly ReplacementMap InstancesFunctionReplacementMapThe;
            public readonly ReplacementMap InstancesFunctionReplacementMapThis;
            public readonly ReplacementMap InstancesFunctionReplacementMapOthers;
            public readonly ReplacementMap CleanupReplacementMap;
#pragma warning restore SA1401 // Fields should be private

            public MapData()
            {
                var typeKeys = CreateTypeReplacementMapKeys();

                var typeKeysStartingWithAc = new List<string>(23016); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAf = new List<string>(14820); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAi = new List<string>(13222); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAx = new List<string>(0); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithAn = new List<string>(13222); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithC = new List<string>(31892); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithD = new List<string>(10826); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithF = new List<string>(14820); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithI = new List<string>(11158); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithP = new List<string>(11066); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithR = new List<string>(13524); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheC = new List<string>(30688); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheF = new List<string>(19570); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheI = new List<string>(13222); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithTheX = new List<string>(0); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysStartingWithThis = new List<string>(21798); // TODO RKN: Adjust number as soon as there are other texts
                var typeKeysOther = new List<string>(1062); // TODO RKN: Adjust number as soon as there are other texts

                foreach (var key in typeKeys)
                {
                    GetTypeDestinationList(key.AsSpan()).Add(key);
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

                var methodKeysStartingWithA = new List<string>(12128); // TODO RKN: Adjust number as soon as there are other texts
                var methodKeysStartingWithThe = new List<string>(12472); // TODO RKN: Adjust number as soon as there are other texts
                var methodKeysStartingWithThis = new List<string>(12472); // TODO RKN: Adjust number as soon as there are other texts
                var methodKeysOther = new List<string>(15936); // TODO RKN: Adjust number as soon as there are other texts

                var functionKeysStartingWithA = new List<string>(12128); // TODO RKN: Adjust number as soon as there are other texts
                var functionKeysStartingWithThe = new List<string>(12472); // TODO RKN: Adjust number as soon as there are other texts
                var functionKeysStartingWithThis = new List<string>(12472); // TODO RKN: Adjust number as soon as there are other texts
                var functionKeysOther = new List<string>(12128); // TODO RKN: Adjust number as soon as there are other texts

                var endingsToSkip = new[] { "instance of a ", "instance of an ", "instances of a ", "instances of an " };

                foreach (var key in methodKeys.SkipWhere(_ => _.EndsWithAny(endingsToSkip)))
                {
                    GetMethodDestinationList(key.AsSpan()).Add(key);
                }

                Initialize("MiKo_2060_Methods_A", methodKeysStartingWithA, out MethodReplacementMapA);
                Initialize("MiKo_2060_Methods_The", methodKeysStartingWithThe, out MethodReplacementMapThe);
                Initialize("MiKo_2060_Methods_This", methodKeysStartingWithThis, out MethodReplacementMapThis);
                Initialize("MiKo_2060_Methods_Other", methodKeysOther, out MethodReplacementMapOthers);

                Initialize("MiKo_2060_Functions_A", functionKeysStartingWithA, out FunctionReplacementMapA);
                Initialize("MiKo_2060_Functions_The", functionKeysStartingWithThe, out FunctionReplacementMapThe);
                Initialize("MiKo_2060_Functions_This", functionKeysStartingWithThis, out FunctionReplacementMapThis);
                Initialize("MiKo_2060_Functions_Other", functionKeysOther, out FunctionReplacementMapOthers);

                var instancesKeysStartingWithA = new List<string>(21004); // TODO RKN: Adjust number as soon as there are other texts
                var instancesKeysStartingWithThe = new List<string>(21600); // TODO RKN: Adjust number as soon as there are other texts
                var instancesKeysStartingWithThis = new List<string>(21600); // TODO RKN: Adjust number as soon as there are other texts
                var instancesKeysOther = new List<string>(27598); // TODO RKN: Adjust number as soon as there are other texts

                var instancesFunctionKeysStartingWithA = new List<string>(21004); // TODO RKN: Adjust number as soon as there are other texts
                var instancesFunctionKeysStartingWithThe = new List<string>(21600); // TODO RKN: Adjust number as soon as there are other texts
                var instancesFunctionKeysStartingWithThis = new List<string>(21600); // TODO RKN: Adjust number as soon as there are other texts
                var instancesFunctionKeysOther = new List<string>(21004); // TODO RKN: Adjust number as soon as there are other texts

                foreach (var key in methodKeys)
                {
                    GetInstancesDestinationList(key.AsSpan()).Add(key);
                }

                const string InstancesReplacement = "instances of the ";

                Initialize("MiKo_2060_instances_A", instancesKeysStartingWithA, InstancesReplacement, out InstancesReplacementMapA);
                Initialize("MiKo_2060_instances_The", instancesKeysStartingWithThe, InstancesReplacement, out InstancesReplacementMapThe);
                Initialize("MiKo_2060_instances_This", instancesKeysStartingWithThis, InstancesReplacement, out InstancesReplacementMapThis);
                Initialize("MiKo_2060_instances_Other", instancesKeysOther, InstancesReplacement, out InstancesReplacementMapOthers);

                Initialize("MiKo_2060_instances_functions_A", instancesFunctionKeysStartingWithA, InstancesReplacement, out InstancesFunctionReplacementMapA);
                Initialize("MiKo_2060_instances_functions_The", instancesFunctionKeysStartingWithThe, InstancesReplacement, out InstancesFunctionReplacementMapThe);
                Initialize("MiKo_2060_instances_functions_This", instancesFunctionKeysStartingWithThis, InstancesReplacement, out InstancesFunctionReplacementMapThis);
                Initialize("MiKo_2060_instances_functions_Other", instancesFunctionKeysOther, InstancesReplacement, out InstancesFunctionReplacementMapOthers);

                CleanupReplacementMap = new ReplacementMap(
                                                       "MiKo_2060_Cleanup",
                                                       new[]
                                                           {
                                                               new Pair("classes", "types"),
                                                               new Pair("class", "type"),
                                                               new Pair("typeifi", "classifi"), // fix typo when 'class' in 'classification' or 'classified' gets changed into 'type'
                                                               new Pair(" based on ", " default values for "),
                                                               new Pair(" based upon ", " default values for "),
                                                               new Pair(" with for ", " with "),
                                                               new Pair(" with with ", " with "),
                                                               new Pair(" with result", " with a result"),
                                                               new Pair(" type with factory", " type with default values"),
                                                               new Pair(" type with type.", " type with default values."),
                                                               new Pair(" type with that ", " type with default values that "),
                                                               new Pair(" type with which ", " type with default values which "),
                                                               new Pair(" creating creates ", " creating "),
                                                               new Pair(" creating create ", " creating "),

                                                               // fix-ups for instances map
                                                               new Pair(" of the s ", " "),
                                                               new Pair(" of the  a new ", " of the "),
                                                               new Pair(" of the  an new ", " of the "),
                                                               new Pair(" of the  new ", " of the "),
                                                               new Pair(" of the  a ", " of the "),
                                                               new Pair(" of the  an ", " of the "),
                                                               new Pair(" of the  and return ", " of "),
                                                               new Pair(" of the  and provide ", " of "),
                                                               new Pair(" of the  and initialize ", " of "),
                                                               new Pair(" of the  and set up ", " "),
                                                               new Pair(" of the  instances of ", " of "),
                                                               new Pair(" of the  instances of the ", " of the "),
                                                               new Pair(" instances and sets up ", " instances "),
                                                               new Pair(" instances and provides ", " instances "),
                                                               new Pair(" instances and initializes ", " instances "),
                                                               new Pair(" instances and returns ", " instances "),
                                                               new Pair(" instances the ", " instances of the "),
                                                               new Pair(" instances a instance of ", " instances of "),
                                                               new Pair(" instances an instance of ", " instances of "),
                                                               new Pair(" instances a new instance of ", " instances of "),
                                                               new Pair(" of the instances of ", " of the "),
                                                               new Pair(" of the instance of ", " of the "),
                                                               new Pair(" instances of ", " instances of the "),
                                                               new Pair(" instances a ", " instances of the "),
                                                               new Pair(" instances new ", " instances of the "),
                                                               new Pair(" instances of the new ", " instances of the "),
                                                               new Pair(" instances of the instances of ", " instances of the "),
                                                               new Pair(" instances instances ", " instances "),
                                                               new Pair(" of the a new instance of ", " of the "),
                                                               new Pair(" of the a new ", " of the "),
                                                               new Pair(" of the a instance of ", " of the "),
                                                               new Pair(" of the an instance of ", " of the "),
                                                               new Pair(" of the a ", " of the "),
                                                               new Pair("the the the", "the"),
                                                               new Pair("the the", "the"),
                                                               new Pair("  ", " "),
                                                           },
                                                       _ => _.ToArray(__ => __.Key));

                return;

                List<string> GetTypeDestinationList(in ReadOnlySpan<char> typeKey)
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

                List<string> GetMethodDestinationList(in ReadOnlySpan<char> typeKey)
                {
                    var isFunction = typeKey.Contains("unction".AsSpan());

                    switch (typeKey[0])
                    {
                        case 'A':
                        case 'a':
                            return isFunction ? functionKeysStartingWithA : methodKeysStartingWithA;

                        case 'T':
                        case 't':
                        {
                            if (typeKey[2] is 'i')
                            {
                                return isFunction ? functionKeysStartingWithThis : methodKeysStartingWithThis;
                            }

                            return isFunction ? functionKeysStartingWithThe : methodKeysStartingWithThe;
                        }

                        default:
                            return isFunction ? functionKeysOther : methodKeysOther;
                    }
                }

                List<string> GetInstancesDestinationList(in ReadOnlySpan<char> typeKey)
                {
                    var isFunction = typeKey.Contains("unction".AsSpan());

                    switch (typeKey[0])
                    {
                        case 'A':
                        case 'a':
                            return isFunction ? instancesFunctionKeysStartingWithA : instancesKeysStartingWithA;

                        case 'T':
                        case 't':
                        {
                            if (typeKey[2] is 'i')
                            {
                                return isFunction ? instancesFunctionKeysStartingWithThis : instancesKeysStartingWithThis;
                            }

                            return isFunction ? instancesFunctionKeysStartingWithThe : instancesKeysStartingWithThe;
                        }

                        default:
                            return isFunction ? instancesFunctionKeysOther : instancesKeysOther;
                    }
                }
            }

            private static void Initialize(string id, List<string> keys, out ReplacementMap map) => Initialize(id, keys, string.Empty, out map);

            private static void Initialize(string id, List<string> keys, string replacement, out ReplacementMap map)
            {
                keys.Sort(AscendingStringComparer.Default);

                var pairs = new Pair[keys.Count];

                for (var i = keys.Count - 1; i >= 0; i--)
                {
                    pairs[i] = new Pair(keys[i], replacement);
                }

                map = new ReplacementMap(id, pairs, _ => GetTermsForQuickLookup(_, quickLookupMode: QuickLookupMode.Contains));
            }

            private static HashSet<string> CreateTypeReplacementMapKeys()
            {
                var allPhrases = CreateAllPhrases();
                var allContinuations = GetAllContinuations();

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

                var rawStrangeTexts = new[]
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
                                              "es that's ", "ethods that's ",
                                              "es that builds ", "ethods that builds ", "es which builds ", "ethods which builds ",
                                              "es that constructs ", "ethods that constructs ", "es which constructs ", "ethods which constructs ",
                                              "es that creates ", "ethods that creates ", "es which creates ", "ethods which creates ",
                                              "es that gets ", "ethods that gets ", "es which gets ", "ethods which gets ",
                                              "es that provides ", "ethods that provides ", "es which provides ", "ethods which provides ",
                                              "es that returns ", "ethods that returns ", "es which returns ", "ethods which returns ",
                                              //// accept phrases such as "to provide that/which is/are" as they are unusual but valid texts
                                          };
                var strangeTextsSet = new HashSet<string>(rawStrangeTexts);

                foreach (var raw in rawStrangeTexts)
                {
                    if (raw.AsSpan().Contains("ethod".AsSpan()))
                    {
                        var function = raw.AsCachedBuilder()
                                          .Replace("method", "function")
                                          .Replace("Method", "Function")
                                          .Replace("ethod", "unction")
                                          .ToStringAndRelease();

                        strangeTextsSet.Add(function);
                    }
                }

                var strangeTexts = strangeTextsSet.OrderDescendingByLengthAndText();

                results.RemoveWhere(_ => _.AsSpan().ContainsAnyOrdinal(strangeTexts));

                return results;
            }

            private static HashSet<string> CreateMethodReplacementMapKeys()
            {
                var subjects = new[]
                                   {
                                       "A factory method",
                                       "A method",
                                       "Factory method",
                                       "Method",
                                       "The factory method",
                                       "The method",
                                       "This factory method",
                                       "This method",
                                   };

                var startingPhrases = new List<string>((5 * 6 * subjects.Length) + (2 * 6));

                foreach (var subject in subjects)
                {
                    startingPhrases.Add(subject + " builds ");
                    startingPhrases.Add(subject + " constructs ");
                    startingPhrases.Add(subject + " creates ");
                    startingPhrases.Add(subject + " gets ");
                    startingPhrases.Add(subject + " returns ");
                    startingPhrases.Add(subject + " initializes ");

                    startingPhrases.Add(subject + " for building ");
                    startingPhrases.Add(subject + " for constructing ");
                    startingPhrases.Add(subject + " for creating ");
                    startingPhrases.Add(subject + " for getting ");
                    startingPhrases.Add(subject + " for returning ");
                    startingPhrases.Add(subject + " for initializing ");

                    startingPhrases.Add(subject + " that builds ");
                    startingPhrases.Add(subject + " that constructs ");
                    startingPhrases.Add(subject + " that creates ");
                    startingPhrases.Add(subject + " that gets ");
                    startingPhrases.Add(subject + " that returns ");
                    startingPhrases.Add(subject + " that initializes ");

                    startingPhrases.Add(subject + " which builds ");
                    startingPhrases.Add(subject + " which constructs ");
                    startingPhrases.Add(subject + " which creates ");
                    startingPhrases.Add(subject + " which gets ");
                    startingPhrases.Add(subject + " which returns ");
                    startingPhrases.Add(subject + " which initializes ");

                    startingPhrases.Add(subject + " building ");
                    startingPhrases.Add(subject + " constructing ");
                    startingPhrases.Add(subject + " creating ");
                    startingPhrases.Add(subject + " getting ");
                    startingPhrases.Add(subject + " returning ");
                    startingPhrases.Add(subject + " initializing ");
                }

                startingPhrases.Add("Used for building ");
                startingPhrases.Add("Used for constructing ");
                startingPhrases.Add("Used for creating ");
                startingPhrases.Add("Used for getting ");
                startingPhrases.Add("Used for returning ");
                startingPhrases.Add("Used for initializing ");

                startingPhrases.Add("Used to build ");
                startingPhrases.Add("Used to construct ");
                startingPhrases.Add("Used to create ");
                startingPhrases.Add("Used to get ");
                startingPhrases.Add("Used to return ");
                startingPhrases.Add("Used to initialize ");

                // avoid duplicates
                var results = new HashSet<string>();

                foreach (var phrase in startingPhrases.Concat(startingPhrases.Select(_ => _.AsCachedBuilder().Replace("s ", " ").Replace("Thi ", "This ").ToStringAndRelease())))
                {
                    results.Add(phrase);
                    results.Add(phrase.Replace("method", "function"));
                    results.Add(phrase.Replace("Method", "Function"));
                }

                results.Add("Create");
                results.Add("Creates");
                results.Add("Creating");
                results.Add("Construct");
                results.Add("Constructs");
                results.Add("Constructing");
                results.Add("Return");
                results.Add("Returns");
                results.Add("Returning");
                results.Add("Get");
                results.Add("Gets");
                results.Add("Getting");
                results.Add("Initialize");
                results.Add("Initializes");
                results.Add("Initializing");

                var startingWords = results.ToArray();

                var continuations = new[]
                                        {
                                            string.Empty,
                                            " and initialize",
                                            " and initializes",
                                            " and initializing",
                                            " and provide",
                                            " and provides",
                                            " and providing",
                                            " and return",
                                            " and returns",
                                            " and returning",
                                            " and set up",
                                            " and sets up",
                                            " and setting up",
                                        };

                var startingWordsLength = startingWords.Length;
                var continuationsLength = continuations.Length;

                for (var wordIndex = 0; wordIndex < startingWordsLength; wordIndex++)
                {
                    var word = startingWords[wordIndex].Trim();

                    for (var continuationsIndex = 0; continuationsIndex < continuationsLength; continuationsIndex++)
                    {
                        var continuation = continuations[continuationsIndex];
                        var start = word + continuation; // TODO RKN: Change string creation

                        //// results.Add(start + " an new instances of the "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " an new instances of an "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " an new instances of a "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " an new instances of "); // currently ignored as this contains typos which we did not see in the wild
                        results.Add(start + " an new instance of the ");
                        results.Add(start + " an new instance of an ");
                        results.Add(start + " an new instance of a ");
                        results.Add(start + " an new instance of ");
                        //// results.Add(start + " an instances of the "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " an instances of an "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " an instances of a "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " an instances of "); // currently ignored as this contains typos which we did not see in the wild
                        results.Add(start + " an instance of the ");
                        results.Add(start + " an instance of an ");
                        results.Add(start + " an instance of a ");
                        results.Add(start + " an instance of ");
                        results.Add(start + " an ");
                        results.Add(start + " a factory ");
                        //// results.Add(start + " a new instances of the "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " a new instances of an "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " a new instances of a "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " a new instances of "); // currently ignored as this contains typos which we did not see in the wild
                        results.Add(start + " a new instance of the ");
                        results.Add(start + " a new instance of an ");
                        results.Add(start + " a new instance of a ");
                        results.Add(start + " a new instance of ");
                        //// results.Add(start + " a instances of the "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " a instances of an "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " a instances of a "); // currently ignored as this contains typos which we did not see in the wild
                        //// results.Add(start + " a instances of "); // currently ignored as this contains typos which we did not see in the wild
                        results.Add(start + " a instance of the ");
                        results.Add(start + " a instance of an ");
                        results.Add(start + " a instance of a ");
                        results.Add(start + " a instance of ");
                        results.Add(start + " a new ");
                        results.Add(start + " a ");
                        results.Add(start + " the instance of the ");
                        results.Add(start + " the instance of an ");
                        results.Add(start + " the instance of a ");
                        results.Add(start + " the instance of ");
                        results.Add(start + " the new ");
                        results.Add(start + " the ");
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

                var strangeTexts = new[]
                                       {
                                           " a factory",
                                           "ize and init", "izes and init", "izing and init",
                                           "urn and ret", "urns and ret", "urning and ret",
                                           "ing and initialize ", "ing and provide ", "ing and return ", "ing and set ",
                                           "ing and initializes", "ing and provides", "ing and returns", "ing and sets",
                                           "s and initializing", "s and providing", "s and returning", "s and setting",
                                           "unction build ", "ethod build ",
                                           "unction construct ", "ethod construct ",
                                           "unction create ", "ethod create ",
                                           "unction get ", "ethod get ",
                                           "unction initialize ", "ethod initialize ",
                                           "unction return ", "ethod return ",
                                           "onstruct and ",
                                           "A function gets ", "A method gets ",
                                           "A factory function gets ", "A factory method gets ",
                                           "Factory function gets ", "Factory method gets ",
                                           "Function gets ", "Method gets ",
                                       };

                results.RemoveWhere(_ => _.AsSpan().ContainsAnyOrdinal(strangeTexts));

                return results;
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
                    results.Add(phrase.Replace("method", "function"));
                }

                return results;
            }

            private static HashSet<string> GetAllContinuations()
            {
                var set = new HashSet<string>();

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
                    set.Add(" that can return " + continuation);
                    set.Add(" that return " + continuation);
                    set.Add(" that returns " + continuation);
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
                    set.Add(" which can return " + continuation);
                    set.Add(" which return " + continuation);
                    set.Add(" which returns " + continuation);
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
                    set.Add(" for returning of " + continuation);
                    set.Add(" for returning " + continuation);
                    set.Add(" for " + continuation);

                    set.Add(" building " + continuation);
                    set.Add(" builds " + continuation);
                    set.Add(" constructing " + continuation);
                    set.Add(" constructs " + continuation);
                    set.Add(" creating " + continuation);
                    set.Add(" creates " + continuation);
                    set.Add(" providing " + continuation);
                    set.Add(" provides " + continuation);
                    set.Add(" returning " + continuation);
                    set.Add(" returns " + continuation);

                    set.Add(" that's able to build " + continuation);
                    set.Add(" that is able to build " + continuation);
                    set.Add(" which is able to build " + continuation);
                    set.Add(" that's capable to build " + continuation);
                    set.Add(" that is capable to build " + continuation);
                    set.Add(" which is capable to build " + continuation);
                    set.Add(" that's able to construct " + continuation);
                    set.Add(" that is able to construct " + continuation);
                    set.Add(" which is able to construct " + continuation);
                    set.Add(" that's capable to construct " + continuation);
                    set.Add(" that is capable to construct " + continuation);
                    set.Add(" which is capable to construct " + continuation);
                    set.Add(" that is able to create " + continuation);
                    set.Add(" that's able to create " + continuation);
                    set.Add(" which is able to create " + continuation);
                    set.Add(" that's capable to create " + continuation);
                    set.Add(" that is capable to create " + continuation);
                    set.Add(" which is capable to create " + continuation);
                    set.Add(" that's able to provide " + continuation);
                    set.Add(" that is able to provide " + continuation);
                    set.Add(" which is able to provide " + continuation);
                    set.Add(" that's capable to provide " + continuation);
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

                return set;
            }
        }
//// ncrunch: no coverage end
//// ncrunch: rdi default
    }
}