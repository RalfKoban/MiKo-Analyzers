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

//// ncrunch: collect values off

        internal static IEnumerable<T> Ancestors<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors().OfType<T>(); // value.AncestorsAndSelf().OfType<T>();

        internal static bool Contains(this SyntaxNode value, char c) => value?.ToString().Contains(c) ?? false;

        internal static IEnumerable<T> ChildNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).Cast<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().Where(predicate);

//// ncrunch: collect values default

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

//// ncrunch: collect values off

        internal static T FirstAncestor<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault();

        internal static T FirstAncestor<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault(predicate);

        internal static T FirstAncestor<T>(this SyntaxNode value, ISet<SyntaxKind> kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        internal static T FirstAncestor<T>(this SyntaxNode value, params SyntaxKind[] kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        internal static SyntaxNode FirstChild(this SyntaxNode value) => value.ChildNodes().FirstOrDefault();

        internal static T FirstChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault();

        internal static SyntaxNode FirstChild(this SyntaxNode value, Func<SyntaxNode, bool> predicate) => value.ChildNodes().FirstOrDefault(predicate);

        internal static T FirstChild<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.ChildNodes().OfKind(kind).FirstOrDefault() as T;

        internal static T FirstChild<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault(predicate);

        internal static SyntaxToken FirstChildToken(this SyntaxNode value) => value.ChildTokens().FirstOrDefault();

        internal static SyntaxToken FirstChildToken(this SyntaxNode value, SyntaxKind kind) => value.ChildTokens().OfKind(kind).First();

        internal static T FirstDescendant<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault();

        internal static T FirstDescendant<T>(this SyntaxNode value, SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).FirstOrDefault() as T;

        internal static T FirstDescendant<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault(predicate);

        internal static T LastChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().LastOrDefault();

//// ncrunch: collect values default

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

                var listCount = list.Count;

                // try to find the first syntax that is not only an XmlCommentExterior
                for (var index = 0; index < listCount; index++)
                {
                    first = list[index];

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

                    if (token.IsDefaultValue())
                    {
                        // we did not find it, so it seems like an empty text
                        return firstText.SpanStart;
                    }

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

                    if (token.IsDefaultValue())
                    {
                        // we did not find it
                    }
                    else
                    {
                        var text = token.Text;

                        var offset = text.Length - text.TrimEnd().Length;

                        end = token.Span.End - offset;
                    }
                }

                return end;
            }
        }

        internal static XmlTextAttributeSyntax GetNameAttribute(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlElementSyntax e: return e.GetAttributes<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Attribute.Name);
                case XmlEmptyElementSyntax ee: return ee.Attributes.OfType<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() == Constants.XmlTag.Attribute.Name);
                default: return null;
            }
        }

//// ncrunch: collect values off

        internal static string GetParameterName(this XmlElementSyntax value) => value.GetAttributes<XmlNameAttributeSyntax>().FirstOrDefault()?.Identifier.GetName();

        internal static string GetParameterName(this XmlEmptyElementSyntax value) => value.Attributes.OfType<XmlNameAttributeSyntax>().FirstOrDefault()?.Identifier.GetName();

        internal static XmlElementSyntax GetParameterComment(this DocumentationCommentTriviaSyntax value, string parameterName) => value.FirstDescendant<XmlElementSyntax>(_ => _.GetName() == Constants.XmlTag.Param && _.GetParameterName() == parameterName);

//// ncrunch: collect values default

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

        internal static ExpressionSyntax GetRelatedCondition(this SyntaxNode value)
        {
            var coalesceExpression = value.FirstAncestorOrSelf<BinaryExpressionSyntax>(_ => _.IsKind(SyntaxKind.CoalesceExpression));

            if (coalesceExpression != null)
            {
                return coalesceExpression;
            }

            // most probably it's a if/else, but it might be a switch statement as well
            var condition = value.GetRelatedIfStatement()?.Condition ?? value.GetEnclosing<SwitchStatementSyntax>()?.Expression;

            return condition;
        }

        internal static ParameterSyntax GetUsedParameter(this ObjectCreationExpressionSyntax value)
        {
            var parameters = CollectParameters(value);

            if (parameters.Any())
            {
                // there might be multiple parameters, so we have to find out which parameter is meant
                var condition = GetRelatedCondition(value);

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

        internal static HashSet<string> GetAllUsedVariables(this SyntaxNode value, SemanticModel semanticModel)
        {
            var dataFlow = semanticModel.AnalyzeDataFlow(value);

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

//// ncrunch: collect values off

        internal static IEnumerable<T> GetAttributes<T>(this XmlEmptyElementSyntax value) where T : XmlAttributeSyntax => value?.Attributes.OfType<XmlAttributeSyntax, T>() ?? Enumerable.Empty<T>();

        internal static IEnumerable<T> GetAttributes<T>(this XmlElementSyntax value) where T : XmlAttributeSyntax => value?.StartTag.Attributes.OfType<XmlAttributeSyntax, T>() ?? Enumerable.Empty<T>();

        internal static T GetEnclosing<T>(this SyntaxNode value) where T : SyntaxNode => value.FirstAncestorOrSelf<T>();

//// ncrunch: collect values default

        internal static SyntaxNode GetEnclosing(this SyntaxNode value, ISet<SyntaxKind> syntaxKinds)
        {
            var node = value;

            while (true)
            {
                if (node is null)
                {
                    return null;
                }

                if (node.IsAnyKind(syntaxKinds))
                {
                    return node;
                }

                node = node.Parent;
            }
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

                if (node.IsAnyKind(syntaxKinds))
                {
                    return node;
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

        internal static SyntaxTrivia GetLeadingComment(this SyntaxNode value)
        {
            while (true)
            {
                if (value is null)
                {
                    return default;
                }

                var list = value.GetLeadingTrivia();

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var listCount = list.Count;

                for (var index = 0; index < listCount; index++)
                {
                    var trivia = list[index];

                    if (trivia.IsComment())
                    {
                        return trivia;
                    }
                }

                value = value.FirstChild();
            }
        }

        internal static string GetMethodName(this ParameterSyntax value)
        {
            var enclosingNode = value.GetEnclosing(MethodNameSyntaxKinds);

            switch (enclosingNode)
            {
                case MethodDeclarationSyntax m: return m.GetName();
                case ConstructorDeclarationSyntax c: return c.GetName();
                case ConversionOperatorDeclarationSyntax c: return c.GetName();
                case DestructorDeclarationSyntax d: return d.GetName();
                case OperatorDeclarationSyntax o: return o.GetName();
                default:
                    return null;
            }
        }

        internal static string GetName(this ArgumentSyntax value) => value.Expression.GetName();

        internal static string GetName(this AttributeSyntax value)
        {
            switch (value.Name)
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
                case ConversionOperatorDeclarationSyntax c: return c.GetName();
                case DestructorDeclarationSyntax d: return d.GetName();
                case OperatorDeclarationSyntax o: return o.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this BasePropertyDeclarationSyntax value)
        {
            switch (value)
            {
                case IndexerDeclarationSyntax i: return i.GetName();
                case PropertyDeclarationSyntax p: return p.GetName();
                case EventDeclarationSyntax e: return e.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ConversionOperatorDeclarationSyntax value) => value?.OperatorKeyword.ValueText;

        internal static string GetName(this DestructorDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this EventDeclarationSyntax value) => value?.Identifier.ValueText;

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

                    if (text == "nameof" && value.Ancestors<MemberAccessExpressionSyntax>().None())
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

//// ncrunch: collect values off

        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this IndexerDeclarationSyntax value) => value?.ThisKeyword.ValueText;

        internal static string GetName(this LiteralExpressionSyntax value) => value?.Token.ValueText;

        internal static string GetName(this LocalFunctionStatementSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this NameEqualsSyntax value) => value?.Name.GetName();

        internal static string GetName(this OperatorDeclarationSyntax value) => value?.OperatorToken.ValueText;

        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this UsingDirectiveSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.GetName();

        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlElementStartTagSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlElementEndTagSyntax value) => value?.Name.GetName();

        internal static string GetName(this XmlNameSyntax value) => value?.LocalName.ValueText;

//// ncrunch: collect values default

        internal static string GetXmlTagName(this SyntaxNode value)
        {
            switch (value)
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

        internal static int GetPositionWithinStartLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetStartingLine(this SyntaxNode value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxNode value) => value.GetLocation().GetEndingLine();

        internal static ISymbol GetSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var symbolInfo = GetSymbolInfo();

            var symbol = symbolInfo.Symbol;

            if (symbol is null)
            {
                if (symbolInfo.CandidateReason == CandidateReason.OverloadResolutionFailure)
                {
                    // we did not find the symbol, so we take the first one, assuming that this is the right one
                    return symbolInfo.CandidateSymbols.FirstOrDefault();
                }
            }

            return symbol;

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

        internal static int GetCharacterPositionInStartLine(this SyntaxNode value) => value.GetPositionWithinStartLine();

        internal static LinePosition GetStartPosition(this SyntaxNode value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxNode value) => value.GetLocation().GetEndPosition();

//// ncrunch: collect values off

        internal static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this SyntaxNode value)
        {
            if (value is null)
            {
                return null;
            }

            var token = value.DescendantTokens().First();

            if (token.HasStructuredTrivia)
            {
                // 'HasLeadingTrivia' creates the list as well and checks for a count greater than zero so we can save some time and memory by doing it by ourselves
                var leadingTrivia = token.LeadingTrivia;
                var count = leadingTrivia.Count;

                for (var index = 0; index < count; index++)
                {
                    var trivia = leadingTrivia[index];

                    if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) && trivia.GetStructure() is DocumentationCommentTriviaSyntax syntax)
                    {
                        return syntax;
                    }
                }
            }

            return null;
        }

//// ncrunch: collect values default

        internal static ReadOnlySpan<char> GetTextTrimmed(this XmlElementSyntax value)
        {
            if (value is null)
            {
                return ReadOnlySpan<char>.Empty;
            }

            return value.GetTextWithoutTrivia().Without(Constants.EnvironmentNewLine).WithoutParaTagsAsSpan().Trim();
        }

        internal static string GetTextWithoutTrivia(this XmlTextAttributeSyntax value)
        {
            if (value is null)
            {
                return null;
            }

            var builder = new StringBuilder();

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                builder.Append(token.WithoutTrivia().ValueText);
            }

            var result = builder.ToString();

            return result.Trim();
        }

        internal static ReadOnlySpan<char> GetTextWithoutTrivia(this XmlTextSyntax value)
        {
            if (value is null)
            {
                return null;
            }

            var builder = new StringBuilder();

            foreach (var valueText in value.GetTextWithoutTriviaLazy())
            {
                builder.Append(valueText);
            }

            var result = builder.ToString();

            return result.AsSpan().Trim();
        }

        internal static StringBuilder GetTextWithoutTrivia(this XmlElementSyntax value) => new StringBuilder(value.Content.ToString()).WithoutXmlCommentExterior();

        internal static string GetTextWithoutTrivia(this XmlEmptyElementSyntax value) => value.WithoutXmlCommentExterior();

        internal static IEnumerable<string> GetTextWithoutTriviaLazy(this XmlTextSyntax value)
        {
            if (value is null)
            {
                yield break;
            }

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                yield return token.WithoutTrivia().ValueText;
            }
        }

        internal static IEnumerable<XmlElementSyntax> GetExampleXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Example);

        internal static IEnumerable<XmlElementSyntax> GetExceptionXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Exception);

        internal static IEnumerable<XmlElementSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Summary);

        internal static IEnumerable<XmlNodeSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value, IEnumerable<string> tags)
        {
            var summaryXmls = value.GetSummaryXmls();

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

        internal static IEnumerable<XmlElementSyntax> GetRemarksXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Remarks);

        internal static IEnumerable<XmlElementSyntax> GetReturnsXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Returns);

        internal static IEnumerable<XmlElementSyntax> GetValueXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Value);

//// ncrunch: collect values off

        /// <summary>
        /// Only gets the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
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
        internal static IEnumerable<XmlElementSyntax> GetXmlSyntax(this SyntaxNode value, string tag)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return value.DescendantNodes(_ => true, true).OfType<XmlElementSyntax>()
                        .Where(_ => _.GetName() == tag);
        }

        /// <summary>
        /// Only gets the XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
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
        internal static IEnumerable<XmlElementSyntax> GetXmlSyntax(this SyntaxNode value, IEnumerable<string> tags)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return value.DescendantNodes(_ => true, true).OfType<XmlElementSyntax>()
                        .Where(_ => tags.Contains(_.GetName()));
        }

        /// <summary>
        /// Only gets the XML elements that are empty (have NO content) and the given tag out of the documentation syntax.
        /// </summary>
        /// <param name="value">
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
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode value, string tag)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return value.DescendantNodes(_ => true, true).OfType<XmlEmptyElementSyntax>()
                        .Where(_ => _.GetName() == tag);
        }

        /// <summary>
        /// Only gets the XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
        /// </summary>
        /// <param name="value">
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
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode value, IEnumerable<string> tags)
        {
            // we have to delve into the trivias to find the XML syntax nodes
            return value.DescendantNodes(_ => true, true).OfType<XmlEmptyElementSyntax>()
                        .Where(_ => tags.Contains(_.GetName()));
        }

//// ncrunch: collect values default

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

        internal static bool HasComment(this SyntaxNode value) => value.HasLeadingComment() || value.HasTrailingComment();

        internal static bool HasLeadingComment(this SyntaxNode value) => value.GetLeadingTrivia().Any(_ => _.IsComment());

        internal static bool HasTrailingComment(this SyntaxNode value) => value != null && value.GetTrailingTrivia().Any(_ => _.IsComment());

        internal static bool HasTrailingEndOfLine(this SyntaxNode value) => value != null && value.GetTrailingTrivia().Any(_ => _.IsEndOfLine());

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

        internal static bool IsAnyKind(this SyntaxNode value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        internal static bool IsAnyKind(this SyntaxNode value, params SyntaxKind[] kinds)
        {
            var valueKind = value.Kind();

            // ReSharper disable once LoopCanBeConvertedToQuery  : For performance reasons we use indexing instead of an enumerator
            // ReSharper disable once ForCanBeConvertedToForeach : For performance reasons we use indexing instead of an enumerator
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

        internal static bool IsInside(this SyntaxNode node, ISet<SyntaxKind> kinds)
        {
            foreach (var ancestor in node.Ancestors())
            {
                var kind = ancestor.Kind();

                if (kinds.Contains(kind))
                {
                    return true;
                }

                switch (kind)
                {
                    case SyntaxKind.MethodDeclaration:
                    case SyntaxKind.IndexerDeclaration:
                    case SyntaxKind.ConstructorDeclaration:
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.EventDeclaration:
                    case SyntaxKind.EventFieldDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.RecordDeclaration:
                        return false;
                }
            }

            return false;
        }

        internal static bool IsInsideMoqCall(this MemberAccessExpressionSyntax syntax)
        {
            if (syntax.Parent is InvocationExpressionSyntax i && i.Parent is LambdaExpressionSyntax lambda)
            {
                return IsMoqCall(lambda);
            }

            return false;
        }

        internal static bool IsMoqCall(this LambdaExpressionSyntax lambda)
        {
            if (lambda.Parent is ArgumentSyntax a && a.Parent?.Parent is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax m)
            {
                switch (m.GetName())
                {
                    case Constants.Moq.Setup:
                    case Constants.Moq.SetupGet:
                    case Constants.Moq.SetupSet:
                    case Constants.Moq.SetupSequence:
                    case Constants.Moq.Verify:
                    case Constants.Moq.VerifyGet:
                    case Constants.Moq.VerifySet:
                    case Constants.Moq.Of when m.Expression is IdentifierNameSyntax ins && ins.GetName() == Constants.Moq.Mock:
                    {
                        // here we assume that we have a Moq call
                        return true;
                    }
                }
            }

            return false;
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

            if (name.Contains("Command") && name.Contains("CommandManager") is false)
            {
                return semanticModel.LookupSymbols(value.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol && symbol.IsCommand();
            }

            return false;
        }

        internal static bool IsConst(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.ConstKeyword);

        internal static bool IsEventRegistration(this StatementSyntax value, SemanticModel semanticModel)
        {
            if (value is ExpressionStatementSyntax e && e.Expression is AssignmentExpressionSyntax assignment)
            {
                return IsEventRegistration(assignment, semanticModel);
            }

            return false;
        }

        internal static bool IsEventRegistration(this AssignmentExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value.Right is IdentifierNameSyntax)
            {
                switch (value.Left)
                {
                    case MemberAccessExpressionSyntax maes:
                        return maes.GetSymbol(semanticModel) is IEventSymbol;

                    case IdentifierNameSyntax identifier:
                        return identifier.GetSymbol(semanticModel) is IEventSymbol;
                }
            }

            return false;
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
            foreach (var a in value.Ancestors<ArgumentSyntax>())
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

        internal static bool IsGenerated(this TypeDeclarationSyntax value) => value.HasAttributeName(Constants.Names.GeneratedAttributeNames);

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

        internal static bool IsCallTo(this IfStatementSyntax value, string methodName)
        {
            var ifExpression = value.FirstChild<MemberAccessExpressionSyntax>();

            if (ifExpression.IsCallTo(methodName))
            {
                return true;
            }

            var binaryExpression = value.FirstChild<BinaryExpressionSyntax>();

            if (binaryExpression.IsBinaryCallTo(methodName))
            {
                return true;
            }

            return false;
        }

        internal static bool IsCallTo(this ExpressionSyntax value, string methodName) => value is MemberAccessExpressionSyntax m && m.Name.ToString() == methodName;

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

        internal static bool IsAbstract(this MethodDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.AbstractKeyword);

        internal static bool IsAccessOnObjectUnderTest(this ExpressionSyntax value)
        {
            if (value is MemberAccessExpressionSyntax mae)
            {
                switch (mae.Expression)
                {
                    case IdentifierNameSyntax ins when Constants.Names.ObjectUnderTestNames.Contains(ins.GetName()):
                    case InvocationExpressionSyntax i when i.IsInvocationOnObjectUnderTest():
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool IsAsync(this BasePropertyDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.AsyncKeyword);

        internal static bool IsAsync(this MethodDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.AsyncKeyword);

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

        internal static bool IsParameter(this IdentifierNameSyntax value, SemanticModel semanticModel) => value.EnclosingMethodHasParameter(value.GetName(), semanticModel);

        internal static bool IsPara(this SyntaxNode value) => value.IsXmlTag(Constants.XmlTag.Para);

        internal static bool IsReadOnly(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);

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

        internal static bool IsStringConcatenation(this ExpressionSyntax value, SemanticModel semanticModel = null)
        {
            if (value is BinaryExpressionSyntax b && b.IsKind(SyntaxKind.AddExpression))
            {
                if (b.Left.IsStringLiteral() || b.Right.IsStringLiteral())
                {
                    return true;
                }

                if (b.Left.IsStringConcatenation(semanticModel) || b.Right.IsStringConcatenation(semanticModel))
                {
                    return true;
                }

                if (semanticModel != null)
                {
                    if (b.Left.IsString(semanticModel) || b.Right.IsString(semanticModel))
                    {
                        return true;
                    }
                }
            }

            return false;
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

//// ncrunch: collect values off

        internal static bool IsInsideTestClass(this SyntaxNode value) => value.Ancestors<ClassDeclarationSyntax>().Any(_ => _.IsTestClass());

        internal static bool IsTestClass(this TypeDeclarationSyntax value) => value is ClassDeclarationSyntax declaration && IsTestClass(declaration);

        internal static bool IsTestClass(this ClassDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestClassAttributeNames);

        internal static bool IsTestMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestMethodAttributeNames);

        internal static bool IsTestOneTimeSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestOneTimeSetupAttributeNames);

        internal static bool IsTestOneTimeTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestOneTimeTearDownAttributeNames);

        internal static bool IsTestSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestSetupAttributeNames);

        internal static bool IsTestTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestTearDownAttributeNames);

        internal static bool IsTypeUnderTestCreationMethod(this MethodDeclarationSyntax value) => Constants.Names.TypeUnderTestMethodNames.Contains(value.GetName());

        internal static bool IsTypeUnderTestVariable(this VariableDeclaratorSyntax value) => Constants.Names.TypeUnderTestVariableNames.Contains(value.GetName());

        internal static bool IsValueBool(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Booleans);

        internal static bool IsValueNull(this SyntaxNode value) => value.Is(Constants.XmlTag.Value, Nulls);

        internal static bool IsVoid(this TypeSyntax value) => value is PredefinedTypeSyntax p && p.Keyword.IsKind(SyntaxKind.VoidKeyword);

//// ncrunch: collect values default

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes<InvocationExpressionSyntax>(_ => IsLinqExtensionMethod(_, semanticModel));

        internal static IReadOnlyList<TResult> OfKind<TResult, TSyntaxNode>(this SeparatedSyntaxList<TSyntaxNode> source, SyntaxKind kind) where TSyntaxNode : SyntaxNode where TResult : TSyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount == 0)
            {
                return Array.Empty<TResult>();
            }

            var results = new List<TResult>();

            for (var index = 0; index < sourceCount; index++)
            {
                var item = source[index];

                if (item.IsKind(kind))
                {
                    results.Add((TResult)item);
                }
            }

            return results;
        }

        internal static IReadOnlyList<T> OfKind<T>(this IReadOnlyList<T> source, SyntaxKind kind) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount == 0)
            {
                return Array.Empty<T>();
            }

            var results = new List<T>();

            for (var index = 0; index < sourceCount; index++)
            {
                var item = source[index];

                if (item.IsKind(kind))
                {
                    results.Add(item);
                }
            }

            return results;
        }

        internal static IEnumerable<T> OfKind<T>(this IEnumerable<T> source, SyntaxKind kind) where T : SyntaxNode
        {
            if (source is IReadOnlyList<T> list)
            {
                return list.OfKind(kind);
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            return OfKindLocal();

            IEnumerable<T> OfKindLocal()
            {
                foreach (var item in source)
                {
                    if (item.IsKind(kind))
                    {
                        yield return item;
                    }
                }
            }
        }

        internal static IReadOnlyList<TResult> OfType<TResult>(this SyntaxList<XmlNodeSyntax> source) where TResult : XmlNodeSyntax => source.OfType<XmlNodeSyntax, TResult>();

        internal static IReadOnlyList<TResult> OfType<TResult>(this SyntaxList<XmlAttributeSyntax> source) where TResult : XmlAttributeSyntax => source.OfType<XmlAttributeSyntax, TResult>();

        internal static IReadOnlyList<TResult> OfType<T, TResult>(this SyntaxList<T> source) where T : SyntaxNode where TResult : T
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount == 0)
            {
                return Array.Empty<TResult>();
            }

            var results = new List<TResult>();

            for (var index = 0; index < sourceCount; index++)
            {
                if (source[index] is TResult result)
                {
                    results.Add(result);
                }
            }

            return results;
        }

        internal static BaseTypeDeclarationSyntax RemoveNodeAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, SyntaxNode node)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            return value.Without(node)
                        .WithOpenBraceToken(openBraceToken);
        }

        internal static BaseTypeDeclarationSyntax RemoveNodesAndAdjustOpenCloseBraces(this BaseTypeDeclarationSyntax value, IEnumerable<SyntaxNode> nodes)
        {
            // to avoid line-ends before the first node, we simply create a new open brace without the problematic trivia
            var openBraceToken = value.OpenBraceToken.WithoutTrivia().WithEndOfLine();

            return value.Without(nodes)
                        .WithOpenBraceToken(openBraceToken);
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
            var resultCount = result.Count;

            for (var index = 0; index < resultCount; index++)
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

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

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

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

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

        internal static bool ReturnsImmediately(this IfStatementSyntax node)
        {
            switch (node?.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool ReturnsImmediately(this ElseClauseSyntax node)
        {
            switch (node?.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        internal static SyntaxNode PreviousSibling(this SyntaxNode value)
        {
            var parent = value?.Parent;

            if (parent is null)
            {
                return default;
            }

            SyntaxNode previousChild = default;

            foreach (var child in parent.ChildNodes())
            {
                if (child == value)
                {
                    return previousChild;
                }

                previousChild = child;
            }

            return default;
        }

        internal static SyntaxNodeOrToken PreviousSiblingNodeOrToken(this SyntaxNode value)
        {
            var parent = value?.Parent;

            if (parent is null)
            {
                return default;
            }

            SyntaxNodeOrToken previousChild = default;

            foreach (var child in parent.ChildNodesAndTokens())
            {
                if (child == value)
                {
                    return previousChild;
                }

                previousChild = child;
            }

            return default;
        }

        internal static SyntaxNode NextSibling(this SyntaxNode value)
        {
            var parent = value?.Parent;

            if (parent is null)
            {
                return default;
            }

            using (var enumerator = parent.ChildNodes().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current == value)
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

        internal static IList<SyntaxNode> Siblings(this SyntaxNode value) => Siblings<SyntaxNode>(value);

        internal static IList<T> Siblings<T>(this SyntaxNode value) where T : SyntaxNode
        {
            var parent = value?.Parent;

            if (parent != null)
            {
                return parent.ChildNodes<T>().ToList();
            }

            return Array.Empty<T>();
        }

        internal static bool Throws<T>(this SyntaxNode value) where T : Exception
        {
            switch (value)
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

        internal static T WithLeadingSpace<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithTrailingSpace<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithAdditionalLeadingSpaces<T>(this T value, int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces == 0)
            {
                return value;
            }

            var currentSpaces = value.GetPositionWithinStartLine();

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static T WithAdditionalLeadingSpacesOnDescendants<T>(this T node, IReadOnlyCollection<SyntaxNodeOrToken> descendants, int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces == 0)
            {
                return node;
            }

            if (descendants.Count == 0)
            {
                return node;
            }

            return node.ReplaceSyntax(
                                  descendants.Where(_ => _.IsNode).Select(_ => _.AsNode()),
                                  (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                  descendants.Where(_ => _.IsToken).Select(_ => _.AsToken()),
                                  (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                  Enumerable.Empty<SyntaxTrivia>(),
                                  (original, rewritten) => rewritten);
        }

        internal static T WithLeadingSpaces<T>(this T value, int count) where T : SyntaxNode
        {
            if (count <= 0)
            {
                return value;
            }

            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count == 0)
            {
                return value.WithLeadingTrivia(SyntaxFactory.Whitespace(new string(' ', count)));
            }

            // re-construct leading comment with correct amount of spaces but keep comments
            // (so we have to find out each white-space trivia and have to replace it with the correct amount of spaces)
            var finalTrivia = leadingTrivia.ToArray();

            var resetFinalTrivia = false;

            for (var index = 0; index < finalTrivia.Length; index++)
            {
                var trivia = finalTrivia[index];

                if (trivia.IsWhiteSpace())
                {
                    finalTrivia[index] = SyntaxFactory.Whitespace(new string(' ', count));
                }

                if (trivia.IsComment())
                {
                    resetFinalTrivia = true;

                    // we do not need to adjust further as we found a comment and have to fix them based on their specific lines
                    break;
                }
            }

            if (resetFinalTrivia)
            {
                finalTrivia = CalculateWhitespaceTriviaWithComment(count, leadingTrivia.ToArray());
            }

            return value.WithLeadingTrivia(finalTrivia);
        }

        internal static T WithLeadingXmlComment<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithLeadingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia().WithLeadingXmlComment());

        internal static SyntaxNode WithModifiers(this FieldDeclarationSyntax value, IEnumerable<SyntaxKind> modifiers)
        {
            var oldModifiers = value.Modifiers;

            var newModifiers = SyntaxFactory.TokenList(modifiers.Select(_ => _.AsToken()));

            if (oldModifiers.Count > 0)
            {
                // keep comments
                newModifiers = newModifiers.Replace(newModifiers[0], newModifiers[0].WithTriviaFrom(oldModifiers[0]));

                return value.WithModifiers(newModifiers);
            }

            var declaration = value.Declaration;
            var type = declaration.Type;

            // keep comments
            newModifiers = newModifiers.Replace(newModifiers[0], newModifiers[0].WithLeadingTriviaFrom(type));

            return value.WithModifiers(newModifiers)
                        .WithDeclaration(declaration.WithType(type.WithLeadingSpace()));
        }

        internal static SyntaxNode WithModifiers(this FieldDeclarationSyntax value, params SyntaxKind[] modifiers) => value.WithModifiers((IEnumerable<SyntaxKind>)modifiers);

        internal static T WithAdditionalModifier<T>(this T value, SyntaxKind keyword) where T : MemberDeclarationSyntax
        {
            var modifiers = value.Modifiers;
            var position = modifiers.IndexOf(SyntaxKind.PartialKeyword);

            var syntaxToken = keyword.AsToken();

            if (modifiers.Count == 0)
            {
                var commentedChild = value.FirstChildToken();

                // remove comment from previous first child
                value = value.ReplaceToken(commentedChild, commentedChild.WithLeadingSpace());

                // add comment to new first child
                syntaxToken = syntaxToken.WithLeadingTriviaFrom(commentedChild);
            }

            var newModifiers = position > -1
                               ? value.Modifiers.Insert(position, syntaxToken)
                               : value.Modifiers.Add(syntaxToken);

            return (T)value.WithModifiers(newModifiers);
        }

        internal static T WithLeadingXmlCommentExterior<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(XmlCommentExterior);

        internal static T Without<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, IEnumerable<SyntaxNode> nodes) where T : SyntaxNode => value.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, params SyntaxNode[] nodes) where T : SyntaxNode => value.Without((IEnumerable<SyntaxNode>)nodes);

        internal static SyntaxList<XmlNodeSyntax> WithoutFirstXmlNewLine(this SyntaxList<XmlNodeSyntax> values)
        {
            if (values.FirstOrDefault() is XmlTextSyntax text)
            {
                var newText = text.WithoutFirstXmlNewLine();

                return newText.TextTokens.Count != 0
                       ? values.Replace(text, newText)
                       : values.Remove(text);
            }

            return values;
        }

        internal static XmlElementSyntax WithoutFirstXmlNewLine(this XmlElementSyntax value)
        {
            return value.WithContent(value.Content.WithoutFirstXmlNewLine());
        }

        internal static XmlTextSyntax WithoutFirstXmlNewLine(this XmlTextSyntax value)
        {
            return value.WithTextTokens(value.TextTokens.WithoutFirstXmlNewLine()).WithoutLeadingTrivia();
        }

        internal static XmlTextSyntax WithoutLastXmlNewLine(this XmlTextSyntax value)
        {
            var textTokens = value.TextTokens.WithoutLastXmlNewLine();

            return value.WithTextTokens(textTokens);
        }

        internal static T WithTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.WithLeadingTriviaFrom(node)
                                                                                                        .WithTrailingTriviaFrom(node);

        internal static T WithTriviaFrom<T>(this T value, SyntaxToken token) where T : SyntaxNode => value.WithLeadingTriviaFrom(token)
                                                                                                          .WithTrailingTriviaFrom(token);

        internal static T WithLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithLeadingTriviaFrom<T>(this T value, SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithTrailingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetTrailingTrivia();

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static T WithTrailingTriviaFrom<T>(this T value, SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingTrivia(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values[0], values[0].WithoutLeadingTrivia());

        internal static T WithoutLeadingEndOfLine<T>(this T value) where T : SyntaxNode
        {
            var trivia = value.GetLeadingTrivia();

            if (trivia.Count > 0 && trivia[0].IsEndOfLine())
            {
                return value.WithLeadingTrivia(trivia.RemoveAt(0));
            }

            return value;
        }

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

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valuesCount = values.Count;

            for (var index = 0; index < valuesCount; index++)
            {
                if (values[index] is XmlTextSyntax s)
                {
                    var originalTextTokens = s.TextTokens;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var originalTextTokensCount = originalTextTokens.Count;

                    if (originalTextTokensCount == 0)
                    {
                        continue;
                    }

                    var textTokens = originalTextTokens.ToList();

                    var modified = false;

                    for (var i = 0; i < originalTextTokensCount; i++)
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
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

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
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

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
            var texts = value.Content.OfType<XmlTextSyntax>();

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

            var tokens = value.TextTokens;

            if (tokens.Count == 0)
            {
                return value;
            }

            var textTokens = tokens.ToList();
            var textTokensCount = textTokens.Count;

            for (var i = 0; i < textTokensCount; i++)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

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

            return value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this XmlElementSyntax value, string startText, FirstWordHandling firstWordHandling = FirstWordHandling.MakeLowerCase) => value.Content.WithStartText(startText, firstWordHandling);

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this SyntaxList<XmlNodeSyntax> values, string startText, FirstWordHandling firstWordHandling = FirstWordHandling.MakeLowerCase)
        {
            if (values.Count > 0)
            {
                return values[0] is XmlTextSyntax textSyntax
                       ? values.Replace(textSyntax, textSyntax.WithStartText(startText, firstWordHandling))
                       : values.Insert(0, XmlText(startText));
            }

            return new SyntaxList<XmlNodeSyntax>(XmlText(startText));
        }

        internal static XmlTextSyntax WithStartText(this XmlTextSyntax value, string startText, FirstWordHandling firstWordHandling = FirstWordHandling.MakeLowerCase)
        {
            var tokens = value.TextTokens;

            if (tokens.Count == 0)
            {
                return XmlText(startText);
            }

            var textTokens = tokens.ToList();

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
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var originalText = token.Text;

                if (originalText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var space = i == 0 ? string.Empty : " ";

                // replace 3rd person word by infinite word if configured
                var continuation = originalText.AdjustFirstWord(firstWordHandling);

                var modifiedText = space + startText + continuation;

                textTokens[i] = token.WithText(modifiedText);

                replaced = true;

                break;
            }

            if (replaced)
            {
                return XmlText(textTokens);
            }

            return XmlText(startText);
        }

        internal static T WithTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingNewLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use an elastic one to enforce the line break

        // internal static T WithTrailingXmlComment<T>(this T value, int spaces) where T : SyntaxNode => value.WithTrailingTrivia(
        //                                                                                                                    SyntaxFactory.ElasticCarriageReturnLineFeed, // use elastic one to allow formatting to be done automatically
        //                                                                                                                    SyntaxFactory.ElasticWhitespace(new string(' ', spaces)),
        //                                                                                                                    XmlCommentExterior);
        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

        internal static SyntaxNode WithUsing(this SyntaxNode value, string usingNamespace)
        {
            var usings = value.DescendantNodes<UsingDirectiveSyntax>().ToList();

            if (usings.Any(_ => _.Name?.ToFullString() == usingNamespace))
            {
                // already set
                return value;
            }

            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingNamespace));

            var usingsCount = usings.Count;

            if (usingsCount == 0)
            {
                return value.InsertNodeBefore(value.FirstChild(), directive);
            }

            for (var index = 0; index < usingsCount; index++)
            {
                var usingDirective = usings[index];

                var usingName = usingDirective.Name?.ToFullString();

                if (usingName == "System")
                {
                    // skip 'System' namespace
                    continue;
                }

                if (usingName?.StartsWith("System.", StringComparison.Ordinal) is true)
                {
                    // skip all 'System' sub-namespaces
                    continue;
                }

                if (string.Compare(usingName, usingNamespace, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    // add using at correct place inside the using block
                    return value.InsertNodeBefore(usingDirective, directive);
                }
            }

            return value.InsertNodeAfter(usings.Last(), directive);
        }

        internal static SyntaxNode WithoutUsing(this SyntaxNode value, string usingNamespace)
        {
            var root = value.SyntaxTree.GetRoot();

            return root.DescendantNodes<UsingDirectiveSyntax>(_ => _.Name?.ToFullString() == usingNamespace)
                       .Select(root.Without)
                       .FirstOrDefault();
        }

        internal static bool HasAttributeName(this TypeDeclarationSyntax value, IEnumerable<string> names)
        {
            var attributeLists = value.AttributeLists;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var attributeListsCount = attributeLists.Count;

            for (var i = 0; i < attributeListsCount; i++)
            {
                var attributes = attributeLists[i].Attributes;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var attributesCount = attributes.Count;

                for (var index = 0; index < attributesCount; index++)
                {
                    var attribute = attributes[index];
                    var name = attribute.GetName();

                    if (names.Contains(name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool HasAttributeName(this MethodDeclarationSyntax value, IEnumerable<string> names)
        {
            var attributeLists = value.AttributeLists;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var attributeListsCount = attributeLists.Count;

            for (var i = 0; i < attributeListsCount; i++)
            {
                var attributes = attributeLists[i].Attributes;

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var attributesCount = attributes.Count;

                for (var index = 0; index < attributesCount; index++)
                {
                    var attribute = attributes[index];
                    var name = attribute.GetName();

                    if (names.Contains(name))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static SeparatedSyntaxList<ParameterSyntax> CollectParameters(ObjectCreationExpressionSyntax syntax)
        {
            var method = syntax.GetEnclosing<BaseMethodDeclarationSyntax>();

            if (method != null)
            {
                return method.ParameterList.Parameters;
            }

            var indexer = syntax.GetEnclosing<IndexerDeclarationSyntax>();

            if (indexer != null)
            {
                var parameters = indexer.ParameterList.Parameters;

                // 'value' is a special parameter that is not part of the parameter list
                return parameters.Insert(0, Parameter(indexer.Type));
            }

            var property = syntax.GetEnclosing<PropertyDeclarationSyntax>();

            var result = SyntaxFactory.SeparatedList<ParameterSyntax>();

            return property is null
                   ? result
                   : result.Add(Parameter(property.Type)); // 'value' is a special parameter that is not part of the parameter list

            ParameterSyntax Parameter(TypeSyntax type) => SyntaxFactory.Parameter(default, default, type, SyntaxFactory.Identifier(Constants.Names.DefaultPropertyParameterName), default);
        }

        private static XmlCrefAttributeSyntax GetCref(SyntaxList<XmlAttributeSyntax> syntax)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var syntaxCount = syntax.Count;

            for (var index = 0; index < syntaxCount; index++)
            {
                if (syntax[index] is XmlCrefAttributeSyntax a)
                {
                    return a;
                }
            }

            return default;
        }

        private static bool Is(this SyntaxNode value, string tagName, params string[] contents)
        {
            if (value is XmlElementSyntax syntax && string.Equals(tagName, syntax.GetName(), StringComparison.OrdinalIgnoreCase))
            {
                var content = syntax.Content.ToString().AsSpan().Trim();

                return content.EqualsAny(contents);
            }

            return false;
        }

        private static bool IsBinaryCallTo(this BinaryExpressionSyntax value, string methodName)
        {
            if (value is null)
            {
                return false;
            }

            if (value.OperatorToken.IsKind(SyntaxKind.AmpersandAmpersandToken))
            {
                if (value.Left.IsCallTo(methodName) || value.Right.IsCallTo(methodName))
                {
                    return true;
                }

                // maybe it is a combined one
                if (value.Left is BinaryExpressionSyntax left && IsBinaryCallTo(left, methodName))
                {
                    return true;
                }

                // maybe it is a combined one
                if (value.Right is BinaryExpressionSyntax right && IsBinaryCallTo(right, methodName))
                {
                    return true;
                }
            }

            return false;
        }

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

                if (info.CandidateReason == CandidateReason.None)
                {
                    return info.Symbol.IsLinqExtensionMethod();
                }
                else
                {
                    return info.CandidateSymbols.Any(_ => _.IsLinqExtensionMethod());
                }
            }

            return false;
        }

        private static SyntaxTrivia[] CalculateWhitespaceTriviaWithComment(int count, SyntaxTrivia[] finalTrivia)
        {
            var triviaGroupedByLines = finalTrivia.GroupBy(_ => _.GetStartingLine());

            foreach (var triviaGroup in triviaGroupedByLines)
            {
                var trivia1 = triviaGroup.ElementAt(0);
                var index1 = finalTrivia.IndexOf(trivia1);

                if (trivia1.IsWhiteSpace())
                {
                    var spaces = count;

                    if (triviaGroup.MoreThan(1))
                    {
                        var trivia2 = triviaGroup.ElementAt(1);

                        if (trivia2.IsMultiLineComment())
                        {
                            var commentLength = trivia2.FullSpan.Length;
                            var remainingSpaces = triviaGroup.Skip(2).Sum(_ => _.FullSpan.Length);

                            spaces = count - commentLength - remainingSpaces;

                            if (spaces < 0)
                            {
                                // it seems we want to remove some spaces, so 'count' is already correct
                                spaces = count;
                            }
                        }
                    }

                    finalTrivia[index1] = SyntaxFactory.Whitespace(new string(' ', spaces));
                }
            }

            return finalTrivia;
        }

        private static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        private static XmlTextSyntax XmlText(SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

        private static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(SyntaxFactory.TokenList(textTokens));
    }
}