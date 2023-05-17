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

        private static readonly HashSet<string> Tags = new HashSet<string>
                                                           {
                                                               Constants.XmlTag.See,
                                                               Constants.XmlTag.SeeAlso,
                                                           };

        public MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        internal static string GetReferencedName(SyntaxNode node)
        {
            var name = node.GetCref().GetCrefType().GetName();

            if (name.IsNullOrWhiteSpace())
            {
                var nameAttribute = node.GetNameAttribute();

                if (nameAttribute != null)
                {
                    name = nameAttribute.TextTokens.First().ValueText;
                }
            }

            return name;
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml, DocumentationCommentTriviaSyntax comment)
        {
            switch (symbol)
            {
                case IMethodSymbol method when method.IsGenericMethod || method.ContainingType.IsGenericType:
                {
                    return AnalyzeComment(comment, method.TypeParameters.Concat(method.ContainingType.TypeParameters));
                }

                case INamedTypeSymbol type when type.IsGenericType:
                {
                    return AnalyzeComment(comment, type.TypeParameters);
                }

                default:
                {
                    return Enumerable.Empty<Diagnostic>();
                }
            }
        }

        private IEnumerable<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, IEnumerable<ITypeParameterSymbol> parameters)
        {
            var names = parameters.ToHashSet(_ => _.Name);

            if (names.Count > 0)
            {
                foreach (var node in comment.DescendantNodes(_ => true, true))
                {
                    switch (node)
                    {
                        case XmlElementSyntax _:
                        case XmlEmptyElementSyntax _:
                        {
                            var tag = node.GetXmlTagName();

                            if (Tags.Contains(tag))
                            {
                                var name = GetReferencedName(node);

                                if (names.Contains(name))
                                {
                                    yield return Issue(node);
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}