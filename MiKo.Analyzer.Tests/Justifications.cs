namespace MiKoSolutions.Analyzers
{
    /// <summary>
    /// Provides justifications for <see cref="System.Diagnostics.CodeAnalysis.SuppressMessageAttribute"/>.
    /// </summary>
    public static class Justifications
    {
        public static class StyleCop
        {
            /// <summary>
            /// The justification for StyleCop readability rule "<c>SA1116:SplitParametersMustStartOnLineAfterDeclaration</c>".
            /// </summary>
            public const string SA1116 = "Would look strange otherwise.";

            /// <summary>
            /// The justification for StyleCop readability rule "<c>SA1117:ParametersMustBeOnSameLineOrSeparateLines</c>".
            /// </summary>
            public const string SA1117 = "Would look strange otherwise.";

            /// <summary>
            /// The justification for StyleCop readability rule "<c>SA1118:ParameterMustNotSpanMultipleLines</c>".
            /// </summary>
            public const string SA1118 = "Would look strange otherwise.";
        }
    }
}