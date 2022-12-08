using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3036_CodeFixProvider)), Shared]
    public sealed class MiKo_3036_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3036_TimeSpanCtorUsageAnalyzer.Id;

        protected override string Title => Resources.MiKo_3036_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IReadOnlyCollection<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(CodeFixContext context, SyntaxNode syntax, Diagnostic issue)
        {
            var creation = (ObjectCreationExpressionSyntax)syntax;

            return GetUpdatedSyntax(creation.ArgumentList) ?? syntax;
        }

        private static SyntaxNode GetUpdatedSyntax(ArgumentListSyntax argumentList)
        {
            if (argumentList is null)
            {
                return null;
            }

            var args = argumentList.Arguments;
            switch (args.Count)
            {
                case 1: // public TimeSpan (long ticks);
                    return GetUpdatedSyntax(args[0]);

                case 3: // public TimeSpan (int hours, int minutes, int seconds);
                    return GetUpdatedSyntax(args[0], args[1], args[2]);

                case 4: // public TimeSpan (int days, int hours, int minutes, int seconds);
                    return GetUpdatedSyntax(args[0], args[1], args[2], args[3]);

                case 5: // public TimeSpan (int days, int hours, int minutes, int seconds, int milliseconds);
                    return GetUpdatedSyntax(args[0], args[1], args[2], args[3], args[4]);

                case 6: // public TimeSpan (int days, int hours, int minutes, int seconds, int milliseconds, int microseconds);
                    return GetUpdatedSyntax(args[0], args[1], args[2], args[3], args[4], args[5]);

                default:
                    return null;
            }
        }

        private static SyntaxNode GetUpdatedSyntax(ArgumentSyntax ticks) => FromTicks(ticks);

        private static SyntaxNode GetUpdatedSyntax(ArgumentSyntax hours, ArgumentSyntax minutes, ArgumentSyntax seconds)
        {
            var zeroHours = IsZero(hours);
            var zeroMinutes = IsZero(minutes);
            var zeroSeconds = IsZero(seconds);

            if (zeroMinutes && zeroSeconds) return FromHours(hours);

            if (zeroHours && zeroSeconds) return FromMinutes(minutes);

            if (zeroHours && zeroMinutes) return FromSeconds(seconds);

            return null;
        }

        private static SyntaxNode GetUpdatedSyntax(ArgumentSyntax days, ArgumentSyntax hours, ArgumentSyntax minutes, ArgumentSyntax seconds)
        {
            var zeroDays = IsZero(days);
            var zeroHours = IsZero(hours);
            var zeroMinutes = IsZero(minutes);
            var zeroSeconds = IsZero(seconds);

            if (zeroHours && zeroMinutes && zeroSeconds) return FromDays(days);

            if (zeroDays && zeroMinutes && zeroSeconds) return FromHours(hours);

            if (zeroDays && zeroHours && zeroSeconds) return FromMinutes(minutes);

            if (zeroDays && zeroHours && zeroMinutes) return FromSeconds(seconds);

            return null;
        }

        private static SyntaxNode GetUpdatedSyntax(ArgumentSyntax days, ArgumentSyntax hours, ArgumentSyntax minutes, ArgumentSyntax seconds, ArgumentSyntax milliseconds)
        {
            var zeroDays = IsZero(days);
            var zeroHours = IsZero(hours);
            var zeroMinutes = IsZero(minutes);
            var zeroSeconds = IsZero(seconds);
            var zeroMilliseconds = IsZero(milliseconds);

            if (zeroHours && zeroMinutes && zeroSeconds && zeroMilliseconds) return FromDays(days);

            if (zeroDays && zeroMinutes && zeroSeconds && zeroMilliseconds) return FromHours(hours);

            if (zeroDays && zeroHours && zeroSeconds && zeroMilliseconds) return FromMinutes(minutes);

            if (zeroDays && zeroHours && zeroMinutes && zeroMilliseconds) return FromSeconds(seconds);

            if (zeroDays && zeroHours && zeroMinutes && zeroSeconds) return FromMilliseconds(milliseconds);

            return null;
        }

        private static SyntaxNode GetUpdatedSyntax(ArgumentSyntax days, ArgumentSyntax hours, ArgumentSyntax minutes, ArgumentSyntax seconds, ArgumentSyntax milliseconds, ArgumentSyntax microseconds)
        {
            var zeroDays = IsZero(days);
            var zeroHours = IsZero(hours);
            var zeroMinutes = IsZero(minutes);
            var zeroSeconds = IsZero(seconds);
            var zeroMilliseconds = IsZero(milliseconds);
            var zeroMicroseconds = IsZero(microseconds);

            if (zeroHours && zeroMinutes && zeroSeconds && zeroMilliseconds && zeroMicroseconds) return FromDays(days);

            if (zeroDays && zeroMinutes && zeroSeconds && zeroMilliseconds && zeroMicroseconds) return FromHours(hours);

            if (zeroDays && zeroHours && zeroSeconds && zeroMilliseconds && zeroMicroseconds) return FromMinutes(minutes);

            if (zeroDays && zeroHours && zeroMinutes && zeroMilliseconds && zeroMicroseconds) return FromSeconds(seconds);

            if (zeroDays && zeroHours && zeroMinutes && zeroSeconds && zeroMicroseconds) return FromMilliseconds(milliseconds);

            if (zeroDays && zeroHours && zeroMinutes && zeroSeconds && zeroMilliseconds) return FromMicroseconds(microseconds);

            return null;
        }

        private static bool IsZero(ArgumentSyntax argument) => argument.Expression is LiteralExpressionSyntax literal && literal.IsKind(SyntaxKind.NumericLiteralExpression) && double.TryParse(literal.Token.Text, out var number) && number == 0d;

        private static InvocationExpressionSyntax FromDays(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), nameof(TimeSpan.FromDays), argument);

        private static InvocationExpressionSyntax FromHours(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), nameof(TimeSpan.FromHours), argument);

        private static InvocationExpressionSyntax FromMinutes(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), nameof(TimeSpan.FromMinutes), argument);

        private static InvocationExpressionSyntax FromSeconds(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), nameof(TimeSpan.FromSeconds), argument);

        private static InvocationExpressionSyntax FromMilliseconds(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), nameof(TimeSpan.FromMilliseconds), argument);

        private static InvocationExpressionSyntax FromMicroseconds(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), "FromMicroseconds", argument);

        private static InvocationExpressionSyntax FromTicks(ArgumentSyntax argument) => Invocation(nameof(TimeSpan), nameof(TimeSpan.FromTicks), argument);
    }
}