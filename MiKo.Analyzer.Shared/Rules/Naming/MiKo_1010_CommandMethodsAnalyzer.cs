using System;
using System.Collections.Generic;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1010_CommandMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1010";

        private static readonly string[] SpecialOnParts = { "OnExecuted", "OnCanExecute" };
        private static readonly string[] SpecialParts = { "Executed", "CanExecute" };

        private static readonly ICollection<string> ExcludedNames = new HashSet<string>
                                                                        {
                                                                            nameof(ICommand.CanExecute),
                                                                            nameof(ICommand.Execute),
                                                                            nameof(ICommand.CanExecute) + Constants.AsyncSuffix,
                                                                            nameof(ICommand.Execute) + Constants.AsyncSuffix,
                                                                            nameof(ICommand.CanExecute) + Constants.AsyncCoreSuffix,
                                                                            nameof(ICommand.Execute) + Constants.AsyncCoreSuffix,
                                                                            nameof(ICommand.CanExecuteChanged),
                                                                            "On" + nameof(ICommand.CanExecuteChanged),
                                                                            "Raise" + nameof(ICommand.CanExecuteChanged),
                                                                        };

        public MiKo_1010_CommandMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsInterfaceImplementationOf<ICommand>() is false && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var issueCanExecute = AnalyzeMethodName(nameof(ICommand.CanExecute), symbol);

            yield return issueCanExecute;

            if (issueCanExecute is null)
            {
                // CanExecute is not contained, thus we can check for execute (otherwise 'Execute' would already be part of the method's name)
                yield return AnalyzeMethodName(nameof(ICommand.Execute), symbol);
            }
        }

        private static string GetProposal(string methodName, string forbiddenName)
        {
            var enhancedForbiddenName = forbiddenName + "d";

            var phrase = methodName.Contains(enhancedForbiddenName)
                         ? enhancedForbiddenName
                         : forbiddenName;

            // TODO RKN: find better name by inspecting method assignment (?)
            return methodName.Without(phrase);
        }

        private Diagnostic AnalyzeMethodName(string forbiddenName, IMethodSymbol method)
        {
            var methodName = method.Name;

            if (ExcludedNames.Contains(methodName))
            {
                return null;
            }

            if (methodName.StartsWith("On", StringComparison.OrdinalIgnoreCase))
            {
                if (methodName.StartsWithAny(SpecialOnParts, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if (methodName.EndsWithAny(SpecialParts, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            if (methodName.EndsWithAny(SpecialOnParts, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (methodName.Contains(forbiddenName))
            {
                var proposal = GetProposal(methodName, forbiddenName);

                return Issue(method, proposal, CreateBetterNameProposal(proposal));
            }

            return null;
        }
    }
}