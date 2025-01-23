﻿using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3045_EventManagerRegisterUsesNameofAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3045";

        public MiKo_3045_EventManagerRegisterUsesNameofAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation)
        {
            var arguments = symbol.GetInvocationArgumentsFrom(Constants.EventManager.RegisterRoutedEvent);

            if (arguments.Count == 4)
            {
                var expression = arguments[0].Expression;

                if (expression.IsKind(SyntaxKind.StringLiteralExpression))
                {
                    return new[] { Issue(symbol.Name, expression) };
                }
            }

            return Array.Empty<Diagnostic>();
        }
    }
}