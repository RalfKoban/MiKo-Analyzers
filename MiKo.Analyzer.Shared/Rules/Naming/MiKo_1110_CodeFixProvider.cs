﻿using System.Collections.Generic;
using System.Composition;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MiKo_1110_CodeFixProvider)), Shared]
    public sealed class MiKo_1110_CodeFixProvider : NamingCodeFixProvider
    {
        public override string FixableDiagnosticId => "MiKo_1110";

        protected override SyntaxNode GetSyntax(IEnumerable<SyntaxNode> syntaxNodes) => syntaxNodes.FirstOrDefault(_ => _.IsAnyKind(MethodKinds));
    }
}