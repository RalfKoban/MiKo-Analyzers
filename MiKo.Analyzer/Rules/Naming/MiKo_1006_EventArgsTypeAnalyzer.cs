using System;
using System.Linq;
using System.Windows.Input;

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

            var typeName = type?.Name;
            if (typeName == null)
                return null; // ignore unknown type

            var identifier = declaration.Variables.Select(_ => _.Identifier).FirstOrDefault();

            var eventName = identifier.ValueText;
            if (eventName == nameof(ICommand.CanExecuteChanged) && typeName == nameof(EventHandler))
                return null; // ignore event that we cannot change anymore

            // we either nave no correct event handler or too few/less type arguments, so try to guess the EventArgs
            var eventArgsType = type.TypeArguments.Length == 1 ? type.TypeArguments[0] : null;
            var expectedName = eventName + nameof(EventArgs);

            if (IsProperlyNamed(eventArgsType, expectedName))
                return null;

            if (IsInherited(identifier, semanticModel))
                return null; // ignore inherited events that we cannot change anymore

            return ReportIssue(typeName, declarationType.GetLocation(), expectedName);
        }

        private static bool IsProperlyNamed(ITypeSymbol type, string expectedName) => type?.IsEventArgs() == true && type.Name == expectedName;

        private static bool IsInherited(SyntaxToken syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.GetSymbol(semanticModel);
            return symbol.IsInterfaceImplementation();
        }
    }
}