using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Transactions;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off
namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1098_TypeNameFollowsInterfaceNameSchemeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_correctly_named_type_without_interface() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
    public void DoSomething(object value) { }
}
");

        [TestCase(nameof(ICloneable))]
        [TestCase(nameof(IDisposable))]
        [TestCase(nameof(IAsyncDisposable))]
        [TestCase(nameof(INotifyPropertyChanging))]
        [TestCase(nameof(INotifyPropertyChanged))]
        [TestCase(nameof(INotifyDataErrorInfo))]
        [TestCase(nameof(IChangeTracking))]
        [TestCase(nameof(INotifyCollectionChanged))]
        [TestCase(nameof(IEnlistmentNotification))]
        [TestCase(nameof(IDeserializationCallback))]
        [TestCase("IDragSource")]
        [TestCase("IDropTarget")]
        public void No_issue_is_reported_for_correctly_named_type_with_(string interfaceName) => No_issue_is_reported_for(@"
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Runtime.Serialization;

public abstract class TestMe : " + interfaceName + @"
{
}
");

        [TestCase("IValueConverter")]
        [TestCase("IMultiValueConverter")]
        public void No_issue_is_reported_for_correctly_named_converter_type_with_(string interfaceName) => No_issue_is_reported_for(@"
using System;
using System.Windows.Data;

public abstract class TestMeConverter : " + interfaceName + @"
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_type() => No_issue_is_reported_for(@"
using System;

public interface ITestCandidate
{
}

public abstract class TestCandidate : ITestCandidate
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_type_with_number_suffix() => No_issue_is_reported_for(@"
using System;

public interface ITestCandidate42
{
}

public abstract class TestCandidate : ITestCandidate42
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_type_with_Has_ability_interface() => No_issue_is_reported_for(@"
using System;

public interface IHasSomeName
{
}

public abstract class TestMe : IHasSomeName
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_type_with_Provider_suffix() => No_issue_is_reported_for(@"
using System;

public interface ISomeProvider
{
}

public abstract class TestMe : ISomeProvider
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_type_with_Extended_interface() => No_issue_is_reported_for(@"
using System;

public interface IExtendedTestMe
{
}

public abstract class TestMe : IExtendedTestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_named_command_type() => No_issue_is_reported_for(@"
using System;

public interface ISomeCommand
{
}

public abstract class TestMeCommand : ISomeCommand
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_type() => An_issue_is_reported_for(@"
using System;

public interface ITestCandidate
{
}

public abstract class TestMe : ITestCandidate
{
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_named_type_for_HashTable() => An_issue_is_reported_for(@"
using System;

public interface IHashTable
{
}

public abstract class TestMe : IHashTable
{
}
");

        [Test]
        public void An_issue_is_reported_for_correctly_named_type_with_Extended_interface() => An_issue_is_reported_for(@"
using System;

public interface IExtendedSomething
{
}

public abstract class TestMe : IExtendedSomething
{
}
");

        protected override string GetDiagnosticId() => MiKo_1098_TypeNameFollowsInterfaceNameSchemeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1098_TypeNameFollowsInterfaceNameSchemeAnalyzer();
    }
}