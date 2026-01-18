using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1117_TestMethodsShouldBeNamedMorePreciseAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1117";

        private static readonly string[] UnclearTerms =
                                                        {
                                                            "_exception_thrown",
                                                            "_throws_exception",
                                                            "Event_Is_Raised",
                                                            "EventFired",
                                                            "EventIsFired",
                                                            "EventIsRaised",
                                                            "EventOccured",
                                                            "EventOccurred",
                                                            "EventRaised",
                                                            "ExceptionThrown",
                                                            "FiresEvent",
                                                            "OccuredEvent",
                                                            "OccurredEvent",
                                                            "Raised_Event",
                                                            "RaisedEvent",
                                                            "Raises_Event",
                                                            "RaisesEvent",
                                                            "ThrowsException",
                                                            "handle",
                                                            "not_handle",
                                                            "NotHandle",
                                                            "properly",
                                                            "correct",
                                                            "works",
                                                            "proper_way",
                                                            "properWay",
                                                        };

        private static readonly string[] KnownExceptions =
                                                           {
                                                               "NoException",
                                                               nameof(ArgumentException),
                                                               nameof(ArgumentNullException),
                                                               nameof(ArgumentOutOfRangeException),
                                                               nameof(InvalidOperationException),
                                                               "JsonException",
                                                               nameof(KeyNotFoundException),
                                                               nameof(NotImplementedException),
                                                               nameof(NotSupportedException),
                                                               nameof(NullReferenceException),
                                                               nameof(ObjectDisposedException),
                                                               nameof(OperationCanceledException),
                                                               nameof(TaskCanceledException),
                                                               nameof(UnauthorizedAccessException),
                                                               "ValidationException",
                                                           };

        public MiKo_1117_TestMethodsShouldBeNamedMorePreciseAnalyzer() : base(Id)
        {
        }

        protected override bool IsUnitTestAnalyzer => true;

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => HasIssue(symbol.Name.AsCachedBuilder().ReplaceAllWithProbe(KnownExceptions, "#").ToStringAndRelease())
                                                                                                                 ? new[] { Issue(symbol) }
                                                                                                                 : Array.Empty<Diagnostic>();

        private static bool HasIssue(string symbolName) => symbolName.ContainsAny(UnclearTerms, StringComparison.OrdinalIgnoreCase);
    }
}