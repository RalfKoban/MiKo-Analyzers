using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer : ObjectCreationExpressionMaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3012";

        private static readonly HashSet<string> AllowedExceptionTypes = new HashSet<string>
                                                                            {
                                                                                nameof(ArgumentOutOfRangeException),
                                                                                nameof(InvalidEnumArgumentException),
                                                                                TypeNames.ArgumentOutOfRangeException,
                                                                                TypeNames.InvalidEnumArgumentException,
                                                                            };

        public MiKo_3012_ArgumentOutOfRangeExceptionActualValueAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel) => AllowedExceptionTypes.Contains(node.Type.ToString());

        protected override IEnumerable<Diagnostic> AnalyzeObjectCreation(ObjectCreationExpressionSyntax node, SemanticModel semanticModel)
        {
            var list = node.ArgumentList;

            if (list is null)
            {
                // incomplete, so no issue
                return Enumerable.Empty<Diagnostic>();
            }

            var arguments = list.Arguments;
            switch (arguments.Count)
            {
                case 2:
                    // both are strings, so we have an issue
                    if (arguments[1].IsString(semanticModel))
                    {
                        break;
                    }

                    // it's either the serialization ctor or the one with the inner exception
                    return Enumerable.Empty<Diagnostic>();

                case 3: // correct call
                    return Enumerable.Empty<Diagnostic>();
            }

            return new[] { Issue(node.Type.ToString(), node) };
        }
    }
}