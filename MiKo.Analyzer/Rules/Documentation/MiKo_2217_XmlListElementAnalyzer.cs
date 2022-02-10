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

        public MiKo_2217_XmlListElementAnalyzer() : base(Id)
        {
        }

        internal static IEnumerable<XmlElementSyntax> GetProblematicElements(DocumentationCommentTriviaSyntax comment) => comment.DescendantNodes<XmlElementSyntax>(_ => _.IsXmlTag(Constants.XmlTag.List));

        internal static XmlTextAttributeSyntax GetListType(XmlElementSyntax list) => list.GetAttributes<XmlTextAttributeSyntax>()
                                                                                         .FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Attribute.Type);

        internal static string GetListType(XmlTextAttributeSyntax listType) => listType.GetTextWithoutTrivia();

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            var comment = symbol.GetDocumentationCommentTriviaSyntax();
            if (comment is null)
            {
                // it might be that there is no documentation comment available (while the comment XML contains something like " <member name='xyz' ...> ")
                return Enumerable.Empty<Diagnostic>();
            }

            return GetProblematicElements(comment).SelectMany(AnalyzeList);
        }

        private IEnumerable<Diagnostic> AnalyzeList(XmlElementSyntax list)
        {
            var listType = GetListType(list);
            if (listType is null)
            {
                // no type specified, so it seems to be a bullet, hence analyze it
            }

            foreach (var issue in AnalyzeList(list, listType))
            {
                yield return issue;
            }
        }

        private IEnumerable<Diagnostic> AnalyzeList(XmlElementSyntax list, XmlTextAttributeSyntax listType)
        {
            var type = GetListType(listType);
            switch (type.ToLowerCase())
            {
                case null: // no list type specified
                case Constants.XmlTag.ListType.Bullet:
                case Constants.XmlTag.ListType.Number:
                    return AnalyzeBulletOrNumberList(list);

                case Constants.XmlTag.ListType.Table:
                    return AnalyzeTable(list);

                default: // unknown type
                    return new[] { Issue(string.Empty, listType, string.Format(Resources.MiKo_2217_MessageArgument_UnknownTypeSpecified, type)) };
            }
        }

        private IEnumerable<Diagnostic> AnalyzeBulletOrNumberList(XmlElementSyntax list)
        {
            foreach (var child in list.ChildNodes<XmlElementSyntax>())
            {
                switch (child.GetName())
                {
                    case Constants.XmlTag.ListHeader:
                    {
                        // no header allowed
                        yield return Issue(string.Empty, child, Resources.MiKo_2217_MessageArgument_NoHeaderAllowed);

                        break;
                    }

                    case Constants.XmlTag.Item:
                    {
                        var termFound = false;
                        var descriptionFound = false;

                        foreach (var grandChild in child.ChildNodes<XmlElementSyntax>())
                        {
                            switch (grandChild.GetName())
                            {
                                case Constants.XmlTag.Description when descriptionFound:
                                {
                                    yield return Issue(string.Empty, grandChild, Resources.MiKo_2217_MessageArgument_OnlySingleDescriptionAllowed); // there should be only a single description

                                    break;
                                }

                                case Constants.XmlTag.Description:
                                {
                                    descriptionFound = true; // a single description is allowed

                                    break;
                                }

                                case Constants.XmlTag.Term when termFound:
                                {
                                    yield return Issue(string.Empty, grandChild, Resources.MiKo_2217_MessageArgument_OnlySingleTermAllowed); // there should be only a single term

                                    break;
                                }

                                case Constants.XmlTag.Term:
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
            foreach (var child in list.ChildNodes<XmlElementSyntax>())
            {
                switch (child.GetName())
                {
                    case Constants.XmlTag.ListHeader:
                    case Constants.XmlTag.Item:
                    {
                        var termFound = false;
                        var descriptionFound = false;

                        foreach (var grandChild in child.ChildNodes<XmlElementSyntax>())
                        {
                            switch (grandChild.GetName())
                            {
                                case Constants.XmlTag.Description when descriptionFound:
                                {
                                    yield return Issue(string.Empty, grandChild, Resources.MiKo_2217_MessageArgument_OnlySingleDescriptionAllowed); // there should be only a single description

                                    break;
                                }

                                case Constants.XmlTag.Description:
                                {
                                    descriptionFound = true; // a single description is allowed

                                    break;
                                }

                                case Constants.XmlTag.Term:
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