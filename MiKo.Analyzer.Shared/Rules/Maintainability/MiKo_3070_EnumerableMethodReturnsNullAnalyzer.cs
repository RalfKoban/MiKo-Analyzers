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

            if (returnType.SpecialType == SpecialType.System_Void)
            {
                return false;
            }

            if (returnType.IsString())
            {
                return false;
            }

            if (returnType.IsByteArray())
            {
                return false;
            }

            if (returnType.IsEnumerable())
            {
                return returnType.InheritsFrom<XmlNode>() is false;
            }

            return false;
        }
    }
}