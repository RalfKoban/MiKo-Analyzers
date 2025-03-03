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

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeComment, DocumentationCommentTrivia);

        private void AnalyzeComment(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is DocumentationCommentTriviaSyntax comment)
            {
                var symbol = context.ContainingSymbol;

                switch (symbol?.Kind)
                {
                    case SymbolKind.NamedType:
                    case SymbolKind.Method:
                    {
                        var issues = AnalyzeComment(comment, symbol);

                        if (issues.Count > 0)
                        {
                            ReportDiagnostics(context, issues);
                        }

                        break;
                    }
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, ISymbol symbol)
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
                    return Array.Empty<Diagnostic>();
                }
            }
        }

        private IReadOnlyList<Diagnostic> AnalyzeComment(DocumentationCommentTriviaSyntax comment, IEnumerable<ITypeParameterSymbol> parameters)
        {
            var names = parameters.ToHashSet(_ => _.Name);

            if (names.Count == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            List<Diagnostic> results = null;

            foreach (var node in comment.AllDescendantNodes())
            {
                if (node.IsXml())
                {
                    var tag = node.GetXmlTagName();

                    if (Tags.Contains(tag))
                    {
                        var name = node.GetReferencedName();

                        if (names.Contains(name))
                        {
                            if (results is null)
                            {
                                results = new List<Diagnostic>(1);
                            }

                            results.Add(Issue(node));
                        }
                    }
                }
            }

            return (IReadOnlyList<Diagnostic>)results ?? Array.Empty<Diagnostic>();
        }
    }
}