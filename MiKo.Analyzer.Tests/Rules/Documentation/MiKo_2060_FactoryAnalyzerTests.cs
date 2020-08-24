﻿using Microsoft.CodeAnalysis.CodeFixes;
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
    public IList<IWhatever<int>> Create() => new Whatever<int>();
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
    public IList<IWhatever<int>> Create() => new Whatever<int>();
}
");

        [Test]
        public void Code_gets_fixed_for_class_summary()
        {
            const string OriginalText = @"
/// <summary>
/// Something.
/// </summary>
public class TestMeFactory
{
    public string Create() => new string();
}
";

            const string FixedText = @"
/// <summary>
/// Provides support for creating something.
/// </summary>
public class TestMeFactory
{
    public string Create() => new string();
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_method_summary()
        {
            const string OriginalText = @"
public class TestMeFactory
{
    /// <summary>
    /// A result.
    /// </summary>
    public string Create() => new string();
}
";

            const string FixedText = @"
public class TestMeFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref=""string""/> type with a result.
    /// </summary>
    public string Create() => new string();
}
";

            VerifyCSharpFix(OriginalText, FixedText);
        }

        [Test]
        public void Code_gets_fixed_for_collection_method_summary()
        {
            const string OriginalText = @"
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

            const string FixedText = @"
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

            VerifyCSharpFix(OriginalText, FixedText);
        }

        protected override string GetDiagnosticId() => MiKo_2060_FactoryAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2060_FactoryAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider() => new MiKo_2060_CodeFixProvider();
    }
}