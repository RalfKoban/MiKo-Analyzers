using System.Xml;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3070_EnumerableMethodReturnsNullAnalyzer : MethodReturnsNullAnalyzer
    {
        public const string Id = "MiKo_3070";

        public MiKo_3070_EnumerableMethodReturnsNullAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            var returnType = symbol.ReturnType;

            switch (returnType.SpecialType)
            {
                case SpecialType.System_Void:
                case SpecialType.System_String:
                    return false;

                default:
                {
                    if (returnType is IArrayTypeSymbol array)
                    {
                        return array.ElementType.IsByte() is false;
                    }

                    if (returnType.IsEnumerable())
                    {
                        return returnType.InheritsFrom<XmlNode>() is false;
                    }

                    return false;
                }
            }
        }
    }
}