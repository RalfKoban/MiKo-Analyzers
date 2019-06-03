using System;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxExtensions
    {
        internal static ISymbol GetSymbol(this SyntaxToken token, SemanticModel semanticModel)
        {
            var position = token.GetLocation().SourceSpan.Start;
            var name = token.ValueText;
            var syntaxNode = token.Parent;

            if (syntaxNode is ParameterSyntax node)
            {
                // we might have a ctor here and no method
                var methodName = node.GetEnclosing<MethodDeclarationSyntax>()?.Identifier.ValueText ?? node.GetEnclosing<ConstructorDeclarationSyntax>()?.Identifier.ValueText;
                var methodSymbols = semanticModel.LookupSymbols(position, name: methodName).OfType<IMethodSymbol>();
                var parameterSymbol = methodSymbols.SelectMany(_ => _.Parameters).FirstOrDefault(_ => _.Name == name);
                return parameterSymbol;

                // if it's no method parameter, then it is a local one (but Roslyn cannot handle that currently)
                //var symbol = semanticModel.LookupSymbols(position).First(_ => _.Kind == SymbolKind.Local);
            }

            var symbols = semanticModel.LookupSymbols(position, name: name);
            if (symbols.Length > 0)
                return symbols[0];

            // nothing is found, so maybe it is an identifier syntax token within a foreach statement
            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);
            return symbol;
        }

        internal static INamedTypeSymbol GetTypeSymbol(this ExpressionSyntax syntax, SemanticModel semanticModel) => semanticModel.GetTypeInfo(syntax).Type as INamedTypeSymbol;

        internal static INamedTypeSymbol GetTypeSymbol(this TypeSyntax syntax, SemanticModel semanticModel) => semanticModel.GetTypeInfo(syntax).Type as INamedTypeSymbol;

        internal static INamedTypeSymbol GetTypeSymbol(this VariableDeclarationSyntax syntax, SemanticModel semanticModel) => syntax.Type.GetTypeSymbol(semanticModel);

        internal static ISymbol GetEnclosingSymbol(this SyntaxNode node, SemanticModel semanticModel)
        {
            switch (node)
            {
                case MethodDeclarationSyntax s:
                    return semanticModel.GetDeclaredSymbol(s);
                case PropertyDeclarationSyntax p:
                    return semanticModel.GetDeclaredSymbol(p);
                case ConstructorDeclarationSyntax c:
                    return semanticModel.GetDeclaredSymbol(c);
                case FieldDeclarationSyntax f:
                    return semanticModel.GetDeclaredSymbol(f);
                case EventDeclarationSyntax e:
                    return semanticModel.GetDeclaredSymbol(e);
                default:
                    return semanticModel.GetEnclosingSymbol(node.GetLocation().SourceSpan.Start);
            }
        }

        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNodeAnalysisContext context) => GetEnclosingMethod(context.Node, context.SemanticModel);

        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNode node, SemanticModel semanticModel) => node.GetEnclosingSymbol(semanticModel) as IMethodSymbol;

        internal static T GetEnclosing<T>(this SyntaxNode node) where T : SyntaxNode
        {
            while (true)
            {
                switch (node)
                {
                    case null: return null;
                    case T t: return t;
                }

                node = node.Parent;
            }
        }

        internal static SyntaxNode GetEnclosing(this SyntaxNode node, params SyntaxKind[] syntaxKinds)
        {
            while (true)
            {
                if (node == null)
                    return null;

                foreach (var syntaxKind in syntaxKinds)
                {
                    if (node.IsKind(syntaxKind))
                        return node;
                }

                node = node.Parent;
            }
        }

        internal static string GetNameOnlyPart(this TypeSyntax syntax) => syntax.ToString().GetNameOnlyPart();

        internal static bool IsCommand(this TypeSyntax syntax, SemanticModel semanticModel)
        {
            var name = syntax.ToString();

            return name.Contains("Command")
                && semanticModel.LookupSymbols(syntax.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol
                && symbol.IsCommand();
        }

        internal static bool IsString(this ExpressionSyntax syntax, SemanticModel semanticModel) => syntax.GetTypeSymbol(semanticModel)?.SpecialType == SpecialType.System_String;

        internal static bool IsStruct(this ExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var type = syntax.GetTypeSymbol(semanticModel);
            switch (type?.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Enum:
                    return true;

                default:
                    return false;
            }
        }

        internal static string ToCleanedUpString(this ExpressionSyntax s) => s?.ToString().RemoveAll(Constants.WhiteSpaces);

        internal static bool IsInsideIfStatementWithCallTo(this SyntaxNode node, string methodName)
        {
            var ifStatement = GetEnclosingIfStatement(node);
            var ifExpression = ifStatement?.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

            var inside = ifExpression?.Name.ToString() == methodName;
            return inside;
        }

        private static IfStatementSyntax GetEnclosingIfStatement(SyntaxNode node)
        {
            // consider brackets:
            //                    if (true)
            //                    {
            //                      xyz();
            //                    }
            //
            //  and no brackets:
            //                    if (true)
            //                      xyz();
            //
            var enclosingNode = node.GetEnclosing(SyntaxKind.Block, SyntaxKind.IfStatement);
            if (enclosingNode is BlockSyntax)
                enclosingNode = enclosingNode.Parent;

            return enclosingNode as IfStatementSyntax;
        }
    }
}