using System;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FindSymbols;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1007_EventArgsNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1007";

        public MiKo_1007_EventArgsNamespaceAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);

        private static bool IsInherited(SyntaxToken syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.GetSymbol(semanticModel);
            return symbol.IsInterfaceImplementation();
        }

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (EventFieldDeclarationSyntax)context.Node;

            var diagnostic = AnalyzeVariableDeclaration(node.Declaration, context.ContainingSymbol, context.SemanticModel);
            if (diagnostic != null)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private Diagnostic AnalyzeVariableDeclaration(VariableDeclarationSyntax declaration, ISymbol containingSymbol, SemanticModel semanticModel)
        {
            var type = declaration.GetTypeSymbol(semanticModel) as INamedTypeSymbol;

            if (type is null)
            {
                return null; // ignore unknown type
            }

            var identifier = declaration.Variables.Select(_ => _.Identifier).FirstOrDefault();

            var eventName = identifier.ValueText;
            if (eventName == nameof(ICommand.CanExecuteChanged) && type.Name == nameof(EventHandler))
            {
                return null; // ignore event that we cannot change anymore
            }

            // we either nave no correct event handler or too few/less type arguments, so try to guess the EventArgs
            if (type.TypeArguments.Length != 1)
            {
                return null;
            }

            var eventArgsType = type.TypeArguments[0];
            if (eventArgsType.IsEventArgs() is false)
            {
                return null;
            }

            if (IsInherited(identifier, semanticModel))
            {
                return null; // ignore inherited events that we cannot change anymore
            }

            var eventTypeNamespace = eventArgsType.ContainingNamespace.FullyQualifiedName();
            var eventUsageNamespace = containingSymbol.ContainingNamespace.FullyQualifiedName();

            if (eventUsageNamespace == eventTypeNamespace)
            {
                return null; // ignore same namespaces
            }

            var location = declaration.Type is GenericNameSyntax g
                               ? g.TypeArgumentList.Arguments[0].GetLocation()
                               : eventArgsType.Locations[0];

            return Issue(eventName, location, eventArgsType.Name, eventUsageNamespace);
        }
    }
}