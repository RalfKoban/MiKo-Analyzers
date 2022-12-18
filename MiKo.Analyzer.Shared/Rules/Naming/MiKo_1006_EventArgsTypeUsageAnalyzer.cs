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
    public sealed class MiKo_1006_EventArgsTypeUsageAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1006";

        public MiKo_1006_EventArgsTypeUsageAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);

        private static bool IsProperlyNamed(ITypeSymbol type, string expectedName) => type?.IsEventArgs() is true && type.Name == expectedName;

        private static bool IsInherited(SyntaxToken syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.GetSymbol(semanticModel);

            return symbol.IsInterfaceImplementation();
        }

        private static bool SkipType(string typeName) => typeName is null // ignore unknown type
                                                      || Constants.Names.KnownWindowsEventHandlers.Contains(typeName);

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (EventFieldDeclarationSyntax)context.Node;
            var issue = AnalyzeVariableDeclaration(node.Declaration, context.SemanticModel);

            ReportDiagnostics(context, issue);
        }

        private Diagnostic AnalyzeVariableDeclaration(VariableDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            var type = declaration.GetTypeSymbol(semanticModel) as INamedTypeSymbol;

            var typeName = type?.Name;

            if (SkipType(typeName))
            {
                return null;
            }

            var identifier = declaration.Variables.Select(_ => _.Identifier).FirstOrDefault();

            var eventName = identifier.ValueText;

            if (eventName == nameof(ICommand.CanExecuteChanged) && typeName == nameof(EventHandler))
            {
                return null; // ignore event that we cannot change anymore
            }

            // we either nave no correct event handler or too few/less type arguments, so try to guess the EventArgs
            var eventArgsType = type?.TypeArguments.Length == 1 ? type.TypeArguments[0] : null;
            var expectedName = eventName + nameof(EventArgs);

            if (IsProperlyNamed(eventArgsType, expectedName))
            {
                return null;
            }

            if (IsInherited(identifier, semanticModel))
            {
                return null; // ignore inherited events that we cannot change anymore
            }

            return Issue(typeName, declaration.Type, expectedName);
        }
    }
}