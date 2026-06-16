using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3126_TheoryMethodsDoNotHaveTestAttributeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3126";

        private static readonly HashSet<string> TestAttributeNames = new HashSet<string>
                                                                         {
                                                                             Constants.Names.TestAttribute,
                                                                             Constants.Names.TestAttributeFullName,
                                                                             Constants.Names.TestCaseAttribute,
                                                                             Constants.Names.TestCaseAttributeFullName,
                                                                             Constants.Names.FactAttribute,
                                                                             Constants.Names.FactAttributeFullName,
                                                                         };

        public MiKo_3126_TheoryMethodsDoNotHaveTestAttributeAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var attributes = symbol.GetAttributes();

            if (attributes.Length > 0)
            {
                var attributeNames = attributes.ToHashSet(_ => _.AttributeClass?.Name);

                if (attributeNames.Contains(Constants.Names.TheoryAttributeFullName))
                {
                    if (attributeNames.Except(Constants.Names.TheoryAttributeFullName).IsProperSubsetOf(TestAttributeNames))
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
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool IsTest(NameSyntax name) => IsTest(name.GetName());

        private static bool IsTest(string name) => TestAttributeNames.Contains(name);
    }
}