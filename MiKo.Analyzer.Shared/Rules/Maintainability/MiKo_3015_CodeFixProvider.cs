using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3015_CodeFixProvider)), Shared]
    public sealed class MiKo_3015_CodeFixProvider : ObjectCreationExpressionMaintainabilityCodeFixProvider
    {
        private const string Namespace = "System.ComponentModel";

        private static readonly HashSet<string> ComponentModelTypes = typeof(InvalidEnumArgumentException).Assembly.GetTypes().Where(_ => _.Namespace == Namespace).ToHashSet(_ => _.Name);

        public override string FixableDiagnosticId => "MiKo_3015";

        protected override TypeSyntax GetUpdatedSyntaxType(ObjectCreationExpressionSyntax syntax) => nameof(InvalidOperationException).AsTypeSyntax();

        protected override ArgumentListSyntax GetUpdatedArgumentListSyntax(ObjectCreationExpressionSyntax syntax)
        {
            var argumentList = syntax.ArgumentList;
            var arguments = argumentList.Arguments;

            var errorMessage = arguments.Count is 3
                               ? GetUpdatedErrorMessage(arguments.RemoveAt(1)) // actual argument seems to be part of the exception, so we have to ignore it when trying to find the error message
                               : GetUpdatedErrorMessage(argumentList);

            return ArgumentList(errorMessage);
        }

        protected override Task<SyntaxNode> GetUpdatedSyntaxRootAsync(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue, CancellationToken cancellationToken)
        {
            var updatedSyntax = GetUpdatedSyntaxRoot(root);

            return Task.FromResult(updatedSyntax);
        }

        private static SyntaxNode GetUpdatedSyntaxRoot(SyntaxNode root)
        {
            if (root.DescendantNodes<IdentifierNameSyntax>().None(_ => ComponentModelTypes.Contains(_.GetName())))
            {
                return root.WithoutUsing(Namespace); // remove unused "using System.ComponentModel;"
            }

            return root;
        }
    }
}