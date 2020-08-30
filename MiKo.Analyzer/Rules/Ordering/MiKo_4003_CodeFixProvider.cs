﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MiKoSolutions.Analyzers.Rules.Ordering
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_4003_CodeFixProvider)), Shared]
    public sealed class MiKo_4003_CodeFixProvider : OrderingCodeFixProvider
    {
        public override string FixableDiagnosticId => MiKo_4003_DisposeMethodsOrderedAfterCtorsAndFinalizersAnalyzer.Id;

        protected override string Title => "Place Dispose method after ctors and finalizers";

        protected override SyntaxNode GetUpdatedTypeSyntax(BaseTypeDeclarationSyntax syntax)
        {
            var disposeMethod = GetDisposeMethod(syntax);

            // remove method so that it can be added again
            var modifiedType = syntax.RemoveNode(disposeMethod, SyntaxRemoveOptions.KeepNoTrivia);

            var syntaxNode = FindLastCtorOrFinalizer(modifiedType);
            if (syntaxNode is null)
            {
                // none found, so insert method before first method
                var method = modifiedType.ChildNodes().OfType<MethodDeclarationSyntax>().First();

                // method needs to be intended and a CRLF needs to be added
                var modifiedDisposeMethod = disposeMethod.WithIntentation().WithEndOfLine();

                return modifiedType.InsertNodesBefore(method, new[] { modifiedDisposeMethod });
            }

            // insert method after found ctor or finalizer
            return modifiedType.InsertNodesAfter(syntaxNode, new[] { disposeMethod });
        }

        private static MethodDeclarationSyntax GetDisposeMethod(SyntaxNode type)
        {
            var disposeMethod = type.ChildNodes()
                                    .OfType<MethodDeclarationSyntax>()
                                    .Where(_ => _.Modifiers.Any(__ => __.IsKind(SyntaxKind.PublicKeyword)) || _.ExplicitInterfaceSpecifier != null)
                                    .Where(_ => _.ParameterList.Parameters.None())
                                    .Where(_ => _.ReturnType.IsVoid())
                                    .First(_ => _.GetName() == nameof(IDisposable.Dispose));
            return disposeMethod;
        }

        private static SyntaxNode FindLastCtorOrFinalizer(SyntaxNode modifiedType)
        {
            SyntaxNode ctor = modifiedType.ChildNodes().OfType<ConstructorDeclarationSyntax>().LastOrDefault();
            SyntaxNode finalizer = modifiedType.ChildNodes().OfType<DestructorDeclarationSyntax>().LastOrDefault();

            return finalizer ?? ctor;
        }
    }
}