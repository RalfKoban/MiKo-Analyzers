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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_2044_CodeFixProvider)), Shared]
    public sealed class MiKo_2044_CodeFixProvider : DocumentationCodeFixProvider
    {
        private static readonly HashSet<string> TagNames = new HashSet<string>
                                                               {
                                                                   Constants.XmlTag.See,
                                                                   Constants.XmlTag.SeeAlso,
                                                               };

        public override string FixableDiagnosticId => MiKo_2044_InvalidSeeParameterInXmlAnalyzer.Id;

        protected override string Title => "Use <paramref> tag for parameter";

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => GetXmlSyntax(syntaxNodes);

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax)
        {
            var comment = (DocumentationCommentTriviaSyntax)syntax;

            var method = comment.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();
            var parameters = method.ParameterList.Parameters.Select(_ => _.Identifier.ValueText).ToHashSet();

            var map = new Dictionary<XmlEmptyElementSyntax, string>();

            var tags = GetEmptyXmlSyntax(comment, TagNames);
            foreach (var tag in tags)
            {
                foreach (var parameterName in tag.Attributes
                                                 .SelectMany(_ => _.ChildNodes().OfType<NameMemberCrefSyntax>())
                                                 .Select(_ => _.Name.ToString())
                                                 .Where(parameters.Contains))
                {
                    map.Add(tag, parameterName);
                }
            }

            return comment.ReplaceNodes(map.Keys, (_, __) => SyntaxFactory.XmlParamRefElement(map[_]));
        }
    }
}