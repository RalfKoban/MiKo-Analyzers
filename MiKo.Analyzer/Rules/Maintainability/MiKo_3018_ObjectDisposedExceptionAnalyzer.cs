using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3018_ObjectDisposedExceptionAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3018";

        public MiKo_3018_ObjectDisposedExceptionAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event);

        protected override bool ShallAnalyze(INamedTypeSymbol symbol) => symbol.TypeKind != TypeKind.Interface && symbol.Implements<IDisposable>();

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (ShallAnalyze(symbol.ContainingType) && IsPubliclyVisible(symbol))
            {
                switch (symbol.MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.Destructor:
                    case MethodKind.UserDefinedOperator:
                    case MethodKind.StaticConstructor:
                        return false;

                    default:
                        return symbol.Name != nameof(IDisposable.Dispose); // dispose methods are allowed to NOT throw ObjectDisposedExceptions
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            if (ThrowsObjectDisposedException(symbol) is false)
            {
                yield return Issue(symbol);
            }
        }

        private static bool IsPubliclyVisible(ISymbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return false;

                default:
                    return true;
            }
        }

        private static bool ThrowsObjectDisposedException(IMethodSymbol symbol)
        {
            var methods = symbol.ContainingType.GetNamedMethods().Select(_ => _.Name).ToHashSet();

            foreach (var node in symbol.GetSyntax().DescendantNodes())
            {
                if (node is ThrowStatementSyntax t && t.Expression is ObjectCreationExpressionSyntax o && o.Type.IsException<ObjectDisposedException>())
                {
                    return true;
                }

                // Inspect code for calls of private helper methods
                if (node is InvocationExpressionSyntax i && i.Expression is IdentifierNameSyntax ii)
                {
                    var name = ii.GetName();

                    if (name.Contains("Dispos") && methods.Contains(name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}