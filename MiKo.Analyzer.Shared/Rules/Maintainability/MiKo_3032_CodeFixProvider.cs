using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_3032_CodeFixProvider)), Shared]
    public sealed class MiKo_3032_CodeFixProvider : MaintainabilityCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.Id;

        protected override string Title => Resources.MiKo_3032_CodeFixTitle;

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.OfType<InvocationExpressionSyntax>().FirstOrDefault();

        protected override SyntaxNode GetUpdatedSyntax(Document document, SyntaxNode syntax, Diagnostic issue)
        {
            var expression = NameOf(issue);

            if (issue.Properties.ContainsKey(MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.GetPropertyName))
            {
                return expression;
            }

            if (issue.Properties.ContainsKey(MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.CreateArgs))
            {
                var argument = Argument(expression);
                var typeSyntax = SyntaxFactory.ParseTypeName(nameof(PropertyChangedEventArgs));

                return SyntaxFactory.ObjectCreationExpression(typeSyntax, ArgumentList(argument), null);
            }

            return syntax;
        }

        private static InvocationExpressionSyntax NameOf(Diagnostic diagnostic)
        {
            diagnostic.Properties.TryGetValue(MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.PropertyName, out var propertyName);

            if (diagnostic.Properties.TryGetValue(MiKo_3032_PropertyChangeEventArgsViaCinchAnalyzer.PropertyTypeName, out var propertyTypeName))
            {
                return NameOf(propertyTypeName, propertyName);
            }

            return NameOf(propertyName);
        }
    }
}