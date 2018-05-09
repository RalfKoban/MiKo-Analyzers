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

        public MiKo_1003_EventHandlingMethodNamePrefixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && method.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var expectedName = FindProperName(method);

            return method.Name == expectedName
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { ReportIssue(method, expectedName) };
        }

        private static string FindProperName(IMethodSymbol method)
        {
            var name = FindProperNameInClass(method);
            if (name is null)
            {
                name = method.Name.StartsWith("On", StringComparison.Ordinal)
                           ? method.Name.Substring(2)
                           : method.Name;
            }

            return "On" + name.Replace("_", string.Empty);
        }

        private static string FindProperNameInClass(IMethodSymbol method)
        {
            foreach (var reference in method.DeclaringSyntaxReferences)
            {
                var owningClass = reference.GetSyntax().GetEnclosing<ClassDeclarationSyntax>();

                foreach (var assignment in owningClass.DescendantNodes()
                                                      .OfType<AssignmentExpressionSyntax>()
                                                      .Where(_ => _.Kind() == SyntaxKind.AddAssignmentExpression))
                {
                    var rightIdentifier = (assignment.Right as IdentifierNameSyntax)?.Identifier.ValueText;
                    if (rightIdentifier == method.Name)
                    {
                        switch (assignment.Left)
                        {
                            case IdentifierNameSyntax s: return s.Identifier.ValueText;
                            case MemberAccessExpressionSyntax s: return s.Name.Identifier.ValueText;
                        }
                    }
                }
            }

            return null;
        }
    }
}