
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

        protected override bool ShallAnalyze(IMethodSymbol method)
        {
            var returnType = method.ReturnType;
            switch (returnType.SpecialType)
            {
                case SpecialType.System_Void:
                case SpecialType.System_String:
                    return false;

                default:
                {
                    if (returnType.TypeKind == TypeKind.Array)
                        return returnType is IArrayTypeSymbol r && r.ElementType.SpecialType != SpecialType.System_Byte;

                    if (returnType.IsEnumerable())
                        return !returnType.InheritsFrom<XmlNode>();

                    return false;
                }
            }
        }
    }
}