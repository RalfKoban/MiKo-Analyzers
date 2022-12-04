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

        private static IEnumerable<string> CreatePhrases(IMethodSymbol method)
        {
            var type = method.Parameters[1].Type;
            var typeName = type.Name;

            var defaultStart = GetEventArgsStartingPhrase(typeName);
            var defaultEnding = GetEventArgsEndingPhrase();

            return new[]
                       {
                           $"{defaultStart}<see cref=\"{typeName}\" />{defaultEnding}.", // just used for the proposal
                           $"{defaultStart}<see cref=\"{type}\" />{defaultEnding}.",
                           $"{defaultStart}<see cref=\"{type}\" />{defaultEnding}",
                           $"{defaultStart}<see cref=\"{type}\"/>{defaultEnding}.",
                           $"{defaultStart}<see cref=\"{type}\"/>{defaultEnding}",
                       };
        }

        private IEnumerable<Diagnostic> VerifyParameterComments(IMethodSymbol method, string xml)
        {
            var sender = method.Parameters[0];
            var senderComment = sender.GetComment(xml);

            if (Constants.Comments.EventSourcePhrase.None(_ => _ == senderComment))
            {
                yield return Issue(sender, sender.Name, Constants.Comments.EventSourcePhrase.ElementAt(0), new Dictionary<string, string> { { IsSender, string.Empty } });
            }

            var phrases = CreatePhrases(method).Concat(Constants.Comments.UnusedPhrase);

            var eventArgs = method.Parameters[1];
            var eventArgsComment = eventArgs.GetComment(xml);

            if (phrases.None(_ => _ == eventArgsComment))
            {
                yield return Issue(eventArgs, eventArgs.Name, phrases.ElementAt(0), new Dictionary<string, string>());
            }
        }
    }
}