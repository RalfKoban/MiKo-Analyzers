using System.Text;

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

        private static readonly WordCombination[] Words =
                                                          [
                                                              new("access", "accessing", "accesses", "accessed", "accessed", "access"),
                                                              new("acquire", "acquiring", "acquires", "acquired", "acquired", "acquisition", "aquisition", "acquiration", "aquiration"),
                                                              new("act", "acting", "acts", "acted", "acted", "act"),
                                                              new("activate", "activating", "activates", "activated", "activated", "activation"),
                                                              new("adapt", "adapting", "adapts", "adapted", "adapted", "adaptation"),
                                                              new("add", "adding", "adds", "added", "added", "addition"),
                                                              new("admit", "admitting", "admits", "admitted", "admitted", "admission"),
                                                              new("adopt", "adopting", "adopts", "adopted", "adopted", "adoption", "adopter", "adopters"),
                                                              new("alter", "altering", "alters", "altered", "altered", "alteration"),
                                                              new("analyze", "analyzing", "analyzes", "analyzed", "analyzed", "analysis", "analyzer", "analyzers"),
                                                              new("apply", "applying", "applies", "applied", "applied", "application"),
                                                              new("authorize", "authorizing", "authorizes", "authorized", "authorized", "authorization"),
                                                              new("automate", "automating", "automates", "automated", "automated", "automation"),
                                                              new("ban", "banning", "bans", "banned", "banned", "ban"),
                                                              //// new("be", "being", "are", "were", "been"),
                                                              //// new("be", "being", "is", "was", "been"),
                                                              new("begin", "beginning", "begins", "began", "begun", "beginning"),
                                                              new("bend", "bending", "bends", "bent", "bent", "bend"),
                                                              new("benefit", "benefiting", "benefits", "benefited", "benefited", "benefit"),
                                                              new("blaze", "blazing", "blazes", "blazed", "blazed", "blaze"),
                                                              new("break", "breaking", "breaks", "broken", "broken", "break"),
                                                              new("bubble", "bubbling", "bubbles", "bubbled", "bubbled", "bubble"),
                                                              new("budget", "budgeting", "budgets", "budgeted", "budgeted", "budget"),
                                                              new("build", "building", "builds", "built", "built", "building"),
                                                              new("burn", "burning", "burns", "burnt", "burnt", "burn"),
                                                              new("buzz", "buzzing", "buzzes", "buzzed", "buzzed", "buzz"),
                                                              new("cache", "caching", "caches", "cached", "cached", "cache"),
                                                              new("calculate", "calculating", "calculates", "calculated", "calculated", "calculation"),
                                                              new("cancel", "canceling", "cancels", "canceled", "canceled", "cancellation"),
                                                              new("catch", "catching", "catches", "caught", "caught", "catch"),
                                                              new("challenge", "challenging", "challenges", "challenged", "challenged", "challenge"),
                                                              new("change", "changing", "changes", "changed", "changed", "change"),
                                                              new("choose", "choosing", "chooses", "chosen", "chosen", "choice"),
                                                              new("chop", "chopping", "chops", "chopped", "chopped", "chop"),
                                                              new("clone", "cloning", "clones", "cloned", "cloned", "clone"),
                                                              new("close", "closing", "closes", "closed", "closed", "closure"),
                                                              new("collect", "collecting", "collects", "collected", "collected", "collection"),
                                                              new("colorize", "colorizing", "colorizes", "colorized", "colorized", "color", "colors"),
                                                              new("commit", "committing", "commits", "committed", "committed", "commitment"),
                                                              new("compare", "comparing", "compares", "compared", "compared", "comparison"),
                                                              new("compel", "compelling", "compels", "compelled", "compelled", "compulsion"),
                                                              new("complete", "completing", "completes", "completed", "completed", "completion"),
                                                              new("compose", "composing", "composes", "composed", "composed", "composition"),
                                                              new("configure", "configuring", "configures", "configured", "configured", "configuration"),
                                                              new("connect", "connecting", "connects", "connected", "connected", "connection"),
                                                              new("conquer", "conquering", "conquers", "conquered", "conquered", "conquest"),
                                                              new("construct", "constructing", "constructs", "constructed", "constructed", "construction"),
                                                              new("continue", "continuing", "continues", "continued", "continued", "continuation"),
                                                              new("control", "controlling", "controls", "controlled", "controlled", "control"),
                                                              new("correct", "correcting", "corrects", "corrected", "corrected", "correction"),
                                                              new("create", "creating", "creates", "created", "created", "creation"),
                                                              new("cut", "cutting", "cuts", "cut", "cut", "cut"),
                                                              new("deal", "dealing", "deals", "dealt", "dealt", "dealing"),
                                                              new("debug", "debugging", "debugs", "debugged", "debugged"),
                                                              new("define", "defining", "defines", "defined", "defined", "definition"),
                                                              new("deliver", "delivering", "delivers", "delivered", "delivered", "delivery"),
                                                              new("describe", "describing", "describes", "described", "described", "description"),
                                                              new("determine", "determining", "determines", "determined", "determined", "determination"),
                                                              new("develop", "developing", "develops", "developed", "developed", "development"),
                                                              new("display", "displaying", "displays", "displayed", "displayed", "display"),
                                                              new("dispose", "disposing", "disposes", "disposed", "disposed", "disposition"),
                                                              new("do", "doing", "does", "done", "done"),
                                                              new("document", "documenting", "documents", "documented", "documented", "documentation"),
                                                              new("drag", "dragging", "drags", "dragged", "dragged", "drag"),
                                                              new("dream", "dreaming", "dreams", "dreamt", "dreamt", "dream"),
                                                              new("drive", "driving", "drives", "drove", "driven", "driver"),
                                                              new("drop", "dropping", "drops", "dropped", "dropped", "drop"),
                                                              new("edit", "editing", "edits", "edited", "edited", "edit"),
                                                              new("emit", "emitting", "emits", "emitted", "emitted", "emission"),
                                                              new("enter", "entering", "enters", "entered", "entered", "entry"),
                                                              new("equip", "equipping", "equips", "equipped", "equipped", "equipment"),
                                                              new("estimate", "estimating", "estimates", "estimated", "estimated", "estimation"),
                                                              new("exit", "exiting", "exits", "exited", "exited", "exit"),
                                                              //// new("except", "excepting", "excepts", "excepted", "excepted", "exception"),
                                                              new("expel", "expelling", "expels", "expelled", "expelled", "expulsion"),
                                                              new("export", "exporting", "exports", "exported", "exported", "export"),
                                                              new("extend", "extending", "extends", "extended", "extended", "extension"),
                                                              new("fascinate", "fascinating", "fascinates", "fascinated", "fascinated", "fascination"),
                                                              new("feel", "feeling", "feels", "felt", "felt", "feeling"),
                                                              new("finish", "finishing", "finishes", "finished", "finished", "finish"),
                                                              new("fire", "firing", "fires", "fired", "fired", "fire"),
                                                              new("fit", "fitting", "fits", "fitted", "fitted", "fit"),
                                                              new("fix", "fixing", "fixes", "fixed", "fixed", "fix"),
                                                              new("flag", "flagging", "flags", "flagged", "flagged", "flag"),
                                                              new("fly", "flying", "flies", "flew", "flown", "fly"),
                                                              new("fry", "frying", "fries", "fried", "fried", "fry"),
                                                              new("forget", "forgetting", "forgets", "forgot", "forgotten"),
                                                              new("forgive", "forgiving", "forgives", "forgave", "forgiven"),
                                                              new("forge", "forging", "forges", "forged", "forged", "forge"),
                                                              new("fork", "forking", "forks", "forked", "forked", "fork"),
                                                              new("format", "formatting", "formats", "formatted", "formatted", "format"),
                                                              new("freeze", "freezing", "freezes", "frozen", "frozen", "freeze"),
                                                              new("frustrate", "frustrating", "frustrates", "frustrated", "frustrated", "frustration"),
                                                              new("gain", "gaining", "gains", "gained", "gained", "gain"),
                                                              new("gamble", "gambling", "gambles", "gambled", "gambled", "gamble"),
                                                              new("gather", "gathering", "gathers", "gathered", "gathered", "gathering"),
                                                              new("get", "getting", "gets", "got", "got"),
                                                              new("give", "giving", "gives", "gave", "given"),
                                                              new("gleam", "gleaming", "gleams", "gleamed", "gleamed", "gleam"),
                                                              new("grab", "grabbing", "grabs", "grabbed", "grabbed", "grab", "grabber", "grabbers"),
                                                              new("grey", "greying", "greys", "greyed", "greyed", "grey"),
                                                              new("handle", "handling", "handles", "handled", "handled", "handle"),
                                                              new("hang", "hanging", "hangs", "hung", "hung"),
                                                              new("happen", "happening", "happens", "happened", "happened"),
                                                              new("hash", "hashing", "hashes", "hashed", "hashed", "hash"),
                                                              new("have", "having", "has", "had", "had"),
                                                              new("heat", "heating", "heats", "heated", "heated", "heat"),
                                                              new("help", "helping", "helps", "helped", "helped", "help"),
                                                              new("hide", "hiding", "hides", "hid", "hidden", "hide"),
                                                              new("hit", "hitting", "hits", "hit", "hit", "hit"),
                                                              new("identify", "identifying", "identifies", "identified", "identified", "identification"),
                                                              new("illustrate", "illustrating", "illustrates", "illustrated", "illustrated", "illustration"),
                                                              new("implement", "implementing", "implements", "implemented", "implemented", "implementation", "implementations", "implementer", "implementers"),
                                                              new("import", "importing", "imports", "imported", "imported", "import"),
                                                              new("influence", "influencing", "influences", "influenced", "influenced", "influence", "influencer", "influencers"),
                                                              new("inform", "informing", "informs", "informed", "informed", "information"),
                                                              new("initialize", "initializing", "initializes", "initialized", "initialized", "initialization"),
                                                              new("input", "inputting", "inputs", "input", "input", "input"),
                                                              new("inquire", "inquiring", "inquires", "inquired", "inquired", "inquiry", "inquisition"),
                                                              new("inspire", "inspiring", "inspires", "inspired", "inspired", "inspiration"),
                                                              new("install", "installing", "installs", "installed", "installed", "installation"),
                                                              new("invert", "inverting", "inverts", "inverted", "inverted", "inversion"),
                                                              new("invite", "inviting", "invites", "invited", "invited", "invitation"),
                                                              new("invoke", "invoking", "invokes", "invoked", "invoked", "invocation"),
                                                              new("join", "joining", "joins", "joined", "joined"),
                                                              new("jot", "jotting", "jots", "jotted", "jotted", "jot"),
                                                              new("keep", "keeping", "keeps", "kept", "kept", "keep"),
                                                              new("knit", "knitting", "knits", "knitted", "knitted"),
                                                              new("label", "labeling", "labels", "labeled", "labeled", "label"),
                                                              new("leap", "leaping", "leaps", "leapt", "leapt", "leap"),
                                                              new("learn", "learning", "learns", "learnt", "learnt"),
                                                              new("lend", "lending", "lends", "lent", "lent"),
                                                              new("let", "letting", "lets", "let", "let"),
                                                              new("lie", "lying", "lies", "lied", "lied", "lie"),
                                                              new("limit", "limiting", "limits", "limited", "limited", "limit"),
                                                              new("listen", "listening", "listens", "listened", "listened"),
                                                              new("load", "loading", "loads", "loaded", "loaded", "load"),
                                                              new("locate", "locating", "locates", "located", "located", "location"),
                                                              new("log", "logging", "logs", "logged", "logged", "log"),
                                                              new("maintain", "maintaining", "maintains", "maintained", "maintained", "maintenance"),
                                                              new("manage", "managing", "manages", "managed", "managed", "management", "managements", "manager", "managers"),
                                                              new("manipulate", "manipulating", "manipulates", "manipulated", "manipulated", "manipulation"),
                                                              new("map", "mapping", "maps", "mapped", "mapped", "map"),
                                                              new("mark", "marking", "marks", "marked", "marked", "mark"),
                                                              new("match", "matching", "matches", "matched", "matched", "match"),
                                                              new("mean", "meaning", "means", "meant", "meant", "means"),
                                                              new("merge", "merging", "merges", "merged", "merged", "merge", "merger", "mergers"),
                                                              new("mix", "mixing", "mixes", "mixed", "mixed", "mix"),
                                                              new("modernize", "modernizing", "modernizes", "modernized", "modernized", "modernization"),
                                                              new("modify", "modifying", "modifies", "modified", "modified", "modification"),
                                                              new("monitor", "monitoring", "monitors", "monitored", "monitored", "monitor"),
                                                              new("multiply", "multiplying", "multiplies", "multiplied", "multiplied", "multiplication"),
                                                              new("need", "needing", "needs", "needed", "needed", "need"),
                                                              //// new("object", "objecting", "objects", "objected", "objected", "objection"),
                                                              new("occupy", "occupying", "occupies", "occupied", "occupied", "occupation"),
                                                              new("occur", "occurring", "occurs", "occurred", "occurred", "occurrence"),
                                                              new("offer", "offering", "offers", "offered", "offered", "offer"),
                                                              new("offset", "offsetting", "offsets", "offset", "offset", "offset"),
                                                              new("omit", "omitting", "omits", "omitted", "omitted", "omission"),
                                                              new("open", "opening", "opens", "opened", "opened", "opening"),
                                                              new("operate", "operating", "operates", "operated", "operated", "operation"),
                                                              new("output", "outputting", "outputs", "output", "output", "output"),
                                                              new("pass", "passing", "passes", "passed", "passed", "pass"),
                                                              new("permit", "permitting", "permits", "permitted", "permitted", "permit"),
                                                              new("pin", "pinning", "pins", "pinned", "pinned", "pin"),
                                                              new("ping", "pinging", "pings", "pinged", "pinged", "ping"),
                                                              new("plan", "planning", "plans", "planned", "planned", "plan"),
                                                              new("play", "playing", "plays", "played", "played", "play"),
                                                              new("plug", "plugging", "plugs", "plugged", "plugged", "plug"),
                                                              new("pop", "popping", "pops", "popped", "popped", "pop"),
                                                              new("postpone", "postponing", "postpones", "postponed", "postponed", "postponement"),
                                                              new("pot", "potting", "pots", "potted", "potted", "pot"),
                                                              new("prefer", "preferring", "prefers", "preferred", "preferred", "preference"),
                                                              new("prevent", "preventing", "prevents", "prevented", "prevented", "prevention"),
                                                              new("put", "putting", "puts", "put", "put"),
                                                              new("quit", "quitting", "quits", "quit", "quit"),
                                                              new("rage", "raging", "rages", "raged", "raged", "rage"),
                                                              new("read", "reading", "reads", "read", "read", "read"),
                                                              new("reboot", "rebooting", "reboots", "rebooted", "rebooted", "reboot"),
                                                              new("recalculate", "recalculating", "recalculates", "recalculated", "recalculated", "recalculation"),
                                                              new("recap", "recapping", "recaps", "recapped", "recapped", "recap"),
                                                              new("recommend", "recommending", "recommends", "recommended", "recommended", "recommendation"),
                                                              new("reduce", "reducing", "reduces", "reduced", "reduced", "reduction"),
                                                              new("refactor", "refactoring", "refactors", "refactored", "refactored", "refactoring"),
                                                              new("refer", "referring", "refers", "referred", "referred", "reference"),
                                                              new("refresh", "refreshing", "refreshes", "refreshed", "refreshed", "refresh"),
                                                              new("register", "registering", "registers", "registered", "registered", "registration"),
                                                              new("regret", "regretting", "regrets", "regretted", "regretted", "regret"),
                                                              new("reject", "rejecting", "rejects", "rejected", "rejected", "rejection"),
                                                              new("remap", "remapping", "remaps", "remapped", "remapped"),
                                                              new("rent", "renting", "rents", "rented", "rented", "rent"),
                                                              new("repeat", "repeating", "repeats", "repeated", "repeated", "repetition"),
                                                              new("report", "reporting", "reports", "reported", "reported", "report"),
                                                              new("represent", "representing", "represents", "represented", "represented", "representation"),
                                                              new("request", "requesting", "requests", "requested", "requested", "request"),
                                                              new("require", "requiring", "requires", "required", "required", "requirement"),
                                                              new("reset", "resetting", "resets", "reset", "reset", "reset"),
                                                              new("revoke", "revoking", "revokes", "revoked", "revoked", "revocation"),
                                                              new("ring", "ringing", "rings", "rang", "rung", "ring"),
                                                              new("run", "running", "runs", "ran", "run", "run"),
                                                              new("scintillate", "scintillating", "scintillates", "scintillated", "scintillated", "scintillation"),
                                                              new("select", "selecting", "selects", "selected", "selected", "selection"),
                                                              new("send", "sending", "sends", "sent", "sent"),
                                                              new("set", "setting", "sets", "set", "set", "set"),
                                                              new("ship", "shipping", "ships", "shipped", "shipped", "ship"),
                                                              new("show", "showing", "shows", "showed", "shown", "show"),
                                                              new("shut", "shutting", "shuts", "shut", "shut"),
                                                              new("sing", "singing", "sings", "sang", "sung", "singer"),
                                                              new("skip", "skipping", "skips", "skipped", "skipped", "skip"),
                                                              new("sleep", "sleeping", "sleeps", "slept", "slept", "sleep"),
                                                              new("slice", "slicing", "slices", "sliced", "sliced", "slice"),
                                                              new("slip", "slipping", "slips", "slipped", "slipped", "slip"),
                                                              new("smell", "smelling", "smells", "smelt", "smelt", "smell"),
                                                              new("sort", "sorting", "sorts", "sorted", "sorted", "sort"),
                                                              new("sparkle", "sparkling", "sparkles", "sparkled", "sparkled", "sparkle"),
                                                              new("spend", "spending", "spends", "spent", "spent"),
                                                              new("spot", "spotting", "spots", "spotted", "spotted", "spot"),
                                                              new("stabilize", "stabilizing", "stabilizes", "stabilized", "stabilized", "stabilization"),
                                                              new("start", "starting", "starts", "started", "started", "start"),
                                                              new("step", "stepping", "steps", "stepped", "stepped", "step"),
                                                              new("stop", "stopping", "stops", "stopped", "stopped", "stop"),
                                                              new("strip", "stripping", "strips", "stripped", "stripped", "strip"),
                                                              new("submit", "submitting", "submits", "submitted", "submitted", "submission"),
                                                              new("subscribe", "subscribing", "subscribes", "subscribed", "subscribed", "subscription", "subscriber", "subscribers"),
                                                              new("subtract", "subtracting", "subtracts", "subtracted", "subtracted", "subtraction"),
                                                              new("succeed", "succeeding", "succeeds", "succeeded", "succeeded", "success", "successor", "successors"),
                                                              new("suppress", "suppressing", "suppresses", "suppressed", "suppressed", "suppression"),
                                                              new("survey", "surveying", "surveys", "surveyed", "surveyed", "survey"),
                                                              new("sweep", "sweeping", "sweeps", "swept", "swept", "sweep"),
                                                              new("tag", "tagging", "tags", "tagged", "tagged", "tag"),
                                                              new("tap", "tapping", "taps", "tapped", "tapped", "tap"),
                                                              new("target", "targeting", "targets", "targeted", "targeted", "target"),
                                                              new("test", "testing", "tests", "tested", "tested", "test"),
                                                              new("transcribe", "transcribing", "transcribes", "transcribed", "transcribed", "transcript", "transcriptor", "transcriptors"),
                                                              new("transmit", "transmitting", "transmits", "transmitted", "transmitted", "transmission"),
                                                              new("transport", "transporting", "transports", "transported", "transported", "transport", "transportation", "transporter", "transporters"),
                                                              new("travel", "traveling", "travels", "traveled", "traveled", "travel"),
                                                              new("trial", "trialing", "trials", "trialed", "trialed", "trial"),
                                                              new("trigger", "triggering", "triggers", "triggered", "triggered", "trigger"),
                                                              new("trim", "trimming", "trims", "trimmed", "trimmed", "trim"),
                                                              new("try", "trying", "tries", "tried", "tried", "try"),
                                                              new("uninstall", "uninstalling", "uninstalls", "uninstalled", "uninstalled", "uninstallation"),
                                                              new("unplug", "unplugging", "unplugs", "unplugged", "unplugged"),
                                                              new("unwrap", "unwrapping", "unwraps", "unwrapped", "unwrapped", "unwrapper", "unwrappers"),
                                                              new("unzip", "unzipping", "unzips", "unzipped", "unzipped", "unzipper", "unzippers"),
                                                              new("upset", "upsetting", "upsets", "upset", "upset", "upset"),
                                                              new("use", "using", "uses", "used", "used", "use"),
                                                              new("validate", "validating", "validates", "validated", "validated", "validation"),
                                                              new("verify", "verifying", "verifies", "verified", "verified", "verification"),
                                                              new("version", "versioning", "versions", "versioned", "versioned", "version"),
                                                              new("visit", "visiting", "visits", "visited", "visited", "visit"),
                                                              new("wait", "waiting", "waits", "waited", "waited", "wait"),
                                                              new("wake", "waking", "wakes", "woke", "woken", "wake"),
                                                              new("weep", "weeping", "weeps", "wept", "wept"),
                                                              new("will", "will", "will", "will", "will"),
                                                              new("wrap", "wrapping", "wraps", "wrapped", "wrapped", "wrapper", "wrappers"),
                                                              new("write", "writing", "writes", "wrote", "written"),
                                                              new("zip", "zipping", "zips", "zipped", "zipped", "zip"),
                                                          ];

        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase(" ", ExpectedResult = false)]
        [TestCase("Creating something", ExpectedResult = true)]
        [TestCase("creating something", ExpectedResult = true)]
        [TestCase(": Creating something", ExpectedResult = true)]
        [TestCase("Create something", ExpectedResult = false)]
        [TestCase("Creates something", ExpectedResult = false)]
        [TestCase("Created something", ExpectedResult = false)]
        public static bool IsFirstWordGerundVerb_detects_gerund_(string text) => Verbalizer.IsFirstWordGerundVerb(text);

        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase(" ", ExpectedResult = false)]
        [TestCase("Create something", ExpectedResult = true)]
        [TestCase("create something", ExpectedResult = true)]
        [TestCase(": Create something", ExpectedResult = true)]
        [TestCase("Creates something", ExpectedResult = false)]
        [TestCase("Created something", ExpectedResult = false)]
        [TestCase("Creating something", ExpectedResult = false)]
        public static bool IsFirstWordInfiniteVerb_detects_infinite_verb_(string text) => Verbalizer.IsFirstWordInfiniteVerb(text);

        [TestCase(null, ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        [TestCase(" ", ExpectedResult = false)]
        [TestCase("Created something", ExpectedResult = true)]
        [TestCase("created something", ExpectedResult = true)]
        [TestCase(": Created something", ExpectedResult = true)]
        [TestCase("Create something", ExpectedResult = false)]
        [TestCase("Creates something", ExpectedResult = false)]
        [TestCase("Creating something", ExpectedResult = false)]
        [TestCase("Opened something", ExpectedResult = true)]
        [TestCase("Opening something", ExpectedResult = false)]
        [TestCase("Open something", ExpectedResult = false)]
        [TestCase("Was something", ExpectedResult = true)]
        [TestCase("was something", ExpectedResult = true)]
        [TestCase("Were at something", ExpectedResult = true)]
        [TestCase("were at something", ExpectedResult = true)]
        public static bool IsFirstWordPastTenseVerb_detects_past_tense_(string text) => Verbalizer.IsFirstWordPastTenseVerb(text);

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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Verbalizer.IsGerundVerb(name), Is.False, "lower-case not detected");
                Assert.That(Verbalizer.IsGerundVerb(name.ToUpperCaseAt(0)), Is.False, "upper-case not detected");
            }
        }

        [TestCase("Canceled", ExpectedResult = true)]
        [TestCase("Cancel", ExpectedResult = false)]
        [TestCase("Canceling", ExpectedResult = false)]
        [TestCase("Finished", ExpectedResult = true)]
        [TestCase("Finish", ExpectedResult = false)]
        [TestCase("Finishing", ExpectedResult = false)]
        public static bool IsPastTense_detects_past_tensed_word_(string name) => Verbalizer.IsPastTenseVerb(name);

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
        [TestCase("factors", ExpectedResult = true)]
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
        [TestCase("monitors", ExpectedResult = true)]
        [TestCase("passes", ExpectedResult = true)]
        [TestCase("pops", ExpectedResult = true)]
        [TestCase("refactors", ExpectedResult = true)]
        [TestCase("registers", ExpectedResult = true)]
        [TestCase("runs", ExpectedResult = true)]
        [TestCase("siblings", ExpectedResult = false)]
        [TestCase("test", ExpectedResult = false)]
        [TestCase("tests", ExpectedResult = true)]
        [TestCase("transcriptors", ExpectedResult = false)]
        [TestCase("transistors", ExpectedResult = false)]
        [TestCase("triggers", ExpectedResult = true)]
        [TestCase("weaklings", ExpectedResult = false)]
        [TestCase("will", ExpectedResult = true)]
        public static bool IsThirdPersonSingularVerb_detects_3rd_person_singular_verb_(string name) => Verbalizer.IsThirdPersonSingularVerb(name);

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
        public static string MakeFirstWordInfiniteVerb_finds_matching_infinite_verb_(string name) => Verbalizer.MakeFirstWordInfiniteVerb(name);

        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = " ")]
        [TestCase("anything", ExpectedResult = "anything")]
        [TestCase("Anything", ExpectedResult = "Anything")]
        [TestCase("are", ExpectedResult = "are")]
        [TestCase("bing", ExpectedResult = "bing")]
        [TestCase("blaze", ExpectedResult = "blazing")]
        [TestCase("everything", ExpectedResult = "everything")]
        [TestCase("Everything", ExpectedResult = "Everything")]
        [TestCase("has", ExpectedResult = "having")]
        [TestCase("Has", ExpectedResult = "Having")]
        [TestCase("is", ExpectedResult = "is")]
        [TestCase("king", ExpectedResult = "king")]
        [TestCase("ming", ExpectedResult = "ming")]
        [TestCase("shutdown", ExpectedResult = "shuttingDown")]
        [TestCase("Shutdown", ExpectedResult = "ShuttingDown")]
        [TestCase("something", ExpectedResult = "something")]
        [TestCase("Something", ExpectedResult = "Something")]
        [TestCase("thing", ExpectedResult = "thing")]
        public static string MakeGerundVerb_finds_matching_gerund_verb_(string name) => Verbalizer.MakeGerundVerb(name);

        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = " ")]
        [TestCase("about", ExpectedResult = "about")]
        [TestCase("absent", ExpectedResult = "absent")]
        [TestCase("abstract", ExpectedResult = "abstract")]
        [TestCase("are", ExpectedResult = "be")]
        [TestCase("Are", ExpectedResult = "Be")]
        [TestCase("argument", ExpectedResult = "argument")]
        [TestCase("be", ExpectedResult = "be")]
        [TestCase("Be", ExpectedResult = "Be")]
        [TestCase("bing", ExpectedResult = "bing")]
        [TestCase("blazing", ExpectedResult = "blaze")]
        [TestCase("congruent", ExpectedResult = "congruent")]
        [TestCase("continues", ExpectedResult = "continue")]
        [TestCase("continuing", ExpectedResult = "continue")]
        [TestCase("correcting", ExpectedResult = "correct")]
        [TestCase("current", ExpectedResult = "current")]
        [TestCase("does", ExpectedResult = "do")]
        [TestCase("estimation", ExpectedResult = "estimate")]
        [TestCase("Estimation", ExpectedResult = "Estimate")]
        [TestCase("has", ExpectedResult = "have")]
        [TestCase("Has", ExpectedResult = "Have")]
        [TestCase("int", ExpectedResult = "int")]
        [TestCase("is", ExpectedResult = "be")]
        [TestCase("Is", ExpectedResult = "Be")]
        [TestCase("just", ExpectedResult = "just")]
        [TestCase("king", ExpectedResult = "king")]
        [TestCase("ming", ExpectedResult = "ming")]
        [TestCase("not", ExpectedResult = "not")]
        [TestCase("object", ExpectedResult = "object")]
        [TestCase("that", ExpectedResult = "that")]
        [TestCase("thing", ExpectedResult = "thing")]
        [TestCase("unit", ExpectedResult = "unit")]
        [TestCase("was", ExpectedResult = "be")]
        [TestCase("Was", ExpectedResult = "Be")]
        [TestCase("were", ExpectedResult = "be")]
        [TestCase("Were", ExpectedResult = "Be")]
        public static string MakeInfiniteVerb_finds_matching_infinite_verb_(string name) => Verbalizer.MakeInfiniteVerb(name);

        [Test]
        public static void MakeInfiniteVerb_finds_matching_past_infinite_verb_([ValueSource(nameof(Words))] WordCombination word)
        {
            using (Assert.EnterMultipleScope())
            {
                var expected = word.InfiniteVerb;

                AssertVerb(word.InfiniteVerb, expected);
                AssertVerb(word.ThirdPersonVerb, expected);
                AssertVerb(word.GerundVerb, expected);
                AssertVerb(word.PastVerb, expected);

                foreach (var noun in word.Nouns.WhereNotNull())
                {
                    AssertVerb(noun, expected);
                }
            }

            static void AssertVerb(string currentWord, string expected)
            {
                Assert.That(Verbalizer.MakeInfiniteVerb(currentWord), Is.EqualTo(expected), $"'{currentWord}' should become '{expected}'");

                // now upper case
                var currentUpper = currentWord.ToUpperCaseAt(0);
                var expectedUpper = expected.ToUpperCaseAt(0);

                Assert.That(Verbalizer.MakeInfiniteVerb(currentUpper), Is.EqualTo(expectedUpper), $"'{currentUpper}' should become '{expectedUpper}'");
            }
        }

        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = "")]
        [TestCase(" ", ExpectedResult = " ")]
        public static string MakePastTenseVerb_finds_matching_past_tense_verb_(string name) => Verbalizer.MakePastTenseVerb(name);

        [Test]
        public static void MakePastTenseVerb_finds_matching_past_tense_verb_([ValueSource(nameof(Words))] WordCombination word)
        {
            using (Assert.EnterMultipleScope())
            {
                var expected = word.PastVerb;

                AssertVerb(word.InfiniteVerb, expected);
                AssertVerb(word.ThirdPersonVerb, expected);
                AssertVerb(word.GerundVerb, expected);
                AssertVerb(word.PastVerb, expected);

                foreach (var noun in word.Nouns.WhereNotNull())
                {
                    AssertVerb(noun, expected);
                }
            }

            static void AssertVerb(string currentWord, string expected)
            {
                Assert.That(Verbalizer.MakePastTenseVerb(currentWord), Is.EqualTo(expected), $"'{currentWord}' should become '{expected}'");

                // now upper case
                var currentUpper = currentWord.ToUpperCaseAt(0);
                var expectedUpper = expected.ToUpperCaseAt(0);

                Assert.That(Verbalizer.MakePastTenseVerb(currentUpper), Is.EqualTo(expectedUpper), $"'{currentUpper}' should become '{expectedUpper}'");
            }
        }

        [TestCase("calc", ExpectedResult = "calculates")]
        [TestCase("recalc", ExpectedResult = "recalculates")]
        [TestCase("shutdown", ExpectedResult = "shuts down")]
        [TestCase("Shutdown", ExpectedResult = "Shuts down")]
        public static string MakeThirdPersonSingularVerb_finds_matching_3rd_person_singular_verb_(string name) => Verbalizer.MakeThirdPersonSingularVerb(name);

        [Test]
        public static void MakeThirdPersonSingularVerb_finds_matching_3rd_person_singular_verb_([ValueSource(nameof(Words))] WordCombination word)
        {
            using (Assert.EnterMultipleScope())
            {
                var expected = word.ThirdPersonVerb;

                AssertVerb(word.InfiniteVerb, expected);
                AssertVerb(word.ThirdPersonVerb, expected);
                AssertVerb(word.GerundVerb, expected);
                AssertVerb(word.PastVerb, expected);

                foreach (var noun in word.Nouns.WhereNotNull())
                {
                    AssertVerb(noun, expected);
                }
            }

            static void AssertVerb(string currentWord, string expected)
            {
                Assert.That(Verbalizer.MakeThirdPersonSingularVerb(currentWord), Is.EqualTo(expected), $"'{currentWord}' should become '{expected}'");

                // now upper case
                var currentUpper = currentWord.ToUpperCaseAt(0);
                var expectedUpper = expected.ToUpperCaseAt(0);

                Assert.That(Verbalizer.MakeThirdPersonSingularVerb(currentUpper), Is.EqualTo(expectedUpper), $"'{currentUpper}' should become '{expectedUpper}'");
            }
        }

        [TestCase(null, ExpectedResult = null, Description = "There is no verb available")]
        [TestCase("", ExpectedResult = "", Description = "There is no verb available")]
        [TestCase(" ", ExpectedResult = " ", Description = "There is no verb available")]
        [TestCase("Action", ExpectedResult = "Action", Description = "There is no verb available")]
        [TestCase("Caption", ExpectedResult = "Caption", Description = "There is no verb available")]
        [TestCase("Destination", ExpectedResult = "Destination", Description = "There is no verb available")]
        [TestCase("DoAction", ExpectedResult = "DoAction")]
        [TestCase("DoFunc", ExpectedResult = "DoFunction")]
        [TestCase("DoFunction", ExpectedResult = "DoFunction")]
        [TestCase("Func", ExpectedResult = "Function")]
        [TestCase("Function", ExpectedResult = "Function", Description = "There is no verb available")]
        [TestCase("HasAction", ExpectedResult = "HasAction")]
        [TestCase("IsRelevantAction", ExpectedResult = "IsRelevantAction")]
        [TestCase("IsRelevantFunc", ExpectedResult = "IsRelevantFunc")]
        [TestCase("IsRelevantFunction", ExpectedResult = "IsRelevantFunction")]
        [TestCase("validation", ExpectedResult = "validate")]
        public static string TryMakeVerb_finds_matching_verb_(string name)
        {
            Verbalizer.TryMakeVerb(name, out var result);

            return result;
        }

        [Test]
        public static void TryMakeVerb_finds_matching_verb_([ValueSource(nameof(Words))] WordCombination word)
        {
            using (Assert.EnterMultipleScope())
            {
                var expected = word.InfiniteVerb;

                AssertVerb(word.InfiniteVerb, expected);
                AssertVerb(word.ThirdPersonVerb, expected);
                AssertVerb(word.GerundVerb, expected);
                AssertVerb(word.PastVerb, expected);

                foreach (var noun in word.Nouns.WhereNotNull())
                {
                    AssertVerb(noun, expected);
                }
            }

            static void AssertVerb(string currentWord, string expected)
            {
                if (Verbalizer.TryMakeVerb(currentWord, out var value))
                {
                    Assert.That(value, Is.EqualTo(expected), $"'{value}' should become '{expected}'");
                }
                else
                {
                    Assert.That(value, Is.EqualTo(currentWord), $"'{value}' should stay '{currentWord}'");
                }

                // now upper case
                var currentUpperCase = currentWord.ToUpperCaseAt(0);
                var expectedUpperCase = expected.ToUpperCaseAt(0);

                if (Verbalizer.TryMakeVerb(currentUpperCase, out var valueUpperCase))
                {
                    Assert.That(valueUpperCase, Is.EqualTo(expectedUpperCase), $"'{valueUpperCase}' should become '{expectedUpperCase}'");
                }
                else
                {
                    Assert.That(valueUpperCase, Is.EqualTo(currentUpperCase), $"'{valueUpperCase}' should stay '{currentUpperCase}'");
                }
            }
        }

        public record WordCombination(string InfiniteVerb, string GerundVerb, string ThirdPersonVerb, string PastVerb, string PastParticiple, params string[] Nouns)
        {
            public override string ToString()
            {
                var sb = new StringBuilder().Append('[')
                                            .Append(InfiniteVerb).Append('|')
                                            .Append(GerundVerb).Append('|')
                                            .Append(ThirdPersonVerb).Append('|')
                                            .Append(PastVerb).Append('|')
                                            .Append(PastParticiple).Append('|')
                                            .Append('(').Append(Nouns.ConcatenatedWith("|")).Append(')')
                                            .Append(']');

                return sb.ToString();
            }
        }
    }
}