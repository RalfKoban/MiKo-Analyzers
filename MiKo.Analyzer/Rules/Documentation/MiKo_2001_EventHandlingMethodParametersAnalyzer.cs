﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Extensions;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_2001_EventHandlingMethodParametersAnalyzer : DocumentationAnalyzer
    {
        public const string Id = "MiKo_2001";

        public MiKo_2001_EventHandlingMethodParametersAnalyzer() : base(Id, SymbolKind.Method)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (!method.IsEventHandler()) return Enumerable.Empty<Diagnostic>();

            var xml = method.GetDocumentationCommentXml();
            if (xml.IsNullOrWhiteSpace()) return Enumerable.Empty<Diagnostic>();

            var diagnostics = new List<Diagnostic>();
            VerifyParameterComment(diagnostics, method, xml, 0, "The source of the event.", "Unused.");

            var eventArgs = method.Parameters[1].Type.Name;
            var defaultStart = eventArgs.StartsWithAny("A", "E", "I", "O", "U") ? "An" : "A";
            VerifyParameterComment(diagnostics, method, xml, 1, $"{defaultStart} <see cref=\"{eventArgs}\" /> that contains the event data.", "Unused.");
            return diagnostics;
        }

        private void VerifyParameterComment(ICollection<Diagnostic> diagnostics, IMethodSymbol method, string commentXml, int parameterIndex, params string[] allExpected)
        {
            var parameter = method.Parameters[parameterIndex];

            var paramElements = GetCommentElements(commentXml, @"param");
            var comments = paramElements.Where(_ => _.Attribute("name")?.Value == parameter.Name);
            var comment = comments.Nodes().Concatenated().Replace("T:", string.Empty).Trim();

            if (allExpected.All(_ => _ != comment))
            {
                diagnostics.Add(ReportIssue(method, parameter.Name, allExpected[0]));
            }
        }
    }
}