using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3013";

        private static readonly HashSet<string> AllowedExceptionTypes = new HashSet<string>
                                                                            {
                                                                                nameof(ArgumentException),
                                                                                nameof(ArgumentNullException),
                                                                                TypeNames.ArgumentException,
                                                                                TypeNames.ArgumentNullException,
                                                                            };

        public MiKo_3013_ArgumentOutOfRangeExceptionSwitchStatementAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => AllowedExceptionTypes.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var switchSection = node.GetEnclosing<SwitchSectionSyntax>();

            // we are in the 'default:' clause if there is a 'default' switch label in the specific switch section
            var isDefaultClause = switchSection?.DescendantNodes().OfType<DefaultSwitchLabelSyntax>().Any() is true;
            return isDefaultClause
                       ? new[] { Issue(node.Type.ToString(), node.GetLocation()) }
                       : Enumerable.Empty<Diagnostic>();
        }
    }
}