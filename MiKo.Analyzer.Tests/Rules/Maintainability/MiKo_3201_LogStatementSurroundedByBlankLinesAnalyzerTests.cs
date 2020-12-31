using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_another_Log_call() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        void Debug();
        void Info();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            Log.Debug();
            Log.Info();
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_block_statement() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething()
        {
            if (Log.IsDebugEnabled)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_followed_by_if_block_separated_by_empty_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            Log.Debug();

            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void No_issue_is_reported_for_Log_call_preceded_by_if_block_separated_by_empty_line() => No_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }

            Log.Debug();
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_followed_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            Log.Debug();
            if (something)
            {
                // some comment
            }
        }
    }
}
");

        [Test]
        public void An_issue_is_reported_for_Log_call_preceded_by_if_block() => An_issue_is_reported_for(@"
namespace Bla
{
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        void Debug();
    }

    public class TestMe
    {
        private static ILog Log = null;

        public void DoSomething(bool something)
        {
            if (something)
            {
                // some comment
            }
            Log.Debug();
        }
    }
}
");

        protected override string GetDiagnosticId() => MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3201_LogStatementSurroundedByBlankLinesAnalyzer();
    }
}