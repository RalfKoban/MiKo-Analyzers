using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2046";

        public MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, DocumentationCommentTriviaSyntax comment)
        {
            switch (symbol)
            {
                case IMethodSymbol method:
                    return AnalyzeComment(method, comment);

                case INamedTypeSymbol type:
                    return AnalyzeComment(type, comment);

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        private static XmlAttributeSyntax FindRelevantAttribute(SyntaxNode node)
        {
            switch (node)
            {
                case XmlElementSyntax e when IsRelevantTag(e.GetName()):
                    return e.StartTag.Attributes.First();

                case XmlEmptyElementSyntax ee when IsRelevantTag(ee.GetName()):
                    return ee.Attributes.First();

                default:
                    return null;
            }
        }

        private static bool IsRelevantTag(string tagName)
        {
            switch (tagName)
            {
                case Constants.XmlTag.See:
                case Constants.XmlTag.SeeAlso:
                    return true;

                default:
                    return false;
            }
        }

        private static string GetAttributeValue(XmlAttributeSyntax attribute)
        {
            switch (attribute)
            {
                case XmlNameAttributeSyntax name:
                    return name.Identifier.GetName();

                case XmlCrefAttributeSyntax cref:
                    return cref.Cref.ToString();

                default:
                    return string.Empty;
            }
        }

        private static string CreateProposal(string parameterName) => string.Intern(Constants.Comments.XmlElementStartingTag + Constants.XmlTag.TypeParamRef + " name=\"" + parameterName + "\"" + Constants.Comments.XmlElementEndingTag);

        private IEnumerable<Diagnostic> AnalyzeComment(INamedTypeSymbol type, DocumentationCommentTriviaSyntax comment)
        {
            if (type.IsGenericType)
            {
                var typeNames = type.TypeParameters.ToHashSet(_ => _.Name);

                return InspectPhrases(type, comment, typeNames);
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeComment(IMethodSymbol method, DocumentationCommentTriviaSyntax comment)
        {
            if (method.IsGenericMethod || method.ContainingType.IsGenericType)
            {
                var typeNames = method.TypeParameters.Concat(method.ContainingType.TypeParameters).ToHashSet(_ => _.Name);

                return InspectPhrases(method, comment, typeNames);
            }

            return Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> InspectPhrases(ISymbol symbol, DocumentationCommentTriviaSyntax comment, HashSet<string> typeParameterNames)
        {
            foreach (var node in comment.DescendantNodes())
            {
                var attribute = FindRelevantAttribute(node);
                if (attribute != null)
                {
                    var parameterName = GetAttributeValue(attribute);

                    if (typeParameterNames.Contains(parameterName) is false)
                    {
                        var proposal = CreateProposal(parameterName);
                        var phrase = node.ToString();

                        yield return Issue(symbol.Name, node, phrase, proposal);
                    }
                }
            }
        }
    }
}