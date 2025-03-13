﻿using System;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1007_EventArgsNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1007";

        public MiKo_1007_EventArgsNamespaceAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeEventFieldDeclaration, SyntaxKind.EventFieldDeclaration);

        private static bool IsInherited(SyntaxToken syntax, SemanticModel semanticModel)
        {
            var symbol = syntax.GetSymbol(semanticModel);

            return symbol.IsInterfaceImplementation();
        }

        private void AnalyzeEventFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            var node = (EventFieldDeclarationSyntax)context.Node;
            var issue = AnalyzeVariableDeclaration(node.Declaration, context.ContainingSymbol, context.SemanticModel);

            if (issue != null)
            {
                ReportDiagnostics(context, issue);
            }
        }

        private Diagnostic AnalyzeVariableDeclaration(VariableDeclarationSyntax declaration, ISymbol containingSymbol, SemanticModel semanticModel)
        {
            if (declaration.GetTypeSymbol(semanticModel) is INamedTypeSymbol type)
            {
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

                if (eventArgsType.FullyQualifiedName() == "System.EventArgs")
                {
                    return null; // ignore special event args
                }

                var eventTypeNamespace = eventArgsType.ContainingNamespace.FullyQualifiedName();
                var eventUsageNamespace = containingSymbol.ContainingNamespace.FullyQualifiedName();

                if (eventUsageNamespace == eventTypeNamespace)
                {
                    return null; // ignore same namespaces
                }

                if (declaration.Type is GenericNameSyntax g)
                {
                    return Issue(eventName, g.TypeArgumentList.Arguments[0], eventArgsType.Name, eventUsageNamespace);
                }

                return Issue(eventName, eventArgsType, eventArgsType.Name, eventUsageNamespace);
            }

            return null; // ignore unknown type
        }
    }
}