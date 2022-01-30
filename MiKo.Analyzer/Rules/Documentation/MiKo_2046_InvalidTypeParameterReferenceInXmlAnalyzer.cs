using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2046";

        private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

        public MiKo_2046_InvalidTypeParameterReferenceInXmlAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method);

        protected override IEnumerable<Diagnostic> AnalyzeComment(ISymbol symbol, Compilation compilation, string commentXml)
        {
            switch (symbol)
            {
                case IMethodSymbol method:
                    return AnalyzeComment(method, commentXml);

                case INamedTypeSymbol type:
                    return AnalyzeComment(type, commentXml);

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        private static IEnumerable<string> CreatePhrases(string parameterName) => new[]
                                                                                      {
                                                                                          string.Intern("<see cref=\"" + parameterName + "\""),
                                                                                          string.Intern("<see name=\"" + parameterName + "\""),
                                                                                          string.Intern("<seealso cref=\"" + parameterName + "\""),
                                                                                          string.Intern("<seealso name=\"" + parameterName + "\""),
                                                                                      };

        private static string GetReplacement(ITypeParameterSymbol parameter) => string.Intern(Constants.Comments.XmlElementStartingTag + Constants.XmlTag.TypeParamRef + " name=\"" + parameter.Name + "\"" + Constants.Comments.XmlElementEndingTag);

        private IEnumerable<Diagnostic> AnalyzeComment(INamedTypeSymbol type, string commentXml)
        {
            List<Diagnostic> findings = null;

            if (type.IsGenericType)
            {
                var comment = commentXml.Without(Constants.Markers.Symbols);

                foreach (var parameter in type.TypeParameters)
                {
                    InspectPhrases(parameter, comment, ref findings);
                }
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeComment(IMethodSymbol method, string commentXml)
        {
            List<Diagnostic> findings = null;

            if (method.IsGenericMethod || method.ContainingType.IsGenericType)
            {
                var comment = commentXml.Without(Constants.Markers.Symbols);

                foreach (var parameter in method.TypeParameters.Concat(method.ContainingType.TypeParameters))
                {
                    InspectPhrases(parameter, comment, ref findings);
                }
            }

            return findings ?? Enumerable.Empty<Diagnostic>();
        }

        private void InspectPhrases(ITypeParameterSymbol parameter, string commentXml, ref List<Diagnostic> findings)
        {
            var phrases = CreatePhrases(parameter.Name);

            foreach (var phrase in phrases
                                   .Where(_ => commentXml.Contains(_, Comparison))
                                   .Select(_ => _.StartsWith(Constants.Comments.XmlElementStartingTag, Comparison) ? _ + Constants.Comments.XmlElementEndingTag : _))
            {
                if (findings is null)
                {
                    findings = new List<Diagnostic>(1);
                }

                var replacement = GetReplacement(parameter);
                findings.Add(Issue(parameter, phrase, replacement));
            }
        }
    }
}