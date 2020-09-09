using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2060_FactoryAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_undocumented_non_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_documented_non_factory_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Does something.
/// </summary>
public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_undocumented_factory_class() => No_issue_is_reported_for(@"
using System;

public class TestMeFactory
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_class() => No_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides support for creating something.
/// </summary>
public class TestMeFactory
{
}
");

        [Test]
        public void An_issue_is_reported_for_inccorrectly_documented_factory_class() => An_issue_is_reported_for(@"
using System;

/// <summary>
/// Provides something.
/// </summary>
public class TestMeFactory
{
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_([Values(" ", "")] string gap) => No_issue_is_reported_for(@"
using System;

public class Whatever : IWhatever
{
}

public interface IWhatever
{
}

public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IWhatever""" + gap + @"/> type with a result.
    /// </summary>
    public IWhatever Create() => new Whatever();
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method_([Values(" ", "")] string gap) => An_issue_is_reported_for(@"
using System;

public class Whatever : IWhatever
{
}

public interface IWhatever
{
}

public class TestMeFactory
{
    /// <summary>
    /// Create a <see cref=""IWhatever""" + gap + @"/> with a result.
    /// </summary>
    public IWhatever Create() => new Whatever();
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_generic_type_([Values(" ", "")] string gap) => No_issue_is_reported_for(@"
using System;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IWhatever{T}""" + gap + @"/> type with a result.
    /// </summary>
    public IWhatever<int> Create() => new Whatever<int>();
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method_on_generic_type_([Values(" ", "")] string gap) => An_issue_is_reported_for(@"
using System;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Create <see cref=""IWhatever{T}""" + gap + @"/> with a result.
    /// </summary>
    public IWhatever<int> Create() => new Whatever<int>();
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_generic_collection_type_([Values(" ", "")] string gap) => No_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Creates a collection of new instances of the <see cref=""IWhatever{T}""" + gap + @"/> type with a result.
    /// </summary>
    public IList<IWhatever<int>> Create() => new List<Whatever<int>>();
}
");

        [Test]
        public void An_issue_is_reported_for_incorrectly_documented_factory_method_on_generic_collection_type_([Values(" ", "")] string gap) => An_issue_is_reported_for(@"
using System;
using System.Collections.Generic;

public class Whatever<T> : IWhatever<T>
{
}

public interface IWhatever<T>
{
}

public class TestMeFactory
{
    /// <summary>
    /// Create <see cref=""IWhatever{T}""" + gap + @"/> with a result.
    /// </summary>
    public IList<IWhatever<int>> Create() => new List<Whatever<int>>();
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_string() => No_issue_is_reported_for(@"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with a result.
    /// </summary>
    public string Create() => string.Empty;
}
");

        [Test]
        public void No_issue_is_reported_for_correctly_documented_factory_method_on_generic_string_collection() => No_issue_is_reported_for(@"
using System.Collections.Generic;

public class TestMeFactory
{
    /// <summary>
    /// Creates a collection of new instances of the <see cref=""string""/> type with a result.
    /// </summary>
    public IList<string> Create() => new string[0];
}
");

        [Test]
        public void Code_gets_fixed_for_class_summary()
        {
            const string OriginalCode = @"
/// <summary>
/// Something.
/// </summary>
public class TestMeFactory
{
    public string Create() => new string();
}
";

            const string FixedCode = @"
/// <summary>
/// Provides support for creating something.
/// </summary>
public class TestMeFactory
{
    public string Create() => new string();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_method_summary()
        {
            const string OriginalCode = @"
public class TestMeFactory
{
    /// <summary>
    /// A result.
    /// </summary>
    public string Create() => new string();
}
";

            const string FixedCode = @"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with a result.
    /// </summary>
    public string Create() => new string();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test]
        public void Code_gets_fixed_for_collection_method_summary()
        {
            const string OriginalCode = @"
using System;
using System.Collections.Generic;

public class TestMeFactory
{
    /// <summary>
    /// A result.
    /// </summary>
    public IEnumerable<string> Create() => new string[0];
}
";

            const string FixedCode = @"
using System;
using System.Collections.Generic;

public class TestMeFactory
{
    /// <summary>
    /// Creates a collection of new instances of the <see cref=""string""/> type with a result.
    /// </summary>
    public IEnumerable<string> Create() => new string[0];
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        [Test] // https://github.com/dotnet/roslyn/issues/47550
        public void Code_gets_fixed_working_around_Roslyn_bug_47550()
        {
            const string OriginalCode = @"
internal interface IFactory
{
    /// <summary>
    /// Blah <see cref=""Xyz""/> blah.
    /// </summary>
    /// <returns></returns>
    IXyz Create();
}
";

            const string FixedCode = @"
internal interface IFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""IXyz""/> type with blah <see cref=""Xyz""/> blah.
    /// </summary>
    /// <returns></returns>
    IXyz Create();
}
";

            VerifyCSharpFix(OriginalCode, FixedCode);
        }

        protected override string GetDiagnosticId() => MiKo_2060_FactoryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2060_FactoryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2060_CodeFixProvider();
    }
}