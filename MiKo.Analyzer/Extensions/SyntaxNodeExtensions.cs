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
    internal static class SyntaxNodeExtensions
    {
        internal static readonly SyntaxTrivia XmlCommentExterior = SyntaxFactory.DocumentationCommentExterior("/// ");

        internal static readonly SyntaxTrivia[] XmlCommentStart =
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

        internal static T GetEnclosing<T>(this Location value, SemanticModel semanticModel) where T : class, ISymbol
        {
            var node = value.SourceTree?.GetRoot().FindNode(value.SourceSpan);

            return node.GetEnclosingSymbol(semanticModel) as T;
        }

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

        internal static string GetMethodName(this ParameterSyntax node)
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

        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.Name.LocalName.ValueText;

        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this BaseMethodDeclarationSyntax value)
        {
            switch (value)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this LiteralExpressionSyntax value) => value?.Token.ValueText;

        internal static string GetName(this ExpressionSyntax value)
        {
            switch (value)
            {
                case InvocationExpressionSyntax i:
                {
                    if (i.Expression is IdentifierNameSyntax identifier)
                    {
                        var text = identifier.GetName();

                        if (text != "nameof")
                        {
                            return text;
                        }

                        if (i.Ancestors().OfType<MemberAccessExpressionSyntax>().None())
                        {
                            // nameof
                            var arguments = i.ArgumentList.Arguments;
                            if (arguments.Count > 0)
                            {
                                return arguments[0].ToString();
                            }
                        }
                    }

                    return string.Empty;
                }

                case LiteralExpressionSyntax l: return l.GetName();
                case IdentifierNameSyntax i: return i.GetName();
                case MemberAccessExpressionSyntax m: return m.GetName();
                default: return string.Empty;
            }
        }

        internal static string GetName(this ArgumentSyntax nameArgument) => nameArgument.Expression.GetName();

        internal static string GetNameOnlyPart(this TypeSyntax value) => value.ToString().GetNameOnlyPart();

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
            var symbol = value.Identifier.GetSymbol(semanticModel);

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

        internal static bool IsCode(this SyntaxNode value) => value is XmlElementSyntax xes && xes.IsCode();

        internal static bool IsCode(this XmlElementSyntax value) => value.GetName() == Constants.XmlTag.Code;

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
                var ifStatement = value.GetEnclosingIfStatement();
                if (ifStatement != null)
                {
                    if (ifStatement.IsCallTo(methodName))
                    {
                        return true;
                    }

                    // maybe a nested one, so check parent
                    value = ifStatement.Parent;
                    continue;
                }

                // maybe an else block
                var elseStatement = value.GetEnclosingElseStatement();
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

        internal static bool IsPara(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax xees when xees.GetName() == Constants.XmlTag.Para:
                case XmlElementSyntax xes when xes.GetName() == Constants.XmlTag.Para:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsSeeLangword(this SyntaxNode value)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();
                switch (attribute?.GetName())
                {
                    case Constants.XmlTag.Attribute.Langword:
                    case Constants.XmlTag.Attribute.Langref:
                    {
                        return true;
                    }
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

        internal static BaseTypeDeclarationSyntax RemoveNodeAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, SyntaxNode node)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            // avoid lost trivia, such as #endregion
            var closeBraceToken = value.CloseBraceToken.WithoutTrivia().WithLeadingEndOfLine()
                                                        .WithLeadingTrivia(value.CloseBraceToken.LeadingTrivia)
                                                        .WithTrailingTrivia(value.CloseBraceToken.TrailingTrivia);

            return value.Without(node)
                        .WithOpenBraceToken(openBraceToken)
                        .WithCloseBraceToken(closeBraceToken);
        }

        internal static BaseTypeDeclarationSyntax RemoveNodesAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, IEnumerable<SyntaxNode> nodes)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            // avoid lost trivia, such as #endregion
            var closeBraceToken = value.CloseBraceToken.WithoutTrivia().WithLeadingEndOfLine()
                                                        .WithLeadingTrivia(value.CloseBraceToken.LeadingTrivia)
                                                        .WithTrailingTrivia(value.CloseBraceToken.TrailingTrivia);

            return value.Without(nodes)
                        .WithOpenBraceToken(openBraceToken)
                        .WithCloseBraceToken(closeBraceToken);
        }

        internal static T ReplaceNodes<T, TNode>(this T value, IEnumerable<TNode> nodes, Func<TNode, IEnumerable<SyntaxNode>> computeReplacementNodes)
            where T : SyntaxNode
            where TNode : SyntaxNode
        {
            // replace all nodes by following algorithm:
            // 1. Create a dictionary with SyntaxAnnotations and replacement nodes for the node to annotate (new SyntaxAnnotation)
            // 2. Annotate the node to keep track (node.WithAnnotation())
            // 3. Loop over all annotated nodes and replace them with the replacement nodes (document.GetAnnotatedNodes(annotation))
            var annotation = new SyntaxAnnotation();

            var result = value.ReplaceNodes(nodes, (original, rewritten) => original.WithAnnotation(annotation));

            while (true)
            {
                var oldNode = result.GetAnnotatedNodes(annotation).OfType<TNode>().FirstOrDefault();
                if (oldNode is null)
                {
                    // nothing left
                    break;
                }

                // create replacement nodes
                var replacements = computeReplacementNodes(oldNode);

                // and remove the annotations in case we get the same node back (to avoid endless while loop as we would always get the same node again)
                result = result.ReplaceNode(oldNode, replacements.Select(_ => _.WithoutAnnotations(annotation)));
            }

            return result;
        }

        internal static string ToCleanedUpString(this ExpressionSyntax source) => source?.ToString().Without(Constants.WhiteSpaces);

        internal static T WithAnnotation<T>(this T value, SyntaxAnnotation annotation) where T : SyntaxNode => value.WithAdditionalAnnotations(annotation);

        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithAdditionalLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivias) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivias));
        }

        internal static T WithFirstLeadingTrivia<T>(this T value, SyntaxTrivia trivia) where T : SyntaxNode
        {
            if (value.HasLeadingTrivia)
            {
                // Attention: leading trivia contains XML comments, so we have to keep them!
                var leadingTrivia = value.GetLeadingTrivia();

                // remove leading end-of-line as otherwise we would have multiple empty lines left over
                if (leadingTrivia[0].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    leadingTrivia = leadingTrivia.RemoveAt(0);
                }

                return value.WithLeadingTrivia(leadingTrivia.Insert(0, trivia));
            }

            return value.WithLeadingTrivia(trivia);
        }

        internal static T WithIndentation<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithLeadingEndOfLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithLeadingXmlComment<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithLeadingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia().WithLeadingXmlComment());

        internal static T WithLeadingXmlCommentExterior<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentExterior);

        internal static T Without<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, IEnumerable<SyntaxNode> nodes) where T : SyntaxNode => value.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, params SyntaxNode[] nodes) where T : SyntaxNode => value.Without((IEnumerable<SyntaxNode>)nodes);

        internal static SyntaxList<XmlNodeSyntax> WithoutFirstXmlNewLine(this SyntaxList<XmlNodeSyntax> list)
        {
            if (list.Count > 1 && list[0] is XmlTextSyntax text)
            {
                var newText = text.WithoutFirstXmlNewLine();

                return newText.TextTokens.Count != 0
                           ? list.Replace(text, newText)
                           : list.Remove(text);
            }

            return list;
        }

        internal static XmlElementSyntax WithoutFirstXmlNewLine(this XmlElementSyntax value)
        {
            return value.WithContent(value.Content.WithoutFirstXmlNewLine());
        }

        internal static XmlTextSyntax WithoutFirstXmlNewLine(this XmlTextSyntax value)
        {
            return value.WithTextTokens(value.TextTokens.WithoutFirstXmlNewLine()).WithoutLeadingTrivia();
        }

        internal static XmlTextSyntax WithoutLastXmlNewLine(this XmlTextSyntax syntax)
        {
            var textTokens = syntax.TextTokens.WithoutLastXmlNewLine();

            return syntax.WithTextTokens(textTokens);
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingTrivia(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia());

        internal static XmlTextSyntax WithoutLeadingXmlComment(this XmlTextSyntax value)
        {
            var tokens = value.TextTokens;
            if (tokens.Count >= 2)
            {
                var newTokens = tokens.WithoutFirstXmlNewLine();

                if (newTokens.Count > 0)
                {
                    var token = newTokens[0];

                    newTokens = newTokens.Replace(token, token.WithText(token.Text.TrimStart()));
                }

                return XmlText(newTokens);
            }

            return value;
        }

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
                                textTokens[i] = token.WithText(modifiedText);
                            }

                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        contents[index] = XmlText(textTokens);
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

                        textTokens[i] = token.WithText(modifiedText);
                        replaced = true;
                        break;
                    }
                }
            }

            if (replaced)
            {
                return XmlText(textTokens);
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

                    textTokens[i] = token.WithText(modifiedText);
                    replaced = true;
                    break;
                }
            }

            if (replaced)
            {
                return XmlText(textTokens);
            }

            return value;
        }

        internal static XmlTextSyntax WithoutTrailingXmlComment(this XmlTextSyntax value)
        {
            if (value.TextTokens.Count > 2)
            {
                // remove last "\r\n" token and remove '  /// ' trivia of last token
                return value.WithoutLastXmlNewLine();
            }

            return value;
        }

        internal static XmlElementSyntax WithoutWhitespaceOnlyComment(this XmlElementSyntax value)
        {
            if (value.Content.Count > 0 && value.Content[0] is XmlTextSyntax t)
            {
                var newT = t.WithoutLeadingXmlComment();

                return newT.TextTokens.Count == 1 && newT.TextTokens[0].ValueText.IsNullOrWhiteSpace()
                           ? value.Without(t)
                           : value.ReplaceNode(t, newT);
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
                           : values.Insert(0, XmlText(startText));
            }

            return new SyntaxList<XmlNodeSyntax>(XmlText(startText));
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

                    textTokens[i] = token.WithText(modifiedText);

                    replaced = true;

                    break;
                }
            }

            if (replaced)
            {
                return XmlText(textTokens);
            }

            return XmlText(startText);
        }

        internal static T WithTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingNewLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

        private static IEnumerable<string> GetAttributeNames(this MethodDeclarationSyntax value) => value.AttributeLists.SelectMany(_ => _.Attributes).Select(_ => _.Name.GetNameOnlyPart());

        private static ElseClauseSyntax GetEnclosingElseStatement(this SyntaxNode node)
        {
            var enclosingNode = node.GetEnclosing(SyntaxKind.Block, SyntaxKind.ElseClause);
            if (enclosingNode is BlockSyntax)
            {
                enclosingNode = enclosingNode.Parent;
            }

            return enclosingNode as ElseClauseSyntax;
        }

        private static IfStatementSyntax GetEnclosingIfStatement(this SyntaxNode node)
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

        private static bool Is(this SyntaxNode value, string tagName, params string[] contents)
        {
            if (value is XmlElementSyntax syntax && syntax.GetName() == tagName)
            {
                var content = syntax.Content.ToString().Trim();

                return contents.Any(expected => expected.Equals(content, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        private static bool IsBinaryCallTo(this BinaryExpressionSyntax expression, string methodName)
        {
            if (expression?.OperatorToken.Kind() == SyntaxKind.AmpersandAmpersandToken)
            {
                if (expression.Left.IsCallTo(methodName) || expression.Right.IsCallTo(methodName))
                {
                    return true;
                }

                // maybe it is a combined one
                if (expression.Left is BinaryExpressionSyntax left && IsBinaryCallTo(left, methodName))
                {
                    return true;
                }

                // maybe it is a combined one
                if (expression.Right is BinaryExpressionSyntax right && IsBinaryCallTo(right, methodName))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsCallTo(this IfStatementSyntax ifStatement, string methodName)
        {
            var ifExpression = ifStatement.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

            if (ifExpression.IsCallTo(methodName))
            {
                return true;
            }

            var binaryExpression = ifStatement.ChildNodes().OfType<BinaryExpressionSyntax>().FirstOrDefault();
            if (binaryExpression.IsBinaryCallTo(methodName))
            {
                return true;
            }

            return false;
        }

        private static bool IsCallTo(this ExpressionSyntax expression, string methodName) => expression is MemberAccessExpressionSyntax m && m.Name.ToString() == methodName;

        private static bool IsEmpty(this SyntaxNode value, string tagName, IEnumerable<string> attributeNames)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

        private static bool IsNonEmpty(this SyntaxNode value, string tagName, IEnumerable<string> attributeNames)
        {
            if (value is XmlElementSyntax syntax && syntax.GetName() == tagName)
            {
                var attribute = syntax.StartTag.Attributes.FirstOrDefault();

                return attributeNames.Contains(attribute?.GetName());
            }

            return false;
        }

        private static bool IsLinqExtensionMethod(SymbolInfo info) => info.Symbol.IsLinqExtensionMethod() || info.CandidateSymbols.Any(_ => _.IsLinqExtensionMethod());

        private static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        private static XmlTextSyntax XmlText(SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

        private static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(SyntaxFactory.TokenList(textTokens));
    }
}