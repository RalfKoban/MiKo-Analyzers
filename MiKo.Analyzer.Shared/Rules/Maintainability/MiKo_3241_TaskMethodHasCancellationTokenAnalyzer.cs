using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3241_TaskMethodHasCancellationTokenAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3241";

        public MiKo_3241_TaskMethodHasCancellationTokenAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (base.ShallAnalyze(symbol))
            {
                if (symbol.ReturnType.IsTask() || symbol.ReturnType.IsValueTask())
                {
                    if (symbol.IsInterfaceImplementation())
                    {
                        return false;
                    }

                    if (symbol.IsTestMethod()
                     || symbol.IsTestSetUpMethod()
                     || symbol.IsTestOneTimeSetUpMethod()
                     || symbol.IsTestTearDownMethod()
                     || symbol.IsTestOneTimeTearDownMethod()
                     || symbol.IsTestAssemblyWideSetUpMethod()
                     || symbol.IsTestAssemblyWideTearDownMethod())
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (symbol.IsMainMethod(compilation))
            {
                // nothing to report here for the main method as that is the entry point of an application and as to be named 'Main'
                return Array.Empty<Diagnostic>();
            }

            foreach (var parameter in symbol.Parameters)
            {
                if (parameter.Type.IsCancellationToken())
                {
                    return Array.Empty<Diagnostic>();
                }
            }

            // seems we have no cancellation token, so inspect the overloads (as when we have an overload, we can use that to cancel the operation, thus we do not need to report it for this method)
            var methodsWithSameName = symbol.ContainingType.GetMethods(symbol.Name);

            if (methodsWithSameName.Count > 1)
            {
                foreach (var overload in methodsWithSameName.Except(symbol))
                {
                    if (overload.DeclaredAccessibility == symbol.DeclaredAccessibility)
                    {
                        var returnType = overload.ReturnType;

                        if (returnType.IsTask() || returnType.IsValueTask())
                        {
                            foreach (var parameter in overload.Parameters)
                            {
                                if (parameter.Type.IsCancellationToken())
                                {
                                    // do not report this method as we just found an overload with a cancellation token
                                    return Array.Empty<Diagnostic>();
                                }
                            }
                        }
                    }
                }
            }

            return new[] { Issue(symbol) };
        }
    }
}