using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3098_SuppressMessageAttributeHasJustificationAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3098";

        private const int MinimumJustificationLength = 10;
        private const string BadJustificationGapFillers = " .<>-*";

        private static readonly string[] BadJustificationMarkers = { "as design", "by design", "suppress", "pending", "review", "temp", "temporarily", "intent", "intend", "todo", "to do", "fixme", "fix me" };

        public MiKo_3098_SuppressMessageAttributeHasJustificationAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeAttributeArgument, SyntaxKind.AttributeArgument);

        private static bool HasIssue(ReadOnlySpan<char> text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return true; // no justification text means that there is an issue
            }

            if (text.Length < MinimumJustificationLength)
            {
                return true; // justification is too short
            }

            if (text.ToString().ContainsAny(BadJustificationMarkers, StringComparison.OrdinalIgnoreCase))
            {
                return true; // no justification that explains
            }

            return false;
        }

        private void AnalyzeAttributeArgument(SyntaxNodeAnalysisContext context)
        {
            var argument = (AttributeArgumentSyntax)context.Node;

            if (argument.Parent?.Parent is AttributeSyntax attribute)
            {
                var name = attribute.GetName();

                switch (name)
                {
                    case "SuppressMessage":
                    case nameof(SuppressMessageAttribute):
                    {
                        if (argument.NameEquals.GetName() == nameof(SuppressMessageAttribute.Justification)
                         && argument.Expression is LiteralExpressionSyntax literal
                         && HasIssue(literal.Token.ValueText.AsSpan().Trim(BadJustificationGapFillers.AsSpan())))
                        {
                            ReportDiagnostics(context, Issue(literal));
                        }

                        break;
                    }
                }
            }
        }
    }
}