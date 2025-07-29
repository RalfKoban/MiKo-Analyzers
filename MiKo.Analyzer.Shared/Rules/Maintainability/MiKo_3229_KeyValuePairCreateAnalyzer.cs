using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3229_KeyValuePairCreateAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3229";

        public MiKo_3229_KeyValuePairCreateAnalyzer() : base(Id)
        {
        }

        protected override bool IsApplicable(CompilationStartAnalysisContext context)
        {
            var type = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.KeyValuePair");

            if (type != null)
            {
                return type.GetMembers("Create").Length > 0;
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.Type.GetName() == "KeyValuePair")
            {
                yield return Issue(node);
            }
        }
    }
}