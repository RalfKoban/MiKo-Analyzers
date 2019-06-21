using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1010_CommandMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1010";

        private static readonly ICollection<string> ExcludedNames = new HashSet<string>
                                                                        {
                                                                            nameof(ICommand.CanExecute),
                                                                            nameof(ICommand.Execute),
                                                                            nameof(ICommand.CanExecute) + Constants.AsyncSuffix,
                                                                            nameof(ICommand.Execute) + Constants.AsyncSuffix,
                                                                            nameof(ICommand.CanExecuteChanged),
                                                                            "On" + nameof(ICommand.CanExecuteChanged),
                                                                            "Raise" + nameof(ICommand.CanExecuteChanged),
                                                                        };

        public MiKo_1010_CommandMethodsAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => base.ShallAnalyze(method) && !method.IsInterfaceImplementationOf<ICommand>() && !method.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            List<Diagnostic> diagnostics = null;

            if (!VerifyMethodName(nameof(ICommand.CanExecute), method, ref diagnostics))
            {
                // CanExecute is not contained, thus we can check for execute (otherwise 'Execute' would already be part of the method's name)
                VerifyMethodName(nameof(ICommand.Execute), method, ref diagnostics);
            }

            return diagnostics ?? Enumerable.Empty<Diagnostic>();
        }

        private static string GetProposal(string methodName, string forbiddenName)
        {
            var enhancedForbiddenName = forbiddenName + "d";

            var phrase = methodName.Contains(enhancedForbiddenName)
                             ? enhancedForbiddenName
                             : forbiddenName;

            // TODO RKN: find better name by inspecting method assignment (?)
            return methodName.Remove(phrase);
        }

        private bool VerifyMethodName(string forbiddenName, IMethodSymbol method, ref List<Diagnostic> diagnostics)
        {
            var methodName = method.Name;

            if (ExcludedNames.Contains(methodName))
                return false;

            if (methodName.StartsWith("On", StringComparison.OrdinalIgnoreCase) && methodName.EndsWith("CommandExecuted", StringComparison.OrdinalIgnoreCase))
                return false;

            var forbidden = methodName.Contains(forbiddenName);
            if (forbidden)
            {
                if (diagnostics == null) diagnostics = new List<Diagnostic>();

                var proposal = GetProposal(methodName, forbiddenName);
                diagnostics.Add(Issue(method, proposal));
            }

            return forbidden;
        }
    }
}