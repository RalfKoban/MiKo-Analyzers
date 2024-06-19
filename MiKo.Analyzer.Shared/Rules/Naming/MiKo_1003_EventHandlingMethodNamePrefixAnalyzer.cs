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
        private const string OnCanExecuteSuffix = "OnCanExecute";
        private const string OnExecutedSuffix = "OnExecuted";

        public MiKo_1003_EventHandlingMethodNamePrefixAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsEventHandler();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => true;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => ShallAnalyze(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var suffix = FindProperNameSuffix(symbol);

            var methodName = symbol.Name;

            var nameFits = methodName.StartsWith(Prefix, StringComparison.Ordinal)
                        && methodName.EndsWith(suffix, StringComparison.Ordinal)
                        && methodName.Contains(Constants.Underscore) is false;

            if (nameFits)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var proposal = Prefix + suffix;

            return new[] { Issue(symbol, proposal, CreateBetterNameProposal(proposal)) };
        }

        private static string FindProperNameSuffix(IMethodSymbol method)
        {
            var name = FindProperNameInClass(method);

            if (name is null)
            {
                name = method.Name.StartsWith(Prefix, StringComparison.Ordinal)
                       ? method.Name.Substring(Prefix.Length)
                       : method.Name;

                if (name.EndsWith(OnCanExecuteSuffix, StringComparison.Ordinal))
                {
                    name = "CanExecute" + name.Substring(0, name.Length - OnCanExecuteSuffix.Length);
                }
                else if (name.EndsWith(OnExecutedSuffix, StringComparison.Ordinal))
                {
                    name = "Executed" + name.Substring(0, name.Length - OnExecutedSuffix.Length);
                }
            }

            return name.Without(Constants.Underscore).ToUpperCaseAt(0);
        }

        private static string FindProperNameInClass(IMethodSymbol method)
        {
            var owningClass = method.GetSyntax().GetEnclosing<ClassDeclarationSyntax>();

            if (owningClass is null)
            {
                // may happen in case the class is currently in uncompilable state (such as it contains an additional bracket)
                return null;
            }

            var methodName = method.Name;

            foreach (var assignment in owningClass.DescendantNodes<AssignmentExpressionSyntax>(SyntaxKind.AddAssignmentExpression))
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