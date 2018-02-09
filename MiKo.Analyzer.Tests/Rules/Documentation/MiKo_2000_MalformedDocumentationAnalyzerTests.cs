using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Documentation
{
    [TestFixture]
    public sealed class MiKo_2000_MalformedDocumentationAnalyzerTests : CodeFixVerifier
    {
        [Test]
        public void Malformed_documentation_is_reported_on_class() => Issue_is_reported(@"
/// <summary>
/// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_method() => Issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_property() => Issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_event() => Issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    public Event EventHandler Malform;
}
");

        [Test]
        public void Malformed_documentation_is_reported_on_field() => Issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Saves & Loads the relevant layout inforamtion of the ribbon within <see cref=""XmlRibbonLayout""/>
    /// </summary>
    private string Malform;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_class() => No_issue_is_reported(@"
/// <summary>
/// Something valid.
/// </summary>
public sealed class TestMe
{
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_method() => No_issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public void Malform() { }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_property() => No_issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public int Malform { get; set; }
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_event() => No_issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    public Event EventHandler Malform;
}
");

        [Test]
        public void Valid_documentation_is_not_reported_on_field() => No_issue_is_reported(@"
public sealed class TestMe
{
    /// <summary>
    /// Something valid.
    /// </summary>
    private string Malform;
}
");

        [Test]
        public void Invalid_documentation_is_reported_on_class() => Issue_is_reported(@"
      /// <summary>
      /// Library related functions used for converting projects.
      /// Related: See email   Kevin Ketterle 2015-11-23.
      /// 
      /// ""Dein Code geht im Moment davon aus, dass der Name einer Bibliothek auch dem Platzhalternamen entspricht. 
      ///  Das ist aber nicht in allen Fällen so und wird sicher früher oder später Probleme bereiten. 
      ///  Außerdem sind nicht alle Bibliotheken dafür geeignet, durch Platzhalter ersetzt zu werden. 
      ///  Interface-Bibliotheken beispielsweise sollten immer mit * referenziert werden, Container-Bibliotheken ohne Platzhalter mit fester Version. 
      ///  Details dazu stehen in unserem „Library Development Summary“, welches unter anderem im Online-Hilfe-Verzeichnis einer CODESYS-Installation zu finden ist (LibDevSummary.chm)""
      /// 
      /// (!)Above conditions are fulfilled. But keep in mind that is class is responsible only to fix placeholders for outdated R1 DTPs.
      /// It cannot be used in general(!)
      /// 
#pragma warning disable 1574
        /// The method <see cref=""ConvertLibsToPlaceHoldersRecursive""/> was added as an additional work around. 
#pragma warning restore 1574
        /// WAGO started to reference only libraries that are needed for device in the device description 
        /// that is used to install the device to CODESYS. 
        /// When a device is updated to this new device version
        /// only the libraries directly referenced or by placeholder are updated to their new version in the 
        /// corresponding library manager. 
        /// This method updates the further (indirectly referenced) libraries to this version.  
        /// In the near future this job will be done by CODESYS, and the workaround can be removed again.   
        /// 
        /// </summary>");

        protected override string GetDiagnosticId() => MiKo_2000_MalformedDocumentationAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_2000_MalformedDocumentationAnalyzer();
    }
}