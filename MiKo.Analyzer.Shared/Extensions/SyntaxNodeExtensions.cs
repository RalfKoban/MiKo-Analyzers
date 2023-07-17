using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using MiKoSolutions.Analyzers.Linguistics;

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

        private static readonly string[] Booleans = { "true", "false", "True", "False", "TRUE", "FALSE" };

        private static readonly string[] Nulls = { "null", "Null", "NULL" };

        private static readonly SyntaxKind[] MethodNameSyntaxKinds = { SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration };

        internal static IEnumerable<T> Ancestors<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors().OfType<T>(); // value.AncestorsAndSelf().OfType<T>();

        internal static bool Contains(this SyntaxNode value, char c) => value?.ToString().Contains(c) ?? false;

        internal static IEnumerable<T> ChildNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes<T>(_ => _.IsKind(kind));

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().Where(predicate);

        internal static bool EnclosingMethodHasParameter(this SyntaxNode value, string parameterName, SemanticModel semanticModel)
        {
            var method = value.GetEnclosingMethod(semanticModel);

            if (method is null)
            {
                return false;
            }

            var parameters = method.Parameters;

            return parameters.Length > 0 && parameters.Any(_ => _.Name == parameterName);
        }

        internal static T FirstAncestor<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault();

        internal static T FirstAncestor<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault(predicate);

        internal static T FirstAncestor<T>(this SyntaxNode value, params SyntaxKind[] kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        internal static SyntaxNode FirstChild(this SyntaxNode value) => value.ChildNodes().FirstOrDefault();

        internal static T FirstChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault();

        internal static SyntaxNode FirstChild(this SyntaxNode value, Func<SyntaxNode, bool> predicate) => value.ChildNodes().FirstOrDefault(predicate);

        internal static T FirstChild<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.FirstChild<T>(_ => _.IsKind(kind));

        internal static T FirstChild<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault(predicate);

        internal static SyntaxToken FirstChildToken(this SyntaxNode value) => value.ChildTokens().FirstOrDefault();

        internal static SyntaxToken FirstChildToken(this SyntaxNode value, SyntaxKind kind) => value.ChildTokens().First(_ => _.IsKind(kind));

        internal static T FirstDescendant<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault();

        internal static T FirstDescendant<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.FirstDescendant<T>(_ => _.IsKind(kind));

        internal static T FirstDescendant<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault(predicate);

        internal static T LastChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().LastOrDefault();

        internal static Location GetContentsLocation(this XmlElementSyntax value)
        {
            var contents = value.Content;
            var span = contents.Span;

            if (contents.Count > 0)
            {
                var start = FindStart(contents);
                var end = FindEnd(contents);

                span = TextSpan.FromBounds(start, end);
            }

            if (span.IsEmpty)
            {
                var start = value.StartTag.GreaterThanToken.SpanStart;
                var end = value.EndTag.LessThanSlashToken.SpanStart + 1;

                span = TextSpan.FromBounds(start, end);
            }

            return Location.Create(value.SyntaxTree, span);

            int FindStart(SyntaxList<XmlNodeSyntax> list)
            {
                XmlNodeSyntax first = null;

                // try to find the first syntax that is not only an XmlCommentExterior
                for (var i = 0; i < list.Count; i++)
                {
                    first = list[i];

                    if (first is XmlTextSyntax t && t.IsWhiteSpaceOnlyText())
                    {
                        continue;
                    }

                    break;
                }

                var start = first?.SpanStart ?? -1;

                // try to get rid of white-spaces at the beginning
                if (first is XmlTextSyntax firstText)
                {
                    var token = firstText.TextTokens.FirstOrDefault(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken) && _.Text.IsNullOrWhiteSpace() is false);
                    var text = token.Text;

                    var offset = text.Length - text.TrimStart().Length;

                    start = token.SpanStart + offset;
                }

                return start;
            }

            int FindEnd(SyntaxList<XmlNodeSyntax> list)
            {
                XmlNodeSyntax last = null;

                // try to find the last syntax that is not only an XmlCommentExterior
                for (var i = list.Count - 1; i > -1; i--)
                {
                    last = list[i];

                    if (last is XmlTextSyntax t && t.IsWhiteSpaceOnlyText())
                    {
                        continue;
                    }

                    break;
                }

                var end = last?.Span.End ?? -1;

                // try to get rid of white-spaces at the end
                if (last is XmlTextSyntax lastText)
                {
                    var token = lastText.TextTokens.LastOrDefault(_ => _.IsKind(SyntaxKind.XmlTextLiteralToken) && _.Text.IsNullOrWhiteSpace() is false);
                    var text = token.Text;

                    var offset = text.Length - text.TrimEnd().Length;

                    end = token.Span.End - offset;
                }

                return end;
            }
        }

        internal static XmlTextAttributeSyntax GetNameAttribute(this SyntaxNode syntax)
        {
            switch (syntax)
            {
                case XmlElementSyntax e: return e.GetAttributes<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Attribute.Name);
                case XmlEmptyElementSyntax ee: return ee.Attributes.OfType<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Attribute.Name);
                default: return null;
            }
        }

        internal static string GetParameterName(this XmlElementSyntax syntax) => syntax.GetAttributes<XmlNameAttributeSyntax>().FirstOrDefault()?.Identifier.GetName();

        internal static string GetParameterName(this XmlEmptyElementSyntax syntax) => syntax.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault()?.Identifier.GetName();

        internal static XmlElementSyntax GetParameterComment(this DocumentationCommentTriviaSyntax comment, string parameterName) => comment.FirstDescendant<XmlElementSyntax>(_ => _.GetName() == Constants.XmlTag.Param && _.GetParameterName() == parameterName);

        internal static IfStatementSyntax GetRelatedIfStatement(this SyntaxNode value)
        {
            var ifStatement = value.FirstAncestorOrSelf<IfStatementSyntax>();

            if (ifStatement is null)
            {
                // maybe part of a block outside the if statement
                var block = value.FirstAncestorOrSelf<BlockSyntax>();

                if (block != null)
                {
                    // try to find the corresponding if statement
                    ifStatement = block.FirstChild<IfStatementSyntax>();
                }
            }

            return ifStatement;
        }

        internal static ExpressionSyntax GetRelatedCondition(this SyntaxNode syntax)
        {
            var coalesceExpression = syntax.FirstAncestorOrSelf<BinaryExpressionSyntax>(_ => _.IsKind(SyntaxKind.CoalesceExpression));

            if (coalesceExpression != null)
            {
                return coalesceExpression;
            }

            // most probably it's a if/else, but it might be a switch statement as well
            var condition = syntax.GetRelatedIfStatement()?.Condition ?? syntax.GetEnclosing<SwitchStatementSyntax>()?.Expression;

            return condition;
        }

        internal static ParameterSyntax GetUsedParameter(this ObjectCreationExpressionSyntax syntax)
        {
            var parameters = CollectParameters(syntax);

            if (parameters.Any())
            {
                // there might be multiple parameters, so we have to find out which parameter is meant
                var condition = GetRelatedCondition(syntax);

                if (condition is null)
                {
                    // nothing found
                    return null;
                }

                var identifiers = condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().ToHashSet(_ => _.GetName());

                return parameters.FirstOrDefault(_ => identifiers.Contains(_.GetName()));
            }

            return null;
        }

        internal static HashSet<string> GetAllUsedVariables(this SyntaxNode statementOrExpression, SemanticModel semanticModel)
        {
            var dataFlow = semanticModel.AnalyzeDataFlow(statementOrExpression);

            var result = new HashSet<string>();

            // do not use the declared ones as we are interested in parameters, not unused variables
            foreach (var variable in dataFlow.ReadInside)
            {
                result.Add(variable.Name);
            }

            foreach (var variable in dataFlow.ReadOutside)
            {
                result.Add(variable.Name);
            }

            // do not include the ones that are written outside as those are the ones that are not used at all
            foreach (var variable in dataFlow.WrittenInside)
            {
                result.Add(variable.Name);
            }

            return result;
        }

        internal static IEnumerable<T> GetAttributes<T>(this XmlEmptyElementSyntax value) => value?.Attributes.OfType<T>() ?? Enumerable.Empty<T>();

        internal static IEnumerable<T> GetAttributes<T>(this XmlElementSyntax value) => value?.StartTag.Attributes.OfType<T>() ?? Enumerable.Empty<T>();

        internal static T GetEnclosing<T>(this SyntaxNode value) where T : SyntaxNode => value.FirstAncestorOrSelf<T>();

        internal static SyntaxNode GetEnclosing(this SyntaxNode value, params SyntaxKind[] syntaxKinds)
        {
            var node = value;

            while (true)
            {
                if (node is null)
                {
                    return null;
                }

                for (var index = 0; index < syntaxKinds.Length; index++)
                {
                    if (node.IsKind(syntaxKinds[index]))
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
                case MethodDeclarationSyntax s: return semanticModel.GetDeclaredSymbol(s);
                case PropertyDeclarationSyntax p: return semanticModel.GetDeclaredSymbol(p);
                case ConstructorDeclarationSyntax c: return semanticModel.GetDeclaredSymbol(c);
                case FieldDeclarationSyntax f: return semanticModel.GetDeclaredSymbol(f);
                case EventDeclarationSyntax e: return semanticModel.GetDeclaredSymbol(e);
                default:
                    return semanticModel.GetEnclosingSymbol(value.GetLocation().SourceSpan.Start);
            }
        }

        internal static string GetMethodName(this ParameterSyntax node)
        {
            var enclosingNode = node.GetEnclosing(MethodNameSyntaxKinds);

            switch (enclosingNode)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                default:
                    return null;
            }
        }

        internal static string GetName(this ArgumentSyntax argument) => argument.Expression.GetName();

        internal static string GetName(this AttributeSyntax attribute)
        {
            switch (attribute.Name)
            {
                case QualifiedNameSyntax q: return q.Right.GetName();
                case SimpleNameSyntax s: return s.GetName();
                default: return string.Empty;
            }
        }

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

        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ExpressionSyntax value)
        {
            switch (value)
            {
                case IdentifierNameSyntax i: return i.GetName();
                case InvocationExpressionSyntax i: return i.GetName();
                case LiteralExpressionSyntax l: return l.GetName();
                case MemberAccessExpressionSyntax m: return m.GetName();
                case SimpleNameSyntax s: return s.GetName();
                default: return string.Empty;
            }
        }

        internal static string GetName(this InvocationExpressionSyntax value)
        {
            switch (value?.Expression)
            {
                case IdentifierNameSyntax identifier:
                {
                    var text = identifier.GetName();

                    if (text == "nameof" && value.Ancestors().OfType<MemberAccessExpressionSyntax>().None())
                    {
                        // nameof
                        var arguments = value.ArgumentList.Arguments;

                        if (arguments.Count > 0)
                        {
                            return arguments[0].ToString();
                        }
                    }

                    return text;
                }

                case MemberAccessExpressionSyntax m:
                {
                    return m.Expression.GetName();
                }
            }

            return string.Empty;
        }

        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this LiteralExpressionSyntax value) => value?.Token.ValueText;

        internal static string GetName(this LocalFunctionStatementSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this UsingDirectiveSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.GetName();

        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlElementStartTagSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlNameSyntax value) => value?.LocalName.ValueText;

        internal static string GetXmlTagName(this SyntaxNode node)
        {
            switch (node)
            {
                case XmlEmptyElementSyntax ee: return ee.GetName();
                case XmlElementSyntax e: return e.GetName();
                case XmlElementStartTagSyntax est: return est.GetName();
                case XmlNameSyntax n: return n.GetName();
                default: return string.Empty;
            }
        }

        internal static string GetNameOnlyPart(this TypeSyntax value) => value?.ToString().GetNameOnlyPart();

        internal static string GetNameOnlyPartWithoutGeneric(this TypeSyntax value)
        {
            var type = GetNameLocal();

            return type.GetNameOnlyPart();

            ReadOnlySpan<char> GetNameLocal()
            {
                switch (value)
                {
                    case GenericNameSyntax generic: return generic.GetName().AsSpan();
                    case SimpleNameSyntax simple: return simple.GetName().AsSpan();
                    default:
                        return value is null
                               ? ReadOnlySpan<char>.Empty
                               : value.ToString().AsSpan();
                }
            }
        }

        internal static ParameterSyntax[] GetParameters(this XmlElementSyntax value)
        {
            foreach (var ancestor in value.Ancestors())
            {
                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax method:
                        return method.ParameterList.Parameters.ToArray();

                    case IndexerDeclarationSyntax indexer:
                        return indexer.ParameterList.Parameters.ToArray();
                }
            }

            return Array.Empty<ParameterSyntax>();
        }

        internal static string[] GetParameterNames(this XmlElementSyntax value)
        {
            foreach (var ancestor in value.Ancestors())
            {
                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax method:
                        return method.ParameterList.Parameters.Select(_ => _.GetName()).ToArray();

                    case IndexerDeclarationSyntax indexer:
                        return indexer.ParameterList.Parameters.Select(_ => _.GetName()).ToArray();

                    case BasePropertyDeclarationSyntax property:
                        return property?.AccessorList?.Accessors.Any(_ => _.IsKind(SyntaxKind.SetAccessorDeclaration)) is true
                               ? Constants.Names.DefaultPropertyParameterNames
                               : Array.Empty<string>();
                }
            }

            return Array.Empty<string>();
        }

        internal static int GetStartingLine(this SyntaxNode value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxNode value) => value.GetLocation().GetEndingLine();

        internal static ISymbol GetSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var symbolInfo = GetSymbolInfo();

            return symbolInfo.Symbol;

            SymbolInfo GetSymbolInfo()
            {
                switch (value)
                {
                    case ConstructorInitializerSyntax cis:
                        return semanticModel.GetSymbolInfo(cis);

                    default:
                        return semanticModel.GetSymbolInfo(value);
                }
            }
        }

        internal static ISymbol GetSymbol(this SyntaxNode value, Compilation compilation)
        {
            return value.GetSymbol(compilation.GetSemanticModel(value.SyntaxTree));
        }

        internal static IMethodSymbol GetSymbol(this LocalFunctionStatementSyntax value, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetDeclaredSymbol(value);

            return symbol as IMethodSymbol;
        }

        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, Compilation compilation)
        {
            return value.GetTypeSymbol(compilation.GetSemanticModel(value.SyntaxTree));
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
            var symbol = value.Identifier.GetSymbol(semanticModel);

            return symbol as ITypeSymbol;
        }

        internal static ITypeSymbol GetTypeSymbol(this RecordDeclarationSyntax value, SemanticModel semanticModel)
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

        internal static LinePosition GetStartPosition(this SyntaxNode value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxNode value) => value.GetLocation().GetEndPosition();

        internal static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this SyntaxNode syntaxNode)
        {
            if (syntaxNode is null)
            {
                return null;
            }

            switch (syntaxNode)
            {
                case BaseTypeDeclarationSyntax type:
                {
                    // inspect for attributes
                    var attributeListSyntax = type.AttributeLists.FirstOrDefault();

                    if (attributeListSyntax != null)
                    {
                        return FindDocumentationCommentTriviaSyntaxForNode(attributeListSyntax);
                    }

                    return FindDocumentationCommentTriviaSyntaxForNode(syntaxNode);
                }

                case BaseMethodDeclarationSyntax method:
                {
                    var attributeListSyntax = method.AttributeLists.FirstOrDefault();

                    if (attributeListSyntax != null)
                    {
                        return FindDocumentationCommentTriviaSyntaxForNode(attributeListSyntax);
                    }

                    var commentOnCode = FindDocumentationCommentTriviaSyntaxForNode(syntaxNode);

                    if (commentOnCode != null)
                    {
                        return commentOnCode;
                    }

                    if (method.FirstChild() is SyntaxNode child)
                    {
                        return FindDocumentationCommentTriviaSyntaxForNode(child);
                    }

                    return null;
                }

                case BasePropertyDeclarationSyntax property:
                {
                    var attributeListSyntax = property.AttributeLists.FirstOrDefault();

                    if (attributeListSyntax != null)
                    {
                        return FindDocumentationCommentTriviaSyntaxForNode(attributeListSyntax);
                    }

                    var commentOnCode = FindDocumentationCommentTriviaSyntaxForNode(syntaxNode);

                    if (commentOnCode != null)
                    {
                        return commentOnCode;
                    }

                    if (property.FirstChild() is SyntaxNode child)
                    {
                        return FindDocumentationCommentTriviaSyntaxForNode(child);
                    }

                    return null;
                }

                default:
                {
                    return FindDocumentationCommentTriviaSyntaxForNode(syntaxNode);
                }
            }

            DocumentationCommentTriviaSyntax FindDocumentationCommentTriviaSyntaxForNode(SyntaxNode node)
            {
                if (node.HasStructuredTrivia)
                {
                    var childToken = node.FirstChildToken();

                    if (childToken.HasStructuredTrivia)
                    {
                        // 'HasLeadingTrivia' creates the list as well and checks for a count greater than zero
                        // so we can save some time and memory by doing it by ourselves
                        var leadingTrivia = childToken.LeadingTrivia;

                        if (leadingTrivia.Count > 0)
                        {
                            foreach (var trivia in leadingTrivia)
                            {
                                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) && trivia.GetStructure() is DocumentationCommentTriviaSyntax syntax)
                                {
                                    return syntax;
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }

        internal static ReadOnlySpan<char> GetTextTrimmed(this XmlElementSyntax element)
        {
            if (element is null)
            {
                return ReadOnlySpan<char>.Empty;
            }

            return element.GetTextWithoutTrivia().Without(Constants.EnvironmentNewLine).WithoutParaTagsAsSpan().Trim();
        }

        internal static string GetTextWithoutTrivia(this XmlTextAttributeSyntax text)
        {
            if (text is null)
            {
                return null;
            }

            var builder = new StringBuilder();

            foreach (var token in text.TextTokens)
            {
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                builder.Append(token.WithoutTrivia().ValueText);
            }

            var result = builder.ToString();

            return result.Trim();
        }

        internal static ReadOnlySpan<char> GetTextWithoutTrivia(this XmlTextSyntax text)
        {
            if (text is null)
            {
                return null;
            }

            var builder = new StringBuilder();

            foreach (var valueText in text.GetTextWithoutTriviaLazy())
            {
                builder.Append(valueText);
            }

            var result = builder.ToString();

            return result.AsSpan().Trim();
        }

        internal static StringBuilder GetTextWithoutTrivia(this XmlElementSyntax element) => new StringBuilder(element.Content.ToString()).WithoutXmlCommentExterior();

        internal static string GetTextWithoutTrivia(this XmlEmptyElementSyntax element) => element.WithoutXmlCommentExterior();

        internal static IEnumerable<string> GetTextWithoutTriviaLazy(this XmlTextSyntax text)
        {
            if (text is null)
            {
                yield break;
            }

            foreach (var token in text.TextTokens)
            {
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                yield return token.WithoutTrivia().ValueText;
            }
        }

        internal static IEnumerable<XmlElementSyntax> GetExampleXmls(this DocumentationCommentTriviaSyntax comment) => comment.GetXmlSyntax(Constants.XmlTag.Example);

        internal static IEnumerable<XmlElementSyntax> GetExceptionXmls(this DocumentationCommentTriviaSyntax comment) => comment.GetXmlSyntax(Constants.XmlTag.Exception);

        internal static IEnumerable<XmlElementSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax comment) => comment.GetXmlSyntax(Constants.XmlTag.Summary);

        internal static IEnumerable<XmlNodeSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax comment, IEnumerable<string> tags)
        {
            var summaryXmls = comment.GetSummaryXmls();

            foreach (var summary in summaryXmls)
            {
                foreach (var node in summary.GetXmlSyntax(tags))
                {
                    yield return node;
                }

                foreach (var node in summary.GetEmptyXmlSyntax(tags))
                {
                    yield return node;
                }
            }
        }

        internal static IEnumerable<XmlElementSyntax> GetRemarksXmls(this DocumentationCommentTriviaSyntax comment) => comment.GetXmlSyntax(Constants.XmlTag.Remarks);

        internal static IEnumerable<XmlElementSyntax> GetReturnsXmls(this DocumentationCommentTriviaSyntax comment) => comment.GetXmlSyntax(Constants.XmlTag.Returns);

        internal static IEnumerable<XmlElementSyntax> GetValueXmls(this DocumentationCommentTriviaSyntax comment) => comment.GetXmlSyntax(Constants.XmlTag.Value);

        /// <summary>
        /// Only gets the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="syntax">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,IEnumerable{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,IEnumerable{string})"/>
        internal static IEnumerable<XmlElementSyntax> GetXmlSyntax(this SyntaxNode syntax, string tag)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntax.DescendantNodes(_ => true, true).OfType<XmlElementSyntax>()
                         .Where(_ => _.GetName() == tag);
        }

        /// <summary>
        /// Only gets the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="syntax">
        /// The documentation syntax.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,IEnumerable{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        internal static IEnumerable<XmlElementSyntax> GetXmlSyntax(this SyntaxNode syntax, IEnumerable<string> tags)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntax.DescendantNodes(_ => true, true).OfType<XmlElementSyntax>()
                         .Where(_ => tags.Contains(_.GetName()));
        }

        /// <summary>
        /// Only gets the XML elements that are empty (have NO content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="syntax">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the XML elements that are empty (have NO content) and the given tag out of the documentation syntax.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,IEnumerable{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,IEnumerable{string})"/>
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode syntax, string tag)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntax.DescendantNodes(_ => true, true).OfType<XmlEmptyElementSyntax>()
                         .Where(_ => _.GetName() == tag);
        }

        /// <summary>
        /// Only gets the XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </summary>
        /// <param name="syntaxNode">
        /// The starting point of the XML elements to consider.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </returns>
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,IEnumerable{string})"/>
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode syntaxNode, IEnumerable<string> tags)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return syntaxNode.DescendantNodes(_ => true, true).OfType<XmlEmptyElementSyntax>()
                             .Where(_ => tags.Contains(_.GetName()));
        }

        internal static XmlCrefAttributeSyntax GetCref(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax e: return GetCref(e.Attributes);
                case XmlElementSyntax e: return GetCref(e.StartTag.Attributes);
                default: return null;
            }
        }

        internal static XmlCrefAttributeSyntax GetCref(this SyntaxNode value, string name)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax e when e.GetName() == name: return GetCref(e.Attributes);
                case XmlElementSyntax e when e.GetName() == name: return GetCref(e.StartTag.Attributes);
                default: return null;
            }
        }

        internal static TypeSyntax GetCrefType(this XmlCrefAttributeSyntax value)
        {
            if (value != null)
            {
                switch (value.Cref)
                {
                    case NameMemberCrefSyntax n: return n.Name;
                    case QualifiedCrefSyntax q when q.Member is NameMemberCrefSyntax n: return n.Name;
                }
            }

            return null;
        }

        internal static bool HasLinqExtensionMethod(this SyntaxNode value, SemanticModel semanticModel) => value.LinqExtensionMethods(semanticModel).Any();

        internal static bool HasPrimaryConstructor(this RecordDeclarationSyntax value) => value.ParameterList != null;

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

        internal static bool IsAnyKind(this SyntaxNode value, params SyntaxKind[] kinds)
        {
            var valueKind = value.Kind();

            // ReSharper disable once LoopCanBeConvertedToQuery: For performance reasons we use indexing instead of an enumerator
            for (var index = 0; index < kinds.Length; index++)
            {
                if (kinds[index] == valueKind)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsBoolean(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.BoolKeyword);
            }

            switch (value.ToString())
            {
                case nameof(Boolean):
                case nameof(System) + "." + nameof(Boolean):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsByte(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.ByteKeyword);
            }

            switch (value.ToString())
            {
                case nameof(Byte):
                case nameof(System) + "." + nameof(Byte):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsWrongBooleanTag(this SyntaxNode value) => value.IsCBool() || value.IsBBool() || value.IsValueBool() || value.IsCodeBool();

        internal static bool IsWrongNullTag(this SyntaxNode value) => value.IsCNull() || value.IsBNull() || value.IsValueNull() || value.IsCodeNull();

        internal static bool IsBooleanTag(this SyntaxNode value) => value.IsSeeLangwordBool() || value.IsWrongBooleanTag();

        internal static bool IsBBool(this SyntaxNode value) => value.Is("b", Booleans);

        internal static bool IsBNull(this SyntaxNode value) => value.Is("b", Nulls);

        internal static bool IsCBool(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Booleans);

        internal static bool IsCNull(this SyntaxNode value) => value.Is(Constants.XmlTag.C, Nulls);

        internal static bool IsCodeBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Code, Booleans);

        internal static bool IsCodeNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Code, Nulls);

        internal static bool IsCode(this SyntaxNode value) => value is XmlElementSyntax xes && xes.IsCode();

        internal static bool IsCode(this XmlElementSyntax value) => value.GetName() == Constants.XmlTag.Code;

        internal static bool IsCommand(this TypeSyntax value, SemanticModel semanticModel)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var name = value.ToString();

            return name.Contains("Command")
                && semanticModel.LookupSymbols(value.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol
                && symbol.IsCommand();
        }

        internal static bool IsException(this TypeSyntax value) => value.IsException<Exception>();

        internal static bool IsException<T>(this TypeSyntax value) where T : Exception => value.IsException(typeof(T));

        internal static bool IsException(this XmlElementSyntax value) => value.GetName() == Constants.XmlTag.Exception;

        internal static bool IsException(this TypeSyntax value, Type exceptionType)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var s = value.ToString();

            return s == exceptionType.Name || s == exceptionType.FullName;
        }

        internal static bool IsExceptionCommentFor<T>(this XmlElementSyntax value) where T : Exception => IsExceptionComment(value, typeof(T));

        internal static bool IsExceptionComment(this XmlElementSyntax value, Type exceptionType)
        {
            var type = value?.StartTag.Attributes.OfType<XmlCrefAttributeSyntax>().FirstOrDefault().GetCrefType();

            return type != null && type.IsException(exceptionType);
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

        internal static bool IsGenerated(this TypeDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.GeneratedAttributeNames.Contains);

        internal static bool IsInsideIfStatementWithCallTo(this SyntaxNode value, string methodName)
        {
            while (true)
            {
                var ifStatement = value.GetEnclosing<IfStatementSyntax>();

                if (ifStatement != null)
                {
                    if (ifStatement.IsCallTo(methodName))
                    {
                        var elseStatement = value.GetEnclosing<ElseClauseSyntax>();

                        if (elseStatement != null && ifStatement.Equals(elseStatement.Parent))
                        {
                            // we are in the else part, not the if part, so fail
                            return false;
                        }

                        return true;
                    }

                    // maybe a nested one, so check parent
                    value = ifStatement.Parent;
                }
                else
                {
                    // maybe an else block
                    var elseStatement = value.GetEnclosing<ElseClauseSyntax>();

                    if (elseStatement != null)
                    {
                        value = elseStatement.Parent;

                        continue;
                    }

                    return false;
                }
            }
        }

        internal static bool IsInvocationOnObjectUnderTest(this ExpressionStatementSyntax value)
        {
            switch (value.Expression)
            {
                case InvocationExpressionSyntax i when i.IsInvocationOnObjectUnderTest():
                case AwaitExpressionSyntax a when a.Expression is InvocationExpressionSyntax inv && inv.IsInvocationOnObjectUnderTest():
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsInvocationOnObjectUnderTest(this InvocationExpressionSyntax value) => value.Expression.IsAccessOnObjectUnderTest();

        internal static bool IsAccessOnObjectUnderTest(this ExpressionSyntax value)
        {
            if (value is MemberAccessExpressionSyntax mae)
            {
                switch (mae.Expression)
                {
                    case IdentifierNameSyntax ins when Constants.Names.ObjectUnderTestNames.Contains(ins.GetName()):
                    case InvocationExpressionSyntax i when i.IsInvocationOnObjectUnderTest():
                        return true;
                }
            }

            return false;
        }

        internal static bool IsAsync(this BasePropertyDeclarationSyntax value)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var modifier in value.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.AsyncKeyword))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsAsync(this MethodDeclarationSyntax value)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var modifier in value.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.AsyncKeyword))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsLocalVariableDeclaration(this SyntaxNode value, string identifierName) => value is LocalDeclarationStatementSyntax l && l.Declaration.Variables.Any(__ => __.Identifier.ValueText == identifierName);

        internal static bool IsWhiteSpaceOnlyText(this SyntaxNode value) => value is XmlTextSyntax text && text.IsWhiteSpaceOnlyText();

        internal static bool IsWhiteSpaceOnlyText(this XmlTextSyntax value)
        {
            foreach (var text in value.GetTextWithoutTriviaLazy())
            {
                if (text.IsNullOrWhiteSpace() is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsParameter(this IdentifierNameSyntax node, SemanticModel semanticModel) => node.EnclosingMethodHasParameter(node.GetName(), semanticModel);

        internal static bool IsPara(this SyntaxNode value) => value.IsXmlTag(Constants.XmlTag.Para);

        internal static bool IsXmlTag(this SyntaxNode value, string tagName)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax xees when xees.GetName() == tagName:
                case XmlElementSyntax xes when xes.GetName() == tagName:
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

        internal static bool IsSee(this XmlEmptyElementSyntax value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.See, attributeNames);

        internal static bool IsSee(this XmlElementSyntax value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.See, attributeNames);

        internal static bool IsSeeAlso(this XmlEmptyElementSyntax value, HashSet<string> attributeNames) => value.IsEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        internal static bool IsSeeAlso(this XmlElementSyntax value, HashSet<string> attributeNames) => value.IsNonEmpty(Constants.XmlTag.SeeAlso, attributeNames);

        internal static bool IsSerializationInfo(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var s = value.ToString();

            return s == nameof(SerializationInfo) || s == TypeNames.SerializationInfo;
        }

        internal static bool IsStreamingContext(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var s = value.ToString();

            return s == nameof(StreamingContext) || s == TypeNames.StreamingContext;
        }

        internal static bool IsString(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.StringKeyword);
            }

            switch (value.ToString())
            {
                case nameof(String):
                case nameof(System) + "." + nameof(String):
                    return true;
            }

            return false;
        }

        internal static bool IsString(this ArgumentSyntax value, SemanticModel semanticModel)
        {
            if (value.IsStringLiteral())
            {
                return true;
            }

            return value.GetTypeSymbol(semanticModel).IsString();
        }

        internal static bool IsString(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value.IsStringLiteral())
            {
                return true;
            }

            return value.GetTypeSymbol(semanticModel).IsString();
        }

        internal static bool IsStringLiteral(this ArgumentSyntax value) => value?.Expression.IsStringLiteral() is true;

        internal static bool IsStringLiteral(this ExpressionSyntax value)
        {
            switch (value?.Kind())
            {
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.InterpolatedStringExpression:
                    return true;

                default:
                    return false;
            }
        }

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

        internal static bool IsObject(this TypeSyntax value)
        {
            if (value is PredefinedTypeSyntax predefined)
            {
                return predefined.Keyword.IsKind(SyntaxKind.ObjectKeyword);
            }

            switch (value.ToString())
            {
                case nameof(Object):
                case nameof(System) + "." + nameof(Object):
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

        internal static bool IsInsideTestClass(this SyntaxNode value) => value.Ancestors<ClassDeclarationSyntax>().Any(_ => _.IsTestClass());

        internal static bool IsTestClass(this TypeDeclarationSyntax value) => value is ClassDeclarationSyntax declaration && IsTestClass(declaration);

        internal static bool IsTestClass(this ClassDeclarationSyntax value) => value.GetAttributeNames().Any(Constants.Names.TestClassAttributeNames.Contains);

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

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes<InvocationExpressionSyntax>(_ => IsLinqExtensionMethod(_, semanticModel));

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

        internal static SyntaxList<XmlNodeSyntax> ReplaceText(this SyntaxList<XmlNodeSyntax> source, string phrase, string replacement)
        {
            return source.ReplaceText(new[] { phrase }, replacement);
        }

        internal static SyntaxList<XmlNodeSyntax> ReplaceText(this SyntaxList<XmlNodeSyntax> source, string[] phrases, string replacement)
        {
            var result = source.ToList();

            for (var index = 0; index < result.Count; index++)
            {
                var value = result[index];

                if (value is XmlTextSyntax text)
                {
                    result[index] = text.ReplaceText(phrases, replacement);
                }
            }

            return SyntaxFactory.List(result);
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, string[] phrases, string replacement)
        {
            var map = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var token in value.TextTokens)
            {
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var replaced = false;

                var result = new StringBuilder(text);

                foreach (var phrase in phrases.Where(_ => text.Contains(_)))
                {
                    result.ReplaceWithCheck(phrase, replacement);

                    replaced = true;
                }

                if (replaced)
                {
                    map[token] = token.WithText(result);
                }
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static XmlTextSyntax ReplaceFirstText(this XmlTextSyntax value, string phrase, string replacement)
        {
            var map = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var token in value.TextTokens)
            {
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var index = text.IndexOf(phrase, StringComparison.Ordinal);

                if (index > -1)
                {
                    var result = string.Concat(text.Substring(0, index), replacement, text.Substring(index + phrase.Length));

                    map[token] = token.WithText(result);
                }
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static SyntaxNode PreviousSibling(this SyntaxNode node)
        {
            var parent = node?.Parent;

            if (parent is null)
            {
                return default;
            }

            SyntaxNode previousChild = default;

            foreach (var child in parent.ChildNodes())
            {
                if (child == node)
                {
                    return previousChild;
                }

                previousChild = child;
            }

            return default;
        }

        internal static SyntaxNode NextSibling(this SyntaxNode node)
        {
            var parent = node?.Parent;

            if (parent is null)
            {
                return default;
            }

            using (var enumerator = parent.ChildNodes().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == node)
                    {
                        var nextSibling = enumerator.MoveNext()
                                          ? enumerator.Current
                                          : default;

                        return nextSibling;
                    }
                }
            }

            return default;
        }

        internal static IList<SyntaxNode> Siblings(this SyntaxNode node) => Siblings<SyntaxNode>(node);

        internal static IList<T> Siblings<T>(this SyntaxNode node) where T : SyntaxNode
        {
            var parent = node?.Parent;

            if (parent != null)
            {
                return parent.ChildNodes<T>().ToList();
            }

            return Array.Empty<T>();
        }

        internal static bool Throws<T>(this SyntaxNode node) where T : Exception
        {
            switch (node)
            {
                case ThrowStatementSyntax ts when ts.Expression is ObjectCreationExpressionSyntax tso && tso.Type.IsException<T>():
                case ThrowExpressionSyntax te when te.Expression is ObjectCreationExpressionSyntax teo && teo.Type.IsException<T>():
                    return true;

                default:
                    return false;
            }
        }

        internal static string ToCleanedUpString(this ExpressionSyntax source) => source?.ToString().Without(Constants.WhiteSpaces);

        internal static T WithAnnotation<T>(this T value, SyntaxAnnotation annotation) where T : SyntaxNode => value.WithAdditionalAnnotations(annotation);

        internal static T WithAdditionalLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivias) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivias));
        }

        internal static T WithAttribute<T>(this T value, XmlAttributeSyntax attribute) where T : XmlNodeSyntax
        {
            switch (value)
            {
                case XmlElementSyntax xes:
                {
                    var newAttributes = xes.StartTag.Attributes.Add(attribute);
                    var newStartTag = xes.StartTag.WithAttributes(newAttributes);

                    return xes.ReplaceNode(xes.StartTag, newStartTag) as T;
                }

                case XmlEmptyElementSyntax xees:
                {
                    var newAttributes = xees.Attributes.Add(attribute);

                    return xees.WithAttributes(newAttributes) as T;
                }

                default:
                    return value;
            }
        }

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(SyntaxFactory.List(contents));

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(SyntaxFactory.List(contents));

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(SyntaxFactory.List(contents));

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(SyntaxFactory.List(contents));

        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithFirstLeadingTrivia<T>(this T value, SyntaxTrivia trivia) where T : SyntaxNode
        {
            // Attention: leading trivia contains XML comments, so we have to keep them!
            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count > 0)
            {
                // remove leading end-of-line as otherwise we would have multiple empty lines left over
                if (leadingTrivia[0].IsEndOfLine())
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

        internal static T WithAdditionalLeadingSpaces<T>(this T value, int additionalSpaces) where T : SyntaxNode
        {
            var currentSpaces = value.GetStartPosition().Character;

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static T WithLeadingSpaces<T>(this T value, int count) where T : SyntaxNode
        {
            var space = SyntaxFactory.Space; // use non-elastic one to prevent formatting to be done automatically

            var trivia = value.GetLeadingTrivia();

            if (trivia.Count == 0)
            {
                return value.WithLeadingTrivia(Enumerable.Repeat(space, count));
            }

            if (trivia[0].IsEndOfLine())
            {
                // keep empty line at beginning
                return value.WithLeadingTrivia(Enumerable.Repeat(space, count)).WithLeadingEmptyLine();
            }

            // re-construct leading comment with correct amount of spaces
            var finalTrivia = new List<SyntaxTrivia>();

            foreach (var t in trivia)
            {
                if (t.IsWhiteSpace())
                {
                    finalTrivia.AddRange(Enumerable.Repeat(space, count));
                }
                else
                {
                    finalTrivia.Add(t);
                }
            }

            return value.WithLeadingTrivia(finalTrivia);
        }

        internal static T WithLeadingXmlComment<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithLeadingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia().WithLeadingXmlComment());

        internal static T WithLeadingXmlCommentExterior<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentExterior);

        internal static T Without<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, IEnumerable<SyntaxNode> nodes) where T : SyntaxNode => value.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, params SyntaxNode[] nodes) where T : SyntaxNode => value.Without((IEnumerable<SyntaxNode>)nodes);

        internal static SyntaxList<XmlNodeSyntax> WithoutFirstXmlNewLine(this SyntaxList<XmlNodeSyntax> list)
        {
            if (list.FirstOrDefault() is XmlTextSyntax text)
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

        internal static T WithTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            return value
                    .WithLeadingTriviaFrom(node)
                    .WithTrailingTriviaFrom(node);
        }

        internal static T WithLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            return node.HasLeadingTrivia
                   ? value.WithLeadingTrivia(node.GetLeadingTrivia())
                   : value;
        }

        internal static T WithTrailingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            return node.HasTrailingTrivia
                   ? value.WithTrailingTrivia(node.GetTrailingTrivia())
                   : value;
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

                    newTokens = newTokens.Replace(token, token.WithText(token.Text.AsSpan().TrimStart()));
                }

                return XmlText(newTokens);
            }

            return value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, string text) => value.Content.WithoutText(text);

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this SyntaxList<XmlNodeSyntax> values, string text)
        {
            var contents = values.ToList();

            for (var index = 0; index < values.Count; index++)
            {
                if (values[index] is XmlTextSyntax s)
                {
                    var originalTextTokens = s.TextTokens;
                    var textTokens = originalTextTokens.ToList();

                    var modified = false;

                    for (var i = 0; i < originalTextTokens.Count; i++)
                    {
                        var token = originalTextTokens[i];

                        if (token.IsKind(SyntaxKind.XmlTextLiteralToken) && token.Text.Contains(text))
                        {
                            // do not trim the end as we want to have a space before <param> or other tags
                            var modifiedText = new StringBuilder(token.Text).Without(text).ReplaceWithCheck("  ", " ").ToString();

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
            var textTokens = value.TextTokens.ToList();

            var replaced = false;

            for (var i = textTokens.Count - 1; i >= 0; i--)
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

        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, params string[] texts) => texts.Aggregate(value, WithoutTrailing);

        internal static XmlTextSyntax WithoutTrailingCharacters(this XmlTextSyntax value, char[] characters)
        {
            var textTokens = value.TextTokens.ToList();

            var replaced = false;

            for (var i = textTokens.Count - 1; i >= 0; i--)
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
            var texts = value.Content.OfType<XmlTextSyntax>().ToList();

            if (texts.Count > 0)
            {
                var text = texts.Count == 1
                           ? texts[0]
                           : texts[texts.Count - 2];

                return WithoutWhitespaceOnlyComment(text);
            }

            return value;

            XmlElementSyntax WithoutWhitespaceOnlyComment(XmlTextSyntax text)
            {
                var newText = text.WithoutLeadingXmlComment();
                var newTextTokens = newText.TextTokens;

                switch (newTextTokens.Count)
                {
                    case 0:
                    case 1 when newTextTokens[0].ValueText.IsNullOrWhiteSpace():
                        return value.Without(text);

                    default:
                        return value.ReplaceNode(text, newText);
                }
            }
        }

        internal static StringBuilder WithoutXmlCommentExterior(this StringBuilder value) => value.Without("///");

        internal static string WithoutXmlCommentExterior(this SyntaxNode value) => new StringBuilder().WithoutXmlCommentExterior(value).ToString().Trim();

        internal static StringBuilder WithoutXmlCommentExterior(this StringBuilder value, SyntaxNode node) => value.Append(node).WithoutXmlCommentExterior();

        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this XmlElementSyntax value, params string[] startTexts) => value.Content.WithoutStartText(startTexts);

        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this SyntaxList<XmlNodeSyntax> values, params string[] startTexts)
        {
            if (values.Count > 0 && values[0] is XmlTextSyntax textSyntax)
            {
                return values.Replace(textSyntax, textSyntax.WithoutStartText(startTexts));
            }

            return values;
        }

        internal static XmlTextSyntax WithoutStartText(this XmlTextSyntax value, params string[] startTexts)
        {
            if (startTexts is null || startTexts.Length == 0)
            {
                return value;
            }

            var textTokens = value.TextTokens.ToList();

            for (var i = 0; i < textTokens.Count; i++)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                {
                    var originalText = token.Text.AsSpan().TrimStart();

                    if (originalText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    foreach (var startText in startTexts)
                    {
                        if (originalText.StartsWith(startText, StringComparison.Ordinal))
                        {
                            var modifiedText = originalText.Slice(startText.Length);

                            textTokens[i] = token.WithText(modifiedText);

                            return XmlText(textTokens);
                        }
                    }
                }
            }

            return value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this XmlElementSyntax value, string startText, FirstWordHandling firstWordHandling = FirstWordHandling.None) => value.Content.WithStartText(startText, firstWordHandling);

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this SyntaxList<XmlNodeSyntax> values, string startText, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            if (values.Count > 0)
            {
                return values[0] is XmlTextSyntax textSyntax
                       ? values.Replace(textSyntax, textSyntax.WithStartText(startText, firstWordHandling))
                       : values.Insert(0, XmlText(startText));
            }

            return new SyntaxList<XmlNodeSyntax>(XmlText(startText));
        }

        internal static XmlTextSyntax WithStartText(this XmlTextSyntax value, string startText, FirstWordHandling firstWordHandling = FirstWordHandling.None)
        {
            var textTokens = value.TextTokens.ToList();

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
                    var originalText = token.Text.AsSpan();

                    if (originalText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    var space = i == 0 ? string.Empty : " ";

                    var continuation = originalText.TrimStart().ToLowerCaseAt(0);

                    // replace 3rd person word by infinite word if configured
                    if (firstWordHandling == FirstWordHandling.MakeInfinite)
                    {
                        if (continuation[0] != '<')
                        {
                            var word = continuation.FirstWord();

                            continuation = Verbalizer.MakeInfiniteVerb(word) + continuation.Substring(word.Length);
                        }
                    }

                    var modifiedText = space + startText + continuation;

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

        // internal static T WithTrailingXmlComment<T>(this T value, int spaces) where T : SyntaxNode => value.WithTrailingTrivia(
        //                                                                                                                    SyntaxFactory.ElasticCarriageReturnLineFeed, // use elastic one to allow formatting to be done automatically
        //                                                                                                                    SyntaxFactory.ElasticWhitespace(new string(' ', spaces)),
        //                                                                                                                    XmlCommentExterior);
        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

        internal static SyntaxNode WithUsing(this SyntaxNode root, string usingNamespace)
        {
            var usings = root.DescendantNodes<UsingDirectiveSyntax>().ToList();

            if (usings.Any(_ => _.Name.ToFullString() == usingNamespace))
            {
                // already set
                return root;
            }

            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingNamespace));

            if (usings.Count == 0)
            {
                return root.InsertNodeBefore(root.FirstChild(), directive);
            }

            foreach (var usingDirective in usings)
            {
                var usingName = usingDirective.Name.ToFullString();

                if (usingName == "System")
                {
                    // skip 'System' namespace
                    continue;
                }

                if (usingName.StartsWith("System.", StringComparison.Ordinal))
                {
                    // skip all 'System' sub-namespaces
                    continue;
                }

                if (string.Compare(usingName, usingNamespace, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // add using at correct place inside the using block
                    return root.InsertNodeBefore(usingDirective, directive);
                }
            }

            return root.InsertNodeAfter(usings.Last(), directive);
        }

        internal static SyntaxNode WithoutUsing(this SyntaxNode node, string usingNamespace)
        {
            var root = node.SyntaxTree.GetRoot();

            return root.DescendantNodes<UsingDirectiveSyntax>(_ => _.Name.ToFullString() == usingNamespace)
                       .Select(root.Without)
                       .FirstOrDefault();
        }

        internal static IEnumerable<string> GetAttributeNames(this TypeDeclarationSyntax value)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var attributeList in value.AttributeLists)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var attribute in attributeList.Attributes)
                {
                    var name = attribute.GetName();

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    yield return name;
                }
            }
        }

        internal static IEnumerable<string> GetAttributeNames(this MethodDeclarationSyntax value)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var attributeList in value.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    yield return attribute.GetName();
                }
            }
        }

        private static IEnumerable<ParameterSyntax> CollectParameters(ObjectCreationExpressionSyntax syntax)
        {
            var method = syntax.GetEnclosing<BaseMethodDeclarationSyntax>();

            if (method != null)
            {
                return method.ParameterList.Parameters;
            }

            var indexer = syntax.GetEnclosing<IndexerDeclarationSyntax>();

            if (indexer != null)
            {
                var parameters = indexer.ParameterList.Parameters.ToList();

                // 'value' is a special parameter that is not part of the parameter list
                parameters.Insert(0, Parameter(indexer.Type));

                return parameters;
            }

            var property = syntax.GetEnclosing<PropertyDeclarationSyntax>();

            if (property != null)
            {
                // 'value' is a special parameter that is not part of the parameter list
                return new[] { Parameter(property.Type) };
            }

            return Enumerable.Empty<ParameterSyntax>();

            ParameterSyntax Parameter(TypeSyntax type) => SyntaxFactory.Parameter(default, default, type, SyntaxFactory.Identifier(Constants.Names.DefaultPropertyParameterName), default);
        }

        private static XmlCrefAttributeSyntax GetCref(SyntaxList<XmlAttributeSyntax> syntax) => syntax.OfType<XmlCrefAttributeSyntax>().FirstOrDefault();

        private static bool Is(this SyntaxNode value, string tagName, params string[] contents)
        {
            if (value is XmlElementSyntax syntax && string.Equals(tagName, syntax.GetName(), StringComparison.OrdinalIgnoreCase))
            {
                var content = syntax.Content.ToString().AsSpan().Trim();

                return content.EqualsAny(contents);
            }

            return false;
        }

        private static bool IsBinaryCallTo(this BinaryExpressionSyntax expression, string methodName)
        {
            if (expression is null)
            {
                return false;
            }

            if (expression.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
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
            var ifExpression = ifStatement.FirstChild<MemberAccessExpressionSyntax>();

            if (ifExpression.IsCallTo(methodName))
            {
                return true;
            }

            var binaryExpression = ifStatement.FirstChild<BinaryExpressionSyntax>();

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

        private static bool IsLinqExtensionMethod(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.Expression is MemberAccessExpressionSyntax maes && maes.Expression is PredefinedTypeSyntax)
            {
                return false;
            }

            var name = node.Expression.GetName();

            if (Constants.Names.LinqMethodNames.Contains(name))
            {
                var info = semanticModel.GetSymbolInfo(node);

                return info.Symbol.IsLinqExtensionMethod() || info.CandidateSymbols.Any(_ => _.IsLinqExtensionMethod());
            }

            return false;
        }

        private static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        private static XmlTextSyntax XmlText(SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

        private static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(SyntaxFactory.TokenList(textTokens));
    }
}