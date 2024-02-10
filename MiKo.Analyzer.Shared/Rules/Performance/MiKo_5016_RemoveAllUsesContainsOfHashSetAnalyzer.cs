using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Performance
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_5016_RemoveAllUsesContainsOfHashSetAnalyzer : PerformanceAnalyzer
    {
        public const string Id = "MiKo_5016";

        public MiKo_5016_RemoveAllUsesContainsOfHashSetAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol symbol, Compilation compilation)
        {
            var syntax = symbol.GetSyntax();

            SemanticModel semanticModel = null;

            foreach (var node in syntax.DescendantNodes<MemberAccessExpressionSyntax>(_ => _.IsKind(SyntaxKind.SimpleMemberAccessExpression)))
            {
                var name = node.GetName();

                if (name != nameof(List<object>.RemoveAll))
                {
                    continue;
                }

                // inspect for 'Contains' inside lambda or method group
                foreach (var maes in node.Parent.DescendantNodes<MemberAccessExpressionSyntax>(_ => _.GetName() == nameof(Enumerable.Contains)))
                {
                    if (semanticModel is null)
                    {
                        // lazy load semantic model to avoid unnecessary model creations
                        semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
                    }

                    var type = maes.GetTypeSymbol(semanticModel);

                    if (type != null && type.Name != "HashSet" && type.Name != "ISet")
                    {
                        yield return Issue(maes);
                    }
                }
            }
        }
    }
}