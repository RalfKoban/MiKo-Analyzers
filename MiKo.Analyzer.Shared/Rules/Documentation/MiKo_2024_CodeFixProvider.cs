using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2024_CodeFixProvider)), Shared]
    public sealed class MiKo_2024_CodeFixProvider : ParameterDocumentationCodeFixProvider
    {
        private static readonly Pair[] FlagsReplacementMap = CreateFlagsReplacementMapKeys().Select(_ => new Pair(_))
                                                                                            .Concat(new[]
                                                                                                        {
                                                                                                            new Pair("Enum for", "specifies"),
                                                                                                            new Pair("Enum indicating", "indicates"),
                                                                                                            new Pair("enum indicating", "indicates"),
                                                                                                            new Pair("Indicator for", "indicates"),
                                                                                                            new Pair("Value indicating", "indicates"),
                                                                                                            new Pair("value indicating", "indicates"),
                                                                                                            new Pair("Value for", "specifies"),
                                                                                                        })
                                                                                            .ToArray();

        private static readonly string[] FlagsReplacementMapKeys = FlagsReplacementMap.ToArray(_ => _.Key);

        private static readonly Pair[] FlagsCleanupMap =
                                                         {
                                                             new Pair("umeration values that a ", "umeration members that specifies a "),
                                                             new Pair("umeration values that an ", "umeration members that specifies an "),
                                                             new Pair("umeration values that the ", "umeration members that specifies the "),
                                                             new Pair("umeration members that a ", "umeration members that specifies a "),
                                                             new Pair("umeration members that an ", "umeration members that specifies an "),
                                                             new Pair("umeration members that the ", "umeration members that specifies the "),
                                                             new Pair(" that toes ", " that specifies the value to "),
                                                         };

        private static readonly string[] FlagsCleanupMapKeys = GetTermsForQuickLookup(FlagsCleanupMap);

        private static readonly Pair[] ReplacementMap = CreateReplacementMapKeys().ConcatenatedWith("Specifies", "Determines").ToArray(_ => new Pair(_));

        private static readonly string[] ReplacementMapKeys = GetTermsForQuickLookup(ReplacementMap);

        public override string FixableDiagnosticId => "MiKo_2024";

        protected override XmlElementSyntax Comment(Document document, XmlElementSyntax comment, ParameterSyntax parameter, in int index, Diagnostic issue)
        {
            var phrase = GetStartingPhraseProposal(issue);

            var preparedComment = PrepareComment(comment, issue);
            var updatedComment = CommentStartingWith(preparedComment, phrase);

            return Comment(updatedComment, FlagsCleanupMapKeys, FlagsCleanupMap);
        }

        private static XmlElementSyntax PrepareComment(XmlElementSyntax comment, Diagnostic issue)
        {
            var isFlagged = bool.Parse(issue.Properties[Constants.AnalyzerCodeFixSharedData.IsFlagged]);

            // TODO RKN: Update comment base on whether we have a Flags enum or not (defined as part of the properties of the reported issue)
            if (isFlagged)
            {
                return Comment(comment, FlagsReplacementMapKeys, FlagsReplacementMap, FirstWordAdjustment.StartLowerCase | FirstWordAdjustment.MakeThirdPersonSingular);
            }

            return Comment(comment, ReplacementMapKeys, ReplacementMap, FirstWordAdjustment.StartLowerCase);
        }

        private static string[] CreateReplacementMapKeys() => new[]
                                                                  {
                                                                      "A value specifying",
                                                                      "A value that specifies",
                                                                      "A value which specifies",
                                                                      "An value specifying",
                                                                      "An value that specifies",
                                                                      "An value which specifies",
                                                                      "The value specifying",
                                                                      "The value that specifies",
                                                                      "The value which specifies",
                                                                      "One of the values which specifies",
                                                                      "One of the enumeration members which specifies",
                                                                      "One of the enumeration values which specifies",
                                                                      "A value determining",
                                                                      "A value that determines",
                                                                      "A value which determines",
                                                                      "An value determining",
                                                                      "An value that determines",
                                                                      "An value which determines",
                                                                      "The value determining",
                                                                      "The value that determines",
                                                                      "The value which determines",
                                                                      "One of the values which determines",
                                                                      "One of the enumeration members which determines",
                                                                      "One of the enumeration values which determines",
                                                                      "Indicator for",
                                                                      "Value indicating",
                                                                      "Value for",
                                                                      "Enum for",
                                                                      "Enum that indicates",
                                                                      "Enum which indicates",
                                                                      "Enum indicating",
                                                                      "enum that indicates",
                                                                      "enum which indicates",
                                                                      "enum indicating",
                                                                  };

        private static string[] CreateFlagsReplacementMapKeys() => new[]
                                                                       {
                                                                           "A value that ",
                                                                           "A value which ",
                                                                           "A value ",
                                                                           "An value that ",
                                                                           "An value which ",
                                                                           "An value ",
                                                                           "The value that ",
                                                                           "The value which ",
                                                                           "The value ",
                                                                           "A flag that ",
                                                                           "A flag which ",
                                                                           "A flag ",
                                                                           "An flag that ",
                                                                           "An flag which ",
                                                                           "An flag ",
                                                                           "The flag that ",
                                                                           "The flag which ",
                                                                           "The flag ",
                                                                           "A bitmask ",
                                                                           "Bitmask ",
                                                                           "One of enumeration members which ",
                                                                           "One of enumeration values which ",
                                                                           "One of the enumeration members which ",
                                                                           "One of the enumeration values which ",
                                                                           "One of enumeration members that ",
                                                                           "One of enumeration values that ",
                                                                           "One of the enumeration members that ",
                                                                           "One of the enumeration values that ",
                                                                           "One of the members which ",
                                                                           "One of the values which ",
                                                                           "One of the members that ",
                                                                           "One of the values that ",
                                                                           "Bitwise combination of values that ",
                                                                           "Bitwise combination of values which ",
                                                                           "Bitwise combination of the values that ",
                                                                           "Bitwise combination of the values which ",
                                                                           "Bitwise combination of enumeration members that ",
                                                                           "Bitwise combination of enumeration members which ",
                                                                           "Bitwise combination of the enumeration members that ",
                                                                           "Bitwise combination of the enumeration members which ",
                                                                           "Enum that ",
                                                                           "Enum which ",
                                                                           "enum that ",
                                                                           "enum which ",
                                                                       };
    }
}