using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="SyntaxNode"/>s.
    /// </summary>
    internal static partial class SyntaxNodeExtensions
    {
        private static readonly SyntaxKind[] LogicalConditions = { SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression };

        private static readonly SyntaxList<TypeParameterConstraintClauseSyntax> EmptyConstraintClauses = SyntaxFactory.List<TypeParameterConstraintClauseSyntax>();

        internal static bool Contains(this SyntaxNode value, in char c) => value?.ToString().Contains(c) ?? false;

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
                    return EmptyConstraintClauses;
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

        internal static AccessorDeclarationSyntax GetGetter(this PropertyDeclarationSyntax value) => value?.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.GetAccessorDeclaration);

        internal static AccessorDeclarationSyntax GetSetter(this PropertyDeclarationSyntax value) => value?.AccessorList?.FirstChild<AccessorDeclarationSyntax>(SyntaxKind.SetAccessorDeclaration);

        internal static XmlElementSyntax GetParameterComment(this DocumentationCommentTriviaSyntax value, string parameterName) => value.FirstDescendant<XmlElementSyntax>(_ => _.GetName() is Constants.XmlTag.Param && _.GetParameterName() == parameterName);

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

        internal static IReadOnlyList<T> GetAttributes<T>(this XmlElementSyntax value) where T : XmlAttributeSyntax
        {
            return value?.StartTag.Attributes.OfType<XmlAttributeSyntax, T>() ?? Array.Empty<T>();
        }

        internal static IReadOnlyList<T> GetAttributes<T>(this XmlEmptyElementSyntax value) where T : XmlAttributeSyntax
        {
            return value?.Attributes.OfType<XmlAttributeSyntax, T>() ?? Array.Empty<T>();
        }

        internal static ExpressionSyntax GetIdentifierExpression(this ExpressionSyntax value)
        {
            switch (value)
            {
                case InvocationExpressionSyntax invocation:
                    return invocation.GetIdentifierExpression();

                case IdentifierNameSyntax identifier:
                    return identifier;

                case MemberAccessExpressionSyntax maes:
                    return maes.Expression;

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

        internal static TypeSyntax[] GetTypes(this MemberAccessExpressionSyntax value)
        {
            if (value.Name is GenericNameSyntax generic)
            {
                return generic.TypeArgumentList.Arguments.ToArray();
            }

            return Array.Empty<TypeSyntax>();
        }

        internal static TypeSyntax[] GetTypes(this InvocationExpressionSyntax value)
        {
            var types = new List<TypeSyntax>();

            var expression = value.Expression;

            while (expression is MemberAccessExpressionSyntax maes)
            {
                types.AddRange(maes.GetTypes());

                expression = maes.Expression;
            }

            return types.ToArray();
        }

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

        internal static bool IsException(this TypeSyntax value, Type exceptionType)
        {
            if (value is PredefinedTypeSyntax)
            {
                return false;
            }

            var s = value.ToString();

            return s == exceptionType.Name || s == exceptionType.FullName;
        }

        internal static bool IsExpressionTree(this SyntaxNode value, SemanticModel semanticModel)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
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

        internal static bool IsParameter(this IdentifierNameSyntax value, SemanticModel semanticModel) => value.EnclosingMethodHasParameter(value.GetName(), semanticModel);

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

        internal static bool IsTestClass(this BaseTypeDeclarationSyntax value) => value is ClassDeclarationSyntax declaration && IsTestClass(declaration);

        internal static bool IsTestClass(this ClassDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestClassAttributeNames);

        internal static bool IsTestMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestMethodAttributeNames);

        internal static bool IsTestOneTimeSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestOneTimeSetupAttributeNames);

        internal static bool IsTestOneTimeTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestOneTimeTearDownAttributeNames);

        internal static bool IsTestSetUpMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestSetupAttributeNames);

        internal static bool IsTestTearDownMethod(this MethodDeclarationSyntax value) => value.HasAttributeName(Constants.Names.TestTearDownAttributeNames);

        internal static bool IsTypeUnderTestCreationMethod(this MethodDeclarationSyntax value) => Constants.Names.TypeUnderTestMethodNames.Contains(value.GetName());

        internal static bool IsTypeUnderTestVariable(this VariableDeclaratorSyntax value) => Constants.Names.TypeUnderTestVariableNames.Contains(value.GetName());

        internal static bool IsVoid(this TypeSyntax value) => value is PredefinedTypeSyntax p && p.Keyword.IsKind(SyntaxKind.VoidKeyword);

        internal static IEnumerable<InvocationExpressionSyntax> LinqExtensionMethods(this SyntaxNode value, SemanticModel semanticModel) => value.DescendantNodes<InvocationExpressionSyntax>(_ => IsLinqExtensionMethod(_, semanticModel));

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
                    for (var index = 0; index < count; index++)
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

        internal static InvocationExpressionSyntax WithArguments(this InvocationExpressionSyntax value, in SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return value.WithArgumentList(SyntaxFactory.ArgumentList(arguments));
        }

        internal static InvocationExpressionSyntax WithArguments(this InvocationExpressionSyntax value, params ArgumentSyntax[] arguments)
        {
            return value.WithArguments(arguments.ToSeparatedSyntaxList());
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

        internal static SyntaxNode WithoutParenthesisParent(this SyntaxNode value)
        {
            while (true)
            {
                if (value is ParenthesizedExpressionSyntax parenthesized)
                {
                    value = parenthesized.Parent;

                    continue;
                }

                return value;
            }
        }
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

        internal static bool IsBooleanCheck(this IsPatternExpressionSyntax value)
        {
            switch (value.Pattern)
            {
                case ConstantPatternSyntax constant when IsBoolean(constant): return true;
                case UnaryPatternSyntax unary when unary.Pattern is ConstantPatternSyntax constant && IsBoolean(constant): return true;
                default:
                    return false;
            }

            bool IsBoolean(ConstantPatternSyntax constant)
            {
                switch (constant.Expression.Kind())
                {
                    case SyntaxKind.TrueLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                        return true;

                    default:
                        return false;
                }
            }
        }

        internal static bool IsElementAccess(this ExpressionSyntax value) => value is BinaryExpressionSyntax binary && IsElementAccess(binary);

        internal static bool IsElementAccess(this BinaryExpressionSyntax value) => value.Left.IsKind(SyntaxKind.ElementAccessExpression) || value.Right.IsKind(SyntaxKind.ElementAccessExpression);

        internal static bool IsNullCheck(this BinaryExpressionSyntax value) => value.Left.IsKind(SyntaxKind.NullLiteralExpression) || value.Right.IsKind(SyntaxKind.NullLiteralExpression);

        internal static bool IsNullCheck(this IsPatternExpressionSyntax value)
        {
            switch (value.Pattern)
            {
                case ConstantPatternSyntax constant when constant.Expression.IsKind(SyntaxKind.NullLiteralExpression): return true; // is null
                case UnaryPatternSyntax unary when unary.Pattern is ConstantPatternSyntax constant && constant.Expression.IsKind(SyntaxKind.NullLiteralExpression): return true; // is not null
                default:
                    return false;
            }
        }

        internal static bool IsValueType(this ExpressionSyntax value, SemanticModel semanticModel)
        {
            switch (value?.Kind())
            {
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                    return true;

                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
#if VS2022
                case SyntaxKind.Utf8StringLiteralExpression:
#endif
                    return false;

                default:
                    return value.GetTypeSymbol(semanticModel)?.IsValueType is true;
            }
        }

        internal static Stack<ExpressionSyntax> GetLeafs(this BinaryExpressionSyntax value)
        {
            var leafs = new Stack<ExpressionSyntax>();

            CollectLeafs(value);

            return leafs;

            void CollectLeafs(BinaryExpressionSyntax node)
            {
                Collect(node.Right.WithoutParenthesis());
                Collect(node.Left.WithoutParenthesis());
            }

            void Collect(ExpressionSyntax expression)
            {
                if (expression is BinaryExpressionSyntax b && b.IsAnyKind(LogicalConditions))
                {
                    CollectLeafs(b);
                }
                else
                {
                    leafs.Push(expression);
                }
            }
        }

        internal static Stack<SyntaxKind> GetConditionKinds(this BinaryExpressionSyntax value)
        {
            var leafs = new Stack<SyntaxKind>();

            CollectLeafs(value);

            return leafs;

            void CollectLeafs(BinaryExpressionSyntax node)
            {
                Collect(node.Right.WithoutParenthesis());
                Collect(node.Left.WithoutParenthesis());
            }

            void Collect(ExpressionSyntax expression)
            {
                if (expression is BinaryExpressionSyntax b && b.IsAnyKind(LogicalConditions))
                {
                    leafs.Push(expression.Kind());

                    CollectLeafs(b);
                }
            }
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
    }
}