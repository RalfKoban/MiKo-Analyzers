using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

//// ncrunch: collect values off

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [TestFixture]
    public sealed class MiKo_3048_ValueConverterHasAttributeAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void No_issue_is_reported_for_non_ValueConverter_type() => No_issue_is_reported_for(@"
using System;

public class TestMe
{
}
");

        [Test]
        public void No_issue_is_reported_for_ValueConverter_with_ValueConversionAttribute_applied() => No_issue_is_reported_for(@"
using System;
using System.Globalization;

namespace System.Windows.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ValueConversionAttribute : Attribute
    {
        public ValueConversionAttribute(Type a, Type b)
        {
        }
    }
}

namespace Bla
{
    using System.Windows.Data;

    [ValueConversion(typeof(string), typeof(int))]
    public class StringToIntegerConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(""Convert not supported."");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(""ConvertBack not supported."");
        }
    }
}");

        [Test]
        public void An_issue_is_reported_for_ValueConverter_with_no_ValueConversionAttribute_applied() => An_issue_is_reported_for(@"
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bla
{
    public class StringToIntegerConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(""Convert not supported."");
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(""ConvertBack not supported."");
        }
    }
}");

        protected override string GetDiagnosticId() => MiKo_3048_ValueConverterHasAttributeAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_3048_ValueConverterHasAttributeAnalyzer();
    }
}