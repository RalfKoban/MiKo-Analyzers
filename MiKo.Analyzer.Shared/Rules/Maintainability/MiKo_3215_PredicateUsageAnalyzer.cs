using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    /// <inheritdoc />
    /// <seealso cref="MiKo_3001_DelegateAnalyzer"/>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3215_PredicateUsageAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3215";

        public MiKo_3215_PredicateUsageAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol)
                                                                   && (symbol.MethodKind is MethodKind.Ordinary || symbol.MethodKind is MethodKind.Constructor)
                                                                   && symbol.IsInterfaceImplementation() is false;

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsInterfaceImplementation() is false;

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            foreach (var parameter in symbol.Parameters)
            {
                if (IsPredicate(parameter.Type, out ITypeSymbol genericType))
                {
                    yield return Issue(parameter.GetSyntax<ParameterSyntax>().Type, genericType.Name);
                }
            }
        }

        protected override IEnumerable<Diagnostic> Analyze(IPropertySymbol symbol, Compilation compilation)
        {
            if (IsPredicate(symbol.Type, out ITypeSymbol genericType))
            {
                yield return Issue(symbol.GetSyntax<BasePropertyDeclarationSyntax>().Type, genericType.Name);
            }
        }

        protected override IEnumerable<Diagnostic> Analyze(IFieldSymbol symbol, Compilation compilation)
        {
            if (IsPredicate(symbol.Type, out ITypeSymbol genericType))
            {
                yield return Issue(symbol.GetSyntax<FieldDeclarationSyntax>().Declaration.Type, genericType.Name);
            }
        }

        private static bool IsPredicate(ITypeSymbol type, out ITypeSymbol genericType)
        {
            genericType = null;

            return type.TypeKind is TypeKind.Delegate && type.Name is "Predicate" && type.TryGetGenericArgumentType(out genericType);
        }
    }
}