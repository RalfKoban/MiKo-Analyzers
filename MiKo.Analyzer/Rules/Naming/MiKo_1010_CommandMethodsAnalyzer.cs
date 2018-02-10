using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Extensions;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    // TODO: RKN temp. deactivated because of issues in name detection
    // [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1010_CommandMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1010";

        public MiKo_1010_CommandMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsInterfaceImplementationOf<ICommand>()) return Enumerable.Empty<Diagnostic>();

            var diagnostics = new List<Diagnostic>();
            if (!VerifyMethodName(nameof(ICommand.CanExecute), method, diagnostics))
            {
                // CanExecute is not contained, thus we can check for execute (otherwise 'Execute' would already be part of the method's name)
                VerifyMethodName(nameof(ICommand.Execute), method, diagnostics);
            }
            return diagnostics;
        }

        private bool VerifyMethodName(string forbiddenName, IMethodSymbol method, ICollection<Diagnostic> diagnostics)
        {
            var forbidden = method.Name.Contains(forbiddenName);
            if (forbidden)
            {
                diagnostics.Add(ReportIssue(method, method.Name.Replace(nameof(ICommand.Execute), string.Empty)));
            }

            return forbidden;
        }
    }
}