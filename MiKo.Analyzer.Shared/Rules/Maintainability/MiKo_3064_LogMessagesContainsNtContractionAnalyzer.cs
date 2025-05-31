﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3064_LogMessagesContainsNtContractionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3064";

        public MiKo_3064_LogMessagesContainsNtContractionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context) => context.Compilation.GetTypeByMetadataName(Constants.ILog.FullTypeName) != null;

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);

        private static IEnumerable<SyntaxToken> GetTextTokens(ArgumentSyntax argument)
        {
            switch (argument.Expression)
            {
                case LiteralExpressionSyntax literal when literal.IsKind(SyntaxKind.StringLiteralExpression):
                {
                    return new[] { literal.Token };
                }

                case InterpolatedStringExpressionSyntax interpolated:
                {
                    return interpolated.Contents.OfType<InterpolatedStringTextSyntax>().Select(_ => _.TextToken);
                }
            }

            return Array.Empty<SyntaxToken>();
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;
            var issues = AnalyzeInvocation(node, context.SemanticModel);

            ReportDiagnostics(context, issues);
        }

        private IEnumerable<Diagnostic> AnalyzeInvocation(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            var arguments = node.ArgumentList.Arguments;

            if (arguments.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            if (node.Expression is MemberAccessExpressionSyntax methodCall)
            {
                var methodName = methodCall.GetName();

                var argument0 = arguments[0];

                switch (methodName)
                {
                    case Constants.ILog.Debug:
                    case Constants.ILog.Info:
                    case Constants.ILog.Warn:
                    case Constants.ILog.Error:
                    case Constants.ILog.Fatal:
                    {
                        if (argument0.IsStringLiteral())
                        {
                            return AnalyzeCall(methodCall, argument0, semanticModel);
                        }

                        break;
                    }

                    case Constants.ILog.DebugFormat:
                    case Constants.ILog.InfoFormat:
                    case Constants.ILog.WarnFormat:
                    case Constants.ILog.ErrorFormat:
                    case Constants.ILog.FatalFormat:
                    {
                        if (argument0.IsStringLiteral())
                        {
                            return AnalyzeCall(methodCall, argument0, semanticModel);
                        }

                        // TODO: Find correct argument, especially for those with 3 or 4 parameters
                        if (arguments.Count > 1)
                        {
                            var argument1 = arguments[1];

                            if (argument1.IsStringLiteral())
                            {
                                return AnalyzeCall(methodCall, argument1, semanticModel);
                            }
                        }

                        break;
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private IEnumerable<Diagnostic> AnalyzeCall(MemberAccessExpressionSyntax methodCall, ArgumentSyntax argument, SemanticModel semanticModel)
        {
            // only ILog methods shall be reported
            var type = methodCall.GetTypeSymbol(semanticModel);

            // it may happen that in some broken code Roslyn is unable to detect a type (e.g. due to missing code paths), hence 'type' could be null here
            if (type?.Name is Constants.ILog.TypeName)
            {
                var syntaxTree = argument.SyntaxTree;
                var phrases = Constants.Comments.NotContractionPhrase;
                var phrasesLength = phrases.Length;

                foreach (var token in GetTextTokens(argument))
                {
                    var text = token.Text; // use 'Text' and not 'ValueText' here because otherwise the indices do not match (as 'Text' still contains the " )

                    for (var i = 0; i < phrasesLength; i++)
                    {
                        var value = phrases[i];

                        foreach (var index in text.AllIndicesOf(value))
                        {
                            var location = CreateLocation(value, syntaxTree, token.SpanStart, index);

                            yield return Issue(location);
                        }
                    }
                }
            }
        }
    }
}