using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3125_TestMethodsDoNotHaveBothTestAndTestCaseAttributeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3125";

        public MiKo_3125_TestMethodsDoNotHaveBothTestAndTestCaseAttributeAnalyzer() : base(Id)
        {
        }

        protected override DiagnosticSeverity Severity => DiagnosticSeverity.Info; // as both are allowed by NUnit, this is more an information than a warning

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var attributes = symbol.GetAttributes();

            if (attributes.Length > 0)
            {
                var attributeNames = attributes.ToHashSet(_ => _.AttributeClass?.Name);

                if (attributeNames.Contains(Constants.Names.TestAttributeFullName) && attributeNames.Contains(Constants.Names.TestCaseAttributeFullName))
                {
                    var method = symbol.GetSyntax<MethodDeclarationSyntax>();

                    foreach (var attributeList in method.AttributeLists)
                    {
                        foreach (var attribute in attributeList.Attributes)
                        {
                            var attributeName = attribute.Name;

                            if (IsTest(attributeName))
                            {
                                return new[] { Issue(attributeName) };
                            }
                        }
                    }
                }
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool IsTest(NameSyntax name) => IsTest(name.GetName());

        private static bool IsTest(string name) => name is Constants.Names.TestAttribute || name is Constants.Names.TestAttributeFullName;
    }
}