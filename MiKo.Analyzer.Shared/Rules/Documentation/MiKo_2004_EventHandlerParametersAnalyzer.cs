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

        internal const string IsSender = "IsSender";

        public MiKo_2004_EventHandlerParametersAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        internal static string GetEventArgsStartingPhrase(string name) => name.StartsWithAny("AEIOU") ? "An " : "A ";

        internal static string GetEventArgsEndingPhrase() => " that contains the event data";

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsEventHandler() && base.ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation, string commentXml) => commentXml.Contains(Constants.Comments.XmlElementStartingTag + Constants.XmlTag.Inheritdoc)
                                                                                                                                          ? Enumerable.Empty<Diagnostic>()
                                                                                                                                          : VerifyParameterComments(symbol, commentXml);

        private IEnumerable<Diagnostic> VerifyParameterComments(IMethodSymbol method, string xml)
        {
            List<Diagnostic> diagnostics = null;
            VerifyParameterComment(ref diagnostics, method, xml, true, Constants.Comments.EventSourcePhrase);

            var eventArgs = method.Parameters[1].Type;
            var defaultStart = GetEventArgsStartingPhrase(eventArgs.Name);
            var defaultEnding = GetEventArgsEndingPhrase();
            var phrases = new[]
                              {
                                  $"{defaultStart}<see cref=\"{eventArgs.Name}\" />{defaultEnding}.", // just used for the proposal
                                  $"{defaultStart}<see cref=\"{eventArgs}\" />{defaultEnding}.",
                                  $"{defaultStart}<see cref=\"{eventArgs}\" />{defaultEnding}",
                                  $"{defaultStart}<see cref=\"{eventArgs}\"/>{defaultEnding}.",
                                  $"{defaultStart}<see cref=\"{eventArgs}\"/>{defaultEnding}",
                              }.Concat(Constants.Comments.UnusedPhrase).ToList();

            VerifyParameterComment(ref diagnostics, method, xml, false, phrases);

            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }

        private void VerifyParameterComment(ref List<Diagnostic> diagnostics, IMethodSymbol method, string commentXml, bool isSender, IEnumerable<string> allExpected)
        {
            var parameterIndex = isSender ? 0 : 1;
            var parameter = method.Parameters[parameterIndex];
            var comment = parameter.GetComment(commentXml);
            var proposal = allExpected.ElementAt(0);

            if (allExpected.All(_ => _ != comment))
            {
                if (diagnostics is null)
                {
                    diagnostics = new List<Diagnostic>(1);
                }

                var properties = new Dictionary<string, string>();
                if (isSender)
                {
                    properties.Add(IsSender, string.Empty);
                }

                diagnostics.Add(Issue(parameter, parameter.Name, proposal, properties));
            }
        }
    }
}