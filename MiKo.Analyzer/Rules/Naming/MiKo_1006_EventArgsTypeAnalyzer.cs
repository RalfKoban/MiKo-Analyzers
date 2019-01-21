using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1006_EventArgsTypeAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1006";

        public MiKo_1006_EventArgsTypeAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (EventFieldDeclarationSyntax)context.Node;

            var diagnostic = AnalyzeVariableDeclaration(node.Declaration, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeVariableDeclaration(VariableDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            var declarationType = declaration.Type;

            var type = semanticModel.GetTypeInfo(declarationType).Type as INamedTypeSymbol;

            switch (type?.Name)
            {
                case null:
                case nameof(NotifyCollectionChangedEventHandler):
                case nameof(PropertyChangingEventHandler):
                case nameof(PropertyChangedEventHandler):
                    return null; // ignore event handlers that we cannot change anymore

                default:
                    var eventArgsType = type.TypeArguments.Length == 1 ? type.TypeArguments[0] : null;
                    var expectedName = declaration.Variables.Select(_ => _.Identifier.ValueText).FirstOrDefault() + nameof(EventArgs);

                    // we either nave no correct event handler or too few/less type arguments
                    return IsProperlyNamed(eventArgsType, expectedName)
                               ? null
                               : ReportIssue(type.Name, declarationType.GetLocation(), expectedName);
            }
        }

        private static bool IsProperlyNamed(ITypeSymbol type, string expectedName) => type?.IsEventArgs() == true && type.Name == expectedName;
    }
}