using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public static class VerbalizerTests
    {
        [TestCase(null, ExpectedResult = null, Description = "There is no verb available")]
        [TestCase("", ExpectedResult = "", Description = "There is no verb available")]
        [TestCase(" ", ExpectedResult = " ", Description = "There is no verb available")]
        [TestCase("Caption", ExpectedResult = "Caption", Description = "There is no verb available")]
        [TestCase("Destination", ExpectedResult = "Destination", Description = "There is no verb available")]
        [TestCase("Adaptation", ExpectedResult = "Adapt")]
        [TestCase("Adoption", ExpectedResult = "Adopt")]
        [TestCase("Comparison", ExpectedResult = "Compare")]
        [TestCase("Configuration", ExpectedResult = "Configure")]
        [TestCase("Connection", ExpectedResult = "Connect")]
        [TestCase("Creation", ExpectedResult = "Create")]
        [TestCase("Documentation", ExpectedResult = "Document")]
        [TestCase("Description", ExpectedResult = "Describe")]
        [TestCase("Estimation", ExpectedResult = "Estimate")]
        [TestCase("Exception", ExpectedResult = "Exception", Description = "The noun is most-probably meant in such case")]
        [TestCase("Information", ExpectedResult = "Inform")]
        [TestCase("Initialisation", ExpectedResult = "Initialise")]
        [TestCase("Initialization", ExpectedResult = "Initialize")]
        [TestCase("Installation", ExpectedResult = "Install")]
        [TestCase("Location", ExpectedResult = "Locate")]
        [TestCase("Manipulation", ExpectedResult = "Manipulate")]
        [TestCase("Registration", ExpectedResult = "Register")]
        [TestCase("Stabilization", ExpectedResult = "Stabilize")]
        [TestCase("Uninstallation", ExpectedResult = "Uninstall")]
        [TestCase("Action", ExpectedResult = "Action", Description = "There is no verb available")]
        [TestCase("DoAction", ExpectedResult = "DoAction")]
        [TestCase("HasAction", ExpectedResult = "HasAction")]
        [TestCase("IsRelevantAction", ExpectedResult = "IsRelevantAction")]
        [TestCase("Function", ExpectedResult = "Function", Description = "There is no verb available")]
        [TestCase("DoFunction", ExpectedResult = "DoFunction")]
        [TestCase("IsRelevantFunction", ExpectedResult = "IsRelevantFunction")]
        [TestCase("Func", ExpectedResult = "Function")]
        [TestCase("DoFunc", ExpectedResult = "DoFunction")]
        [TestCase("IsRelevantFunc", ExpectedResult = "IsRelevantFunc")]
        [TestCase("Analysis", ExpectedResult = "Analyze")]
        [TestCase("Subtraction", ExpectedResult = "Subtract")]
        [TestCase("Aquisition", ExpectedResult = "Aquire")]
        [TestCase("Inquisition", ExpectedResult = "Inquire")]
        public static string TryMakeVerb_finds_proper_verb_(string name)
        {
            Verbalizer.TryMakeVerb(name, out var result);

            return result;
        }

        [TestCase("tests", ExpectedResult = "test")]
        [TestCase("does", ExpectedResult = "do")]
        [TestCase("caches", ExpectedResult = "cache")]
        [TestCase("hashes", ExpectedResult = "hash")]
        [TestCase("invites", ExpectedResult = "invite")]
        [TestCase("adapts", ExpectedResult = "adapt")]
        [TestCase("adopts", ExpectedResult = "adopt")]
        [TestCase("informs", ExpectedResult = "inform")]
        [TestCase("registers", ExpectedResult = "register")]
        [TestCase("continues", ExpectedResult = "continue")]
        [TestCase("pops", ExpectedResult = "pop")]
        public static string MakeInfiniteVerb_finds_proper_infinite_verb_(string name) => Verbalizer.MakeInfiniteVerb(name);
    }
}