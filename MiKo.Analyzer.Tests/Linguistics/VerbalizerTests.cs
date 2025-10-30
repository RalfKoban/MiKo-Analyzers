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
        [TestCase("Composition", ExpectedResult = "Compose")]
        [TestCase("Configuration", ExpectedResult = "Configure")]
        [TestCase("Connection", ExpectedResult = "Connect")]
        [TestCase("Creation", ExpectedResult = "Create")]
        [TestCase("Description", ExpectedResult = "Describe")]
        [TestCase("Destination", ExpectedResult = "Destination", Description = "There is no verb available")]
        [TestCase("Disposition", ExpectedResult = "Dispose")]
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
        [TestCase("Inspiration", ExpectedResult = "Inspire")]
        [TestCase("Inquisition", ExpectedResult = "Inquire")]
        [TestCase("Installation", ExpectedResult = "Install")]
        [TestCase("Invocation", ExpectedResult = "Invoke")]
        [TestCase("IsRelevantAction", ExpectedResult = "IsRelevantAction")]
        [TestCase("IsRelevantFunc", ExpectedResult = "IsRelevantFunc")]
        [TestCase("IsRelevantFunction", ExpectedResult = "IsRelevantFunction")]
        [TestCase("Location", ExpectedResult = "Locate")]
        [TestCase("Manipulation", ExpectedResult = "Manipulate")]
        [TestCase("Operation", ExpectedResult = "Operate")]
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

        [TestCase("about", ExpectedResult = "about")]
        [TestCase("absent", ExpectedResult = "absent")]
        [TestCase("abstract", ExpectedResult = "abstract")]
        [TestCase("adapt", ExpectedResult = "adapt")]
        [TestCase("adopt", ExpectedResult = "adopt")]
        [TestCase("argument", ExpectedResult = "argument")]
        [TestCase("bent", ExpectedResult = "bend")]
        [TestCase("built", ExpectedResult = "build")]
        [TestCase("burnt", ExpectedResult = "burn")]
        [TestCase("collect", ExpectedResult = "collect")]
        [TestCase("congruent", ExpectedResult = "congruent")]
        [TestCase("construct", ExpectedResult = "construct")]
        [TestCase("current", ExpectedResult = "current")]
        [TestCase("dealt", ExpectedResult = "deal")]
        [TestCase("dreamt", ExpectedResult = "dream")]
        [TestCase("felt", ExpectedResult = "feel")]
        [TestCase("format", ExpectedResult = "format")]
        [TestCase("get", ExpectedResult = "get")]
        [TestCase("Get", ExpectedResult = "Get")]
        [TestCase("int", ExpectedResult = "int")]
        [TestCase("invert", ExpectedResult = "invert")]
        [TestCase("just", ExpectedResult = "just")]
        [TestCase("kept", ExpectedResult = "keep")]
        [TestCase("leapt", ExpectedResult = "leap")]
        [TestCase("learnt", ExpectedResult = "learn")]
        [TestCase("lent", ExpectedResult = "lend")]
        [TestCase("meant", ExpectedResult = "mean")]
        [TestCase("not", ExpectedResult = "not")]
        [TestCase("object", ExpectedResult = "object")]
        [TestCase("prevent", ExpectedResult = "prevent")]
        [TestCase("reject", ExpectedResult = "reject")]
        [TestCase("rent", ExpectedResult = "rent")]
        [TestCase("report", ExpectedResult = "report")]
        [TestCase("request", ExpectedResult = "request")]
        [TestCase("reset", ExpectedResult = "reset")]
        [TestCase("select", ExpectedResult = "select")]
        [TestCase("sent", ExpectedResult = "send")]
        [TestCase("set", ExpectedResult = "set")]
        [TestCase("Set", ExpectedResult = "Set")]
        [TestCase("slept", ExpectedResult = "sleep")]
        [TestCase("smelt", ExpectedResult = "smell")]
        [TestCase("sort", ExpectedResult = "sort")]
        [TestCase("spent", ExpectedResult = "spend")]
        [TestCase("start", ExpectedResult = "start")]
        [TestCase("swept", ExpectedResult = "sweep")]
        [TestCase("that", ExpectedResult = "that")]
        [TestCase("unit", ExpectedResult = "unit")]
        [TestCase("wept", ExpectedResult = "weep")]
        public static string MakeInfiniteVerb_finds_proper_infinite_verb_if_verb_ends_with_t_(string name) => Verbalizer.MakeInfiniteVerb(name);

        [TestCase("access", ExpectedResult = "accesses")]
        [TestCase("acquired", ExpectedResult = "acquires")]
        [TestCase("adapt", ExpectedResult = "adapts")]
        [TestCase("added", ExpectedResult = "adds")]
        [TestCase("adopt", ExpectedResult = "adopts")]
        [TestCase("adopted", ExpectedResult = "adopts")]
        [TestCase("adopts", ExpectedResult = "adopts")]
        [TestCase("are", ExpectedResult = "is")]
        [TestCase("Are", ExpectedResult = "Is")]
        [TestCase("automated", ExpectedResult = "automates")]
        [TestCase("authorized", ExpectedResult = "authorizes")]
        [TestCase("be", ExpectedResult = "is")]
        [TestCase("Be", ExpectedResult = "Is")]
        [TestCase("buzz", ExpectedResult = "buzzes")]
        [TestCase("buzzes", ExpectedResult = "buzzes")]
        [TestCase("cache", ExpectedResult = "caches")]
        [TestCase("caches", ExpectedResult = "caches")]
        [TestCase("constructed", ExpectedResult = "constructs")]
        [TestCase("continue", ExpectedResult = "continues")]
        [TestCase("determine", ExpectedResult = "determines")]
        [TestCase("determines", ExpectedResult = "determines")]
        [TestCase("determining", ExpectedResult = "determines")]
        [TestCase("display", ExpectedResult = "displays")]
        [TestCase("do", ExpectedResult = "does")]
        [TestCase("does", ExpectedResult = "does")]
        [TestCase("frozen", ExpectedResult = "freezes")]
        [TestCase("handled", ExpectedResult = "handles")]
        [TestCase("has", ExpectedResult = "has")]
        [TestCase("hash", ExpectedResult = "hashes")]
        [TestCase("hashes", ExpectedResult = "hashes")]
        [TestCase("have", ExpectedResult = "has")]
        [TestCase("Have", ExpectedResult = "Has")]
        [TestCase("identify", ExpectedResult = "identifies")]
        [TestCase("inform", ExpectedResult = "informs")]
        [TestCase("informs", ExpectedResult = "informs")]
        [TestCase("invite", ExpectedResult = "invites")]
        [TestCase("invites", ExpectedResult = "invites")]
        [TestCase("is", ExpectedResult = "is")]
        [TestCase("lied", ExpectedResult = "lies")]
        [TestCase("maintain", ExpectedResult = "maintains")]
        [TestCase("maintaining", ExpectedResult = "maintains")]
        [TestCase("maintains", ExpectedResult = "maintains")]
        [TestCase("marked", ExpectedResult = "marks")]
        [TestCase("merged", ExpectedResult = "merges")]
        [TestCase("mix", ExpectedResult = "mixes")]
        [TestCase("mixes", ExpectedResult = "mixes")]
        [TestCase("multiply", ExpectedResult = "multiplies")]
        [TestCase("need", ExpectedResult = "needs")]
        [TestCase("needed", ExpectedResult = "needs")]
        [TestCase("pinging", ExpectedResult = "pings")]
        [TestCase("pop", ExpectedResult = "pops")]
        [TestCase("pops", ExpectedResult = "pops")]
        [TestCase("register", ExpectedResult = "registers")]
        [TestCase("registers", ExpectedResult = "registers")]
        [TestCase("represent", ExpectedResult = "represents")]
        [TestCase("representing", ExpectedResult = "represents")]
        [TestCase("represents", ExpectedResult = "represents")]
        [TestCase("ringing", ExpectedResult = "rings")]
        [TestCase("shutdown", ExpectedResult = "shuts down")]
        [TestCase("Shutdown", ExpectedResult = "Shuts down")]
        [TestCase("singing", ExpectedResult = "sings")]
        [TestCase("sliced", ExpectedResult = "slices")]
        [TestCase("submitted", ExpectedResult = "submits")]
        [TestCase("survey", ExpectedResult = "surveys")]
        [TestCase("test", ExpectedResult = "tests")]
        [TestCase("tests", ExpectedResult = "tests")]
        [TestCase("used", ExpectedResult = "uses")]
        [TestCase("will", ExpectedResult = "will")]
        [TestCase("implementation", ExpectedResult = "implements")]
        [TestCase("Implementation", ExpectedResult = "Implements")]
        [TestCase("maintenance", ExpectedResult = "maintains")]
        [TestCase("Maintenance", ExpectedResult = "Maintains")]
        [TestCase("invoked", ExpectedResult = "invokes")]
        [TestCase("Invoked", ExpectedResult = "Invokes")]
        public static string MakeThirdPersonSingularVerb_finds_proper_3rd_person_singular_verb_(string name) => Verbalizer.MakeThirdPersonSingularVerb(name);

        [TestCase("", ExpectedResult = "")]
        [TestCase("   ", ExpectedResult = "   ")]
        [TestCase("Maintains something whatever it takes", ExpectedResult = "Maintain something whatever it takes")]
        [TestCase("maintains something whatever it takes", ExpectedResult = "maintain something whatever it takes")]
        [TestCase(" Maintain something whatever it takes", ExpectedResult = " Maintain something whatever it takes")]
        [TestCase(" maintain something whatever it takes", ExpectedResult = " maintain something whatever it takes")]
        [TestCase("   Maintains something whatever it takes", ExpectedResult = "Maintain something whatever it takes")]
        [TestCase("   maintains something whatever it takes", ExpectedResult = "maintain something whatever it takes")]
        [TestCase("Was something", ExpectedResult = "Be something")]
        [TestCase("was something", ExpectedResult = "be something")]
        [TestCase("Are something", ExpectedResult = "Be something")]
        [TestCase("are something", ExpectedResult = "be something")]
        [TestCase("Done something", ExpectedResult = "Do something")]
        [TestCase("done something", ExpectedResult = "do something")]
        [TestCase("Frozen something", ExpectedResult = "Freeze something")]
        [TestCase("frozen something", ExpectedResult = "freeze something")]
        [TestCase("Broken something", ExpectedResult = "Break something")]
        [TestCase("broken something", ExpectedResult = "break something")]
        [TestCase("Chosen something", ExpectedResult = "Choose something")]
        [TestCase("chosen something", ExpectedResult = "choose something")]
        [TestCase("Wrote something", ExpectedResult = "Write something")]
        [TestCase("wrote something", ExpectedResult = "write something")]
        [TestCase("Written something", ExpectedResult = "Write something")]
        [TestCase("written something", ExpectedResult = "write something")]
        [TestCase("Woke something", ExpectedResult = "Wake something")]
        [TestCase("woke something", ExpectedResult = "wake something")]
        [TestCase("Woken something", ExpectedResult = "Wake something")]
        [TestCase("woken something", ExpectedResult = "wake something")]
        [TestCase("Driven something", ExpectedResult = "Drive something")]
        [TestCase("driven something", ExpectedResult = "drive something")]
        [TestCase("Drove something", ExpectedResult = "Drive something")]
        [TestCase("drove something", ExpectedResult = "drive something")]
        [TestCase("Hid something", ExpectedResult = "Hide something")]
        [TestCase("hid something", ExpectedResult = "hide something")]
        [TestCase("Hidden something", ExpectedResult = "Hide something")]
        [TestCase("hidden something", ExpectedResult = "hide something")]
        public static string MakeFirstWordInfiniteVerb_finds_proper_infinite_verb_(string name) => Verbalizer.MakeFirstWordInfiniteVerb(name);

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