using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2004_EventHandlerParametersAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2004";

        public MiKo_2004_EventHandlerParametersAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (!method.IsEventHandler()) return Enumerable.Empty<Diagnostic>();

            var xml = method.GetDocumentationCommentXml();
            if (xml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();
            if (xml.Contains(Constants.Comments.XmlElementStartingTag + Constants.XmlTag.Inheritdoc)) return Enumerable.Empty<Diagnostic>();

            return VerifyParameterComments(method, xml);
        }

        private IEnumerable<Diagnostic> VerifyParameterComments(IMethodSymbol method, string xml)
        {
            List<Diagnostic> diagnostics = null;
            VerifyParameterComment(ref diagnostics, method, xml, 0, Constants.Comments.EventSourcePhrase);

            var eventArgs = method.Parameters[1].Type;
            var defaultStart = eventArgs.Name.StartsWithAnyChar("AEIOU") ? "An" : "A";
            var phrases = new[]
                              {
                                  $"{defaultStart} <see cref=\"{eventArgs.Name}\" /> that contains the event data.", // just used for the proposal
                                  $"{defaultStart} <see cref=\"{eventArgs}\" /> that contains the event data.",
                                  $"{defaultStart} <see cref=\"{eventArgs}\" /> that contains the event data",
                                  $"{defaultStart} <see cref=\"{eventArgs}\"/> that contains the event data.",
                                  $"{defaultStart} <see cref=\"{eventArgs}\"/> that contains the event data",
                              }.Concat(Constants.Comments.UnusedPhrase).ToList();

            VerifyParameterComment(ref diagnostics, method, xml, 1, phrases);

            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }

        private void VerifyParameterComment(ref List<Diagnostic> diagnostics, IMethodSymbol method, string commentXml, int parameterIndex, IEnumerable<string> allExpected)
        {
            var parameter = method.Parameters[parameterIndex];
            var comment = GetParameterComment(parameter, commentXml);
            var proposal = allExpected.ElementAt(0);
            if (allExpected.All(_ => _ != comment))
            {
                if (diagnostics == null) diagnostics = new List<Diagnostic>();
                diagnostics.Add(ReportIssue(parameter, parameter.Name, proposal));
            }
        }
    }
}