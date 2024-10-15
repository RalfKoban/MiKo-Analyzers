using System;

using NUnit.Framework;

//// ncrunch: rdi off
namespace MiKoSolutions.Analyzers.Linguistics
{
    [TestFixture]
    public static class VerbalizerTests
    {
        private static readonly string[] Nouns =
                                                 [
                                                     "awakening",
                                                     "awning",
                                                     "blessing",
                                                     "booking",
                                                     "briefing",
                                                     "building",
                                                     "ceiling",
                                                     "darling",
                                                     "dealing",
                                                     "drawing",
                                                     "duckling",
                                                     "evening",
                                                     "feeling",
                                                     "finding",
                                                     "fledgling",
                                                     "gathering",
                                                     "guttering",
                                                     "hireling",
                                                     "inkling",
                                                     "leaning",
                                                     "meeting",
                                                     "misgiving",
                                                     "misunderstanding",
                                                     "morning",
                                                     "offering",
                                                     "outing",
                                                     "painting",
                                                     "quisling",
                                                     "reasoning",
                                                     "recording",
                                                     "restructuring",
                                                     "rising",
                                                     "roofing",
                                                     "sapling",
                                                     "seasoning",
                                                     "seating",
                                                     "setting",
                                                     "shooting",
                                                     "shopping",
                                                     "sibling",
                                                     "sitting",
                                                     "standing",
                                                     "tiding",
                                                     "timing",
                                                     "training",
                                                     "underling",
                                                     "understanding",
                                                     "undertaking",
                                                     "upbringing",
                                                     "uprising",
                                                     "warning",
                                                     "wedding",
                                                     "well-being",
                                                     "winning",
                                                     "wording",
                                                 ];

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
        [TestCase("Invocation", ExpectedResult = "Invoke")]
        [TestCase("IsRelevantAction", ExpectedResult = "IsRelevantAction")]
        [TestCase("IsRelevantFunc", ExpectedResult = "IsRelevantFunc")]
        [TestCase("IsRelevantFunction", ExpectedResult = "IsRelevantFunction")]
        [TestCase("Location", ExpectedResult = "Locate")]
        [TestCase("Manipulation", ExpectedResult = "Manipulate")]
        [TestCase("Registration", ExpectedResult = "Register")]
        [TestCase("Revocation", ExpectedResult = "Revoke")]
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
        [TestCase("will", ExpectedResult = "will")]
        [TestCase("is", ExpectedResult = "be")]
        [TestCase("Is", ExpectedResult = "Be")]
        [TestCase("are", ExpectedResult = "be")]
        [TestCase("Are", ExpectedResult = "Be")]
        [TestCase("be", ExpectedResult = "be")]
        [TestCase("Be", ExpectedResult = "Be")]
        [TestCase("has", ExpectedResult = "have")]
        [TestCase("Has", ExpectedResult = "Have")]
        [TestCase("acting", ExpectedResult = "act")]
        [TestCase("accessing", ExpectedResult = "access")]
        [TestCase("adapting", ExpectedResult = "adapt")]
        [TestCase("adopting", ExpectedResult = "adopt")]
        [TestCase("blazing", ExpectedResult = "blaze")]
        [TestCase("bubbling", ExpectedResult = "bubble")]
        [TestCase("challenging", ExpectedResult = "challenge")]
        [TestCase("continuing", ExpectedResult = "continue")]
        [TestCase("correcting", ExpectedResult = "correct")]
        [TestCase("fascinating", ExpectedResult = "fascinate")]
        [TestCase("gambling", ExpectedResult = "gamble")]
        [TestCase("gleaming", ExpectedResult = "gleam")]
        [TestCase("having", ExpectedResult = "have")]
        [TestCase("Having", ExpectedResult = "Have")]
        [TestCase("hanging", ExpectedResult = "hang")]
        [TestCase("inviting", ExpectedResult = "invite")]
        [TestCase("occupying", ExpectedResult = "occupy")]
        [TestCase("occurring", ExpectedResult = "occur")]
        [TestCase("raging", ExpectedResult = "rage")]
        [TestCase("refreshing", ExpectedResult = "refresh")]
        [TestCase("scintillating", ExpectedResult = "scintillate")]
        [TestCase("shutting", ExpectedResult = "shut")]
        [TestCase("Shutting", ExpectedResult = "Shut")]
        [TestCase("sparkling", ExpectedResult = "sparkle")]
        [TestCase("using", ExpectedResult = "use")]
        [TestCase("wrapping", ExpectedResult = "wrap")]
        [TestCase("bing", ExpectedResult = "bing")]
        [TestCase("king", ExpectedResult = "king")]
        [TestCase("ming", ExpectedResult = "ming")]
        [TestCase("pinging", ExpectedResult = "ping")]
        [TestCase("ringing", ExpectedResult = "ring")]
        [TestCase("singing", ExpectedResult = "sing")]
        [TestCase("ping", ExpectedResult = "ping")]
        [TestCase("ring", ExpectedResult = "ring")]
        [TestCase("sing", ExpectedResult = "sing")]
        [TestCase("thing", ExpectedResult = "thing")]
        [TestCase("pings", ExpectedResult = "ping")]
        [TestCase("rings", ExpectedResult = "ring")]
        [TestCase("sings", ExpectedResult = "sing")]
        [TestCase("access", ExpectedResult = "access")]
        [TestCase("suppress", ExpectedResult = "suppress")]
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
        [TestCase("are", ExpectedResult = "is")]
        [TestCase("Are", ExpectedResult = "Is")]
        [TestCase("be", ExpectedResult = "is")]
        [TestCase("Be", ExpectedResult = "Is")]
        [TestCase("have", ExpectedResult = "has")]
        [TestCase("Have", ExpectedResult = "Has")]
        [TestCase("shutdown", ExpectedResult = "shuts down")]
        [TestCase("Shutdown", ExpectedResult = "Shuts down")]
        [TestCase("added", ExpectedResult = "adds")]
        [TestCase("automated", ExpectedResult = "automates")]
        [TestCase("handled", ExpectedResult = "handles")]
        [TestCase("used", ExpectedResult = "uses")]
        [TestCase("sliced", ExpectedResult = "slices")]
        [TestCase("adopted", ExpectedResult = "adopts")]
        [TestCase("constructed", ExpectedResult = "constructs")]
        [TestCase("submitted", ExpectedResult = "submits")]
        [TestCase("need", ExpectedResult = "needs")]
        [TestCase("needed", ExpectedResult = "needs")]
        [TestCase("merged", ExpectedResult = "merges")]
        [TestCase("marked", ExpectedResult = "marks")]
        [TestCase("acquired", ExpectedResult = "acquires")]
        [TestCase("lied", ExpectedResult = "lies")]
        [TestCase("pinging", ExpectedResult = "pings")]
        [TestCase("ringing", ExpectedResult = "rings")]
        [TestCase("singing", ExpectedResult = "sings")]
        public static string MakeThirdPersonSingularVerb_finds_proper_3rd_person_singular_verb_(string name) => Verbalizer.MakeThirdPersonSingularVerb(name);

        [TestCase("access", ExpectedResult = false)]
        [TestCase("accesses", ExpectedResult = true)]
        [TestCase("adapts", ExpectedResult = true)]
        [TestCase("adopter", ExpectedResult = false)]
        [TestCase("adopters", ExpectedResult = false)]
        [TestCase("adopts", ExpectedResult = true)]
        [TestCase("alters", ExpectedResult = true)]
        [TestCase("caches", ExpectedResult = true)]
        [TestCase("catchers", ExpectedResult = false)]
        [TestCase("catches", ExpectedResult = true)]
        [TestCase("colors", ExpectedResult = true)]
        [TestCase("continues", ExpectedResult = true)]
        [TestCase("does", ExpectedResult = true)]
        [TestCase("hashes", ExpectedResult = true)]
        [TestCase("informs", ExpectedResult = true)]
        [TestCase("invites", ExpectedResult = true)]
        [TestCase("manage", ExpectedResult = false)]
        [TestCase("manager", ExpectedResult = false)]
        [TestCase("managers", ExpectedResult = false)]
        [TestCase("manages", ExpectedResult = true)]
        [TestCase("merge", ExpectedResult = false)]
        [TestCase("mergers", ExpectedResult = false)]
        [TestCase("merges", ExpectedResult = true)]
        [TestCase("pops", ExpectedResult = true)]
        [TestCase("registers", ExpectedResult = true)]
        [TestCase("runs", ExpectedResult = true)]
        [TestCase("test", ExpectedResult = false)]
        [TestCase("tests", ExpectedResult = true)]
        [TestCase("transcriptors", ExpectedResult = false)]
        [TestCase("transistors", ExpectedResult = false)]
        [TestCase("will", ExpectedResult = true)]
        public static bool IsThirdPersonSingularVerb_detects_3rd_person_singular_verb_(string name) => Verbalizer.IsThirdPersonSingularVerb(name);

        [TestCase("act", ExpectedResult = "acting")]
        [TestCase("access", ExpectedResult = "accessing")]
        [TestCase("adapt", ExpectedResult = "adapting")]
        [TestCase("adapting", ExpectedResult = "adapting")]
        [TestCase("adopt", ExpectedResult = "adopting")]
        [TestCase("are", ExpectedResult = "are")]
        [TestCase("blaze", ExpectedResult = "blazing")]
        [TestCase("bubble", ExpectedResult = "bubbling")]
        [TestCase("challenge", ExpectedResult = "challenging")]
        [TestCase("continue", ExpectedResult = "continuing")]
        [TestCase("correct", ExpectedResult = "correcting")]
        [TestCase("fascinate", ExpectedResult = "fascinating")]
        [TestCase("gamble", ExpectedResult = "gambling")]
        [TestCase("gleam", ExpectedResult = "gleaming")]
        [TestCase("has", ExpectedResult = "having")]
        [TestCase("Has", ExpectedResult = "Having")]
        [TestCase("Hang", ExpectedResult = "Hanging")]
        [TestCase("invite", ExpectedResult = "inviting")]
        [TestCase("is", ExpectedResult = "is")]
        [TestCase("occupy", ExpectedResult = "occupying")]
        [TestCase("occur", ExpectedResult = "occurring")]
        [TestCase("rage", ExpectedResult = "raging")]
        [TestCase("scintillate", ExpectedResult = "scintillating")]
        [TestCase("shut", ExpectedResult = "shutting")]
        [TestCase("Shut", ExpectedResult = "Shutting")]
        [TestCase("shutdown", ExpectedResult = "shutting down")]
        [TestCase("Shutdown", ExpectedResult = "Shutting down")]
        [TestCase("sparkle", ExpectedResult = "sparkling")]
        [TestCase("use", ExpectedResult = "using")]
        [TestCase("wrap", ExpectedResult = "wrapping")]
        [TestCase("bing", ExpectedResult = "bing")]
        [TestCase("king", ExpectedResult = "king")]
        [TestCase("ming", ExpectedResult = "ming")]
        [TestCase("thing", ExpectedResult = "thing")]
        [TestCase("ping", ExpectedResult = "pinging")]
        [TestCase("ring", ExpectedResult = "ringing")]
        [TestCase("sing", ExpectedResult = "singing")]
        [TestCase("something", ExpectedResult = "something")]
        [TestCase("anything", ExpectedResult = "anything")]
        [TestCase("everything", ExpectedResult = "everything")]
        [TestCase("Something", ExpectedResult = "Something")]
        [TestCase("Anything", ExpectedResult = "Anything")]
        [TestCase("Everything", ExpectedResult = "Everything")]
        public static string MakeGerundVerb_finds_proper_gerund_verb_(string name) => Verbalizer.MakeGerundVerb(name);

        [TestCase("false", ExpectedResult = false)]
        [TestCase("true", ExpectedResult = false)]
        [TestCase("completed", ExpectedResult = false)]
        [TestCase("bing", ExpectedResult = false)]
        [TestCase("king", ExpectedResult = false)]
        [TestCase("ming", ExpectedResult = false)]
        [TestCase("ping", ExpectedResult = false)]
        [TestCase("sing", ExpectedResult = false)]
        [TestCase("ring", ExpectedResult = false)]
        [TestCase("thing", ExpectedResult = false)]
        [TestCase("Thing", ExpectedResult = false)]
        [TestCase("something", ExpectedResult = false)]
        [TestCase("Something", ExpectedResult = false)]
        [TestCase("anything", ExpectedResult = false)]
        [TestCase("Anything", ExpectedResult = false)]
        [TestCase("everything", ExpectedResult = false)]
        [TestCase("Everything", ExpectedResult = false)]
        [TestCase("pinging", ExpectedResult = true)]
        [TestCase("Pinging", ExpectedResult = true)]
        [TestCase("singing", ExpectedResult = true)]
        [TestCase("Singing", ExpectedResult = true)]
        [TestCase("ringing", ExpectedResult = true)]
        [TestCase("Ringing", ExpectedResult = true)]
        [TestCase("challenging", ExpectedResult = true)]
        [TestCase("Challenging", ExpectedResult = true)]
        [TestCase("shutting", ExpectedResult = true)]
        [TestCase("Shutting", ExpectedResult = true)]
        public static bool IsGerundVerb_detects_gerund_verb_(string name) => Verbalizer.IsGerundVerb(name);

        [Test]
        public static void IsGerundVerb_detects_noun_([ValueSource(nameof(Nouns))] string name)
        {
            Assert.Multiple(() =>
                                 {
                                     Assert.That(Verbalizer.IsGerundVerb(name), Is.False, "lower-case not detected");
                                     Assert.That(Verbalizer.IsGerundVerb(name.ToUpperCaseAt(0)), Is.False, "upper-case not detected");
                                 });
        }

        [TestCase("Canceled", ExpectedResult = true)]
        [TestCase("Cancel", ExpectedResult = false)]
        [TestCase("Canceling", ExpectedResult = false)]
        public static bool IsPastTense_detects_past_tensed_word_(string name) => Verbalizer.IsPastTense(name);
    }
}