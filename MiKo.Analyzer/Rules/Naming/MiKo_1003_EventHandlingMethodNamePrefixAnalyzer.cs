using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1003_EventHandlingMethodNamePrefixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1003";

        private const string Prefix = "On";

        public MiKo_1003_EventHandlingMethodNamePrefixAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol method)
        {
            var suffix = FindProperNameSuffix(method);

            return Prefix + suffix;
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var suffix = FindProperNameSuffix(method);

            var methodName = method.Name;

            var nameFits = methodName.StartsWith(Prefix, StringComparison.Ordinal)
                        && methodName.EndsWith(suffix, StringComparison.Ordinal)
                        && methodName.Contains("_") is false;

            return nameFits
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(method, Prefix + suffix) };
        }

        private static string FindProperNameSuffix(IMethodSymbol method)
        {
            var name = FindProperNameInClass(method);
            if (name is null)
            {
                name = method.Name.StartsWith(Prefix, StringComparison.Ordinal)
                           ? method.Name.Substring(Prefix.Length)
                           : method.Name;
            }

            return name.Without("_");
        }

        private static string FindProperNameInClass(IMethodSymbol method)
        {
            var methodName = method.Name;

            var owningClass = method.GetSyntax().GetEnclosing<ClassDeclarationSyntax>();
            if (owningClass is null)
            {
                // may happen in case the class is currently in uncompilable state (such as it contains an additional bracket)
                return null;
            }

            foreach (var assignment in owningClass.DescendantNodes()
                                                  .OfType<AssignmentExpressionSyntax>()
                                                  .Where(_ => _.IsKind(SyntaxKind.AddAssignmentExpression)))
            {
                var rightIdentifier = (assignment.Right as IdentifierNameSyntax).GetName();
                if (rightIdentifier == methodName)
                {
                    switch (assignment.Left)
                    {
                        case IdentifierNameSyntax s: return s.GetName();
                        case MemberAccessExpressionSyntax s: return s.GetName();
                    }
                }
            }

            return null;
        }
    }
}