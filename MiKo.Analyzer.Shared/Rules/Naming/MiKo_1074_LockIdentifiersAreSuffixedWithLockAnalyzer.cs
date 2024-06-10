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
    public sealed class MiKo_1074_LockIdentifiersAreSuffixedWithLockAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1074";

        public MiKo_1074_LockIdentifiersAreSuffixedWithLockAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);

        private static HashSet<string> CollectLockIdentifiers(SyntaxNode declaration)
        {
            var lockIdentifiers = new HashSet<string>();

            foreach (var node in declaration.DescendantNodes<LockStatementSyntax>())
            {
                if (node.Expression is IdentifierNameSyntax identifierNameSyntax)
                {
                    var identifier = identifierNameSyntax.Identifier;
                    var name = identifier.ValueText;

                    if (name.EndsWith("syncRoot", StringComparison.OrdinalIgnoreCase))
                    {
                        // special name, so nothing to report here
                        continue;
                    }

                    if (name.EndsWith("Lock", StringComparison.Ordinal))
                    {
                        // name already ends with 'Lock', so nothing to report here
                        continue;
                    }

                    lockIdentifiers.Add(name);
                }
            }

            return lockIdentifiers;
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var lockIdentifiers = CollectLockIdentifiers(context.Node);

            if (lockIdentifiers.Any() && context.ContainingSymbol is ITypeSymbol type)
            {
                var fields = new Dictionary<string, IFieldSymbol>();

                // it seems that some fields are duplicated, so avoid AD0001 due to thrown exception
                foreach (var field in type.GetFields())
                {
                    fields[field.Name] = field;
                }

                foreach (var identifier in lockIdentifiers)
                {
                    if (fields.TryGetValue(identifier, out var field))
                    {
                        context.ReportDiagnostic(Issue(field));
                    }
                }
            }
        }
    }
}