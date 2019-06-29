using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3040_BooleanMethodParametersAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_parameterless_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething() { }
}
");

        [Test]
        public void No_issue_is_reported_for_non_boolean_parameter_method() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(int x) { }
}
");

        [TestCase("DependencyObject o, bool b")]
        [TestCase("bool b, DependencyObject o")]
        public void No_issue_is_reported_for_boolean_parameter_on_method_that_has_an_additional_DependencyObject_parameter_as_well(string parameters) => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_parameter_on_Dispose_method() => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe
{
    protected virtual void Dispose(bool disposing) { }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_parameter_on_interface_implementation_method() => No_issue_is_reported_for(@"
using System;
using System.Windows;

namespace My
{
    public abstract class TestMe : System.Collections.Generic.ICollection<bool>
    {
        public abstract int Count { get; set; }

        public abstract bool IsReadOnly { get; set; }

        void ICollection<bool>.Add(bool value) { }

        public abstract void Clear();

        public abstract bool Contains(bool item);

        public abstract void CopyTo(bool[] array, int arrayIndex);

        public abstract bool Remove(bool item);

        public abstract IEnumerator<bool> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}");

        [Test]
        public void No_issue_is_reported_for_boolean_parameter_on_overridden_method() => No_issue_is_reported_for(@"
using System;
using System.Windows;

namespace My
{
    public abstract class TestMe : System.Collections.Generic.ICollection<bool>
    {
        public abstract int Count { get; set; }

        public abstract bool IsReadOnly { get; set; }

        void ICollection<bool>.Add(bool value) { }

        public abstract void Clear();

        public abstract bool Contains(bool item);

        public abstract void CopyTo(bool[] array, int arrayIndex);

        public abstract bool Remove(bool item);

        public abstract IEnumerator<bool> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class TestMeConcrete : TestMe
    {
        public override bool Remove(bool item) => false;
    }
}");

        [TestCase("bool b")]
        [TestCase("bool b, int x")]
        [TestCase("int x, bool b, int y")]
        [TestCase("int x, bool b")]
        [TestCase("bool b1, bool b2, bool b3")]
        [TestCase("bool b, DependencyObject o, int x")] // check for method with dependency object but 3 parameters
        public void An_issue_is_reported_for_boolean_parameter_on_method(string parameters) => An_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(" + parameters + @") { }
}
");

        [Test]
        public void No_issue_is_reported_for_boolean_parameter_on_test_method([ValueSource(nameof(Tests))] string test) => No_issue_is_reported_for(@"
using System;
using System.Windows;

public class TestMe
{
    [" + test + @"]
    public void Something(bool value) { }
}
");

        protected override string GetDiagnosticId() => MiKo_3040_BooleanMethodParametersAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3040_BooleanMethodParametersAnalyzer();
    }
}