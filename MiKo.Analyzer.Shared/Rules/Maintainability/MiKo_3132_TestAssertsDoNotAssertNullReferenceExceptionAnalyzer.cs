using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3132_TestAssertsDoNotAssertNullReferenceExceptionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3132";

        private static readonly HashSet<string> AssertionMethods = new HashSet<string>
                                                                       {
                                                                           "That",
                                                                           "Catch",
                                                                           "CatchAsync",
                                                                           "Throws",
                                                                           "ThrowsAsync",
                                                                       };

        public MiKo_3132_TestAssertsDoNotAssertNullReferenceExceptionAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            List<Diagnostic> issues = null;

            var syntax = symbol.GetSyntax();

            var compilationUnit = syntax.FirstAncestor<CompilationUnitSyntax>();
            var alias = compilationUnit?.Usings.FirstOrDefault(_ => _.Name.GetName() is "System.NullReferenceException");

            var nullReferenceExceptionName = alias?.Alias.GetName() ?? nameof(NullReferenceException);

            var types = syntax.DescendantNodes<InvocationExpressionSyntax>()
                              .Where(_ => _.GetIdentifierName() is "Assert")
                              .Where(_ => AssertionMethods.Contains(_.GetName()))
                              .SelectMany(_ => _.DescendantNodes<TypeSyntax>().Where(__ => __.GetName() == nullReferenceExceptionName));

            foreach (var type in types)
            {
                if (type.Parent is ObjectCreationExpressionSyntax)
                {
                    continue; // do not report tests that simulate behavior
                }

                if (issues is null)
                {
                    issues = new List<Diagnostic>(1);
                }

                issues.Add(Issue(type));
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}