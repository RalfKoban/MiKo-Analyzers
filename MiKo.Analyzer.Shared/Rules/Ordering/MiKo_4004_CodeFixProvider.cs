﻿using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4004_CodeFixProvider)), Shared]
    public sealed class MiKo_4004_CodeFixProvider : DisposeOrderingCodeFixProvider
    {
        private const string TargetAnnotationKind = "target method";

        public override string FixableDiagnosticId => "MiKo_4004";

        protected override SyntaxNode GetUpdatedSyntaxRoot(Document document, SyntaxNode root, SyntaxNode syntax, SyntaxAnnotation annotationOfSyntax, Diagnostic issue)
        {
            var typeSyntax = syntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>();

            var updatedTypeSyntax = GetUpdatedTypeSyntax(document, typeSyntax, syntax, issue);

            return root.ReplaceNode(typeSyntax, updatedTypeSyntax);
        }

        protected override SyntaxNode GetUpdatedTypeSyntax(Document document, BaseTypeDeclarationSyntax typeSyntax, SyntaxNode syntax, Diagnostic issue)
        {
            var disposeMethod = (MethodDeclarationSyntax)syntax;
            var targetMethod = FindTargetMethod(document, typeSyntax, disposeMethod);

            var disposeAnnotation = new SyntaxAnnotation(DisposeAnnotationKind);
            var targetAnnotation = new SyntaxAnnotation(TargetAnnotationKind);

            var modifiedType = typeSyntax.ReplaceNodes(
                                                   new[] { targetMethod, disposeMethod },
                                                   (original, rewritten) =>
                                                                           {
                                                                               if (rewritten.IsEquivalentTo(targetMethod))
                                                                               {
                                                                                   return targetMethod.WithAnnotation(targetAnnotation);
                                                                               }

                                                                               if (rewritten.IsEquivalentTo(disposeMethod))
                                                                               {
                                                                                   return disposeMethod.WithAnnotation(disposeAnnotation);
                                                                               }

                                                                               return original;
                                                                           });

            // remove dispose method from modified type to place it at correct location
            var annotatedDisposeMethod = modifiedType.GetAnnotatedNodes(disposeAnnotation).OfType<MethodDeclarationSyntax>().First();
            modifiedType = modifiedType.RemoveNodeAndAdjustOpenCloseBraces(annotatedDisposeMethod);

            // find target method to insert dispose method before that
            var annotatedTargetMethod = modifiedType.GetAnnotatedNodes(targetAnnotation).OfType<MethodDeclarationSyntax>().First();

            return MoveRegionsWithDisposeAnnotation(modifiedType.InsertNodeBefore(annotatedTargetMethod, annotatedDisposeMethod), disposeMethod);
        }

        private static MethodDeclarationSyntax FindTargetMethod(Document document, BaseTypeDeclarationSyntax typeSyntax, MethodDeclarationSyntax disposeMethod)
        {
            var methodSymbol = (IMethodSymbol)disposeMethod.GetSymbol(document);
            var typeSymbol = (INamedTypeSymbol)typeSyntax.GetSymbol(document);

            var methods = typeSymbol.GetMethods(MethodKind.Ordinary).Except(methodSymbol);
            var method = methods.FirstOrDefault(_ => _.DeclaredAccessibility == methodSymbol.DeclaredAccessibility && _.IsStatic is false);

            if (method != null)
            {
                return (MethodDeclarationSyntax)method.GetSyntax();
            }

            // TODO: RKN Fix me (find better node)
            return typeSyntax.FirstDescendant<MethodDeclarationSyntax>();
        }
    }
}