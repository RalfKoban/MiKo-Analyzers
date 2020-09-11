using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SyntaxExtensions
    {
        internal static readonly SyntaxTrivia XmlCommentExterior = SyntaxFactory.DocumentationCommentExterior("/// ");

        private static readonly SyntaxTrivia[] XmlCommentStart =
                                                                {
                                                                    SyntaxFactory.ElasticCarriageReturnLineFeed, // use elastic one to allow formatting to be done automatically
                                                                    XmlCommentExterior,
                                                                };

        internal static bool IsSupported(this SyntaxNodeAnalysisContext context, LanguageVersion expectedVersion)
        {
            var languageVersion = ((CSharpParseOptions)context.Node.SyntaxTree.Options).LanguageVersion;

            // ignore the latest versions (or above)
            return languageVersion >= expectedVersion && expectedVersion < LanguageVersion.LatestMajor;
        }

        internal static bool IsTestMethod(this MethodDeclarationSyntax method) => method.GetAttributeNames().Any(Constants.Names.TestMethodAttributeNames.Contains);

        internal static bool IsTestSetUpMethod(this MethodDeclarationSyntax method) => method.GetAttributeNames().Any(Constants.Names.TestSetupAttributeNames.Contains);

        internal static bool IsTestTearDownMethod(this MethodDeclarationSyntax method) => method.GetAttributeNames().Any(Constants.Names.TestTearDownAttributeNames.Contains);

        internal static bool IsTestOneTimeSetUpMethod(this MethodDeclarationSyntax method) => method.GetAttributeNames().Any(Constants.Names.TestOneTimeSetupAttributeNames.Contains);

        internal static bool IsTestOneTimeTearDownMethod(this MethodDeclarationSyntax method) => method.GetAttributeNames().Any(Constants.Names.TestOneTimeTearDownAttributeNames.Contains);

        internal static bool IsTypeUnderTestCreationMethod(this MethodDeclarationSyntax method) => Constants.Names.TypeUnderTestMethodNames.Contains(method.GetName());

        internal static bool IsTypeUnderTestVariable(this VariableDeclaratorSyntax syntax) => Constants.Names.TypeUnderTestVariableNames.Contains(syntax.GetName());

        internal static ISymbol GetSymbol(this SyntaxToken token, SemanticModel semanticModel)
        {
            var position = token.GetLocation().SourceSpan.Start;
            var name = token.ValueText;
            var syntaxNode = token.Parent;

            if (syntaxNode is ParameterSyntax node)
            {
                // we might have a ctor here and no method
                var methodName = node.GetEnclosing<MethodDeclarationSyntax>().GetName() ?? node.GetEnclosing<ConstructorDeclarationSyntax>().GetName();
                var methodSymbols = semanticModel.LookupSymbols(position, name: methodName).OfType<IMethodSymbol>();
                var parameterSymbol = methodSymbols.SelectMany(_ => _.Parameters).FirstOrDefault(_ => _.Name == name);
                return parameterSymbol;

                // if it's no method parameter, then it is a local one (but Roslyn cannot handle that currently in v3.3)
                // var symbol = semanticModel.LookupSymbols(position).First(_ => _.Kind == SymbolKind.Local);
            }

            var symbols = semanticModel.LookupSymbols(position, name: name);
            if (symbols.Length > 0)
            {
                return symbols[0];
            }

            // nothing is found, so maybe it is an identifier syntax token within a foreach statement
            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);
            return symbol;
        }

        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax syntax, SemanticModel semanticModel)
        {
            var type = syntax.Expression.GetTypeSymbol(semanticModel);
            return type;
        }

        internal static ITypeSymbol GetTypeSymbol(this ExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(syntax);
            return typeInfo.Type;
        }

        internal static ITypeSymbol GetTypeSymbol(this MemberAccessExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var type = syntax.Expression.GetTypeSymbol(semanticModel);
            return type;
        }

        internal static ITypeSymbol GetTypeSymbol(this TypeSyntax syntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(syntax);
            return typeInfo.Type;
        }

        internal static ITypeSymbol GetTypeSymbol(this BaseTypeSyntax syntax, SemanticModel semanticModel)
        {
            var type = syntax.Type.GetTypeSymbol(semanticModel);
            return type;
        }

        internal static ITypeSymbol GetTypeSymbol(this ClassDeclarationSyntax syntax, SemanticModel semanticModel)
        {
            var symbol = GetSymbol(syntax.Identifier, semanticModel);
            return symbol as ITypeSymbol;
        }

        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax syntax, SemanticModel semanticModel) => syntax.Type.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this SyntaxNode syntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(syntax);
            return typeInfo.Type;
        }

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
                if (node is null)
                {
                    return null;
                }

                foreach (var syntaxKind in syntaxKinds)
                {
                    if (node.IsKind(syntaxKind))
                    {
                        return node;
                    }
                }

                node = node.Parent;
            }
        }

        internal static string GetName(this MemberAccessExpressionSyntax syntax) => syntax?.Name.GetName();

        internal static string GetName(this MemberBindingExpressionSyntax syntax) => syntax?.Name.GetName();

        internal static string GetName(this SimpleNameSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetName(this VariableDeclaratorSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetName(this MethodDeclarationSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetName(this PropertyDeclarationSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetName(this ConstructorDeclarationSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetName(this ParameterSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetName(this IdentifierNameSyntax syntax) => syntax?.Identifier.ValueText;

        internal static string GetNameOnlyPart(this TypeSyntax syntax) => syntax.ToString().GetNameOnlyPart();

        internal static bool IsCommand(this TypeSyntax syntax, SemanticModel semanticModel)
        {
            var name = syntax.ToString();

            return name.Contains("Command")
                && semanticModel.LookupSymbols(syntax.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol
                && symbol.IsCommand();
        }

        internal static bool IsString(this ExpressionSyntax syntax, SemanticModel semanticModel) => syntax.GetTypeSymbol(semanticModel)?.SpecialType == SpecialType.System_String;

        internal static bool IsString(this TypeSyntax syntax)
        {
            switch (syntax.ToString())
            {
                case "string":
                case nameof(String):
                case nameof(System) + "." + nameof(String):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsSerializationInfo(this TypeSyntax syntax)
        {
            var s = syntax.ToString();
            return s == nameof(SerializationInfo) || s == typeof(SerializationInfo).FullName;
        }

        internal static bool IsStreamingContext(this TypeSyntax syntax)
        {
            var s = syntax.ToString();
            return s == nameof(StreamingContext) || s == typeof(StreamingContext).FullName;
        }

        internal static bool IsException(this TypeSyntax syntax)
        {
            switch (syntax.ToString())
            {
                case nameof(Exception):
                case nameof(System) + "." + nameof(Exception):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsBoolean(this TypeSyntax syntax)
        {
            switch (syntax.ToString())
            {
                case "bool":
                case nameof(Boolean):
                case nameof(System) + "." + nameof(Boolean):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsByte(this TypeSyntax syntax)
        {
            switch (syntax.ToString())
            {
                case "byte":
                case nameof(Byte):
                case nameof(System) + "." + nameof(Byte):
                    return true;

                default:
                    return false;
            }
        }

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

        internal static bool IsVoid(this TypeSyntax type) => type is PredefinedTypeSyntax p && p.Keyword.IsKind(SyntaxKind.VoidKeyword);

        internal static string ToCleanedUpString(this ExpressionSyntax source) => source?.ToString().Without(Constants.WhiteSpaces);

        internal static bool IsInsideIfStatementWithCallTo(this SyntaxNode node, string methodName)
        {
            while (true)
            {
                var ifStatement = GetEnclosingIfStatement(node);
                if (ifStatement != null)
                {
                    if (IsIfStatementWithCallTo(ifStatement, methodName))
                    {
                        return true;
                    }

                    // maybe a nested one, so check parent
                    node = ifStatement.Parent;
                    continue;
                }

                // maybe an else block
                var elseStatement = GetEnclosingElseStatement(node);
                if (elseStatement != null)
                {
                    node = elseStatement.Parent;
                    continue;
                }

                return false;
            }
        }

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode syntaxNode, SemanticModel semanticModel) => syntaxNode.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                                                                                                                                           .Where(_ => IsLinqExtensionMethod(semanticModel.GetSymbolInfo(_)));

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax comment, string text)
        {
            var contents = new List<XmlNodeSyntax>(comment.Content);

            for (var index = 0; index < comment.Content.Count; index++)
            {
                if (comment.Content[index] is XmlTextSyntax s)
                {
                    var originalTextTokens = s.TextTokens;
                    var textTokens = new List<SyntaxToken>(originalTextTokens);

                    for (var i = 0; i < originalTextTokens.Count; i++)
                    {
                        var token = originalTextTokens[i];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.Text.Contains(text))
                        {
                            var modifiedText = token.Text.Without(text).Replace("  ", " ").TrimEnd();
                            if (modifiedText.IsNullOrWhiteSpace())
                            {
                                textTokens.Remove(token);

                                if (i > 0)
                                {
                                    textTokens.Remove(originalTextTokens[i - 1]);
                                }
                            }
                            else
                            {
                                textTokens[i] = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), modifiedText, modifiedText, token.TrailingTrivia);
                            }
                        }
                    }

                    var xmlText = SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
                    contents[index] = xmlText;
                }
            }

            return SyntaxFactory.List(contents);
        }

        internal static XmlTextSyntax WithStartText(this XmlTextSyntax text, string startText)
        {
            var textTokens = new List<SyntaxToken>(text.TextTokens);

            var replaced = false;

            for (var i = 0; i < textTokens.Count; i++)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                {
                    var originalText = token.Text;

                    if (originalText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    var space = i == 0 ? string.Empty : " ";

                    var modifiedText = space + startText + originalText.TrimStart().ToLowerCaseAt(0);

                    textTokens[i] = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), modifiedText, modifiedText, token.TrailingTrivia);
                    replaced = true;
                    break;
                }
            }

            if (replaced is false)
            {
                return SyntaxFactory.XmlText(startText);
            }

            return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
        }

        internal static SyntaxToken WithLeadingXmlComment(this SyntaxToken token) => token.WithLeadingTrivia(XmlCommentStart);

        internal static T WithLeadingXmlComment<T>(this T node) where T : SyntaxNode => node.WithLeadingTrivia(XmlCommentStart);

        internal static SyntaxToken WithTrailingXmlComment(this SyntaxToken token) => token.WithTrailingTrivia(XmlCommentStart);

        internal static T WithTrailingXmlComment<T>(this T node) where T : SyntaxNode => node.WithTrailingTrivia(XmlCommentStart);

        internal static T WithIntentation<T>(this T node) where T : SyntaxNode => node.WithoutLeadingTrivia().WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithEndOfLine<T>(this T node) where T : SyntaxNode => node.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithEndOfLine(this SyntaxToken token) => token.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithLeadingEndOfLine<T>(this T node) where T : SyntaxNode => node.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingEndOfLine(this SyntaxToken token) => token.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static string WithoutXmlCommentExterior(this SyntaxNode syntaxNode) => syntaxNode.ToString().Replace("///", string.Empty).Trim();

        internal static bool HasLinqExtensionMethod(this SyntaxNode syntaxNode, SemanticModel semanticModel) => syntaxNode.LinqExtensionMethods(semanticModel).Any();

        internal static TRoot InsertNodeBefore<TRoot>(this TRoot root, SyntaxNode nodeInList, SyntaxNode newNode) where TRoot : SyntaxNode
        {
            // method needs to be intended and a CRLF needs to be added
            var modifiedNode = newNode.WithIntentation().WithEndOfLine();

            return root.InsertNodesBefore(nodeInList, new[] { modifiedNode });
        }

        internal static TRoot InsertNodeAfter<TRoot>(this TRoot root, SyntaxNode nodeInList, SyntaxNode newNode) where TRoot : SyntaxNode
        {
            return root.InsertNodesAfter(nodeInList, new[] { newNode });
        }

        internal static BaseTypeDeclarationSyntax RemoveNodeAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax syntax, MethodDeclarationSyntax method)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = syntax.OpenBraceToken.WithoutTrivia().WithEndOfLine();
            var closeBraceToken = syntax.CloseBraceToken.WithoutTrivia().WithLeadingEndOfLine().WithTrailingTrivia(syntax.CloseBraceToken.TrailingTrivia);

            return syntax.RemoveNode(method, SyntaxRemoveOptions.KeepNoTrivia)
                         .WithOpenBraceToken(openBraceToken)
                         .WithCloseBraceToken(closeBraceToken);
        }

        private static IEnumerable<string> GetAttributeNames(this MethodDeclarationSyntax method) => method.AttributeLists.SelectMany(_ => _.Attributes).Select(_ => _.Name.GetNameOnlyPart());

        private static bool IsLinqExtensionMethod(SymbolInfo info) => info.Symbol.IsLinqExtensionMethod() || info.CandidateSymbols.Any(_ => _.IsLinqExtensionMethod());

        private static bool IsIfStatementWithCallTo(IfStatementSyntax ifStatement, string methodName)
        {
            var ifExpression = ifStatement.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (ifExpression?.Name.ToString() == methodName)
            {
                return true;
            }

            var binaryExpression = ifStatement.ChildNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();
            if (binaryExpression?.OperatorToken.Kind() == SyntaxKind.AmpersandAmpersandToken)
            {
                if ((binaryExpression.Left is MemberAccessExpressionSyntax l && l.Name.ToString() == methodName)
                 || (binaryExpression.Right is MemberAccessExpressionSyntax r && r.Name.ToString() == methodName))
                {
                    return true;
                }
            }

            return false;
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
            // ...
            var enclosingNode = node.GetEnclosing(SyntaxKind.Block, SyntaxKind.IfStatement);
            if (enclosingNode is BlockSyntax)
            {
                enclosingNode = enclosingNode.Parent;
            }

            return enclosingNode as IfStatementSyntax;
        }

        private static ElseClauseSyntax GetEnclosingElseStatement(SyntaxNode node)
        {
            var enclosingNode = node.GetEnclosing(SyntaxKind.Block, SyntaxKind.ElseClause);
            if (enclosingNode is BlockSyntax)
            {
                enclosingNode = enclosingNode.Parent;
            }

            return enclosingNode as ElseClauseSyntax;
        }
    }
}