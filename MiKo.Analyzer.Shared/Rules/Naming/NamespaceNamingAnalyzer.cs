using System;
using System.Buffers;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamespaceNamingAnalyzer : NamingAnalyzer
    {
        private const int MaximumNamespaceDepth = 32; // this is a reasonable number for namespaces, so we can use it as the size for the rented array

        private static readonly ArrayPool<SyntaxToken> Pool = ArrayPool<SyntaxToken>.Shared;

        protected NamespaceNamingAnalyzer(string diagnosticId) : base(diagnosticId, (SymbolKind)(-1))
        {
        }

        protected sealed override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);

        protected abstract IReadOnlyList<Diagnostic> AnalyzeNamespaceName(in ReadOnlySpan<SyntaxToken> namespaceNames);

        private static ReadOnlySpan<SyntaxToken> CollectNames(NamespaceDeclarationSyntax node, in SyntaxToken[] rentedArray)
        {
            switch (node.Name)
            {
                case QualifiedNameSyntax name:
                {
                    // as the identifiers arrive in reverse order (due to their recursive nature), we have to start at the end of the rented array and move backwards to collect the identifiers in the correct order
                    var i = MaximumNamespaceDepth - 1;

                    rentedArray[i--] = name.Right.Identifier;

                    var loop = true;

                    while (loop)
                    {
                        if (i < 0)
                        {
                            // stop endless loop as we have reached the maximum namespace depth
                            break;
                        }

                        switch (name.Left)
                        {
                            case QualifiedNameSyntax qualifiedName:
                            {
                                name = qualifiedName;
                                rentedArray[i--] = name.Right.Identifier;

                                continue;
                            }

                            case IdentifierNameSyntax identifierName:
                            {
                                // we are at the last identifier, so we can stop here
                                loop = false;
                                rentedArray[i--] = identifierName.Identifier;

                                continue;
                            }
                        }

                        // stop endless loop as we have an unexpected syntax node
                        break;
                    }

                    return rentedArray.AsSpan(i + 1, MaximumNamespaceDepth - i - 1);
                }

                case IdentifierNameSyntax root:
                {
                    rentedArray[0] = root.Identifier;

                    return rentedArray.AsSpan(0, 1);
                }
            }

            // we have an unexpected syntax node, so we cannot analyze this namespace declaration
            return ReadOnlySpan<SyntaxToken>.Empty;
        }

        private void AnalyzeNamespaceDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (NamespaceDeclarationSyntax)context.Node;

            var rentedArray = Pool.Rent(MaximumNamespaceDepth);

            try
            {
                var names = CollectNames(node, rentedArray);

                if (names.Length > 0)
                {
                    var issues = AnalyzeNamespaceName(names);

                    if (issues.Count > 0)
                    {
                        ReportDiagnostics(context, issues);
                    }
                }
            }
            finally
            {
                Pool.Return(rentedArray);
            }
        }
    }
}