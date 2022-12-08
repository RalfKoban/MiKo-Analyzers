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
    public sealed class MiKo_2044_CodeFixProvider : OverallDocumentationCodeFixProvider
    {
        private static readonly HashSet<string> TagNames = new HashSet<string>
                                                               {
                                                                   Constants.XmlTag.See,
                                                                   Constants.XmlTag.SeeAlso,
                                                               };

        public override string FixableDiagnosticId => MiKo_2044_InvalidSeeParameterInXmlAnalyzer.Id;

        protected override string Title => Resources.MiKo_2044_CodeFixTitle;

        protected override DocumentationCommentTriviaSyntax GetUpdatedSyntax(CodeFixContext context, DocumentationCommentTriviaSyntax syntax, Diagnostic diagnostic)
        {
            var method = syntax.FirstAncestorOrSelf<MethodDeclarationSyntax>();

            if (method is null)
            {
                return syntax;
            }

            var parameters = method.ParameterList.Parameters.ToHashSet(_ => _.GetName());

            var map = new Dictionary<XmlEmptyElementSyntax, string>();

            var tags = syntax.GetEmptyXmlSyntax(TagNames);
            foreach (var tag in tags)
            {
                foreach (var parameterName in tag.Attributes
                                                 .SelectMany(_ => _.ChildNodes<NameMemberCrefSyntax>())
                                                 .Select(_ => _.Name.ToString())
                                                 .Where(parameters.Contains))
                {
                    map.Add(tag, parameterName);
                }
            }

            return syntax.ReplaceNodes(map.Keys, (_, __) => SyntaxFactory.XmlParamRefElement(map[_]));
        }
    }
}