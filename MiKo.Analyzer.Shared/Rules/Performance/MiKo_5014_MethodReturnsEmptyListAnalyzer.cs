using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5014_MethodReturnsEmptyListAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5014";

        public MiKo_5014_MethodReturnsEmptyListAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.ReturnsVoid is false && symbol.ContainingType.TypeKind is TypeKind.Class)
            {
                var returnType = symbol.ReturnType;

                if (returnType.TypeKind is TypeKind.Interface)
                {
                    switch (returnType.OriginalDefinition.SpecialType)
                    {
                        case SpecialType.System_Collections_Generic_IEnumerable_T:
                        case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                        case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                            return true;
                    }
                }
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            switch (symbol.GetSyntax())
            {
                case BaseMethodDeclarationSyntax method:
                    return Analyze(method, symbol.Name);

                case AccessorDeclarationSyntax accessor:
                    return Analyze(accessor, symbol.Name);

                default:
                    return Array.Empty<Diagnostic>();
            }
        }

        private static bool HasIssue(ObjectCreationExpressionSyntax creation)
        {
            if (creation.Type is GenericNameSyntax generic && generic.GetName() is "List")
            {
                var argumentList = creation.ArgumentList;

                if (argumentList is null)
                {
                    var initializer = creation.Initializer;

                    return initializer is null || initializer.Expressions.Count is 0;
                }

                var arguments = argumentList.Arguments;

                switch (arguments.Count)
                {
                    case 0:
                    case 1 when arguments[0].Expression is LiteralExpressionSyntax:
                        return true;
                }
            }

            return false;
        }

        private Diagnostic[] Analyze(AccessorDeclarationSyntax accessor, string symbolName)
        {
            var expressionBody = accessor.ExpressionBody;

            return expressionBody != null
                   ? Analyze(expressionBody, symbolName)
                   : Analyze(accessor.Body, symbolName);
        }

        private Diagnostic[] Analyze(BaseMethodDeclarationSyntax method, string symbolName)
        {
            var expressionBody = method.ExpressionBody;

            return expressionBody != null
                   ? Analyze(expressionBody, symbolName)
                   : Analyze(method.Body, symbolName);
        }

        private Diagnostic[] Analyze(ArrowExpressionClauseSyntax expressionBody, string symbolName)
        {
            return Analyze(expressionBody.DescendantNodes<ObjectCreationExpressionSyntax>(), symbolName);
        }

        private Diagnostic[] Analyze(BlockSyntax body, string symbolName)
        {
            if (body is null)
            {
                return Array.Empty<Diagnostic>();
            }

            return Analyze(body.DescendantNodes<ReturnStatementSyntax>().SelectMany(_ => _.DescendantNodes<ObjectCreationExpressionSyntax>()), symbolName);
        }

        private Diagnostic[] Analyze(IEnumerable<ObjectCreationExpressionSyntax> creations, string symbolName)
        {
            List<Diagnostic> issues = null;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var creation in creations)
            {
                if (HasIssue(creation))
                {
                    if (issues is null)
                    {
                        issues = new List<Diagnostic>(1);
                    }

                    issues.Add(Issue(symbolName, creation));
                }
            }

            return issues?.ToArray() ?? Array.Empty<Diagnostic>();
        }
    }
}