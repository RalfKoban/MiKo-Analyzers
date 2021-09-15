using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private static readonly string[] Booleans = { "true", "false" };

        private static readonly string[] Nulls = { "null" };

        internal static bool EnclosingMethodHasParameter(this SyntaxNode node, string parameterName, SemanticModel semanticModel)
        {
            var method = node.GetEnclosingMethod(semanticModel);
            if (method is null)
            {
                return false;
            }

            return method.Parameters.Any(_ => _.Name == parameterName);
        }

        internal static HashSet<string> GetAllUsedVariables(this SyntaxNode statementOrExpression, SemanticModel semanticModel)
        {
            var dataFlow = semanticModel.AnalyzeDataFlow(statementOrExpression);

            // do not use the declared ones as we are interested in parameters, not unused variables
            // var variablesDeclared = dataFlow.VariablesDeclared;
            var variablesRead = dataFlow.ReadInside.Union(dataFlow.ReadOutside);

            // do not include the ones that are written outside as those are the ones that are not used at all
            var variablesWritten = dataFlow.WrittenInside;

            var used = variablesRead.Union(variablesWritten).Select(_ => _.Name).ToHashSet();

            return used;
        }

        internal static IEnumerable<T> GetAttributes<T>(this XmlElementSyntax value) => value?.StartTag.Attributes.OfType<T>() ?? Enumerable.Empty<T>();

        internal static T GetEnclosing<T>(this SyntaxNode value) where T : SyntaxNode
        {
            var node = value;

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

        internal static T GetEnclosing<T>(this SyntaxToken value) where T : SyntaxNode => value.Parent.GetEnclosing<T>();

        internal static SyntaxNode GetEnclosing(this SyntaxNode value, params SyntaxKind[] syntaxKinds)
        {
            var node = value;

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

        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNodeAnalysisContext value)
        {
            if (value.ContainingSymbol is IMethodSymbol m)
            {
                return m;
            }

            return GetEnclosingMethod(value.Node, value.SemanticModel);
        }

        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNode value, SemanticModel semanticModel) => value.GetEnclosingSymbol(semanticModel) as IMethodSymbol;

        internal static ISymbol GetEnclosingSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            switch (value)
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
                    return semanticModel.GetEnclosingSymbol(value.GetLocation().SourceSpan.Start);
            }
        }

        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.Name.LocalName.ValueText;

        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetNameOnlyPart(this TypeSyntax value) => value.ToString().GetNameOnlyPart();

        internal static ISymbol GetSymbol(this SyntaxToken value, SemanticModel semanticModel)
        {
            var position = value.GetLocation().SourceSpan.Start;
            var name = value.ValueText;
            var syntaxNode = value.Parent;

            if (syntaxNode is ParameterSyntax node)
            {
                // we might have a ctor here and no method
                var methodName = GetMethodName(node);
                var methodSymbols = semanticModel.LookupSymbols(position, name: methodName).OfType<IMethodSymbol>();
                var parameterSymbol = methodSymbols.SelectMany(_ => _.Parameters).FirstOrDefault(_ => _.Name == name);

                return parameterSymbol;

                // if it's no method parameter, then it is a local one (but Roslyn cannot handle that currently in v3.3)
                // var symbol = semanticModel.LookupSymbols(position).First(_ => _.Kind == SymbolKind.Local);
            }

            // try to find the node as that may be faster than to look them up
            var symbol = semanticModel.GetDeclaredSymbol(syntaxNode);
            if (symbol is null)
            {
                var symbols = semanticModel.LookupSymbols(position, name: name);
                if (symbols.Length > 0)
                {
                    return symbols[0];
                }
            }

            return symbol;
        }

        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, SemanticModel semanticModel)
        {
            var type = value.Expression.GetTypeSymbol(semanticModel);

            return type;
        }

        internal static ITypeSymbol GetTypeSymbol(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static ITypeSymbol GetTypeSymbol(this MemberAccessExpressionSyntax value, SemanticModel semanticModel)
        {
            var type = value.Expression.GetTypeSymbol(semanticModel);

            return type;
        }

        internal static ITypeSymbol GetTypeSymbol(this TypeSyntax value, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static ITypeSymbol GetTypeSymbol(this BaseTypeSyntax value, SemanticModel semanticModel)
        {
            var type = value.Type.GetTypeSymbol(semanticModel);

            return type;
        }

        internal static ITypeSymbol GetTypeSymbol(this ClassDeclarationSyntax value, SemanticModel semanticModel)
        {
            var symbol = GetSymbol(value.Identifier, semanticModel);

            return symbol as ITypeSymbol;
        }

        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax value, SemanticModel semanticModel) => value.Type.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static bool HasLinqExtensionMethod(this SyntaxNode value, SemanticModel semanticModel) => value.LinqExtensionMethods(semanticModel).Any();

        internal static TRoot InsertNodeAfter<TRoot>(this TRoot value, SyntaxNode nodeInList, SyntaxNode newNode) where TRoot : SyntaxNode
        {
            return value.InsertNodesAfter(nodeInList, new[] { newNode });
        }

        internal static TRoot InsertNodeBefore<TRoot>(this TRoot value, SyntaxNode nodeInList, SyntaxNode newNode) where TRoot : SyntaxNode
        {
            // method needs to be indented and a CRLF needs to be added
            var modifiedNode = newNode.WithIndentation().WithEndOfLine();

            return value.InsertNodesBefore(nodeInList, new[] { modifiedNode });
        }

        internal static bool IsBoolean(this TypeSyntax value)
        {
            switch (value.ToString())
            {
                case "bool":
                case nameof(Boolean):
                case nameof(System) + "." + nameof(Boolean):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsByte(this TypeSyntax value)
        {
            switch (value.ToString())
            {
                case "byte":
                case nameof(Byte):
                case nameof(System) + "." + nameof(Byte):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsCBool(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Booleans);

        internal static bool IsCNull(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Nulls);

        internal static bool IsCommand(this TypeSyntax value, SemanticModel semanticModel)
        {
            var name = value.ToString();

            return name.Contains("Command")
                && semanticModel.LookupSymbols(value.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol
                && symbol.IsCommand();
        }

        internal static bool IsException(this TypeSyntax value)
        {
            switch (value.ToString())
            {
                case nameof(Exception):
                case nameof(System) + "." + nameof(Exception):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsExpression(this SyntaxNode value, SemanticModel semanticModel)
        {
            foreach (var a in value.Ancestors().OfType<ArgumentSyntax>())
            {
                var convertedType = semanticModel.GetTypeInfo(a.Expression).ConvertedType;
                var isExpression = convertedType?.InheritsFrom<Expression>() is true;
                if (isExpression)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsInsideIfStatementWithCallTo(this SyntaxNode value, string methodName)
        {
            while (true)
            {
                var ifStatement = GetEnclosingIfStatement(value);
                if (ifStatement != null)
                {
                    if (IsIfStatementWithCallTo(ifStatement, methodName))
                    {
                        return true;
                    }

                    // maybe a nested one, so check parent
                    value = ifStatement.Parent;
                    continue;
                }

                // maybe an else block
                var elseStatement = GetEnclosingElseStatement(value);
                if (elseStatement != null)
                {
                    value = elseStatement.Parent;
                    continue;
                }

                return false;
            }
        }

        internal static bool IsLocalVariableDeclaration(this SyntaxNode value, string identifierName)
        {
            return value is LocalDeclarationStatementSyntax l && l.Declaration.Variables.Any(__ => __.Identifier.ValueText == identifierName);
        }

        internal static bool IsParameter(this IdentifierNameSyntax node, SemanticModel semanticModel) => node.EnclosingMethodHasParameter(node.GetName(), semanticModel);

        internal static bool IsSeeLangword(this SyntaxNode value)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();
                switch (attribute?.GetName())
                {
                    case Constants.XmlTag.Attribute.Langword:
                    case Constants.XmlTag.Attribute.Langref:
                        return true;
                }
            }

            return false;
        }

        internal static bool IsSeeLangwordBool(this SyntaxNode value)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();
                switch (attribute?.GetName())
                {
                    case Constants.XmlTag.Attribute.Langword:
                    case Constants.XmlTag.Attribute.Langref:
                        {
                            foreach (var token in attribute.DescendantTokens())
                            {
                                if ("true".Equals(token.ValueText, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }

                                if ("false".Equals(token.ValueText, StringComparison.OrdinalIgnoreCase))
                                {
                                    return true;
                                }
                            }

                            break;
                        }
                }
            }

            return false;
        }

        internal static bool IsEmptySee(this SyntaxNode value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.See, attributeNames);

        internal static bool IsEmptySeeAlso(this SyntaxNode value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        internal static bool IsNonEmptySee(this SyntaxNode value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.See, attributeNames);

        internal static bool IsNonEmptySeeAlso(this SyntaxNode value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        internal static bool IsSerializationInfo(this TypeSyntax value)
        {
            var s = value.ToString();

            return s == nameof(SerializationInfo) || s == TypeNames.SerializationInfo;
        }

        internal static bool IsStreamingContext(this TypeSyntax value)
        {
            var s = value.ToString();

            return s == nameof(StreamingContext) || s == TypeNames.StreamingContext;
        }

        internal static bool IsString(this TypeSyntax value)
        {
            switch (value.ToString())
            {
                case "string":
                case nameof(String):
                case nameof(System) + "." + nameof(String):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsString(this ArgumentSyntax value, SemanticModel semanticModel) => value.GetTypeSymbol(semanticModel).IsString();

        internal static bool IsString(this ExpressionSyntax value, SemanticModel semanticModel) => value.GetTypeSymbol(semanticModel).IsString();

        internal static bool IsStruct(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            var type = value.GetTypeSymbol(semanticModel);

            switch (type?.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Enum:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsSupported(this SyntaxNodeAnalysisContext value, LanguageVersion expectedVersion)
        {
            var languageVersion = ((CSharpParseOptions)value.Node.SyntaxTree.Options).LanguageVersion;

            // ignore the latest versions (or above)
            return languageVersion >= expectedVersion && expectedVersion < LanguageVersion.LatestMajor;
        }

        internal static bool IsTestMethod(this MethodDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.TestMethodAttributeNames.Contains);

        internal static bool IsTestOneTimeSetUpMethod(this MethodDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.TestOneTimeSetupAttributeNames.Contains);

        internal static bool IsTestOneTimeTearDownMethod(this MethodDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.TestOneTimeTearDownAttributeNames.Contains);

        internal static bool IsTestSetUpMethod(this MethodDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.TestSetupAttributeNames.Contains);

        internal static bool IsTestTearDownMethod(this MethodDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.TestTearDownAttributeNames.Contains);

        internal static bool IsTypeUnderTestCreationMethod(this MethodDeclarationSyntax value) => Constants.Names.TypeUnderTestMethodNames.Contains(value.GetName());

        internal static bool IsTypeUnderTestVariable(this VariableDeclaratorSyntax value) => Constants.Names.TypeUnderTestVariableNames.Contains(value.GetName());

        internal static bool IsValueBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Booleans);

        internal static bool IsValueNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Nulls);

        internal static bool IsVoid(this TypeSyntax value) => value is PredefinedTypeSyntax p && p.Keyword.IsKind(SyntaxKind.VoidKeyword);

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes().OfType<InvocationExpressionSyntax>()
                                                                                                                                                 .Where(_ => IsLinqExtensionMethod(semanticModel.GetSymbolInfo(_)));

        internal static BaseTypeDeclarationSyntax RemoveNodeAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, MethodDeclarationSyntax method)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            // avoid lost trivia, such as #endregion
            var closeBraceToken = value.CloseBraceToken.WithoutTrivia().WithLeadingEndOfLine()
                                                        .WithLeadingTrivia(value.CloseBraceToken.LeadingTrivia)
                                                        .WithTrailingTrivia(value.CloseBraceToken.TrailingTrivia);

            return value.RemoveNode(method, SyntaxRemoveOptions.KeepNoTrivia)
                         .WithOpenBraceToken(openBraceToken)
                         .WithCloseBraceToken(closeBraceToken);
        }

        internal static string ToCleanedUpString(this ExpressionSyntax source) => source?.ToString().Without(Constants.WhiteSpaces);

        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithEndOfLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithIndentation<T>(this T value) where T : SyntaxNode => value.WithoutLeadingTrivia().WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(value.GetLeadingTrivia().Insert(0, SyntaxFactory.CarriageReturnLineFeed));

        internal static SyntaxToken WithLeadingEmptyLine(this SyntaxToken value) => value.WithLeadingTrivia(value.LeadingTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed));

        internal static T WithLeadingEndOfLine<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingEndOfLine(this SyntaxToken value) => value.WithLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static SyntaxToken WithLeadingXmlComment(this SyntaxToken value) => value.WithLeadingTrivia(XmlCommentStart);

        internal static T WithLeadingXmlComment<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithLeadingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia().WithLeadingXmlComment());

        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingTrivia(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia());

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, string text) => value.Content.WithoutText(text);

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this SyntaxList<XmlNodeSyntax> values, string text)
        {
            var contents = new List<XmlNodeSyntax>(values);

            for (var index = 0; index < values.Count; index++)
            {
                if (values[index] is XmlTextSyntax s)
                {
                    var originalTextTokens = s.TextTokens;
                    var textTokens = new List<SyntaxToken>(originalTextTokens);

                    var modified = false;

                    for (var i = 0; i < originalTextTokens.Count; i++)
                    {
                        var token = originalTextTokens[i];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.Text.Contains(text))
                        {
                            // do not trim the end as we want to have a space before <param> or other tags
                            var modifiedText = token.Text.Without(text).Replace("  ", " ");
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

                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        var xmlText = SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
                        contents[index] = xmlText;
                    }
                }
            }

            return SyntaxFactory.List(contents);
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, params string[] texts) => value.Content.WithoutText(texts);

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this SyntaxList<XmlNodeSyntax> values, params string[] texts) => texts.Aggregate(values, (current, text) => current.WithoutText(text));

        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, string text)
        {
            var textTokens = new List<SyntaxToken>(value.TextTokens);

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

                    if (originalText.EndsWith(text, StringComparison.OrdinalIgnoreCase))
                    {
                        var modifiedText = originalText.WithoutSuffix(text);

                        textTokens[i] = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), modifiedText, modifiedText, token.TrailingTrivia);
                        replaced = true;
                        break;
                    }
                }
            }

            if (replaced)
            {
                return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
            }

            return value;
        }

        internal static XmlTextSyntax WithoutTrailingCharacters(this XmlTextSyntax value, char[] characters)
        {
            var textTokens = new List<SyntaxToken>(value.TextTokens);

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

                    var modifiedText = originalText.TrimEnd(characters);

                    textTokens[i] = SyntaxFactory.Token(token.LeadingTrivia, token.Kind(), modifiedText, modifiedText, token.TrailingTrivia);
                    replaced = true;
                    break;
                }
            }

            if (replaced)
            {
                return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
            }

            return value;
        }

        internal static string WithoutXmlCommentExterior(this SyntaxNode value) => value.ToString().Replace("///", string.Empty).Trim();

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this XmlElementSyntax value, string startText) => value.Content.WithStartText(startText);

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this SyntaxList<XmlNodeSyntax> values, string startText)
        {
            if (values.Count > 0)
            {
                return values[0] is XmlTextSyntax textSyntax
                           ? values.Replace(textSyntax, textSyntax.WithStartText(startText))
                           : values.Insert(0, SyntaxFactory.XmlText(startText));
            }

            return new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText(startText));
        }

        internal static XmlTextSyntax WithStartText(this XmlTextSyntax value, string startText)
        {
            var textTokens = new List<SyntaxToken>(value.TextTokens);

            var replaced = false;

            if (startText.IsNullOrWhiteSpace())
            {
                // get rid of first new line token as we do not need it anymore
                if (textTokens.Count > 0 && textTokens[0].IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    textTokens.RemoveAt(0);

                    replaced = true;
                }
            }

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

            if (replaced)
            {
                return SyntaxFactory.XmlText(SyntaxFactory.TokenList(textTokens));
            }

            return SyntaxFactory.XmlText(startText);
        }

        internal static T WithTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingEmptyLine(this SyntaxToken value) => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static SyntaxToken WithTrailingXmlComment(this SyntaxToken value) => value.WithTrailingTrivia(XmlCommentStart);

        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

        private static IEnumerable<string> GetAttributeNames(this MethodDeclarationSyntax value) => value.AttributeLists.SelectMany(_ => _.Attributes).Select(_ => _.Name.GetNameOnlyPart());

        private static ElseClauseSyntax GetEnclosingElseStatement(SyntaxNode node)
        {
            var enclosingNode = node.GetEnclosing(SyntaxKind.Block, SyntaxKind.ElseClause);
            if (enclosingNode is BlockSyntax)
            {
                enclosingNode = enclosingNode.Parent;
            }

            return enclosingNode as ElseClauseSyntax;
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

        private static string GetMethodName(ParameterSyntax node)
        {
            var enclosingNode = node.GetEnclosing(SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration);
            switch (enclosingNode)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                default:
                    return null;
            }
        }

        private static bool Is(this SyntaxNode value, string tagName, params string[] contents)
        {
            if (value is XmlElementSyntax syntax && syntax.GetName() == tagName)
            {
                var content = syntax.Content.ToString().Trim();

                return contents.Any(expected => expected.Equals(content, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        private static bool IsEmpty(this SyntaxNode value, string tagName, HashSet<string> attributeNames)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

        private static bool IsNonEmpty(this SyntaxNode value, string tagName, HashSet<string> attributeNames)
        {
            if (value is XmlElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.StartTag.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

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

        private static bool IsLinqExtensionMethod(SymbolInfo info) => info.Symbol.IsLinqExtensionMethod() || info.CandidateSymbols.Any(_ => _.IsLinqExtensionMethod());
    }
}