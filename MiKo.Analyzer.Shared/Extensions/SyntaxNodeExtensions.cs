﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

            return method.Parameters.Any(_ => _.Name == parameterName);
        }

        internal static T FirstAncestor<T>(this SyntaxNode value, params SyntaxKind[] kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        internal static T FirstAncestor<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.Ancestors().OfType<T>().FirstOrDefault(predicate);

        internal static T FirstChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault();

        internal static T FirstChild<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.FirstChild<T>(_ => _.IsKind(kind));

        internal static T FirstChild<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault(predicate);

        internal static T FirstDescendant<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.FirstDescendant<T>(_ => _.IsKind(kind));

        internal static T FirstDescendant<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault(predicate);

        internal static T LastChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().LastOrDefault();

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

            // do not use the declared ones as we are interested in parameters, not unused variables
            // var variablesDeclared = dataFlow.VariablesDeclared;
            var variablesRead = dataFlow.ReadInside.Union(dataFlow.ReadOutside);

            // do not include the ones that are written outside as those are the ones that are not used at all
            var variablesWritten = dataFlow.WrittenInside;

            var used = variablesRead.Union(variablesWritten).ToHashSet(_ => _.Name);

            return used;
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
            var enclosingNode = node.GetEnclosing(SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration);
            switch (enclosingNode)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                default:
                    return null;
            }
        }

        internal static string GetName(this ArgumentSyntax argument) => argument.Expression.GetName();

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

        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.Name.LocalName.ValueText;

        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetName(this XmlElementStartTagSyntax value) => value?.Name.LocalName.ValueText;

        internal static string GetNameOnlyPart(this TypeSyntax value) => value.ToString().GetNameOnlyPart();

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
                                   ? new[] { "value" }
                                   : Array.Empty<string>();
                }
            }

            return Array.Empty<string>();
        }

        internal static ISymbol GetSymbol(this SyntaxNode value, Compilation compilation)
        {
            return value.GetSymbol(compilation.GetSemanticModel(value.SyntaxTree));
        }

        internal static ISymbol GetSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(value);

            return symbolInfo.Symbol;
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

        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax value, SemanticModel semanticModel) => value.Type.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this SyntaxNode syntaxNode)
        {
            if (syntaxNode is null)
            {
                return null;
            }

            var commentOnNode = FindDocumentationCommentTriviaSyntaxForNode(syntaxNode);
            if (commentOnNode != null)
            {
                return commentOnNode;
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

                        return null;
                    }

                case BaseMethodDeclarationSyntax method:
                    {
                        var attributeListSyntax = method.AttributeLists.FirstOrDefault();
                        if (attributeListSyntax != null)
                        {
                            return FindDocumentationCommentTriviaSyntaxForNode(attributeListSyntax);
                        }

                        if (method.ChildNodes().FirstOrDefault() is SyntaxNode child)
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

                        if (property.ChildNodes().FirstOrDefault() is SyntaxNode child)
                        {
                            return FindDocumentationCommentTriviaSyntaxForNode(child);
                        }

                        return null;
                    }

                default:
                    {
                        return null;
                    }
            }

            DocumentationCommentTriviaSyntax FindDocumentationCommentTriviaSyntaxForNode(SyntaxNode node)
            {
                var trivia = node.ChildTokens().SelectMany(_ => _.GetAllTrivia());

                foreach (var t in trivia.Where(_ => _.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)))
                {
                    if (t.GetStructure() is DocumentationCommentTriviaSyntax syntax)
                    {
                        return syntax;
                    }
                }

                return null;
            }
        }

        internal static string GetTextWithoutTrivia(this XmlTextAttributeSyntax text)
        {
            return text != null
                       ? string.Concat(text.TextTokens.Select(_ => _.WithoutTrivia())).Trim()
                       : null;
        }

        internal static string GetTextWithoutTrivia(this XmlTextSyntax text)
        {
            return text != null
                       ? string.Concat(text.TextTokens.Select(_ => _.WithoutTrivia())).Trim()
                       : null;
        }

        internal static string GetTextWithoutTrivia(this XmlElementSyntax element) => element.Content.ToString().WithoutXmlCommentExterior();

        internal static string GetTextWithoutTrivia(this XmlEmptyElementSyntax element) => element.WithoutXmlCommentExterior();

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
        /// Gets only the XML elements that are NOT empty (have some content) and have the given tag.
        /// </summary>
        /// <param name="syntax">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the non-empty XML elements (have some content) and have the given tag.
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
        /// Gets only the XML elements that are NOT empty (have some content) and have any of the given tags.
        /// </summary>
        /// <param name="syntax">
        /// The documentation syntax.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlElementSyntax"/> that are the non-empty XML elements (have some content) and have any of the given tags.
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
        /// Gets only the XML elements that are empty (have NO content) and have the given tag.
        /// </summary>
        /// <param name="syntax">
        /// The documentation syntax.
        /// </param>
        /// <param name="tag">
        /// The tag of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the empty XML elements (have NO content) and have the given tag.
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
        /// Gets only the XML elements that are empty (have NO content) and have any of the the given tags.
        /// </summary>
        /// <param name="syntaxNode">
        /// The starting point of the XML elements to consider.
        /// </param>
        /// <param name="tags">
        /// The tags of the XML elements to consider.
        /// </param>
        /// <returns>
        /// A collection of <see cref="XmlEmptyElementSyntax"/> that are the empty XML elements (have NO content) and have any of the the given tags.
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

        internal static bool IsAnyKind(this SyntaxNode value, params SyntaxKind[] kinds) => kinds.ToHashSet().Contains(value.Kind());

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

        internal static bool IsWrongBooleanTag(this SyntaxNode value) => value.IsCBool() || value.IsBBool() || value.IsValueBool() || value.IsCodeBool();

        internal static bool IsWrongNullTag(this SyntaxNode value) => value.IsCNull() || value.IsBNull() || value.IsValueNull() || value.IsCodeNull();

        internal static bool IsBooleanTag(this SyntaxNode value) => value.IsSeeLangword(Booleans) || value.IsWrongBooleanTag();

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
            var s = value.ToString();

            return s == exceptionType.Name || s == exceptionType.FullName;
        }

        internal static bool IsExceptionCommentFor<T>(this XmlElementSyntax value) where T : Exception => IsExceptionComment(value, typeof(T));

        internal static bool IsExceptionComment(this XmlElementSyntax value, Type exceptionType)
        {
            var attribute = value?.StartTag.Attributes.OfType<XmlCrefAttributeSyntax>().FirstOrDefault();
            if (attribute != null)
            {
                if (attribute.Cref is NameMemberCrefSyntax n && n.Name.IsException(exceptionType))
                {
                    return true;
                }

                if (attribute.Cref is QualifiedCrefSyntax q && q.Member is NameMemberCrefSyntax nn && nn.Name.IsException(exceptionType))
                {
                    return true;
                }
            }

            return false;
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

        internal static bool IsSeeCref(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(emptyElement.Attributes);
                    }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(element.StartTag.Attributes);
                    }

                default:
                    {
                        return false;
                    }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax) => syntax.FirstOrDefault() is XmlCrefAttributeSyntax;
        }

        internal static bool IsSeeCref(this SyntaxNode value, string type)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(emptyElement.Attributes, type);
                    }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(element.StartTag.Attributes, type);
                    }

                default:
                    {
                        return false;
                    }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, string content) => syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute && attribute.Cref.ToString() == content;
        }

        internal static bool IsSeeCref(this SyntaxNode value, TypeSyntax type)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(emptyElement.Attributes, type);
                    }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(element.StartTag.Attributes, type);
                    }

                default:
                    {
                        return false;
                    }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, TypeSyntax t)
            {
                if (syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute)
                {
                    if (attribute.Cref is NameMemberCrefSyntax m)
                    {
                        return t is GenericNameSyntax
                                   ? IsSameGeneric(m.Name, t)
                                   : IsSameName(m.Name, t);
                    }
                }

                return false;
            }
        }

        internal static bool IsSeeCref(this SyntaxNode value, TypeSyntax type, NameSyntax member)
        {
            switch (value)
            {
                case XmlEmptyElementSyntax emptyElement when emptyElement.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(emptyElement.Attributes, type, member);
                    }

                case XmlElementSyntax element when element.GetName() == Constants.XmlTag.See:
                    {
                        return IsCref(element.StartTag.Attributes, type, member);
                    }

                default:
                    {
                        return false;
                    }
            }

            bool IsCref(SyntaxList<XmlAttributeSyntax> syntax, TypeSyntax t, NameSyntax name)
            {
                if (syntax.FirstOrDefault() is XmlCrefAttributeSyntax attribute)
                {
                    if (attribute.Cref is QualifiedCrefSyntax q && IsSameGeneric(q.Container, t))
                    {
                        if (q.Member is NameMemberCrefSyntax m && IsSameName(m.Name, name))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        internal static bool IsWhiteSpaceOnlyText(this SyntaxNode value) => value is XmlTextSyntax text && text.IsWhiteSpaceOnlyText();

        internal static bool IsWhiteSpaceOnlyText(this XmlTextSyntax value) => value.GetTextWithoutTrivia().IsNullOrWhiteSpace();

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

        internal static bool IsSeeLangword(this SyntaxNode value, params string[] texts)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() == Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                if (attribute?.GetName() == Constants.XmlTag.Attribute.Langword)
                {
                    if (texts == null || texts.Length == 0)
                    {
                        return true;
                    }

                    foreach (var token in attribute.DescendantTokens())
                    {
                        foreach (var text in texts)
                        {
                            if (text.Equals(token.ValueText, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }
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

        internal static bool IsStringLiteral(this ArgumentSyntax value)
        {
            switch (value?.Expression?.Kind())
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

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes<InvocationExpressionSyntax>(_ => IsLinqExtensionMethod(semanticModel.GetSymbolInfo(_)));

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
            var result = source.ToList();
            for (var index = 0; index < result.Count; index++)
            {
                var value = result[index];
                if (value is XmlTextSyntax text)
                {
                    result[index] = text.ReplaceText(phrase, replacement);
                }
            }

            return new SyntaxList<XmlNodeSyntax>(result);
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

            return new SyntaxList<XmlNodeSyntax>(result);
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, string phrase, string replacement)
        {
            return ReplaceText(value, new[] { phrase }, replacement);
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, string[] phrases, string replacement)
        {
            var map = new Dictionary<SyntaxToken, SyntaxToken>();

            foreach (var token in value.TextTokens)
            {
                var text = token.ValueText;

                foreach (var phrase in phrases.Where(phrase => text.Contains(phrase)))
                {
                    text = text.Replace(phrase, replacement);
                }

                if (ReferenceEquals(token.ValueText, text) is false)
                {
                    map[token] = token.WithText(text);
                }
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
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

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(new SyntaxList<XmlNodeSyntax>(contents));

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(new SyntaxList<XmlNodeSyntax>(contents));

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(new SyntaxList<XmlNodeSyntax>(contents));

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(new SyntaxList<XmlNodeSyntax>(contents));

        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

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
            var textTokens = new List<SyntaxToken>(value.TextTokens);

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

        internal static string WithoutXmlCommentExterior(this string value) => value.Without("///").Trim();

        internal static string WithoutXmlCommentExterior(this SyntaxNode value) => value.ToString().WithoutXmlCommentExterior();

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
            if (startTexts == null || startTexts.Length == 0)
            {
                return value;
            }

            var textTokens = new List<SyntaxToken>(value.TextTokens);

            for (var i = 0; i < textTokens.Count; i++)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralToken))
                {
                    var originalText = token.Text.TrimStart();

                    if (originalText.IsNullOrWhiteSpace())
                    {
                        continue;
                    }

                    foreach (var startText in startTexts)
                    {
                        if (originalText.StartsWith(startText, StringComparison.Ordinal))
                        {
                            var modifiedText = originalText.Substring(startText.Length);

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

        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

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
                var parameters = new List<ParameterSyntax>(indexer.ParameterList.Parameters);

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

            ParameterSyntax Parameter(TypeSyntax type) => SyntaxFactory.Parameter(default, default, type, SyntaxFactory.Identifier("value"), default);
        }

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
            if (value is XmlElementSyntax syntax && string.Equals(tagName, syntax.GetName(), StringComparison.OrdinalIgnoreCase))
            {
                var content = syntax.Content.ToString().Trim();

                return contents.Any(expected => expected.Equals(content, StringComparison.OrdinalIgnoreCase));
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

        private static bool IsLinqExtensionMethod(SymbolInfo info) => info.Symbol.IsLinqExtensionMethod() || info.CandidateSymbols.Any(_ => _.IsLinqExtensionMethod());

        private static bool IsSameGeneric(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is GenericNameSyntax g1 && t2 is GenericNameSyntax g2)
            {
                if (g1.Identifier.ValueText == g2.Identifier.ValueText)
                {
                    var arguments1 = g1.TypeArgumentList.Arguments;
                    var arguments2 = g2.TypeArgumentList.Arguments;

                    if (arguments1.Count == arguments2.Count)
                    {
                        for (var i = 0; i < arguments1.Count; i++)
                        {
                            if (IsSameName(arguments1[i], arguments2[i]) is false)
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsSameName(TypeSyntax t1, TypeSyntax t2)
        {
            if (t1 is IdentifierNameSyntax n1 && t2 is IdentifierNameSyntax n2)
            {
                return n1.Identifier.ValueText == n2.Identifier.ValueText;
            }

            return t1.ToString() == t2.ToString();
        }

        private static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        private static XmlTextSyntax XmlText(SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

        private static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(SyntaxFactory.TokenList(textTokens));
    }
}