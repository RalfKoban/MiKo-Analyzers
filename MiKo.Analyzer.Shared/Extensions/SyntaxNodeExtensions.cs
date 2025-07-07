using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using MiKoSolutions.Analyzers.Linguistics;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides extensions for <see cref="SyntaxNode"/>s.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        internal static readonly SyntaxTrivia XmlCommentExterior = SyntaxFactory.DocumentationCommentExterior(Constants.Comments.XmlCommentExterior + " ");

        internal static readonly SyntaxTrivia[] XmlCommentStart =
                                                                  {
                                                                      SyntaxFactory.ElasticCarriageReturnLineFeed, // use elastic one to allow formatting to be done automatically
                                                                      XmlCommentExterior,
                                                                  };

        private static readonly string[] Booleans = { "true", "false", "True", "False", "TRUE", "FALSE" };

        private static readonly string[] Nulls = { "null", "Null", "NULL" };

        private static readonly SyntaxKind[] MethodNameSyntaxKinds = { SyntaxKind.MethodDeclaration, SyntaxKind.ConstructorDeclaration };

        private static readonly SyntaxList<TypeParameterConstraintClauseSyntax> Empty = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();

        private static readonly Func<SyntaxNode, bool> DescendIntoChildren = _ => true;

        internal static IEnumerable<SyntaxNode> AllDescendantNodes(this SyntaxNode value) => value?.DescendantNodes(DescendIntoChildren, true) ?? Array.Empty<SyntaxNode>();

        internal static IEnumerable<T> AllDescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.AllDescendantNodes().OfType<T>();

        internal static IEnumerable<SyntaxNodeOrToken> AllDescendantNodesAndTokens(this SyntaxNode value) => value.DescendantNodesAndTokens(DescendIntoChildren);

        internal static IEnumerable<T> Ancestors<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors().OfType<T>(); // value.AncestorsAndSelf().OfType<T>();

        internal static IEnumerable<T> AncestorsWithinMethods<T>(this SyntaxNode value) where T : SyntaxNode
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ancestor in value.Ancestors())
            {
                if (ancestor is T t)
                {
                    yield return t;
                }

                switch (ancestor)
                {
                    case BaseMethodDeclarationSyntax _: // found the surrounding method
                    case LocalFunctionStatementSyntax _: // found the surrounding local function
                    case BasePropertyDeclarationSyntax _: // found the surrounding property, so we already skipped the getters or setters
                        yield break;
                }
            }
        }

        internal static IEnumerable<T> AncestorsWithinDocumentation<T>(this SyntaxNode value) where T : XmlNodeSyntax
        {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ancestor in value.Ancestors())
            {
                if (ancestor is T t)
                {
                    yield return t;
                }

                if (ancestor is DocumentationCommentTriviaSyntax)
                {
                    yield break;
                }
            }
        }

        internal static bool Contains(this SyntaxNode value, in char c) => value?.ToString().Contains(c) ?? false;

        internal static IEnumerable<T> ChildNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes().OfType<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).Cast<T>();

        internal static IEnumerable<T> DescendantNodes<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().Where(predicate);

        internal static IEnumerable<SyntaxToken> DescendantTokens(this SyntaxNode value, in SyntaxKind kind) => value.DescendantTokens().OfKind(kind);

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

        internal static SyntaxToken FindToken<T>(this T value, Diagnostic issue) where T : SyntaxNode
        {
            var position = issue.Location.SourceSpan.Start;
            var token = value.FindToken(position, true);

            return token;
        }

        internal static T FirstAncestor<T>(this SyntaxNode value) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault();

        internal static T FirstAncestor<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.Ancestors<T>().FirstOrDefault(predicate);

        internal static T FirstAncestor<T>(this SyntaxNode value, ISet<SyntaxKind> kinds) where T : SyntaxNode => value.FirstAncestor<T>(_ => _.IsAnyKind(kinds));

        internal static SyntaxNode FirstChild(this SyntaxNode value) => value.ChildNodes().FirstOrDefault();

        internal static T FirstChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault();

        internal static SyntaxNode FirstChild(this SyntaxNode value, Func<SyntaxNode, bool> predicate) => value.ChildNodes().FirstOrDefault(predicate);

        internal static T FirstChild<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.ChildNodes().OfKind(kind).FirstOrDefault() as T;

        internal static T FirstChild<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.ChildNodes<T>().FirstOrDefault(predicate);

        internal static SyntaxToken FirstChildToken(this SyntaxNode value) => value.ChildTokens().FirstOrDefault();

        internal static SyntaxToken FirstChildToken(this SyntaxNode value, in SyntaxKind kind) => value.ChildTokens().OfKind(kind).First();

        internal static T FirstDescendant<T>(this SyntaxNode value) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault();

        internal static T FirstDescendant<T>(this SyntaxNode value, in SyntaxKind kind) where T : SyntaxNode => value.DescendantNodes().OfKind(kind).FirstOrDefault() as T;

        internal static T FirstDescendant<T>(this SyntaxNode value, Func<T, bool> predicate) where T : SyntaxNode => value.DescendantNodes<T>().FirstOrDefault(predicate);

        internal static SyntaxToken FirstDescendantToken(this SyntaxNode value) => value.DescendantTokens().FirstOrDefault();

        internal static SyntaxToken FirstDescendantToken(this SyntaxNode value, in SyntaxKind kind) => value.DescendantTokens().OfKind(kind).First();

        internal static T LastChild<T>(this SyntaxNode value) where T : SyntaxNode => value.ChildNodes<T>().LastOrDefault();

        internal static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(this TypeParameterConstraintClauseSyntax value)
        {
            switch (value.Parent)
            {
                case ClassDeclarationSyntax c: return c.ConstraintClauses;
                case InterfaceDeclarationSyntax i: return i.ConstraintClauses;
                case RecordDeclarationSyntax r: return r.ConstraintClauses;
                case StructDeclarationSyntax s: return s.ConstraintClauses;
                case MethodDeclarationSyntax b: return b.ConstraintClauses;
                case LocalFunctionStatementSyntax f: return f.ConstraintClauses;

                default:
                    return Empty;
            }
        }

        internal static SyntaxToken GetTypeParameterConstraintReferenceToken(this TypeParameterConstraintClauseSyntax value)
        {
            switch (value.Parent)
            {
//// ReSharper disable PossibleNullReferenceException Cannot be null as there is already a type parameter constraint
                case ClassDeclarationSyntax c: return c.TypeParameterList.GreaterThanToken;
                case InterfaceDeclarationSyntax i: return i.TypeParameterList.GreaterThanToken;
                case RecordDeclarationSyntax r: return r.TypeParameterList.GreaterThanToken;
                case StructDeclarationSyntax s: return s.TypeParameterList.GreaterThanToken;
//// ReSharper restore PossibleNullReferenceException

                case MethodDeclarationSyntax b: return b.ParameterList.CloseParenToken;
                case LocalFunctionStatementSyntax f: return f.ParameterList.CloseParenToken;

                default:
                    return default;
            }
        }

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
                for (int index = 0, listCount = list.Count; index < listCount; index++)
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

                    var offset = text.Length - text.AsSpan().TrimStart().Length;

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

                        var offset = text.Length - text.AsSpan().TrimEnd().Length;

                        end = token.Span.End - offset;
                    }
                }

                return end;
            }
        }

        internal static AccessorDeclarationSyntax GetGetter(this PropertyDeclarationSyntax value) => value?.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.GetAccessorDeclaration);

        internal static AccessorDeclarationSyntax GetSetter(this PropertyDeclarationSyntax value) => value?.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.SetAccessorDeclaration);

        internal static XmlTextAttributeSyntax GetNameAttribute(this SyntaxNode value)
        {
            switch (value)
            {
                case XmlElementSyntax e: return e.GetAttributes<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Name);
                case XmlEmptyElementSyntax ee: return ee.Attributes.OfType<XmlTextAttributeSyntax>().FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Name);
                default: return null;
            }
        }

        internal static string GetParameterName(this XmlElementSyntax value)
        {
            var list = value.GetAttributes<XmlNameAttributeSyntax>();

            return list.Count > 0
                   ? list[0].Identifier.GetName()
                   : null;
        }

        internal static string GetParameterName(this XmlEmptyElementSyntax value)
        {
            var attributes = value.Attributes;

            var count = attributes.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    if (attributes[index] is XmlNameAttributeSyntax syntax)
                    {
                        return syntax.Identifier.GetName();
                    }
                }
            }

            return null;
        }

        internal static XmlElementSyntax GetParameterComment(this DocumentationCommentTriviaSyntax value, string parameterName) => value.FirstDescendant<XmlElementSyntax>(_ => _.GetName() is Constants.XmlTag.Param && _.GetParameterName() == parameterName);

        internal static string GetIdentifierNameFromPropertyExpression(this PropertyDeclarationSyntax value)
        {
            var expression = value.GetPropertyExpression();

            return expression is IdentifierNameSyntax identifier
                   ? identifier.GetName()
                   : null;
        }

        internal static ExpressionSyntax GetPropertyExpression(this PropertyDeclarationSyntax value)
        {
            var expression = value.ExpressionBody?.Expression;

            if (expression != null)
            {
                return expression;
            }

            var accessorList = value.AccessorList;

            if (accessorList != null)
            {
                var getter = accessorList.Accessors[0];

                expression = getter.ExpressionBody?.Expression;

                if (expression != null)
                {
                    return expression;
                }

                if (getter.Body?.Statements.FirstOrDefault() is ReturnStatementSyntax r)
                {
                    return r.Expression;
                }
            }

            return null;
        }

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

        internal static SyntaxNode GetExceptionSwallowingNode(this ObjectCreationExpressionSyntax value, Func<SemanticModel> semanticModelCallback)
        {
            var catchClause = value.FirstAncestorOrSelf<CatchClauseSyntax>();

            if (catchClause != null)
            {
                // we found an exception inside a catch block that does not get the caught exception as inner exception
                return catchClause;
            }

            // inspect any 'if' or 'switch' or 'else if' to see if there is an exception involved
            var expression = value.GetRelatedCondition()?.FirstDescendant<ExpressionSyntax>(_ => _.GetTypeSymbol(semanticModelCallback())?.IsException() is true);

            if (expression != null)
            {
                return expression;
            }

            // inspect method arguments
            var parameter = value.GetEnclosing<MethodDeclarationSyntax>()?.ParameterList.Parameters.FirstOrDefault(_ => _.Type.IsException());

            return parameter;
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

        internal static IReadOnlyList<T> GetAttributes<T>(this XmlElementSyntax value) where T : XmlAttributeSyntax
        {
            return value?.StartTag.Attributes.OfType<XmlAttributeSyntax, T>() ?? Array.Empty<T>();
        }

        internal static IReadOnlyList<T> GetAttributes<T>(this XmlEmptyElementSyntax value) where T : XmlAttributeSyntax
        {
            return value?.Attributes.OfType<XmlAttributeSyntax, T>() ?? Array.Empty<T>();
        }

        internal static T GetEnclosing<T>(this SyntaxNode value) where T : SyntaxNode => value.FirstAncestorOrSelf<T>();

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

                node = node is DocumentationCommentTriviaSyntax d
                       ? d.ParentTrivia.Token.Parent
                       : node.Parent;
            }
        }

        internal static SyntaxNode GetEnclosing(this SyntaxNode value, in ReadOnlySpan<SyntaxKind> syntaxKinds)
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

                node = node is DocumentationCommentTriviaSyntax d
                       ? d.ParentTrivia.Token.Parent
                       : node.Parent;
            }
        }

        internal static IMethodSymbol GetEnclosingMethod(this in SyntaxNodeAnalysisContext value)
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
                case FieldDeclarationSyntax f: return semanticModel.GetDeclaredSymbol(f);
                case MethodDeclarationSyntax s: return semanticModel.GetDeclaredSymbol(s);
                case PropertyDeclarationSyntax p: return semanticModel.GetDeclaredSymbol(p);
                case ConstructorDeclarationSyntax c: return semanticModel.GetDeclaredSymbol(c);
                case EventDeclarationSyntax e: return semanticModel.GetDeclaredSymbol(e);
                default:
                    return semanticModel.GetEnclosingSymbol(value.GetLocation().SourceSpan.Start);
            }
        }

        internal static ExpressionSyntax GetIdentifierExpression(this ExpressionSyntax value)
        {
            switch (value)
            {
                case InvocationExpressionSyntax invocation:
                    return invocation.GetIdentifierExpression();

                case IdentifierNameSyntax identifier:
                    return identifier;

                default:
                    return null;
            }
        }

        internal static ExpressionSyntax GetIdentifierExpression(this InvocationExpressionSyntax value)
        {
            switch (value?.Expression)
            {
                case MemberAccessExpressionSyntax maes:
                    return maes.Expression;

                case MemberBindingExpressionSyntax mbes: // find parent conditional access expression as that contains the identifier
                    return mbes.FirstAncestor<ConditionalAccessExpressionSyntax>()?.Expression;

                default:
                    return null;
            }
        }

        internal static string GetIdentifierName(this ExpressionSyntax value) => value.GetIdentifierExpression().GetName();

        internal static string GetIdentifierName(this InvocationExpressionSyntax value) => value.GetIdentifierExpression().GetName();

        internal static SyntaxTrivia[] GetComment(this SyntaxNode value) => value.GetLeadingTrivia().Concat(value.GetTrailingTrivia()).Where(_ => _.IsComment()).ToArray();

        internal static SyntaxNode FindSyntaxNodeWithLeadingComment(this SyntaxNode value)
        {
            while (true)
            {
                if (value is null)
                {
                    return null;
                }

                var list = value.GetLeadingTrivia();

                // keep in local variable to avoid multiple requests (see Roslyn implementation)
                var listCount = list.Count;

                if (listCount > 0)
                {
                    for (var index = 0; index < listCount; index++)
                    {
                        var trivia = list[index];

                        if (trivia.IsComment())
                        {
                            return value;
                        }
                    }
                }

                value = value.FirstChild();
            }
        }

        internal static string[] GetLeadingComments(this SyntaxNode value)
        {
            if (value is null)
            {
                return Array.Empty<string>();
            }

            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count is 0)
            {
                return Array.Empty<string>();
            }

            return leadingTrivia.Where(_ => _.IsComment())
                                .Select(_ => _.ToTextOnlyString())
                                .Where(_ => _.Length > 0)
                                .ToArray();
        }

        internal static XmlTextAttributeSyntax GetListType(this XmlElementSyntax list) => list.GetAttributes<XmlTextAttributeSyntax>()
                                                                                              .FirstOrDefault(_ => _.GetName() is Constants.XmlTag.Attribute.Type);

        internal static string GetListType(this XmlTextAttributeSyntax listType) => listType.GetTextWithoutTrivia();

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

        internal static string GetName(this AccessorDeclarationSyntax value)
        {
            var syntaxNode = value.Parent?.Parent;

            switch (syntaxNode)
            {
                case BasePropertyDeclarationSyntax b:
                    return b.GetName();

                case EventFieldDeclarationSyntax ef:
                    return ef.GetName();
            }

            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ArgumentSyntax value) => value.Expression.GetName();

        internal static string GetName(this AttributeSyntax value)
        {
            switch (value.Name)
            {
                case SimpleNameSyntax s: return s.GetName();
                case QualifiedNameSyntax q: return q.Right.GetName();
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

        internal static string GetName(this BaseTypeDeclarationSyntax value)
        {
            switch (value)
            {
                case TypeDeclarationSyntax s: return s.GetName();
                case EnumDeclarationSyntax s: return s.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this BaseFieldDeclarationSyntax value)
        {
            switch (value)
            {
                case FieldDeclarationSyntax s: return s.Declaration.Variables.FirstOrDefault().GetName();
                case EventFieldDeclarationSyntax s: return s.Declaration.Variables.FirstOrDefault().GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this BasePropertyDeclarationSyntax value)
        {
            switch (value)
            {
                case PropertyDeclarationSyntax p: return p.GetName();
                case IndexerDeclarationSyntax i: return i.GetName();
                case EventDeclarationSyntax e: return e.GetName();
                default:
                    return string.Empty;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ClassDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ConstructorDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ConversionOperatorDeclarationSyntax value) => value?.OperatorKeyword.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this DestructorDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EnumDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EnumMemberDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this EventDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this ExpressionSyntax value)
        {
            switch (value)
            {
                case MemberAccessExpressionSyntax m: return m.GetName();
                case MemberBindingExpressionSyntax b: return b.GetName();
                case InvocationExpressionSyntax i: return i.GetName();
                case LiteralExpressionSyntax l: return l.GetName();
                case TypeSyntax t: return t.GetName();
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

                    if (text is "nameof" && value.Ancestors<MemberAccessExpressionSyntax>().None())
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
                    return m.GetName();
                }

                case MemberBindingExpressionSyntax b:
                {
                    return b.GetName();
                }
            }

            return string.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this IdentifierNameSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this IndexerDeclarationSyntax value) => value?.ThisKeyword.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this InterfaceDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this LiteralExpressionSyntax value) => value?.Token.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this LocalFunctionStatementSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MemberAccessExpressionSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MemberBindingExpressionSyntax value) => value?.Name.GetName();

        internal static string GetName(this MemberDeclarationSyntax value)
        {
            switch (value)
            {
                case BaseTypeDeclarationSyntax s: return s.GetName();
                case BaseMethodDeclarationSyntax s: return s.GetName();
                case BasePropertyDeclarationSyntax s: return s.GetName();
                case BaseFieldDeclarationSyntax s: return s.GetName();
                case EnumMemberDeclarationSyntax s: return s.GetName();
                default:
                    return string.Empty;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this MethodDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this NameColonSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this NameEqualsSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this OperatorDeclarationSyntax value) => value?.OperatorToken.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this ParameterSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this PropertyDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this RecordDeclarationSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this SimpleNameSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this StructDeclarationSyntax value) => value?.Identifier.ValueText;

        internal static string GetName(this TypeDeclarationSyntax value)
        {
            switch (value)
            {
                case ClassDeclarationSyntax s: return s.GetName();
                case InterfaceDeclarationSyntax s: return s.GetName();
                case RecordDeclarationSyntax s: return s.GetName();
                case StructDeclarationSyntax s: return s.GetName();
                default:
                    return string.Empty;
            }
        }

        internal static string GetName(this TypeSyntax value)
        {
            switch (value)
            {
                case null: return string.Empty;
                case IdentifierNameSyntax i: return i.GetName();
                case SimpleNameSyntax s: return s.GetName();
                default:
                    return value.ToString();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this UsingDirectiveSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this VariableDeclaratorSyntax value) => value?.Identifier.ValueText;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlAttributeSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementSyntax value) => value?.StartTag.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlEmptyElementSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementStartTagSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlElementEndTagSyntax value) => value?.Name.GetName();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetName(this XmlNameSyntax value) => value?.LocalName.ValueText;

        internal static IEnumerable<string> GetNames(this BaseFieldDeclarationSyntax value)
        {
            switch (value)
            {
                case FieldDeclarationSyntax s: return s.Declaration.Variables.GetNames();
                case EventFieldDeclarationSyntax s: return s.Declaration.Variables.GetNames();
                default:
                    return Array.Empty<string>();
            }
        }

        internal static IEnumerable<string> GetNames(this in SeparatedSyntaxList<VariableDeclaratorSyntax> value) => value.Select(_ => _.GetName());

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

                    case BaseTypeDeclarationSyntax _:
                        return Array.Empty<ParameterSyntax>();
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
                        return method.ParameterList.Parameters.ToArray(_ => _.GetName());

                    case IndexerDeclarationSyntax indexer:
                        return indexer.ParameterList.Parameters.ToArray(_ => _.GetName());

                    case BasePropertyDeclarationSyntax property:
                        return property.AccessorList?.Accessors.Any(_ => _.IsKind(SyntaxKind.SetAccessorDeclaration)) is true
                               ? Constants.Names.DefaultPropertyParameterNames
                               : Array.Empty<string>();

                    case BaseTypeDeclarationSyntax _:
                        return Array.Empty<string>();
                }
            }

            return Array.Empty<string>();
        }

        internal static int GetPositionWithinStartLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinStartLine();

        internal static int GetPositionWithinEndLine(this SyntaxNode value) => value.GetLocation().GetPositionWithinEndLine();

        internal static int GetStartingLine(this SyntaxNode value) => value.GetLocation().GetStartingLine();

        internal static int GetEndingLine(this SyntaxNode value) => value.GetLocation().GetEndingLine();

        internal static SyntaxToken GetSemicolonToken(this StatementSyntax statement)
        {
            switch (statement)
            {
                case LocalDeclarationStatementSyntax l: return l.SemicolonToken;
                case ExpressionStatementSyntax e: return e.SemicolonToken;
                case ReturnStatementSyntax r: return r.SemicolonToken;
                case ThrowStatementSyntax t: return t.SemicolonToken;

                default:
                    return default;
            }
        }

        internal static SyntaxToken GetSemicolonToken(this MemberDeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case BaseMethodDeclarationSyntax m: return m.SemicolonToken;
                case PropertyDeclarationSyntax p: return p.SemicolonToken;
                case EventDeclarationSyntax e: return e.SemicolonToken;
                case EventFieldDeclarationSyntax ef: return ef.SemicolonToken;
                case FieldDeclarationSyntax f: return f.SemicolonToken;

                default:
                    return default;
            }
        }

        internal static ISymbol GetSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            var symbolInfo = GetSymbolInfo();

            var symbol = symbolInfo.Symbol;

            if (symbol is null)
            {
                if (symbolInfo.CandidateReason is CandidateReason.OverloadResolutionFailure)
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

        internal static ISymbol GetSymbol(this SyntaxNode value, Compilation compilation) => value?.GetSymbol(compilation.GetSemanticModel(value.SyntaxTree));

        internal static IMethodSymbol GetSymbol(this LocalFunctionStatementSyntax value, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetDeclaredSymbol(value);

#if VS2022
            return symbol;
#else
            return symbol as IMethodSymbol;
#endif
        }

        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, Compilation compilation) => value?.GetTypeSymbol(compilation.GetSemanticModel(value.SyntaxTree));

        internal static ITypeSymbol GetTypeSymbol(this ArgumentSyntax value, SemanticModel semanticModel) => value?.Expression.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static ITypeSymbol GetTypeSymbol(this MemberAccessExpressionSyntax value, SemanticModel semanticModel) => value?.Expression.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this TypeSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static ITypeSymbol GetTypeSymbol(this BaseTypeSyntax value, SemanticModel semanticModel) => value?.Type.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this ClassDeclarationSyntax value, SemanticModel semanticModel) => value?.Identifier.GetSymbol(semanticModel) as ITypeSymbol;

        internal static ITypeSymbol GetTypeSymbol(this RecordDeclarationSyntax value, SemanticModel semanticModel) => value?.Identifier.GetSymbol(semanticModel) as ITypeSymbol;

        internal static ITypeSymbol GetTypeSymbol(this VariableDeclarationSyntax value, SemanticModel semanticModel) => value?.Type.GetTypeSymbol(semanticModel);

        internal static ITypeSymbol GetTypeSymbol(this VariableDesignationSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            if (semanticModel.GetDeclaredSymbol(value) is ILocalSymbol symbol)
            {
                return symbol.Type;
            }

            return null;
        }

        internal static ITypeSymbol GetTypeSymbol(this SyntaxNode value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return null;
            }

            var typeInfo = semanticModel.GetTypeInfo(value);

            return typeInfo.Type;
        }

        internal static LinePosition GetStartPosition(this SyntaxNode value) => value.GetLocation().GetStartPosition();

        internal static LinePosition GetEndPosition(this SyntaxNode value) => value.GetLocation().GetEndPosition();

        internal static bool HasDocumentationCommentTriviaSyntax(this SyntaxNode value)
        {
            var token = value.FindStructuredTriviaToken();

            return token.HasStructuredTrivia && token.HasDocumentationCommentTriviaSyntax();
        }

        internal static DocumentationCommentTriviaSyntax[] GetDocumentationCommentTriviaSyntax(this SyntaxNode value, in SyntaxKind kind = SyntaxKind.SingleLineDocumentationCommentTrivia)
        {
            var token = value.FindStructuredTriviaToken();

            if (token.HasStructuredTrivia)
            {
                var comment = token.GetDocumentationCommentTriviaSyntax(kind);

                if (comment != null)
                {
                    return comment;
                }
            }

            return Array.Empty<DocumentationCommentTriviaSyntax>();
        }

        internal static string GetTextTrimmedWithParaTags(this IReadOnlyList<SyntaxToken> values)
        {
            if (values.Count is 0)
            {
                return string.Empty;
            }

            var builder = StringBuilderCache.Acquire();

            GetTextWithoutTrivia(values, builder);

            var trimmed = builder.WithoutNewLines()
                                 .WithoutMultipleWhiteSpaces()
                                 .Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static string GetTextTrimmed(this XmlElementSyntax value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            var builder = StringBuilderCache.Acquire();

            var trimmed = value.GetTextWithoutTrivia(builder)
                               .WithoutParaTags()
                               .WithoutNewLines()
                               .WithoutMultipleWhiteSpaces()
                               .Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static string GetTextWithoutTrivia(this XmlTextAttributeSyntax value)
        {
            if (value is null)
            {
                return null;
            }

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return string.Empty;
            }

            var builder = StringBuilderCache.Acquire();

            GetTextWithoutTrivia(textTokens, builder);

            var trimmed = builder.Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static string GetTextWithoutTrivia(this XmlTextSyntax value)
        {
            if (value is null)
            {
                return null;
            }

            var builder = StringBuilderCache.Acquire();

            GetTextWithoutTrivia(value, builder);

            var trimmed = builder.Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static StringBuilder GetTextWithoutTrivia(this XmlTextSyntax value, StringBuilder builder)
        {
            if (value is null)
            {
                return builder;
            }

            return GetTextWithoutTrivia(value.TextTokens, builder);
        }

        internal static StringBuilder GetTextWithoutTrivia(IReadOnlyList<SyntaxToken> textTokens, StringBuilder builder)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount > 0)
            {
                for (var index = 0; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        continue;
                    }

                    builder.Append(token.ValueText);
                }
            }

            return builder;
        }

        internal static StringBuilder GetTextWithoutTrivia(this XmlElementSyntax value, StringBuilder builder)
        {
            if (value is null)
            {
                return builder;
            }

            var content = value.Content;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var contentCount = content.Count;

            if (contentCount > 0)
            {
                for (var index = 0; index < contentCount; index++)
                {
                    var node = content[index];
                    var tagName = node.GetXmlTagName();

                    switch (tagName)
                    {
                        case "i":
                        case "b":
                        case Constants.XmlTag.C:
                            continue; // ignore code
                    }

                    switch (node)
                    {
                        case XmlTextSyntax text:
                            GetTextWithoutTrivia(text, builder);

                            break;

                        case XmlEmptyElementSyntax empty:
                            builder.Append(empty.WithoutTrivia());

                            break;

                        case XmlElementSyntax e:
                            GetTextWithoutTrivia(e, builder);

                            break;
                    }
                }
            }

            return builder.WithoutXmlCommentExterior();
        }

        internal static IEnumerable<string> GetTextWithoutTriviaLazy(this XmlTextSyntax value)
        {
            if (value is null)
            {
                yield break;
            }

            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount > 0)
            {
                for (var index = 0; index < textTokensCount; index++)
                {
                    var token = textTokens[index];

                    if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        continue;
                    }

                    yield return token.ValueText;
                }
            }
        }

        internal static IReadOnlyList<XmlElementSyntax> GetExampleXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Example);

        internal static IReadOnlyList<XmlElementSyntax> GetExceptionXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Exception);

        internal static IReadOnlyList<XmlElementSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Summary);

        internal static IEnumerable<XmlNodeSyntax> GetSummaryXmls(this DocumentationCommentTriviaSyntax value, ISet<string> tags)
        {
            var summaryXmls = value.GetSummaryXmls();

            if (summaryXmls.Count is 0)
            {
                return Array.Empty<XmlNodeSyntax>();
            }

            return GetSummaryXmlsCore(summaryXmls, tags);
        }

        internal static IReadOnlyList<XmlElementSyntax> GetRemarksXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Remarks);

        internal static IReadOnlyList<XmlElementSyntax> GetReturnsXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Returns);

        internal static IReadOnlyList<XmlElementSyntax> GetValueXmls(this DocumentationCommentTriviaSyntax value) => value.GetXmlSyntax(Constants.XmlTag.Value);

        /// <summary>
        /// Gets only those XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
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
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,ISet{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,ISet{string})"/>
        internal static IReadOnlyList<XmlElementSyntax> GetXmlSyntax(this SyntaxNode value, string tag)
        {
            List<XmlElementSyntax> elements = null;

            foreach (var element in value.AllDescendantNodes<XmlElementSyntax>())
            {
                if (element.GetName() == tag)
                {
                    if (elements is null)
                    {
                        elements = new List<XmlElementSyntax>(1);
                    }

                    elements.Add(element);
                }
            }

            return (IReadOnlyList<XmlElementSyntax>)elements ?? Array.Empty<XmlElementSyntax>();
        }

        /// <summary>
        /// Gets only those XML elements that are NOT empty (have some content) and the given tag out of the documentation syntax.
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
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,ISet{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        internal static IReadOnlyList<XmlElementSyntax> GetXmlSyntax(this SyntaxNode value, ISet<string> tags)
        {
            // we have to delve into the trivia to find the XML syntax nodes
            List<XmlElementSyntax> elements = null;

            foreach (var element in value.AllDescendantNodes<XmlElementSyntax>())
            {
                if (tags.Contains(element.GetName()))
                {
                    if (elements is null)
                    {
                        elements = new List<XmlElementSyntax>(1);
                    }

                    elements.Add(element);
                }
            }

            return (IReadOnlyList<XmlElementSyntax>)elements ?? Array.Empty<XmlElementSyntax>();
        }

        /// <summary>
        /// Gets only those XML elements that are empty (have NO content) and the given tag out of the documentation syntax.
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
        /// <seealso cref="GetEmptyXmlSyntax(SyntaxNode,ISet{string})"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,string)"/>
        /// <seealso cref="GetXmlSyntax(SyntaxNode,ISet{string})"/>
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode value, string tag)
        {
            // we have to delve into the trivia to find the XML syntax nodes
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var element in value.AllDescendantNodes<XmlEmptyElementSyntax>())
            {
                if (element.GetName() == tag)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Gets only those XML elements that are empty (have NO content) and the given tag out of the list of syntax nodes.
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
        /// <seealso cref="GetXmlSyntax(SyntaxNode,ISet{string})"/>
        internal static IEnumerable<XmlEmptyElementSyntax> GetEmptyXmlSyntax(this SyntaxNode value, ISet<string> tags)
        {
            // we have to delve into the trivia to find the XML syntax nodes
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var element in value.AllDescendantNodes<XmlEmptyElementSyntax>())
            {
                if (tags.Contains(element.GetName()))
                {
                    yield return element;
                }
            }
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

        internal static string GetReferencedName(this SyntaxNode node)
        {
            var name = node.GetCref().GetCrefType().GetName();

            if (name.IsNullOrWhiteSpace())
            {
                var nameAttribute = node.GetNameAttribute();

                if (nameAttribute != null)
                {
                    name = nameAttribute.TextTokens[0].ValueText;
                }
            }

            return name;
        }

        internal static bool HasComment(this SyntaxNode value) => value.HasLeadingComment() || value.HasTrailingComment();

        internal static bool HasLeadingComment(this SyntaxNode value) => value.GetLeadingTrivia().HasComment();

        internal static bool HasTrailingComment(this SyntaxNode value) => value != null && value.GetTrailingTrivia().HasComment();

        internal static bool HasTrailingEndOfLine(this SyntaxNode value) => value != null && value.GetTrailingTrivia().HasEndOfLine();

        internal static bool HasMinimumCSharpVersion(this SyntaxTree value, LanguageVersion expectedVersion)
        {
            var languageVersion = ((CSharpParseOptions)value.Options).LanguageVersion;

            // ignore the latest versions (or above)
            return languageVersion >= expectedVersion && expectedVersion < LanguageVersion.LatestMajor;
        }

        internal static bool HasLinqExtensionMethod(this SyntaxNode value, SemanticModel semanticModel) => value.LinqExtensionMethods(semanticModel).Any();

#if VS2022

        internal static bool HasPrimaryConstructor(this ClassDeclarationSyntax value) => value.ParameterList != null;

        internal static bool HasPrimaryConstructor(this StructDeclarationSyntax value) => value.ParameterList != null;

#endif

        internal static bool HasPrimaryConstructor(this RecordDeclarationSyntax value) => value.ParameterList != null;

        internal static bool HasReturnValue(this ArrowExpressionClauseSyntax value) => value?.Parent is BaseMethodDeclarationSyntax method && method.HasReturnValue();

        internal static bool HasReturnValue(this BaseMethodDeclarationSyntax value)
        {
            switch (value)
            {
                case MethodDeclarationSyntax m: return m.ReturnType?.IsVoid() is false;
                case OperatorDeclarationSyntax o: return o.ReturnType?.IsVoid() is false;
                case ConversionOperatorDeclarationSyntax c: return c.Type?.IsVoid() is false;

                default:
                    return false;
            }
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsKind(this SyntaxNode value, in SyntaxKind kind) => value?.RawKind == (int)kind;

        internal static bool IsAnyKind(this SyntaxNode value, ISet<SyntaxKind> kinds) => kinds.Contains(value.Kind());

        internal static bool IsAnyKind(this SyntaxNode value, in ReadOnlySpan<SyntaxKind> kinds)
        {
            var kindsLength = kinds.Length;

            if (kindsLength > 0)
            {
                var valueKind = value.RawKind;

                if (kindsLength is 2)
                {
                    return valueKind == (int)kinds[0] || valueKind == (int)kinds[1];
                }

                for (var index = 0; index < kindsLength; index++)
                {
                    if (valueKind == (int)kinds[index])
                    {
                        return true;
                    }
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

            switch (value?.ToString())
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

            switch (value?.ToString())
            {
                case nameof(Byte):
                case nameof(System) + "." + nameof(Byte):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsGuid(this TypeSyntax value)
        {
            switch (value?.ToString())
            {
                case nameof(Guid):
                case nameof(Guid) + "?":
                case nameof(System) + "." + nameof(Guid):
                case nameof(System) + "." + nameof(Guid) + "?":
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsOnSameLineAs(this SyntaxNode value, SyntaxNode other) => value?.GetStartingLine() == other?.GetStartingLine();

        internal static bool IsOnSameLineAs(this SyntaxNode value, in SyntaxToken other) => value?.GetStartingLine() == other.GetStartingLine();

        internal static bool IsOnSameLineAs(this SyntaxNode value, in SyntaxNodeOrToken other) => value?.GetStartingLine() == other.GetStartingLine();

        internal static bool IsInside(this SyntaxNode value, ISet<SyntaxKind> kinds)
        {
            foreach (var ancestor in value.Ancestors())
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
                    case SyntaxKind.InterfaceDeclaration:
                        return false;
                }
            }

            return false;
        }

        internal static bool IsOnlyNodeInsideRegion(this SyntaxNode value)
        {
            if (value.TryGetRegionDirective(out var regionTrivia))
            {
                var relatedDirectives = regionTrivia.GetRelatedDirectives();

                if (relatedDirectives.Count is 2)
                {
                    var endRegionTrivia = relatedDirectives[1];

                    var otherSyntaxNode = endRegionTrivia.ParentTrivia.Token.Parent;

                    if (otherSyntaxNode != null)
                    {
                        if (otherSyntaxNode.IsEquivalentTo(value.NextSibling()))
                        {
                            return true;
                        }

                        if (otherSyntaxNode.IsEquivalentTo(value.Parent))
                        {
                            // seems like same type
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal static bool IsMoqItIsConditionMatcher(this InvocationExpressionSyntax value) => value.Expression is MemberAccessExpressionSyntax maes
                                                                                              && maes.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                                                              && maes.Expression is IdentifierNameSyntax invokedType
                                                                                              && invokedType.GetName() is Constants.Moq.ConditionMatcher.It
                                                                                              && maes.GetName() is Constants.Moq.ConditionMatcher.Is;

        internal static bool IsInsideMoqCall(this MemberAccessExpressionSyntax value)
        {
            if (value.Parent is InvocationExpressionSyntax i && i.Parent is LambdaExpressionSyntax lambda)
            {
                return IsMoqCall(lambda);
            }

            return false;
        }

        internal static bool IsMoqCall(this LambdaExpressionSyntax value)
        {
            if (value.Parent is ArgumentSyntax a && a.Parent?.Parent is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax m)
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
                    case Constants.Moq.Of when m.Expression is IdentifierNameSyntax ins && ins.GetName() is Constants.Moq.Mock:
                    {
                        // here we assume that we have a Moq call
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool TryGetMoqTypes(this MemberAccessExpressionSyntax value, out TypeSyntax[] result)
        {
            result = null;

            if (value.GetName() is Constants.Moq.Object)
            {
                var expression = value.Expression.WithoutParenthesis(); // let's see if we can fix it in case we remove the surrounding parenthesis

                if (expression is ObjectCreationExpressionSyntax o && o.Type.GetNameOnlyPartWithoutGeneric() is Constants.Moq.Mock && o.Type is GenericNameSyntax genericName)
                {
                    result = genericName.TypeArgumentList.Arguments.ToArray();
                }
            }

            return result != null;
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

        internal static bool IsCode(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.Code;

        internal static bool IsAssignmentOf(this StatementSyntax value, string identifierName) => value is ExpressionStatementSyntax e
                                                                                               && e.Expression is AssignmentExpressionSyntax a
                                                                                               && a.IsKind(SyntaxKind.SimpleAssignmentExpression)
                                                                                               && a.Left is IdentifierNameSyntax i
                                                                                               && i.GetName() == identifierName;

        internal static bool IsCommand(this TypeSyntax value, SemanticModel semanticModel)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var name = value.ToString();

            if (name.Contains(Constants.Names.Command) && name.Contains("CommandManager") is false)
            {
                return semanticModel.LookupSymbols(value.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol && symbol.IsCommand();
            }

            return false;
        }

        internal static bool IsConst(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.ConstKeyword);

        internal static bool IsConst(this SyntaxNode value, in SyntaxNodeAnalysisContext context)
        {
            switch (value)
            {
                case IdentifierNameSyntax i: return i.IsConst(context);
                case MemberAccessExpressionSyntax m: return m.IsConst(context.SemanticModel);

                default:
                    return false;
            }
        }

        internal static bool IsConst(this IdentifierNameSyntax value, ITypeSymbol type)
        {
            var isConst = type.GetFields(value.GetName()).Any(_ => _.IsConst);

            return isConst;
        }

        internal static bool IsConst(this IdentifierNameSyntax value, in SyntaxNodeAnalysisContext context) => value.IsConst(context.FindContainingType());

        internal static bool IsConst(this MemberAccessExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var type = value.GetTypeSymbol(semanticModel);

                if (type is null)
                {
                    // we do not know, so we assume it's not
                    return false;
                }

                if (type.IsEnum())
                {
                    // only get the real enum members, no local variables or something
                    return true;
                }

                if (value.Name is IdentifierNameSyntax identifierName)
                {
                    // find out whether the identifier is a const field
                    return identifierName.IsConst(type);
                }
            }

            return false;
        }

        internal static bool IsEnum(this IsPatternExpressionSyntax value, SemanticModel semanticModel) => value.Expression.IsEnum(semanticModel);

        internal static bool IsEnum(this MemberAccessExpressionSyntax value, SemanticModel semanticModel) => value.Expression.IsEnum(semanticModel);

        internal static bool IsEnum(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            if (value is MemberAccessExpressionSyntax maes)
            {
                return maes.IsEnum(semanticModel);
            }

            var type = value.GetTypeSymbol(semanticModel);

            return type.IsEnum();
        }

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

        internal static bool IsException(this XmlElementSyntax value) => value.GetName() is Constants.XmlTag.Exception;

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
            var list = value.GetAttributes<XmlCrefAttributeSyntax>();

            if (list.Count > 0)
            {
                var type = list[0].GetCrefType();

                return type != null && type.IsException(exceptionType);
            }

            return false;
        }

        internal static bool IsExpressionTree(this SyntaxNode value, SemanticModel semanticModel)
        {
            foreach (var a in value.AncestorsWithinMethods<ArgumentSyntax>())
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
                            // we are in the else part, not inside the 'if' part, so we fail
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

        internal static bool IsLocalVariableDeclaration(this SyntaxNode value, ISet<string> identifierNames) => value is LocalDeclarationStatementSyntax l && l.Declaration.Variables.Any(__ => identifierNames.Contains(__.Identifier.ValueText));

        internal static bool IsLocalVariableDeclaration(this SyntaxNode value, string identifierName) => value is LocalDeclarationStatementSyntax l && l.Declaration.Variables.Any(__ => __.Identifier.ValueText == identifierName);

        internal static bool IsFieldVariableDeclaration(this SyntaxNode value, ISet<string> identifierNames) => value is FieldDeclarationSyntax f && f.Declaration.Variables.Any(__ => identifierNames.Contains(__.Identifier.ValueText));

        internal static bool IsFieldVariableDeclaration(this SyntaxNode value, string identifierName) => value is FieldDeclarationSyntax f && f.Declaration.Variables.Any(__ => __.Identifier.ValueText == identifierName);

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

        internal static bool IsPatternCheckFor(this IsPatternExpressionSyntax value, in SyntaxKind kind) => value?.Pattern.IsPatternCheckFor(kind) is true;

        internal static bool IsPatternCheckFor(this PatternSyntax value, in SyntaxKind kind)
        {
            while (true)
            {
                switch (value)
                {
                    case ConstantPatternSyntax pattern:
                        return pattern.Expression.IsKind(kind);

                    case UnaryPatternSyntax unary:
                        value = unary.Pattern;

                        continue;

                    default:
                        return false;
                }
            }
        }

        internal static bool IsReadOnly(this FieldDeclarationSyntax value) => value.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);

        internal static bool IsSpanningMultipleLines(this SyntaxNode value)
        {
            var lineSpan = value.GetLocation().GetLineSpan();

            var startingLine = lineSpan.StartLinePosition.Line;
            var endingLine = lineSpan.EndLinePosition.Line;

            return startingLine != endingLine;
        }

        internal static bool IsSeeLangword(this SyntaxNode value)
        {
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() is Constants.XmlTag.See)
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
            if (value is XmlEmptyElementSyntax syntax && syntax.GetName() is Constants.XmlTag.See)
            {
                var attribute = syntax.Attributes.FirstOrDefault();

                switch (attribute?.GetName())
                {
                    case Constants.XmlTag.Attribute.Langword:
                    case Constants.XmlTag.Attribute.Langref:
                    {
                        foreach (var token in attribute.DescendantTokens())
                        {
                            var tokenValueText = token.ValueText;

                            if ("true".Equals(tokenValueText, StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }

                            if ("false".Equals(tokenValueText, StringComparison.OrdinalIgnoreCase))
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
            if (value.IsKind(SyntaxKind.AddExpression))
            {
                var b = (BinaryExpressionSyntax)value;

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

        internal static bool IsStringFormat(this InvocationExpressionSyntax value) => value.Expression is MemberAccessExpressionSyntax maes
                                                                                   && maes.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                                                                                   && maes.Expression is TypeSyntax invokedType
                                                                                   && invokedType.IsString()
                                                                                   && maes.GetName() == nameof(string.Format);

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

        internal static bool IsXml(this SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.XmlElement:
                case SyntaxKind.XmlEmptyElement:
                    return true;

                default:
                    return false;
            }
        }

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

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes<InvocationExpressionSyntax>(_ => IsLinqExtensionMethod(_, semanticModel));

        internal static IReadOnlyList<TResult> OfKind<TResult, TSyntaxNode>(this in SeparatedSyntaxList<TSyntaxNode> source, in SyntaxKind kind) where TSyntaxNode : SyntaxNode
                                                                                                                                                 where TResult : TSyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
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

        internal static IReadOnlyList<T> OfKind<T>(this IReadOnlyList<T> source, in SyntaxKind kind) where T : SyntaxNode
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
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

        internal static IEnumerable<T> OfKind<T>(this IEnumerable<T> source, in SyntaxKind kind) where T : SyntaxNode
        {
            if (source is IReadOnlyList<T> list)
            {
                return list.OfKind(kind);
            }

            return OfKindLocal(kind);

            IEnumerable<T> OfKindLocal(SyntaxKind itemKind)
            {
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var item in source)
                {
                    if (item.IsKind(itemKind))
                    {
                        yield return item;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<TResult> OfType<TResult>(this in SyntaxList<XmlNodeSyntax> source) where TResult : XmlNodeSyntax => source.OfType<XmlNodeSyntax, TResult>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IReadOnlyList<TResult> OfType<TResult>(this in SyntaxList<XmlAttributeSyntax> source) where TResult : XmlAttributeSyntax => source.OfType<XmlAttributeSyntax, TResult>();

        internal static IReadOnlyList<TResult> OfType<T, TResult>(this in SyntaxList<T> source) where T : SyntaxNode
                                                                                                where TResult : T
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return Array.Empty<TResult>();
            }

            List<TResult> results = null;

            for (var index = 0; index < sourceCount; index++)
            {
                if (source[index] is TResult result)
                {
                    if (results is null)
                    {
                        results = new List<TResult>(1);
                    }

                    results.Add(result);
                }
            }

            return results ?? (IReadOnlyList<TResult>)Array.Empty<TResult>();
        }

        internal static T RemoveTrivia<T>(this T node, in SyntaxTrivia trivia) where T : SyntaxNode => node.ReplaceTrivia(trivia, SyntaxFactory.ElasticMarker);

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

            var result = value.ReplaceNodes(nodes, (_, rewritten) => rewritten.WithAnnotation(annotation));

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

        internal static SyntaxList<XmlNodeSyntax> ReplaceText(this in SyntaxList<XmlNodeSyntax> source, string phrase, string replacement)
        {
            return source.ReplaceText(new[] { phrase }, replacement);
        }

        internal static SyntaxList<XmlNodeSyntax> ReplaceText(this in SyntaxList<XmlNodeSyntax> source, in ReadOnlySpan<string> phrases, string replacement)
        {
            var sourceCount = source.Count;

            if (sourceCount is 0)
            {
                return source;
            }

            var result = source.ToArray();

            for (var index = 0; index < sourceCount; index++)
            {
                var value = result[index];

                if (value is XmlTextSyntax text)
                {
                    result[index] = text.ReplaceText(phrases, replacement);
                }
            }

            return result.ToSyntaxList();
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, string phrase, string replacement)
        {
            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return value;
            }

            var map = new Dictionary<SyntaxToken, SyntaxToken>(1);

            for (var index = 0; index < textTokensCount; index++)
            {
                var token = textTokens[index];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var replaced = false;

                var result = text;

                if (text.Contains(phrase))
                {
                    result = result.AsCachedBuilder().ReplaceWithProbe(phrase, replacement).ToStringAndRelease();

                    replaced = true;
                }

                if (replaced)
                {
                    map[token] = token.WithText(result);
                }
            }

            if (map.Count is 0)
            {
                return value;
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static XmlTextSyntax ReplaceText(this XmlTextSyntax value, in ReadOnlySpan<string> phrases, string replacement)
        {
            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return value;
            }

            var phrasesLength = phrases.Length;

            var map = new Dictionary<SyntaxToken, SyntaxToken>(1);

            for (var tokenIndex = 0; tokenIndex < textTokensCount; tokenIndex++)
            {
                var token = textTokens[tokenIndex];

                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var text = token.ValueText;

                var replaced = false;

                var result = text.AsCachedBuilder();

                for (var phraseIndex = 0; phraseIndex < phrasesLength; phraseIndex++)
                {
                    var phrase = phrases[phraseIndex];

                    if (text.Contains(phrase))
                    {
                        result.ReplaceWithProbe(phrase, replacement);

                        replaced = true;
                    }
                }

                if (replaced)
                {
                    map[token] = token.WithText(result.ToStringAndRelease());
                }
                else
                {
                    StringBuilderCache.Release(result);
                }
            }

            if (map.Count is 0)
            {
                return value;
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static XmlTextSyntax ReplaceFirstText(this XmlTextSyntax value, string phrase, string replacement)
        {
            var textTokens = value.TextTokens;

            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var textTokensCount = textTokens.Count;

            if (textTokensCount is 0)
            {
                return value;
            }

            var map = new Dictionary<SyntaxToken, SyntaxToken>(1);

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
                    var result = text.AsSpan(0, index).ConcatenatedWith(replacement, text.AsSpan(index + phrase.Length));

                    map[token] = token.WithText(result);
                }
            }

            if (map.Count is 0)
            {
                return value;
            }

            return value.ReplaceTokens(map.Keys, (original, rewritten) => map[original]);
        }

        internal static bool ReturnsImmediately(this IfStatementSyntax value)
        {
            switch (value?.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool ReturnsImmediately(this ElseClauseSyntax value)
        {
            switch (value?.Statement)
            {
                case ReturnStatementSyntax _:
                case BlockSyntax block when block.Statements.FirstOrDefault() is ReturnStatementSyntax:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool ReturnsCompletedTask(this ReturnStatementSyntax value) => value.Expression is MemberAccessExpressionSyntax maes && maes.Expression.GetName() == nameof(Task) && maes.GetName() == nameof(Task.CompletedTask);

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

        internal static bool HasRegionDirective(this SyntaxNode value) => value != null && value.HasStructuredTrivia && value.GetLeadingTrivia().Any(SyntaxKind.RegionDirectiveTrivia);

        internal static bool TryGetRegionDirective(this SyntaxNode source, out DirectiveTriviaSyntax regionDirective)
        {
            if (source != null && source.HasStructuredTrivia)
            {
                var leadingTrivia = source.GetLeadingTrivia();

                var count = leadingTrivia.Count;

                if (count > 0)
                {
                    // we want the innermost region, so we traverse backwards here
                    for (var index = count - 1; index >= 0; index--)
                    {
                        var t = leadingTrivia[index];

                        if (t.IsKind(SyntaxKind.RegionDirectiveTrivia))
                        {
                            regionDirective = t.GetStructure() as DirectiveTriviaSyntax;

                            return true;
                        }
                    }
                }
            }

            regionDirective = null;

            return false;
        }

        internal static string ToCleanedUpString(this ExpressionSyntax source) => source?.ToString().Without(Constants.WhiteSpaces);

        internal static T WithAnnotation<T>(this T value, SyntaxAnnotation annotation) where T : SyntaxNode => value.WithAdditionalAnnotations(annotation);

        internal static T WithAdditionalLeadingTrivia<T>(this T value, in SyntaxTriviaList trivia) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivia));
        }

        internal static T WithAdditionalLeadingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().Add(trivia));
        }

        internal static T WithAdditionalLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
        {
            return value.WithLeadingTrivia(value.GetLeadingTrivia().AddRange(trivia));
        }

        internal static T WithAdditionalTrailingTrivia<T>(this T value, in SyntaxTriviaList trivia) where T : SyntaxNode
        {
            return value.WithTrailingTrivia(value.GetTrailingTrivia().AddRange(trivia));
        }

        internal static T WithAdditionalTrailingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
        {
            return value.WithTrailingTrivia(value.GetTrailingTrivia().Add(trivia));
        }

        internal static T WithAdditionalTrailingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
        {
            return value.WithTrailingTrivia(value.GetTrailingTrivia().AddRange(trivia));
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

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(contents.ToSyntaxList());

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, IEnumerable<XmlNodeSyntax> contents) => value.WithContent(contents.ToSyntaxList());

        internal static DocumentationCommentTriviaSyntax WithContent(this DocumentationCommentTriviaSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(contents.ToSyntaxList());

        internal static XmlElementSyntax WithContent(this XmlElementSyntax value, params XmlNodeSyntax[] contents) => value.WithContent(contents.ToSyntaxList());

        internal static T WithEndOfLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithFirstLeadingTrivia<T>(this T value, in SyntaxTrivia trivia) where T : SyntaxNode
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

        internal static T WithFirstLeadingTrivia<T>(this T value, params SyntaxTrivia[] trivia) where T : SyntaxNode
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

                return value.WithLeadingTrivia(leadingTrivia.InsertRange(0, trivia));
            }

            return value.WithLeadingTrivia(trivia);
        }

        internal static T WithIndentation<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticMarker); // use elastic one to allow formatting to be done automatically

        internal static SyntaxList<T> WithIndentation<T>(this in SyntaxList<T> values) where T : SyntaxNode
        {
            var value = values[0];

            return values.Replace(value, value.WithIndentation());
        }

        internal static T WithAdditionalLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithAdditionalLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static bool HasLeadingEmptyLine(this SyntaxNode value)
        {
            var trivia = value.GetLeadingTrivia();

            if (trivia.Count > 1)
            {
                if (trivia[0].IsEndOfLine())
                {
                    return true;
                }

                if (trivia[0].IsWhiteSpace() && trivia.Count > 1 && trivia[1].IsEndOfLine())
                {
                    return true;
                }
            }

            return false;
        }

        internal static T WithLeadingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use elastic one to prevent formatting it away again

        internal static T WithLeadingEndOfLine<T>(this T value) where T : SyntaxNode => value.WithFirstLeadingTrivia(SyntaxFactory.ElasticCarriageReturnLineFeed); // use elastic one to allow formatting to be done automatically

        internal static T WithLeadingSpace<T>(this T value) where T : SyntaxNode => value.WithLeadingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithTrailingSpace<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.ElasticSpace); // use elastic one to allow formatting to be done automatically

        internal static T WithAdditionalLeadingSpaces<T>(this T value, in int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            var currentSpaces = value.GetPositionWithinStartLine();

            return value.WithLeadingSpaces(currentSpaces + additionalSpaces);
        }

        internal static T WithAdditionalLeadingSpacesAtEnd<T>(this T value, in int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            return value.WithAdditionalLeadingTrivia(WhiteSpaces(additionalSpaces));
        }

        internal static T WithAdditionalLeadingSpacesOnDescendants<T>(this T value, IReadOnlyCollection<SyntaxNodeOrToken> descendants, int additionalSpaces) where T : SyntaxNode
        {
            if (additionalSpaces is 0)
            {
                return value;
            }

            if (descendants.Count is 0)
            {
                return value;
            }

            return value.ReplaceSyntax(
                                   descendants.Where(_ => _.IsNode).Select(_ => _.AsNode()),
                                   (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                   descendants.Where(_ => _.IsToken).Select(_ => _.AsToken()),
                                   (original, rewritten) => rewritten.WithAdditionalLeadingSpaces(additionalSpaces),
                                   Array.Empty<SyntaxTrivia>(),
                                   (original, rewritten) => rewritten);
        }

        internal static T WithLeadingSpaces<T>(this T value, in int count) where T : SyntaxNode
        {
            if (count <= 0)
            {
                return value;
            }

            var leadingTrivia = value.GetLeadingTrivia();

            if (leadingTrivia.Count is 0)
            {
                return value.WithLeadingTrivia(WhiteSpaces(count));
            }

            // re-construct leading comment with correct amount of spaces but keep comments
            // (so we have to find out each white-space trivia and have to replace it with the correct amount of spaces)
            var finalTrivia = leadingTrivia.ToArray();

            var resetFinalTrivia = false;

            for (int index = 0, length = finalTrivia.Length; index < length; index++)
            {
                var trivia = finalTrivia[index];

                if (trivia.IsWhiteSpace())
                {
                    finalTrivia[index] = WhiteSpaces(count);
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

        internal static SyntaxList<XmlNodeSyntax> WithLeadingXmlComment(this in SyntaxList<XmlNodeSyntax> values)
        {
            var value = values[0];

            return values.Replace(value, value.WithoutLeadingTrivia().WithLeadingXmlComment());
        }

        internal static SyntaxNode WithModifiers(this FieldDeclarationSyntax value, IEnumerable<SyntaxKind> modifiers)
        {
            var oldModifiers = value.Modifiers;

            var newModifiers = modifiers.ToTokenList();
            var modifier = newModifiers[0];

            if (oldModifiers.Count > 0)
            {
                // keep comments
                newModifiers = newModifiers.Replace(modifier, modifier.WithTriviaFrom(oldModifiers[0]));

                return value.WithModifiers(newModifiers);
            }

            var declaration = value.Declaration;
            var type = declaration.Type;

            // keep comments
            newModifiers = newModifiers.Replace(modifier, modifier.WithLeadingTriviaFrom(type));

            return value.WithModifiers(newModifiers)
                        .WithDeclaration(declaration.WithType(type.WithLeadingSpace()));
        }

        internal static SyntaxNode WithModifiers(this FieldDeclarationSyntax value, params SyntaxKind[] modifiers) => value.WithModifiers((IEnumerable<SyntaxKind>)modifiers);

        internal static T WithAdditionalModifier<T>(this T value, in SyntaxKind keyword) where T : MemberDeclarationSyntax
        {
            var modifiers = value.Modifiers;
            var position = modifiers.IndexOf(SyntaxKind.PartialKeyword);

            var syntaxToken = keyword.AsToken();

            if (modifiers.Count is 0)
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

        internal static T Without<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var removeOptions = node is DocumentationCommentTriviaSyntax
                                ? SyntaxRemoveOptions.AddElasticMarker
                                : SyntaxRemoveOptions.KeepNoTrivia;

            return value.RemoveNode(node, removeOptions);
        }

        internal static T Without<T>(this T value, IEnumerable<SyntaxNode> nodes) where T : SyntaxNode => value.RemoveNodes(nodes, SyntaxRemoveOptions.KeepNoTrivia);

        internal static T Without<T>(this T value, IEnumerable<SyntaxTrivia> trivia) where T : SyntaxNode => value.ReplaceTrivia(trivia, (original, rewritten) => default);

        internal static T Without<T>(this T value, params SyntaxNode[] nodes) where T : SyntaxNode => value.Without((IEnumerable<SyntaxNode>)nodes);

        internal static SyntaxList<XmlNodeSyntax> WithoutFirstXmlNewLine(this in SyntaxList<XmlNodeSyntax> values)
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

        internal static ExpressionSyntax WithoutParenthesis(this ExpressionSyntax value)
        {
            while (true)
            {
                if (value is ParenthesizedExpressionSyntax parenthesized)
                {
                    value = parenthesized.Expression;

                    continue;
                }

                return value;
            }
        }

        internal static T WithTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode => value.WithLeadingTriviaFrom(node)
                                                                                                        .WithTrailingTriviaFrom(node);

        internal static T WithTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode => value.WithLeadingTriviaFrom(token)
                                                                                                             .WithTrailingTriviaFrom(token);

        internal static T WithAdditionalLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithAdditionalLeadingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.LeadingTrivia;

            return trivia.Count > 0
                   ? value.WithAdditionalLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithLeadingTriviaFrom<T>(this T value, SyntaxNode node) where T : SyntaxNode
        {
            var trivia = node.GetLeadingTrivia();

            return trivia.Count > 0
                   ? value.WithLeadingTrivia(trivia)
                   : value;
        }

        internal static T WithLeadingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
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

        internal static T WithTrailingTriviaFrom<T>(this T value, in SyntaxToken token) where T : SyntaxNode
        {
            var trivia = token.TrailingTrivia;

            return trivia.Count > 0
                   ? value.WithTrailingTrivia(trivia)
                   : value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingTrivia(this in SyntaxList<XmlNodeSyntax> values)
        {
            var value = values[0];

            return values.Replace(value, value.WithoutLeadingTrivia());
        }

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

        internal static SyntaxList<XmlNodeSyntax> WithoutLeadingXmlComment(this in SyntaxList<XmlNodeSyntax> value)
        {
            if (value.FirstOrDefault() is XmlTextSyntax text)
            {
                var replacement = text.WithoutLeadingXmlComment();

                // ensure that we have some text tokens
                if (replacement.TextTokens.Count > 0)
                {
                    return value.Replace(text, replacement.WithoutLeadingTrivia());
                }

                // remove text as no tokens remain
                return value.Remove(text);
            }

            return value;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, string text) => value.Content.WithoutText(text);

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this in SyntaxList<XmlNodeSyntax> values, string text)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            var valuesCount = values.Count;

            if (valuesCount is 0)
            {
                return values;
            }

            var contents = values.ToList();

            for (var index = 0; index < valuesCount; index++)
            {
                if (values[index] is XmlTextSyntax s)
                {
                    var originalTextTokens = s.TextTokens;

                    // keep in local variable to avoid multiple requests (see Roslyn implementation)
                    var originalTextTokensCount = originalTextTokens.Count;

                    if (originalTextTokensCount is 0)
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
                            var modifiedText = token.Text
                                                    .AsCachedBuilder()
                                                    .Without(text)
                                                    .WithoutMultipleWhiteSpaces()
                                                    .ToStringAndRelease();

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

            return contents.ToSyntaxList();
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this XmlElementSyntax value, in ReadOnlySpan<string> texts) => value.Content.WithoutText(texts);

        internal static SyntaxList<XmlNodeSyntax> WithoutText(this in SyntaxList<XmlNodeSyntax> values, in ReadOnlySpan<string> texts)
        {
            var length = texts.Length;

            if (length <= 0)
            {
                return values;
            }

            var result = values;

            for (var index = 0; index < length; index++)
            {
                result = result.WithoutText(texts[index]);
            }

            return result;
        }

        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, string text) => value.WithoutTrailing(new[] { text });

        internal static XmlTextSyntax WithoutTrailing(this XmlTextSyntax value, in ReadOnlySpan<string> texts)
        {
            var textTokens = value.TextTokens.ToArray();

            var replaced = false;

            for (var i = textTokens.Length - 1; i >= 0; i--)
            {
                var token = textTokens[i];

                // ignore trivia such as " /// "
                if (token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    continue;
                }

                var originalText = token.Text.AsSpan();

                if (originalText.IsNullOrWhiteSpace())
                {
                    continue;
                }

                for (int textIndex = 0, textsLength = texts.Length; textIndex < textsLength; textIndex++)
                {
                    var text = texts[textIndex];

                    if (originalText.EndsWith(text, StringComparison.OrdinalIgnoreCase))
                    {
                        var modifiedText = originalText.WithoutSuffix(text, StringComparison.OrdinalIgnoreCase);

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

        internal static XmlTextSyntax WithoutTrailingCharacters(this XmlTextSyntax value, in ReadOnlySpan<char> characters)
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

                var originalText = token.Text.AsSpan();

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

        internal static T WithoutTrailingSpaces<T>(this T value) where T : SyntaxNode
        {
            var trivia = value.GetTrailingTrivia();
            var triviaCount = trivia.Count;

            if (triviaCount <= 0)
            {
                return value;
            }

            var i = triviaCount - 1;

            for (; i > -1; i--)
            {
                if (trivia[i].IsKind(SyntaxKind.WhitespaceTrivia) is false)
                {
                    break;
                }
            }

            return value.WithTrailingTrivia(i > 0 ? trivia.Take(i) : SyntaxTriviaList.Empty);
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutTrailingXmlComment(this in SyntaxList<XmlNodeSyntax> value)
        {
            if (value.LastOrDefault() is XmlTextSyntax text)
            {
                var replacement = text.WithoutTrailingXmlComment();

                // ensure that we have some text tokens
                if (replacement.TextTokens.Count > 0)
                {
                    return value.Replace(text, replacement);
                }

                // remove text as no tokens remain
                return value.Remove(text);
            }

            return value;
        }

        internal static XmlTextSyntax WithoutTrailingXmlComment(this XmlTextSyntax value)
        {
            var tokens = value.TextTokens;

            switch (tokens.Count)
            {
                case 0:
                    return value;

                case 1:
                    var token = tokens[0];

                    if (token.HasTrailingTrivia)
                    {
                        return value.WithTextTokens(tokens.Replace(token, token.WithoutTrailingTrivia()));
                    }

                    return value;

                default:
                {
                    // remove last "\r\n" token and remove '  /// ' trivia of last token
                    return value.WithoutLastXmlNewLine();
                }
            }
        }

        internal static XmlElementSyntax WithoutWhitespaceOnlyComment(this XmlElementSyntax value)
        {
            var texts = value.Content.OfType<XmlTextSyntax>();
            var textsCount = texts.Count;

            if (textsCount > 0)
            {
                var text = textsCount is 1
                           ? texts[0]
                           : texts[textsCount - 2];

                return WithoutWhitespaceOnlyCommentLocal(text);
            }

            return value;

            XmlElementSyntax WithoutWhitespaceOnlyCommentLocal(XmlTextSyntax text)
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

        internal static StringBuilder WithoutXmlCommentExterior(this StringBuilder value) => value.Without(Constants.Comments.XmlCommentExterior);

        internal static string WithoutXmlCommentExterior(this SyntaxNode value)
        {
            var builder = StringBuilderCache.Acquire();

            var trimmed = builder.Append(value).WithoutXmlCommentExterior().Trim();

            StringBuilderCache.Release(builder);

            return trimmed;
        }

        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this XmlElementSyntax value, in ReadOnlySpan<string> startTexts) => value.Content.WithoutStartText(startTexts);

        internal static SyntaxList<XmlNodeSyntax> WithoutStartText(this in SyntaxList<XmlNodeSyntax> values, in ReadOnlySpan<string> startTexts)
        {
            if (values.Count > 0 && values[0] is XmlTextSyntax textSyntax)
            {
                return values.Replace(textSyntax, textSyntax.WithoutStartText(startTexts));
            }

            return values;
        }

        internal static XmlTextSyntax WithoutStartText(this XmlTextSyntax value, in ReadOnlySpan<string> startTexts)
        {
            if (startTexts.Length is 0)
            {
                return value;
            }

            var tokens = value.TextTokens;

            if (tokens.Count is 0)
            {
                return value;
            }

            var textTokens = tokens.ToList();

            for (int i = 0, textTokensCount = textTokens.Count; i < textTokensCount; i++)
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

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this XmlElementSyntax value, string startText, in FirstWordHandling firstWordHandling = FirstWordHandling.StartLowerCase) => value.Content.WithStartText(startText, firstWordHandling);

        internal static SyntaxList<XmlNodeSyntax> WithStartText(this in SyntaxList<XmlNodeSyntax> values, string startText, in FirstWordHandling firstWordHandling = FirstWordHandling.StartLowerCase)
        {
            if (values.Count > 0)
            {
                return values[0] is XmlTextSyntax textSyntax
                       ? values.Replace(textSyntax, textSyntax.WithStartText(startText, firstWordHandling))
                       : values.Insert(0, XmlText(startText));
            }

            return XmlText(startText).ToSyntaxList<XmlNodeSyntax>();
        }

        internal static XmlTextSyntax WithStartText(this XmlTextSyntax value, string startText, in FirstWordHandling firstWordHandling = FirstWordHandling.StartLowerCase)
        {
            var tokens = value.TextTokens;

            if (tokens.Count is 0)
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

            for (int i = 0, count = textTokens.Count; i < count; i++)
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

                var space = i is 0 ? string.Empty : " ";

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

        internal static XmlElementSyntax WithTagsOnSeparateLines(this XmlElementSyntax value)
        {
            var contents = value.Content;

            var updateStartTag = true;
            var updateEndTag = true;

            if (contents.FirstOrDefault() is XmlTextSyntax firstText)
            {
                if (firstText.HasLeadingTrivia)
                {
                    updateStartTag = false;
                }
                else
                {
                    var textTokens = firstText.TextTokens;
                    var length = textTokens.Count;

                    if (length >= 2)
                    {
                        var firstToken = textTokens[0];
                        var nextToken = textTokens[1];

                        if (firstToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                        {
                            updateStartTag = false;
                        }
                        else if (nextToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && firstToken.ValueText.IsNullOrWhiteSpace())
                        {
                            updateStartTag = false;
                        }
                    }
                }
            }

            if (contents.LastOrDefault() is XmlTextSyntax lastText)
            {
                var textTokens = lastText.TextTokens;
                var length = textTokens.Count;

                if (length >= 2)
                {
                    var lastToken = textTokens[length - 1];
                    var secondLastToken = textTokens[length - 2];

                    if (lastToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                    {
                        updateEndTag = false;
                    }
                    else if (secondLastToken.IsKind(SyntaxKind.XmlTextLiteralNewLineToken) && lastToken.ValueText.IsNullOrWhiteSpace())
                    {
                        updateEndTag = false;
                    }
                }
            }

            if (updateStartTag)
            {
                value = value.WithStartTag(value.StartTag.WithTrailingXmlComment());
            }

            if (updateEndTag)
            {
                value = value.WithEndTag(value.EndTag.WithLeadingXmlComment());
            }

            return value;
        }

        internal static T WithAdditionalTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithAdditionalTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingEmptyLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed);

        internal static T WithTrailingNewLine<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed); // do not use an elastic one to enforce the line break

        internal static T WithTrailingXmlComment<T>(this T value) where T : SyntaxNode => value.WithTrailingTrivia(XmlCommentStart);

        internal static SyntaxList<XmlNodeSyntax> WithTrailingXmlComment(this in SyntaxList<XmlNodeSyntax> values) => values.Replace(values.Last(), values.Last().WithoutTrailingTrivia().WithTrailingXmlComment());

        internal static SyntaxNode WithUsing(this SyntaxNode value, string usingNamespace)
        {
            var usings = value.DescendantNodes<UsingDirectiveSyntax>().ToList();

            if (usings.Exists(_ => _.Name?.ToFullString() == usingNamespace))
            {
                // already set
                return value;
            }

            var directive = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(usingNamespace));

            var usingsCount = usings.Count;

            if (usingsCount is 0)
            {
                return value.InsertNodeBefore(value.FirstChild(), directive);
            }

            for (var index = 0; index < usingsCount; index++)
            {
                var usingDirective = usings[index];

                var usingName = usingDirective.Name?.ToFullString();

                if (usingName is "System")
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

            return value.InsertNodeAfter(usings[usingsCount - 1], directive);
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

            for (int i = 0, count = attributeLists.Count; i < count; i++)
            {
                var attributes = attributeLists[i].Attributes;

                for (int index = 0, attributesCount = attributes.Count; index < attributesCount; index++)
                {
                    if (names.Contains(attributes[index].GetName()))
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

            for (int i = 0, count = attributeLists.Count; i < count; i++)
            {
                var attributes = attributeLists[i].Attributes;

                for (int index = 0, attributesCount = attributes.Count; index < attributesCount; index++)
                {
                    if (names.Contains(attributes[index].GetName()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static MemberAccessExpressionSyntax GetFluentAssertionShouldNode(this ExpressionStatementSyntax value)
        {
            var nodes = value.DescendantNodes<MemberAccessExpressionSyntax>(SyntaxKind.SimpleMemberAccessExpression);

            foreach (var node in nodes)
            {
                var name = node.GetName();

                switch (name)
                {
                    // we might have a lambda expression, so the given statement might not be the correct ancestor statement of the 'Should()' node, hence we have to determine whether it's the specific statement that has an issue
                    case Constants.FluentAssertions.Should when node.FirstAncestor<ExpressionStatementSyntax>() == value:
                        return node;

                    case Constants.FluentAssertions.ShouldBeEquivalentTo when node.FirstAncestor<ExpressionStatementSyntax>() == value:
                        return node;
                }
            }

            return null;
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

        private static XmlCrefAttributeSyntax GetCref(in SyntaxList<XmlAttributeSyntax> syntax)
        {
            // keep in local variable to avoid multiple requests (see Roslyn implementation)
            for (int index = 0, count = syntax.Count; index < count; index++)
            {
                if (syntax[index] is XmlCrefAttributeSyntax a)
                {
                    return a;
                }
            }

            return default;
        }

        private static IEnumerable<XmlNodeSyntax> GetSummaryXmlsCore(IReadOnlyList<XmlElementSyntax> summaryXmls, ISet<string> tags)
        {
            for (int index = 0, count = summaryXmls.Count; index < count; index++)
            {
                var summary = summaryXmls[index];

                // we have to delve into the trivia to find the XML syntax nodes
                foreach (var node in summary.AllDescendantNodes())
                {
                    switch (node)
                    {
                        case XmlElementSyntax e when tags.Contains(e.GetName()):
                            yield return e;

                            break;

                        case XmlEmptyElementSyntax ee when tags.Contains(ee.GetName()):
                            yield return ee;

                            break;
                    }
                }
            }
        }

        private static bool Is(this SyntaxNode value, string tagName, in ReadOnlySpan<string> contents)
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

                if (info.CandidateReason is CandidateReason.None)
                {
                    return info.Symbol.IsLinqExtensionMethod();
                }
                else
                {
                    var candidates = info.CandidateSymbols;

                    return candidates.Length > 0 && candidates.Any(_ => _.IsLinqExtensionMethod());
                }
            }

            return false;
        }

        private static SyntaxTrivia[] CalculateWhitespaceTriviaWithComment(in int count, in SyntaxTrivia[] finalTrivia)
        {
            var triviaGroupedByLines = finalTrivia.GroupBy(_ => _.GetStartingLine());

            foreach (var triviaGroup in triviaGroupedByLines)
            {
                var trivia1 = triviaGroup.ElementAt(0);

                if (trivia1.IsWhiteSpace())
                {
                    var index1 = finalTrivia.IndexOf(trivia1);
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

                    finalTrivia[index1] = WhiteSpaces(spaces);
                }
            }

            return finalTrivia;
        }

        private static SyntaxToken FindStructuredTriviaToken(this SyntaxNode value)
        {
            if (value != null)
            {
                if (value.HasStructuredTrivia)
                {
                    var children = value.ChildNodesAndTokens();

                    var count = children.Count;

                    if (count > 0)
                    {
                        for (var index = 0; index < count; index++)
                        {
                            var child = children[index];

                            if (child.IsToken)
                            {
                                var childToken = child.AsToken();

                                if (childToken.HasStructuredTrivia)
                                {
                                    return childToken;
                                }

                                // no structure, so maybe it is the first descendant token to use
                                break;
                            }
                        }
                    }

                    var token = value.FirstDescendantToken();

                    if (token.HasStructuredTrivia)
                    {
                        return token;
                    }
                }
            }

            return default;
        }

        private static XmlTextSyntax XmlText(string text) => SyntaxFactory.XmlText(text);

        private static XmlTextSyntax XmlText(in SyntaxTokenList textTokens) => SyntaxFactory.XmlText(textTokens);

        private static XmlTextSyntax XmlText(IEnumerable<SyntaxToken> textTokens) => XmlText(textTokens.ToTokenList());

        private static SyntaxTrivia WhiteSpaces(in int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}