using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2217_XmlListElementAnalyzer : OverallDocumentationAnalyzer
    {
        public const string Id = "MiKo_2217";

        private const string ListHeaderElement = "listheader";
        private const string ItemElement = "item";

        public MiKo_2217_XmlListElementAnalyzer() : base(Id)
        {
        }

        internal static IEnumerable<XmlElementSyntax> GetProblematicElements(DocumentationCommentTriviaSyntax comment) => comment.DescendantNodes().OfType<XmlElementSyntax>().Where(_ => _.IsXmlTag(Constants.XmlTag.List));

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();
            if (comment is null)
            {
                // it might be that there is no documentation comment available (while the comment XML contains something like " <member name='xyz' ...> ")
                yield break;
            }

            foreach (var list in GetProblematicElements(comment))
            {
                var listType = list.GetAttributes<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() == "type");
                if (listType is null)
                {
                    // no type specified, so it seems to be a bullet, hence analyze it
                }

                foreach (var issue in AnalyzeType(list, listType))
                {
                    yield return issue;
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeType(XmlElementSyntax list, XmlTextAttributeSyntax listType)
        {
            var type = listType.GetTextWithoutTrivia();
            switch (type.ToLowerCase())
            {
                case null: // no list type specified
                case "bullet":
                case "number":
                    return AnalyzeBulletOrNumberList(list);

                case "table":
                    return AnalyzeTable(list);

                default: // unknown type
                    return new[] { Issue(string.Empty, listType, string.Format(Resources.MiKo_2217_MessageArgument_UnknownTypeSpecified, type)) };
            }
        }

        private IEnumerable<Diagnostic> AnalyzeBulletOrNumberList(XmlElementSyntax list)
        {
            foreach (var child in list.ChildNodes().OfType<XmlElementSyntax>())
            {
                switch (child.GetName())
                {
                    case ListHeaderElement:
                    {
                        // no header allowed
                        yield return Issue(string.Empty, child, Resources.MiKo_2217_MessageArgument_NoHeaderAllowed);

                        break;
                    }

                    case ItemElement:
                    {
                        var termFound = false;
                        var descriptionFound = false;

                        foreach (var grandChild in child.ChildNodes().OfType<XmlElementSyntax>())
                        {
                            switch (grandChild.GetName())
                            {
                                case "description" when descriptionFound:
                                {
                                    yield return Issue(string.Empty, grandChild, Resources.MiKo_2217_MessageArgument_OnlySingleDescriptionAllowed); // there should be only a single description

                                    break;
                                }

                                case "description":
                                {
                                    descriptionFound = true; // a single description is allowed

                                    break;
                                }

                                case "term" when termFound:
                                {
                                    yield return Issue(string.Empty, grandChild, Resources.MiKo_2217_MessageArgument_OnlySingleTermAllowed); // there should be only a single term

                                    break;
                                }

                                case "term":
                                {
                                    termFound = true; // a single term is allowed

                                    break;
                                }
                            }
                        }

                        if (descriptionFound is false)
                        {
                            // there should be at least one term and description per item
                            yield return Issue(string.Empty, child, Resources.MiKo_2217_MessageArgument_MissingDescription);
                        }

                        break;
                    }
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeTable(XmlElementSyntax list)
        {
            foreach (var child in list.ChildNodes().OfType<XmlElementSyntax>())
            {
                switch (child.GetName())
                {
                    case ListHeaderElement:
                    case ItemElement:
                    {
                        var termFound = false;
                        var descriptionFound = false;

                        foreach (var grandChild in child.ChildNodes().OfType<XmlElementSyntax>())
                        {
                            switch (grandChild.GetName())
                            {
                                case "description" when descriptionFound:
                                {
                                    yield return Issue(string.Empty, grandChild, Resources.MiKo_2217_MessageArgument_OnlySingleDescriptionAllowed); // there should be only a single description

                                    break;
                                }

                                case "description":
                                {
                                    descriptionFound = true; // a single description is allowed

                                    break;
                                }

                                case "term":
                                {
                                    termFound = true; // multiple terms per item

                                    break;
                                }
                            }
                        }

                        if (termFound is false || descriptionFound is false)
                        {
                            // there should be at least one term and description per item
                            yield return Issue(string.Empty, child, Resources.MiKo_2217_MessageArgument_MissingTermOrDescription);
                        }

                        break;
                    }
                }
            }
        }
    }
}