using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3216";

        public MiKo_3216_AssignedStaticFieldsAreReadOnlyAnalyzer() : base(Id, SymbolKind.Field)
        {
        }

        protected override bool ShallAnalyze(IFieldSymbol symbol)
        {
            if (symbol.IsConst)
            {
                return false;
            }

            if (symbol.IsStatic)
            {
                return symbol.IsReadOnly is false;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation)
        {
            var declaration = symbol.GetSyntax<FieldDeclarationSyntax>();

            if (declaration != null)
            {
                foreach (var variable in declaration.Declaration.Variables)
                {
                    if (variable.Initializer != null)
                    {
                        yield return Issue(variable.Identifier);
                    }
                }
            }
        }
    }
}