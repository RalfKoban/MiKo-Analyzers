﻿using NUnit.Framework;

namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public static class VerbalizerTests
    {
        [TestCase(null, ExpectedResult = null, Description = "There is no verb available")]
        [TestCase("", ExpectedResult = "", Description = "There is no verb available")]
        [TestCase(" ", ExpectedResult = " ", Description = "There is no verb available")]
        [TestCase("Action", ExpectedResult = "Action", Description = "There is no verb available")]
        [TestCase("Adaptation", ExpectedResult = "Adapt")]
        [TestCase("Adoption", ExpectedResult = "Adopt")]
        [TestCase("Analysis", ExpectedResult = "Analyze")]
        [TestCase("Aquisition", ExpectedResult = "Aquire")]
        [TestCase("Cancellation", ExpectedResult = "Cancel")]
        [TestCase("Caption", ExpectedResult = "Caption", Description = "There is no verb available")]
        [TestCase("Comparison", ExpectedResult = "Compare")]
        [TestCase("Configuration", ExpectedResult = "Configure")]
        [TestCase("Connection", ExpectedResult = "Connect")]
        [TestCase("Creation", ExpectedResult = "Create")]
        [TestCase("Description", ExpectedResult = "Describe")]
        [TestCase("Destination", ExpectedResult = "Destination", Description = "There is no verb available")]
        [TestCase("DoAction", ExpectedResult = "DoAction")]
        [TestCase("Documentation", ExpectedResult = "Document")]
        [TestCase("DoFunc", ExpectedResult = "DoFunction")]
        [TestCase("DoFunction", ExpectedResult = "DoFunction")]
        [TestCase("Estimation", ExpectedResult = "Estimate")]
        [TestCase("Exception", ExpectedResult = "Exception", Description = "The noun is most-probably meant in such case")]
        [TestCase("Func", ExpectedResult = "Function")]
        [TestCase("Function", ExpectedResult = "Function", Description = "There is no verb available")]
        [TestCase("HasAction", ExpectedResult = "HasAction")]
        [TestCase("Information", ExpectedResult = "Inform")]
        [TestCase("Initialisation", ExpectedResult = "Initialise")]
        [TestCase("Initialization", ExpectedResult = "Initialize")]
        [TestCase("Inquisition", ExpectedResult = "Inquire")]
        [TestCase("Installation", ExpectedResult = "Install")]
        [TestCase("IsRelevantAction", ExpectedResult = "IsRelevantAction")]
        [TestCase("IsRelevantFunc", ExpectedResult = "IsRelevantFunc")]
        [TestCase("IsRelevantFunction", ExpectedResult = "IsRelevantFunction")]
        [TestCase("Location", ExpectedResult = "Locate")]
        [TestCase("Manipulation", ExpectedResult = "Manipulate")]
        [TestCase("Registration", ExpectedResult = "Register")]
        [TestCase("Stabilization", ExpectedResult = "Stabilize")]
        [TestCase("Subtraction", ExpectedResult = "Subtract")]
        [TestCase("Uninstallation", ExpectedResult = "Uninstall")]
        public static string TryMakeVerb_finds_proper_verb_(string name)
        {
            Verbalizer.TryMakeVerb(name, out var result);

            return result;
        }

        [TestCase("adapts", ExpectedResult = "adapt")]
        [TestCase("adopts", ExpectedResult = "adopt")]
        [TestCase("caches", ExpectedResult = "cache")]
        [TestCase("continues", ExpectedResult = "continue")]
        [TestCase("does", ExpectedResult = "do")]
        [TestCase("hashes", ExpectedResult = "hash")]
        [TestCase("informs", ExpectedResult = "inform")]
        [TestCase("invites", ExpectedResult = "invite")]
        [TestCase("pops", ExpectedResult = "pop")]
        [TestCase("registers", ExpectedResult = "register")]
        [TestCase("tests", ExpectedResult = "test")]
        public static string MakeInfiniteVerb_finds_proper_infinite_verb_(string name) => Verbalizer.MakeInfiniteVerb(name);

        [TestCase("access", ExpectedResult = "accesses")]
        [TestCase("adapt", ExpectedResult = "adapts")]
        [TestCase("adopt", ExpectedResult = "adopts")]
        [TestCase("adopts", ExpectedResult = "adopts")]
        [TestCase("cache", ExpectedResult = "caches")]
        [TestCase("buzz", ExpectedResult = "buzzes")]
        [TestCase("buzzes", ExpectedResult = "buzzes")]
        [TestCase("caches", ExpectedResult = "caches")]
        [TestCase("continue", ExpectedResult = "continues")]
        [TestCase("display", ExpectedResult = "displays")]
        [TestCase("do", ExpectedResult = "does")]
        [TestCase("does", ExpectedResult = "does")]
        [TestCase("hash", ExpectedResult = "hashes")]
        [TestCase("hashes", ExpectedResult = "hashes")]
        [TestCase("identify", ExpectedResult = "identifies")]
        [TestCase("inform", ExpectedResult = "informs")]
        [TestCase("informs", ExpectedResult = "informs")]
        [TestCase("invite", ExpectedResult = "invites")]
        [TestCase("invites", ExpectedResult = "invites")]
        [TestCase("mix", ExpectedResult = "mixes")]
        [TestCase("mixes", ExpectedResult = "mixes")]
        [TestCase("multiply", ExpectedResult = "multiplies")]
        [TestCase("pop", ExpectedResult = "pops")]
        [TestCase("pops", ExpectedResult = "pops")]
        [TestCase("register", ExpectedResult = "registers")]
        [TestCase("registers", ExpectedResult = "registers")]
        [TestCase("survey", ExpectedResult = "surveys")]
        [TestCase("test", ExpectedResult = "tests")]
        [TestCase("tests", ExpectedResult = "tests")]
        [TestCase("will", ExpectedResult = "will")]
        public static string MakeThirdPersonSingularVerb_finds_proper_3rd_person_singular_verb_(string name) => Verbalizer.MakeThirdPersonSingularVerb(name);

        [TestCase("access", ExpectedResult = false)]
        [TestCase("accesses", ExpectedResult = true)]
        [TestCase("adapts", ExpectedResult = true)]
        [TestCase("adopts", ExpectedResult = true)]
        [TestCase("caches", ExpectedResult = true)]
        [TestCase("continues", ExpectedResult = true)]
        [TestCase("does", ExpectedResult = true)]
        [TestCase("hashes", ExpectedResult = true)]
        [TestCase("informs", ExpectedResult = true)]
        [TestCase("invites", ExpectedResult = true)]
        [TestCase("pops", ExpectedResult = true)]
        [TestCase("registers", ExpectedResult = true)]
        [TestCase("test", ExpectedResult = false)]
        [TestCase("tests", ExpectedResult = true)]
        [TestCase("will", ExpectedResult = true)]
        public static bool IsThirdPersonSingularVerb_detects_3rd_person_singular_verb_(string name) => Verbalizer.IsThirdPersonSingularVerb(name);

        [TestCase("adapting", ExpectedResult = "adapting")]
        [TestCase("adapt", ExpectedResult = "adapting")]
        [TestCase("continue", ExpectedResult = "continuing")]
        [TestCase("invite", ExpectedResult = "inviting")]
        [TestCase("wrap", ExpectedResult = "wrapping")]
        public static string MakeGerundVerb_finds_proper_gerund_verb_(string name) => Verbalizer.MakeGerundVerb(name);
    }
}