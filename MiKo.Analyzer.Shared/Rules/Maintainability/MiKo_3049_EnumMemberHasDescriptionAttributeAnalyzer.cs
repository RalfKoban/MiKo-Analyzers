﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3049";

        private static readonly string DescriptionAttributeName = typeof(DescriptionAttribute).FullName;

        public MiKo_3049_EnumMemberHasDescriptionAttributeAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(INamedTypeSymbol symbol)
        {
            if (symbol.IsEnum())
            {
                // ignore Win32 interop types
                return symbol.ContainingType?.Name != "NativeDeclarations" && symbol.ContainingNamespace?.Name != "Interop";
            }

            return false;
        }

        protected override IEnumerable<Diagnostic> Analyze(INamedTypeSymbol symbol, Compilation compilation)
        {
            foreach (var field in symbol.GetFields())
            {
                var attributes = field.GetAttributes();

                if (attributes.Length is 0)
                {
                    yield return Issue(field);
                }
                else
                {
                    var descriptionAttributes = attributes.Where(_ => _.AttributeClass.InheritsFrom(DescriptionAttributeName)).ToList();

                    if (descriptionAttributes.Count is 0)
                    {
                        yield return Issue(field);
                    }
                    else
                    {
                        foreach (var attribute in descriptionAttributes)
                        {
                            if (attribute.ConstructorArguments.Length is 0)
                            {
                                yield return Issue(field);
                            }
                        }
                    }
                }
            }
        }
    }
}