using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MiKoSolutions.Analyzers.Linguistics
{
    /// <summary>
    /// Provides functionality to convert words and phrases between different verb forms.
    /// </summary>
    internal static class Verbalizer
    {
        private static readonly HashSet<char> CharsForTwoCharacterEndingsWithS = new HashSet<char> { 'a', 'h', 'i', 'o', 's', 'u', 'x', 'z' };

        private static readonly string[] NonThirdPersonSingularEndingsWithS = { "pters", "tors", "gers", "chers", "asses", "blings", "klings" };

        private static readonly string[] ExceptionsToNonThirdPersonSingularEndingsWithS = { "actors", "onitors", "riggers", "asses" };

        private static readonly string[] SpecialPastEndings = { "ated", "bbed", "bled", "dled", "ced", "pleted", "ged", "ied", "red", "sed", "ved", "oked", "fined", "ched", "loned", "poned", "ued", "zed", "ermined", "ribed", "vited", "ixed", "kled", "ashed", "eshed", "ished" };

        private static readonly string[] PastEndings = SpecialPastEndings.ConcatenatedWith("led", "eed", "ted", "ded", "med", "ned", "hed", "ped", "yed", "xed", "rked", "nged", "wed").ToArray();

        private static readonly string[] PastEndingsWithoutConsonantDoubling = { "get", "ait", "dit", "sit", "fit", "xit", "imit", "ffer", "ain", "elop", "eat", "oot", "eam" };

        private static readonly string[] PastEndingsWithConsonantDoubling = { "lan", "fer", "ur", "ab", "pel", "trol" };

        private static readonly string[] ThreeCharacterPastEndings = { "bbed", "gged", "mmed", "nned", "pped", "rred", "tted", "elled", "ontrolled" };

        private static readonly string[] TwoCharacterPastEndings = { "tored", "tered", "thered", "ivered", "uered", "peated", "ffered", "inged", };

        private static readonly string[] FourCharacterGerundEndings = { "bbing", "gging", "trolling", "mming", "nning", "pping", "rring", "tting", "pelling", };

        private static readonly string[] ThreeCharacterGerundEndings =
                                                                       {
                                                                           "aining",
                                                                           "cting",
                                                                           "diting",
                                                                           "eaking",
                                                                           "enting",
                                                                           "esting",
                                                                           "fiting",
                                                                           "inging",
                                                                           "ioning",
                                                                           "miting",
                                                                           "oining",
                                                                           "peating",
                                                                           "pting",
                                                                           "rmating",
                                                                           "rting",
                                                                           "ssing",
                                                                           "tching",
                                                                           "uesting",
                                                                           "xiting",
                                                                           "zzing",
                                                                       };

        private static readonly string[] ThreeCharacterGerundEndingsWithE =
                                                                            {
                                                                                "aking",
                                                                                "anging",
                                                                                "aring",
                                                                                "bling",
                                                                                "ching",
                                                                                "dling",
                                                                                "encing",
                                                                                "erating",
                                                                                "eteing",
                                                                                "ging",
                                                                                "idating",
                                                                                "iding",
                                                                                "illating",
                                                                                "imating",
                                                                                "inating",
                                                                                "ining",
                                                                                "iring",
                                                                                "kling",
                                                                                "lating",
                                                                                "licing",
                                                                                "mining",
                                                                                "ocating",
                                                                                "oking",
                                                                                "omating",
                                                                                "oning",
                                                                                "pleting",
                                                                                "reating",
                                                                                "rging",
                                                                                "ribing",
                                                                                "riting",
                                                                                "sing",
                                                                                "trating",
                                                                                "ucing",
                                                                                "uing",
                                                                                "ulating",
                                                                                "uring",
                                                                                "vating",
                                                                                "ving",
                                                                                "viting",
                                                                                "zing",
                                                                            };

        private static readonly string[] ThreeCharacterInfiniteEndingsWithE = { "idation", "illation", "ulation", "mation", "nation", "ration", "sation", "vation", "zation" };

        private static readonly string[] EndingsWithConsonantDoublingForGerund = { "et", "in", "it", "un" };

        private static readonly string[] EndingsWithoutConsonantDoublingForGerund = { "ain", "oin", "ait", "dit", "sit", "fit", "xit", };

        private static readonly string[] VerbPrefixes = { "un", "re", "over", "under", "out", "mis", "for", "up", "down" };

        private static readonly ConcurrentDictionary<string, string> GerundVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                        new[]
                                                                                                                            {
                                                                                                                                Paired("bing", "bing"),
                                                                                                                                Paired("Bing", "Bing"),
                                                                                                                                Paired("has", "having"),
                                                                                                                                Paired("Has", "Having"),
                                                                                                                                Paired("king", "king"),
                                                                                                                                Paired("King", "King"),
                                                                                                                                Paired("ming", "ming"),
                                                                                                                                Paired("Ming", "Ming"),
                                                                                                                                Paired("shutdown", "shuttingDown"),
                                                                                                                                Paired("shutDown", "shuttingDown"),
                                                                                                                                Paired("Shutdown", "ShuttingDown"),
                                                                                                                                Paired("ShutDown", "ShuttingDown"),
                                                                                                                            },
                                                                                                                        StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> InfiniteVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                          new[]
                                                                                                                              {
                                                                                                                                  Paired("alter", "alter"),
                                                                                                                                  Paired("Alter", "Alter"),
                                                                                                                                  Paired("aquiration", "acquire"),
                                                                                                                                  Paired("Aquiration", "Acquire"),
                                                                                                                                  Paired("aquisition", "acquire"),
                                                                                                                                  Paired("Aquisition", "Acquire"),
                                                                                                                                  Paired("are", "be"),
                                                                                                                                  Paired("Are", "Be"),
                                                                                                                                  Paired("began", "begin"),
                                                                                                                                  Paired("Began", "Begin"),
                                                                                                                                  Paired("bent", "bend"),
                                                                                                                                  Paired("Bent", "Bend"),
                                                                                                                                  Paired("broken", "break"),
                                                                                                                                  Paired("Broken", "Break"),
                                                                                                                                  Paired("caught", "catch"),
                                                                                                                                  Paired("Caught", "Catch"),
                                                                                                                                  Paired("choice", "choose"),
                                                                                                                                  Paired("Choice", "Choose"),
                                                                                                                                  Paired("chosen", "choose"),
                                                                                                                                  Paired("Chosen", "Choose"),
                                                                                                                                  Paired("color", "colorize"),
                                                                                                                                  Paired("Color", "Colorize"),
                                                                                                                                  Paired("colors", "colorize"),
                                                                                                                                  Paired("Colors", "Colorize"),
                                                                                                                                  Paired("colour", "colorize"),
                                                                                                                                  Paired("Colour", "Colorize"),
                                                                                                                                  Paired("colours", "colorize"),
                                                                                                                                  Paired("Colours", "Colorize"),
                                                                                                                                  Paired("done", "do"),
                                                                                                                                  Paired("Done", "Do"),
                                                                                                                                  Paired("driven", "drive"),
                                                                                                                                  Paired("Driven", "Drive"),
                                                                                                                                  Paired("drove", "drive"),
                                                                                                                                  Paired("Drove", "Drive"),
                                                                                                                                  Paired("enter", "enter"),
                                                                                                                                  Paired("Enter", "Enter"),
                                                                                                                                  Paired("entry", "enter"),
                                                                                                                                  Paired("Entry", "Enter"),
                                                                                                                                  Paired("felt", "feel"),
                                                                                                                                  Paired("Felt", "Feel"),
                                                                                                                                  Paired("flew", "fly"),
                                                                                                                                  Paired("Flew", "Fly"),
                                                                                                                                  Paired("forge", "forge"),
                                                                                                                                  Paired("Forge", "Forge"),
                                                                                                                                  Paired("forged", "forge"),
                                                                                                                                  Paired("Forged", "Forge"),
                                                                                                                                  Paired("forges", "forge"),
                                                                                                                                  Paired("Forges", "Forge"),
                                                                                                                                  Paired("forging", "forge"),
                                                                                                                                  Paired("Forging", "Forge"),
                                                                                                                                  Paired("fork", "fork"),
                                                                                                                                  Paired("Fork", "Fork"),
                                                                                                                                  Paired("forked", "fork"),
                                                                                                                                  Paired("Forked", "Fork"),
                                                                                                                                  Paired("forking", "fork"),
                                                                                                                                  Paired("Forking", "Fork"),
                                                                                                                                  Paired("forks", "fork"),
                                                                                                                                  Paired("Forks", "Fork"),
                                                                                                                                  Paired("frozen", "freeze"),
                                                                                                                                  Paired("Frozen", "Freeze"),
                                                                                                                                  Paired("gave", "give"),
                                                                                                                                  Paired("Gave", "Give"),
                                                                                                                                  Paired("got", "get"),
                                                                                                                                  Paired("Got", "Get"),
                                                                                                                                  Paired("had", "have"),
                                                                                                                                  Paired("Had", "Have"),
                                                                                                                                  Paired("hanging", "hang"),
                                                                                                                                  Paired("Hanging", "Hang"),
                                                                                                                                  Paired("has", "have"),
                                                                                                                                  Paired("Has", "Have"),
                                                                                                                                  Paired("heated", "heat"),
                                                                                                                                  Paired("Heated", "Heat"),
                                                                                                                                  Paired("hid", "hide"),
                                                                                                                                  Paired("Hid", "Hide"),
                                                                                                                                  Paired("hidden", "hide"),
                                                                                                                                  Paired("Hidden", "Hide"),
                                                                                                                                  Paired("hung", "hang"),
                                                                                                                                  Paired("Hung", "Hang"),
                                                                                                                                  Paired("is", "be"),
                                                                                                                                  Paired("Is", "Be"),
                                                                                                                                  Paired("lent", "lend"),
                                                                                                                                  Paired("Lent", "Lend"),
                                                                                                                                  Paired("lie", "lie"),
                                                                                                                                  Paired("Lie", "Lie"),
                                                                                                                                  Paired("lied", "lie"),
                                                                                                                                  Paired("Lied", "Lie"),
                                                                                                                                  Paired("lies", "lie"),
                                                                                                                                  Paired("Lies", "Lie"),
                                                                                                                                  Paired("lying", "lie"),
                                                                                                                                  Paired("Lying", "Lie"),
                                                                                                                                  Paired("ran", "run"),
                                                                                                                                  Paired("Ran", "Run"),
                                                                                                                                  Paired("rang", "ring"),
                                                                                                                                  Paired("Rang", "Ring"),
                                                                                                                                  Paired("read", "read"),
                                                                                                                                  Paired("Read", "Read"),
                                                                                                                                  Paired("reading", "read"),
                                                                                                                                  Paired("Reading", "Read"),
                                                                                                                                  Paired("reads", "read"),
                                                                                                                                  Paired("Reads", "Read"),
                                                                                                                                  Paired("sang", "sing"),
                                                                                                                                  Paired("Sang", "Sing"),
                                                                                                                                  Paired("sent", "send"),
                                                                                                                                  Paired("Sent", "Send"),
                                                                                                                                  Paired("spent", "spend"),
                                                                                                                                  Paired("Spent", "Spend"),
                                                                                                                                  Paired("spoken", "speak"),
                                                                                                                                  Paired("Spoken", "Speak"),
                                                                                                                                  Paired("was", "be"),
                                                                                                                                  Paired("Was", "Be"),
                                                                                                                                  Paired("were", "be"),
                                                                                                                                  Paired("Were", "Be"),
                                                                                                                                  Paired("will", "will"),
                                                                                                                                  Paired("Will", "Will"),
                                                                                                                                  Paired("woke", "wake"),
                                                                                                                                  Paired("Woke", "Wake"),
                                                                                                                                  Paired("woken", "wake"),
                                                                                                                                  Paired("Woken", "Wake"),
                                                                                                                                  Paired("written", "write"),
                                                                                                                                  Paired("Written", "Write"),
                                                                                                                                  Paired("wrote", "write"),
                                                                                                                                  Paired("Wrote", "Write"),
                                                                                                                              },
                                                                                                                          StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> PastVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                      new[]
                                                                                                                          {
                                                                                                                              Paired("are", "were"),
                                                                                                                              Paired("Are", "Were"),
                                                                                                                              Paired("ban", "banned"),
                                                                                                                              Paired("Ban", "Banned"),
                                                                                                                              Paired("be", "was"),
                                                                                                                              Paired("Be", "Was"),
                                                                                                                              Paired("began", "began"),
                                                                                                                              Paired("Began", "Began"),
                                                                                                                              Paired("begin", "began"),
                                                                                                                              Paired("Begin", "Began"),
                                                                                                                              Paired("bend", "bent"),
                                                                                                                              Paired("Bend", "Bent"),
                                                                                                                              Paired("bent", "bent"),
                                                                                                                              Paired("Bent", "Bent"),
                                                                                                                              Paired("break", "broken"),
                                                                                                                              Paired("Break", "Broken"),
                                                                                                                              Paired("broken", "broken"),
                                                                                                                              Paired("Broken", "Broken"),
                                                                                                                              Paired("burn", "burnt"),
                                                                                                                              Paired("Burn", "Burnt"),
                                                                                                                              Paired("burnt", "burnt"),
                                                                                                                              Paired("Burnt", "Burnt"),
                                                                                                                              Paired("catch", "caught"),
                                                                                                                              Paired("Catch", "Caught"),
                                                                                                                              Paired("catches", "caught"),
                                                                                                                              Paired("Catches", "Caught"),
                                                                                                                              Paired("catching", "caught"),
                                                                                                                              Paired("Catching", "Caught"),
                                                                                                                              Paired("caught", "caught"),
                                                                                                                              Paired("Caught", "Caught"),
                                                                                                                              Paired("choice", "chosen"),
                                                                                                                              Paired("Choice", "Chosen"),
                                                                                                                              Paired("choose", "chosen"),
                                                                                                                              Paired("Choose", "Chosen"),
                                                                                                                              Paired("choosing", "chosen"),
                                                                                                                              Paired("Choosing", "Chosen"),
                                                                                                                              Paired("chosen", "chosen"),
                                                                                                                              Paired("Chosen", "Chosen"),
                                                                                                                              Paired("cut", "cut"),
                                                                                                                              Paired("Cut", "Cut"),
                                                                                                                              Paired("deal", "dealt"),
                                                                                                                              Paired("Deal", "Dealt"),
                                                                                                                              Paired("dealing", "dealt"),
                                                                                                                              Paired("Dealing", "Dealt"),
                                                                                                                              Paired("dealt", "dealt"),
                                                                                                                              Paired("Dealt", "Dealt"),
                                                                                                                              Paired("do", "done"),
                                                                                                                              Paired("Do", "Done"),
                                                                                                                              Paired("done", "done"),
                                                                                                                              Paired("Done", "Done"),
                                                                                                                              Paired("dream", "dreamt"),
                                                                                                                              Paired("Dream", "Dreamt"),
                                                                                                                              Paired("dreamt", "dreamt"),
                                                                                                                              Paired("Dreamt", "Dreamt"),
                                                                                                                              Paired("driver", "drove"),
                                                                                                                              Paired("Driver", "Drove"),
                                                                                                                              Paired("drivers", "drove"),
                                                                                                                              Paired("Drivers", "Drove"),
                                                                                                                              Paired("drove", "drove"),
                                                                                                                              Paired("Drove", "Drove"),
                                                                                                                              Paired("entry", "entered"),
                                                                                                                              Paired("Entry", "Entered"),
                                                                                                                              Paired("feel", "felt"),
                                                                                                                              Paired("Feel", "Felt"),
                                                                                                                              Paired("felt", "felt"),
                                                                                                                              Paired("Felt", "Felt"),
                                                                                                                              Paired("fit", "fitted"),
                                                                                                                              Paired("Fit", "Fitted"),
                                                                                                                              Paired("flew", "flew"),
                                                                                                                              Paired("Flew", "Flew"),
                                                                                                                              Paired("flies", "flew"),
                                                                                                                              Paired("Flies", "Flew"),
                                                                                                                              Paired("fly", "flew"),
                                                                                                                              Paired("Fly", "Flew"),
                                                                                                                              Paired("flying", "flew"),
                                                                                                                              Paired("Flying", "Flew"),
                                                                                                                              Paired("forge", "forged"),
                                                                                                                              Paired("Forge", "Forged"),
                                                                                                                              Paired("forged", "forged"),
                                                                                                                              Paired("Forged", "Forged"),
                                                                                                                              Paired("forges", "forged"),
                                                                                                                              Paired("Forges", "Forged"),
                                                                                                                              Paired("forging", "forged"),
                                                                                                                              Paired("Forging", "Forged"),
                                                                                                                              Paired("fork", "forked"),
                                                                                                                              Paired("Fork", "Forked"),
                                                                                                                              Paired("forked", "forked"),
                                                                                                                              Paired("Forked", "Forked"),
                                                                                                                              Paired("forking", "forked"),
                                                                                                                              Paired("Forking", "Forked"),
                                                                                                                              Paired("forks", "forked"),
                                                                                                                              Paired("Forks", "Forked"),
                                                                                                                              Paired("freeze", "frozen"),
                                                                                                                              Paired("Freeze", "Frozen"),
                                                                                                                              Paired("freezes", "frozen"),
                                                                                                                              Paired("Freezes", "Frozen"),
                                                                                                                              Paired("freezing", "frozen"),
                                                                                                                              Paired("Freezing", "Frozen"),
                                                                                                                              Paired("frozen", "frozen"),
                                                                                                                              Paired("Frozen", "Frozen"),
                                                                                                                              Paired("gave", "gave"),
                                                                                                                              Paired("Gave", "Gave"),
                                                                                                                              Paired("get", "got"),
                                                                                                                              Paired("Get", "Got"),
                                                                                                                              Paired("give", "gave"),
                                                                                                                              Paired("Give", "Gave"),
                                                                                                                              Paired("got", "got"),
                                                                                                                              Paired("Got", "Got"),
                                                                                                                              Paired("had", "had"),
                                                                                                                              Paired("Had", "Had"),
                                                                                                                              Paired("hang", "hung"),
                                                                                                                              Paired("Hang", "Hung"),
                                                                                                                              Paired("has", "had"),
                                                                                                                              Paired("Has", "Had"),
                                                                                                                              Paired("have", "had"),
                                                                                                                              Paired("Have", "Had"),
                                                                                                                              Paired("hid", "hid"),
                                                                                                                              Paired("Hid", "Hid"),
                                                                                                                              Paired("hidden", "hid"),
                                                                                                                              Paired("Hidden", "hid"),
                                                                                                                              Paired("hide", "hid"),
                                                                                                                              Paired("Hide", "Hid"),
                                                                                                                              Paired("hit", "hit"),
                                                                                                                              Paired("Hit", "Hit"),
                                                                                                                              Paired("hung", "hung"),
                                                                                                                              Paired("Hung", "Hung"),
                                                                                                                              Paired("input", "input"),
                                                                                                                              Paired("Input", "Input"),
                                                                                                                              Paired("is", "was"),
                                                                                                                              Paired("Is", "Was"),
                                                                                                                              Paired("lend", "lent"),
                                                                                                                              Paired("Lend", "Lent"),
                                                                                                                              Paired("lends", "lent"),
                                                                                                                              Paired("Lends", "Lent"),
                                                                                                                              Paired("lent", "lent"),
                                                                                                                              Paired("Lent", "Lent"),
                                                                                                                              Paired("let", "let"),
                                                                                                                              Paired("Let", "Let"),
                                                                                                                              Paired("offset", "offset"),
                                                                                                                              Paired("Offset", "Offset"),
                                                                                                                              Paired("output", "output"),
                                                                                                                              Paired("Output", "Output"),
                                                                                                                              Paired("pin", "pinned"),
                                                                                                                              Paired("Pin", "Pinned"),
                                                                                                                              Paired("ping", "pinged"),
                                                                                                                              Paired("Ping", "Pinged"),
                                                                                                                              Paired("put", "put"),
                                                                                                                              Paired("Put", "Put"),
                                                                                                                              Paired("quit", "quit"),
                                                                                                                              Paired("Quit", "Quit"),
                                                                                                                              Paired("rang", "rang"),
                                                                                                                              Paired("Rang", "Rang"),
                                                                                                                              Paired("read", "read"),
                                                                                                                              Paired("Read", "Read"),
                                                                                                                              Paired("reading", "read"),
                                                                                                                              Paired("Reading", "Read"),
                                                                                                                              Paired("reads", "read"),
                                                                                                                              Paired("Reads", "Read"),
                                                                                                                              Paired("reset", "reset"),
                                                                                                                              Paired("Reset", "Reset"),
                                                                                                                              Paired("ring", "rang"),
                                                                                                                              Paired("Ring", "Rang"),
                                                                                                                              Paired("ran", "ran"),
                                                                                                                              Paired("Ran", "Ran"),
                                                                                                                              Paired("run", "ran"),
                                                                                                                              Paired("Run", "Ran"),
                                                                                                                              Paired("sang", "sang"),
                                                                                                                              Paired("Sang", "Sang"),
                                                                                                                              Paired("send", "sent"),
                                                                                                                              Paired("Send", "Sent"),
                                                                                                                              Paired("sent", "sent"),
                                                                                                                              Paired("Sent", "Sent"),
                                                                                                                              Paired("set", "set"),
                                                                                                                              Paired("Set", "Set"),
                                                                                                                              Paired("shut", "shut"),
                                                                                                                              Paired("Shut", "Shut"),
                                                                                                                              Paired("sing", "sang"),
                                                                                                                              Paired("Sing", "Sang"),
                                                                                                                              Paired("singer", "sang"),
                                                                                                                              Paired("Singer", "Sang"),
                                                                                                                              Paired("smell", "smelt"),
                                                                                                                              Paired("Smell", "Smelt"),
                                                                                                                              Paired("spend", "spent"),
                                                                                                                              Paired("Spend", "Spent"),
                                                                                                                              Paired("spent", "spent"),
                                                                                                                              Paired("Spent", "Spent"),
                                                                                                                              Paired("was", "was"),
                                                                                                                              Paired("Was", "Was"),
                                                                                                                              Paired("were", "were"),
                                                                                                                              Paired("Were", "Were"),
                                                                                                                              Paired("will", "will"),
                                                                                                                              Paired("Will", "Will"),
                                                                                                                              Paired("write", "wrote"),
                                                                                                                              Paired("Write", "Wrote"),
                                                                                                                              Paired("wrote", "wrote"),
                                                                                                                              Paired("Wrote", "Wrote"),
                                                                                                                          },
                                                                                                                      StringComparer.Ordinal);

        private static readonly HashSet<string> PastVerbsOnly = PastVerbs.Values.ToHashSet();

        private static readonly ConcurrentDictionary<string, string> ThirdPersonSingularVerbs = new ConcurrentDictionary<string, string>(
                                                                                                                                     new[]
                                                                                                                                         {
                                                                                                                                             Paired("are", "is"),
                                                                                                                                             Paired("Are", "Is"),
                                                                                                                                             Paired("be", "is"),
                                                                                                                                             Paired("Be", "Is"),
                                                                                                                                             Paired("bent", "bends"),
                                                                                                                                             Paired("Bent", "Bends"),
                                                                                                                                             Paired("calc", "calculates"),
                                                                                                                                             Paired("Calc", "Calculates"),
                                                                                                                                             Paired("color", "colorizes"),
                                                                                                                                             Paired("Color", "Colorizes"),
                                                                                                                                             Paired("colors", "colorizes"),
                                                                                                                                             Paired("Colors", "Colorizes"),
                                                                                                                                             Paired("colour", "colorizes"),
                                                                                                                                             Paired("Colour", "Colorizes"),
                                                                                                                                             Paired("colours", "colorizes"),
                                                                                                                                             Paired("Colours", "Colorizes"),
                                                                                                                                             Paired("done", "does"),
                                                                                                                                             Paired("Done", "Does"),
                                                                                                                                             Paired("drove", "drives"),
                                                                                                                                             Paired("Drove", "Drives"),
                                                                                                                                             Paired("enter", "enters"),
                                                                                                                                             Paired("Enter", "Enters"),
                                                                                                                                             Paired("enters", "enters"),
                                                                                                                                             Paired("Enters", "Enters"),
                                                                                                                                             Paired("entry", "enters"),
                                                                                                                                             Paired("Entry", "Enters"),
                                                                                                                                             Paired("flew", "flies"),
                                                                                                                                             Paired("Flew", "Flies"),
                                                                                                                                             Paired("forging", "forges"),
                                                                                                                                             Paired("Forging", "Forges"),
                                                                                                                                             Paired("forking", "forks"),
                                                                                                                                             Paired("Forking", "Forks"),
                                                                                                                                             Paired("frozen", "freezes"),
                                                                                                                                             Paired("Frozen", "Freezes"),
                                                                                                                                             Paired("gave", "gives"),
                                                                                                                                             Paired("Gave", "Gives"),
                                                                                                                                             Paired("got", "gets"),
                                                                                                                                             Paired("Got", "Gets"),
                                                                                                                                             Paired("had", "has"),
                                                                                                                                             Paired("Had", "Has"),
                                                                                                                                             Paired("has", "has"),
                                                                                                                                             Paired("Has", "Has"),
                                                                                                                                             Paired("have", "has"),
                                                                                                                                             Paired("Have", "Has"),
                                                                                                                                             Paired("heated", "heats"),
                                                                                                                                             Paired("Heated", "Heats"),
                                                                                                                                             Paired("hid", "hides"),
                                                                                                                                             Paired("Hid", "Hides"),
                                                                                                                                             Paired("hung", "hangs"),
                                                                                                                                             Paired("Hung", "Hangs"),
                                                                                                                                             Paired("is", "is"),
                                                                                                                                             Paired("Is", "Is"),
                                                                                                                                             Paired("maintenance", "maintains"),
                                                                                                                                             Paired("Maintenance", "Maintains"),
                                                                                                                                             Paired("ran", "runs"),
                                                                                                                                             Paired("Ran", "Runs"),
                                                                                                                                             Paired("rang", "rings"),
                                                                                                                                             Paired("Rang", "Rings"),
                                                                                                                                             Paired("read", "reads"),
                                                                                                                                             Paired("Read", "Reads"),
                                                                                                                                             Paired("reading", "reads"),
                                                                                                                                             Paired("Reading", "Reads"),
                                                                                                                                             Paired("reads", "reads"),
                                                                                                                                             Paired("Reads", "Reads"),
                                                                                                                                             Paired("recalc", "recalculates"),
                                                                                                                                             Paired("Recalc", "Recalculates"),
                                                                                                                                             Paired("sang", "sings"),
                                                                                                                                             Paired("Sang", "Sings"),
                                                                                                                                             Paired("shutdown", "shuts down"),
                                                                                                                                             Paired("Shutdown", "Shuts down"),
                                                                                                                                             Paired("trigger", "triggers"),
                                                                                                                                             Paired("Trigger", "Triggers"),
                                                                                                                                             Paired("triggered", "triggers"),
                                                                                                                                             Paired("Triggered", "Triggers"),
                                                                                                                                             Paired("was", "is"),
                                                                                                                                             Paired("Was", "Is"),
                                                                                                                                             Paired("were", "is"),
                                                                                                                                             Paired("Were", "Is"),
                                                                                                                                             Paired("will", "will"),
                                                                                                                                             Paired("Will", "Will"),
                                                                                                                                             Paired("woke", "wakes"),
                                                                                                                                             Paired("Woke", "Wakes"),
                                                                                                                                             Paired("wrote", "writes"),
                                                                                                                                             Paired("Wrote", "Writes"),
                                                                                                                                         },
                                                                                                                                     StringComparer.Ordinal);

        private static readonly string[] ThirdPersonalSingularVerbExceptions =
                                                                               {
                                                                                   "argument", "Argument",
                                                                                   "exception", "Exception",
                                                                                   "false", "False",
                                                                                   "guid", "Guid",
                                                                                   "int", "Int",
                                                                                   "invalid", "Invalid",
                                                                                   "json", "Json",
                                                                                   "key", "Key",
                                                                                   "no", "No",
                                                                                   "not", "Not",
                                                                                   "null", "Null",
                                                                                   "object", "Object",
                                                                                   "should", "Should",
                                                                                   "string", "String",
                                                                                   "task", "Task",
                                                                                   "true", "True",
                                                                                   "unauthorized", "Unauthorized",
                                                                                   "valid", "Valid",
                                                                               };

        private static readonly string[] Nouns =
                                                 {
                                                     "File",
                                                     "Name",
                                                 };

        private static readonly string[] NounsWithGerundEnding =
                                                                 {
                                                                     "awakening", "awning",
                                                                     "blessing", "booking", "briefing", "building",
                                                                     "ceiling",
                                                                     "darling", "dealing", "drawing", "duckling",
                                                                     "evening",
                                                                     "feeling", "finding", "fledgling",
                                                                     "gathering", "guttering",
                                                                     "hireling",
                                                                     "inkling",
                                                                     "leaning",
                                                                     "meeting", "misgiving", "misunderstanding", "morning",
                                                                     "offering", "outing",
                                                                     "painting",
                                                                     "quisling",
                                                                     "reasoning", "recording", "restructuring", "rising", "roofing",
                                                                     "sapling", "seasoning", "seating", "setting", "shooting", "shopping", "sibling", "sitting", "standing",
                                                                     "tiding", "timing", "training",
                                                                     "underling", "understanding", "undertaking", "upbringing", "uprising",
                                                                     "warning", "wedding", "well-being", "winning", "wording",
                                                                 };

        private static readonly Pair[] Endings =
                                                 {
                                                     new Pair(nameof(Action), nameof(Action)),
                                                     new Pair("Caption", "Caption"),
                                                     new Pair("cution", "cute"),
                                                     new Pair(nameof(Exception), nameof(Exception)),
                                                     new Pair("Func", "Function"),
                                                     new Pair("Function", "Function"),
                                                     new Pair("estination", "estination"),
                                                     new Pair("endation", "end"),
                                                     new Pair("entation", "ent"),
                                                     new Pair("unction", "unction"),
                                                     new Pair("iption", "ibe"),
                                                     new Pair("allation", "all"),
                                                     new Pair("ellation", "el"),
                                                     new Pair("istration", "ister"),
                                                     new Pair("mination", "mine"),
                                                     new Pair("vocation", "voke"),
                                                     new Pair("teration", "ter"), // alteration
                                                     new Pair("eration", "erate"), // enumeration
                                                     new Pair("aquisition", "acquire"),
                                                     new Pair("Aquisition", "Acquire"),
                                                     new Pair("aquiration", "acquire"),
                                                     new Pair("Aquiration", "Acquire"),
                                                     new Pair("ication", "y"),
                                                     new Pair("iration", "ire"),
                                                     new Pair("uisition", "uire"),
                                                     new Pair("isition", "ire"),
                                                     new Pair("isation", "ize"),
                                                     new Pair("itation", "ite"),
                                                     new Pair("ization", "ize"),
                                                     new Pair("nuation", "nue"),
                                                     new Pair("osition", "ose"),
                                                     new Pair("ptation", "pt"),
                                                     new Pair("rmation", "rm"),
                                                     new Pair("uration", "ure"),
                                                     new Pair("pation", "py"),
                                                     new Pair("ortation", "ort"),
                                                     new Pair("ation", "ate"),
                                                     new Pair("duction", "duce"),
                                                     new Pair("ction", "ct"),
                                                     new Pair("ption", "pt"),
                                                     new Pair("rison", "re"),
                                                     new Pair("sis", "ze"),
                                                     new Pair("enance", "ain"),
                                                     new Pair("erence", "er"),
                                                     new Pair("rrence", "r"),
                                                     new Pair("pter", "pt"),
                                                     new Pair("nquire", "nquire"),
                                                     new Pair("equire", "equire"),
                                                     new Pair("aquire", "acquire"),
                                                     new Pair("Aquire", "Acquire"),
                                                     new Pair("acquire", "acquire"),
                                                     new Pair("Acquire", "Acquire"),
                                                 };

        private static readonly string[] IsAre = { "is", "are" };

        private static readonly string[] StartingPhrases = new[]
                                                               {
                                                                   "Abort",
                                                                   "Activate",
                                                                   "Add",
                                                                   "Analyze",
                                                                   "Apply",
                                                                   "Arrange",
                                                                   "Assert",
                                                                   "Begin",
                                                                   "Build",
                                                                   "Calculate",
                                                                   "Can",
                                                                   "Cancel",
                                                                   "Change",
                                                                   "Clear",
                                                                   "Clone",
                                                                   "Close",
                                                                   "Collect",
                                                                   "CompileTimeValidate",
                                                                   "Continue",
                                                                   "Create",
                                                                   "Deactivate",
                                                                   "Debug",
                                                                   "Delay",
                                                                   "Delete",
                                                                   "Deregister",
                                                                   "Deselect",
                                                                   "End",
                                                                   "Ensure",
                                                                   "Find",
                                                                   "Free",
                                                                   "Generate",
                                                                   "Get",
                                                                   "get_",
                                                                   "Handle",
                                                                   "Has",
                                                                   "Init",
                                                                   "Initialize",
                                                                   "Invert",
                                                                   "Is",
                                                                   "JumpTo",
                                                                   "Load",
                                                                   "Log",
                                                                   "Mirror",
                                                                   "Modify",
                                                                   "Move",
                                                                   "NavigateTo",
                                                                   "On",
                                                                   "Open",
                                                                   "Parse",
                                                                   "Pause",
                                                                   "Pop",
                                                                   "Prepare",
                                                                   "PromptFor",
                                                                   "Propagate",
                                                                   "Push",
                                                                   "Query",
                                                                   "Read",
                                                                   "Rebuild",
                                                                   "Record",
                                                                   "Recover",
                                                                   "Redo",
                                                                   "Refresh",
                                                                   "Register",
                                                                   "Release",
                                                                   "Reload",
                                                                   "Remove",
                                                                   "Replace",
                                                                   "Report",
                                                                   "Request",
                                                                   "Reset",
                                                                   "Resolve",
                                                                   "Restart",
                                                                   "Restore",
                                                                   "Resume",
                                                                   "Retrieve",
                                                                   "Rollback",
                                                                   "Save",
                                                                   "Select",
                                                                   "Send",
                                                                   "Set",
                                                                   "set_",
                                                                   "Setup",
                                                                   "Simulate",
                                                                   "Sort",
                                                                   "Start",
                                                                   "Stop",
                                                                   "Store",
                                                                   "Subscribe",
                                                                   "Subtract",
                                                                   "Suspend",
                                                                   "To",
                                                                   "Trace",
                                                                   "Translate",
                                                                   "Trigger",
                                                                   "Try",
                                                                   "Undo",
                                                                   "Unlock",
                                                                   "Unregister",
                                                                   "Unsubscribe",
                                                                   "Update",
                                                                   "Validate",
                                                                   "Verify",
                                                                   "With",
                                                                   "Wrap",
                                                                   "Write",
                                                                   "Zoom",
                                                               }.ToArray(_ => _, AscendingStringComparer.Default);

        private static readonly string[] MiddlePhrases = new[]
                                                             {
                                                                 "InformAbout",
                                                                 "InformsAbout",
                                                                 "InformedAbout",
                                                                 "BelongsTo",
                                                             }.ToArray(_ => _, AscendingStringComparer.Default);

        private static readonly string[] EndingPhrases = new[]
                                                             {
                                                                 "Position",
                                                                 "Parenthesis",
                                                                 "Situation",
                                                             }.ToArray(_ => _, AscendingStringComparer.Default);

        private static readonly char[] SentenceEndingMarkers = ".?!;:,)".ToCharArray();

        /// <summary>
        /// Determines whether the first word of the specified text is a gerund form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first word is a gerund form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFirstWordGerundVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var firstWord = text.AsSpan().TrimStart(Constants.SentenceMarkers).FirstWord();

            return IsGerundVerb(firstWord);
        }

        /// <summary>
        /// Determines whether the first word of the specified text is an infinite form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first word is an infinite form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFirstWordInfiniteVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var trimmed = text.AsSpan().TrimStart(Constants.SentenceMarkers);
            var firstWordSpan = trimmed.FirstWord();
            var firstWord = firstWordSpan.Length == text.Length ? text : firstWordSpan.ToString();

            if (StartingPhrases.Contains(firstWord, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            if (firstWord.EqualsAny(Nouns, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (NounsWithGerundEnding.Contains(firstWord, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            if (IsThirdPersonSingularVerb(firstWord))
            {
                return false;
            }

            if (firstWord.EndsWithAny("eh"))
            {
                return true;
            }

            if (firstWord.EndsWith("at", StringComparison.Ordinal) || firstWord.EndsWith("nd", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the first word of the specified text is a past form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the first word is a past form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsFirstWordPastTenseVerb(string text)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return false;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var firstWord = text.AsSpan().TrimStart(Constants.SentenceMarkers).FirstWord();

            return IsPastTenseVerb(firstWord);
        }

        /// <summary>
        /// Determines whether the specified word is a gerund verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a gerund verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsGerundVerb(string value) => value != null && IsGerundVerb(value.AsSpan());

        /// <summary>
        /// Determines whether the specified word is a gerund verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a gerund verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsGerundVerb(in ReadOnlySpan<char> value)
        {
            if (value.Length <= 4)
            {
                return false;
            }

            if (value.EndsWith("ing"))
            {
                if (value.EndsWith("ling"))
                {
                    return false;
                }

                if (value.EndsWith("thing", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                for (int index = 0, length = NounsWithGerundEnding.Length; index < length; index++)
                {
                    var noun = NounsWithGerundEnding[index];

                    if (value.Equals(noun, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified word is in past tense.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is in past tense; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPastTenseVerb(string value)
        {
            if (value is null)
            {
                return false;
            }

            if (PastVerbsOnly.Contains(value))
            {
                return true;
            }

            return IsPastTenseVerb(value.AsSpan());
        }

        /// <summary>
        /// Determines whether the specified word is in past tense.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is in past tense; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPastTenseVerb(in ReadOnlySpan<char> value) => value.EndsWithAny(PastEndings) || value.EqualsAny(PastVerbsOnly);

        /// <summary>
        /// Determines whether the specified word is a third-person singular verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a third-person singular verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsThirdPersonSingularVerb(in ReadOnlySpan<char> value)
        {
            var length = value.Length;

            if (length < 2)
            {
                return false;
            }

            if (length is 4 && value.Equals("will", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (value[length - 1] != 's')
            {
                return false;
            }

            if (CharsForTwoCharacterEndingsWithS.Contains(value[length - 2]))
            {
                return false;
            }

            if (Pluralizer.IsPlural(value))
            {
                if (value.EndsWithAny(NonThirdPersonSingularEndingsWithS))
                {
                    return value.EndsWithAny(ExceptionsToNonThirdPersonSingularEndingsWithS);
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified word is a third-person singular verb.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word is a third-person singular verb; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsThirdPersonSingularVerb(string value) => value != null && IsThirdPersonSingularVerb(value.AsSpan());

        /// <summary>
        /// Determines whether the specified word ends with a two-character ending followed by 's'.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word ends with a two-character ending followed by 's'; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsTwoCharacterEndingsWithS(string value)
        {
            var length = value?.Length;

            if (length >= 2)
            {
                return value[length.Value - 1] is 's' && CharsForTwoCharacterEndingsWithS.Contains(value[length.Value - 2]);
            }

            return false;
        }

        /// <summary>
        /// Converts the first word of the specified text to its gerund form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its gerund form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordGerundVerb(string text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordGerundVerb(text.AsSpan(), adjustment, wordBoundary);
        }

        /// <summary>
        /// Converts the first word of the specified text to its gerund form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its gerund form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordGerundVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var valueText = text.TrimStart(Constants.SentenceMarkers);

            if (valueText.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // first word
            var firstWord = GetFirstWord(valueText, firstWordAdjustment);

            var verb = MakeGerundVerb(firstWord);

            if (firstWord != verb)
            {
                return verb.ConcatenatedWith(valueText.WithoutFirstWord(wordBoundary));
            }

            return text.ToString();
        }

        /// <summary>
        /// Converts the first word of the specified text to its infinitive form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its infinitive form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordInfiniteVerb(string text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordInfiniteVerb(text.AsSpan(), firstWordAdjustment, wordBoundary);
        }

        /// <summary>
        /// Converts the first word of the specified text to its infinitive form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its infinitive form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordInfiniteVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var valueText = text.TrimStart(Constants.SentenceMarkers);

            if (valueText.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // first word
            var firstWord = GetFirstWord(valueText, firstWordAdjustment);

            var verb = MakeInfiniteVerb(firstWord);

            if (firstWord != verb)
            {
                return verb.ConcatenatedWith(valueText.WithoutFirstWord(wordBoundary));
            }

            return text.ToString();
        }

        /// <summary>
        /// Converts the first word of the specified text to its past form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its past form; or the original text if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordPastTenseVerb(string text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return text;
            }

            return MakeFirstWordPastTenseVerb(text.AsSpan(), firstWordAdjustment, wordBoundary);
        }

        /// <summary>
        /// Converts the first word of the specified text to its past form.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to convert.
        /// </param>
        /// <param name="firstWordAdjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <param name="wordBoundary">
        /// One of the enumeration members that specifies the word boundary method to use.
        /// The default is <see cref="WordBoundary.WhiteSpaces"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the text with the first word converted to its past form; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeFirstWordPastTenseVerb(in ReadOnlySpan<char> text, in FirstWordAdjustment firstWordAdjustment = FirstWordAdjustment.None, in WordBoundary wordBoundary = WordBoundary.WhiteSpaces)
        {
            if (text.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // it may happen that the text starts with a special character, such as an ':'
            // so remove those special characters
            var valueText = text.TrimStart(Constants.SentenceMarkers);

            if (valueText.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // first word
            var firstWord = GetFirstWord(valueText, firstWordAdjustment);

            var verb = MakePastTenseVerb(firstWord);

            if (firstWord != verb)
            {
                return verb.ConcatenatedWith(valueText.WithoutFirstWord(wordBoundary));
            }

            return text.ToString();
        }

        /// <summary>
        /// Converts the specified word to its gerund form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the gerund form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeGerundVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            return GerundVerbs.GetOrAdd(value, CreateGerundVerb);

            string CreateGerundVerb(string word)
            {
                if (TryGetVerbForPrefixedVerb(word.AsSpan(), MakeGerundVerb, out string verb))
                {
                    return verb;
                }

                if (IsPastTenseVerb(word))
                {
                    word = MakeInfiniteVerb(word);
                }

                if (word.Length > 4)
                {
                    if (word.EndsWith("ing", StringComparison.Ordinal))
                    {
                        return word;
                    }
                }
                else
                {
                    if (word.EqualsAny(IsAre, StringComparison.OrdinalIgnoreCase))
                    {
                        return word;
                    }
                }

                if (WantsDoubleConstants(word))
                {
                    // duplicate word's last consonant
                    word += word[word.Length - 1];
                }

                var gerundVerb = word.AsCachedBuilder()
                                     .Append("ing")
                                     .ReplaceWithProbe("ping", "pping")
                                     .ReplaceWithProbe("eing", "ing")
                                     .ReplaceWithProbe("uring", "urring")
                                     .ReplaceWithProbe("uting", "utting")
                                     .ToStringAndRelease();

                return gerundVerb;
            }

            bool WantsDoubleConstants(string word)
            {
                if (word.EndsWithAny(EndingsWithoutConsonantDoublingForGerund))
                {
                    return false;
                }

                return word.EndsWithAny(EndingsWithConsonantDoublingForGerund);
            }
        }

        /// <summary>
        /// Converts the specified word to its infinitive form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the infinitive form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
#pragma warning disable CA1505 // Suppressed because CA1505 counts local functions, but that should not affect the method imho
        public static string MakeInfiniteVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            return InfiniteVerbs.GetOrAdd(value, CreateInfiniteVerb);
#pragma warning restore CA1505

            string CreateInfiniteVerb(string word)
            {
                var wordSpan = word.AsSpan();

                if (TryGetVerbForPrefixedVerb(wordSpan, MakeInfiniteVerb, out string verb))
                {
                    return verb;
                }

                if (wordSpan.EndsWith('g'))
                {
                    if (wordSpan.EndsWith("ing"))
                    {
                        return FromIng(wordSpan, word);
                    }
                }
                else if (wordSpan.EndsWith('r'))
                {
                    if (wordSpan.EndsWith("gister"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("liver"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("gger"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("bber") || wordSpan.EndsWith("pper"))
                    {
                        return word.Substring(0, wordSpan.Length - 3);
                    }

                    if (wordSpan.EndsWith("ter"))
                    {
                        return word.Substring(0, wordSpan.Length - 2);
                    }

                    if (wordSpan.EndsWith("ptor"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith("be");
                    }

                    if (wordSpan.EndsWith("nger"))
                    {
                        return word.Substring(0, wordSpan.Length - 2);
                    }

                    if (wordSpan.EndsWith("iver") || wordSpan.EndsWith("zer") || wordSpan.EndsWith("ger") || wordSpan.EndsWith("ber") || wordSpan.EndsWith("encer"))
                    {
                        return word.Substring(0, wordSpan.Length - 1);
                    }

                    if (wordSpan.EndsWith("uccessor"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith("ed");
                    }
                }
                else if (wordSpan.EndsWith('s'))
                {
                    return FromS(wordSpan, word);
                }
                else if (wordSpan.EndsWith('t'))
                {
                    return FromT(wordSpan, word);
                }
                else if (wordSpan.EndsWith('y'))
                {
                    if (wordSpan.EndsWith("ery"))
                    {
                        return word.Substring(0, wordSpan.Length - 1);
                    }

                    if (wordSpan.EndsWith("iry"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('e');
                    }
                }

                if (IsPastTenseVerb(word))
                {
                    return FromPast(word);
                }

                if (wordSpan.EndsWith('n'))
                {
                    if (wordSpan.EndsWith("ison"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("ion"))
                    {
                        return FromIon(word, wordSpan);
                    }
                }
                else if (wordSpan.EndsWith('e'))
                {
                    if (wordSpan.EndsWith("ete"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("osure"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("rrence"))
                    {
                        return word.Substring(0, wordSpan.Length - 5);
                    }

                    if (wordSpan.EndsWith("luence"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("ence"))
                    {
                        return word.Substring(0, wordSpan.Length - 4);
                    }

                    if (wordSpan.EndsWith("enance"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 6).ConcatenatedWith("ain");
                    }

                    if (wordSpan.EndsWith("nance"))
                    {
                        return word.Substring(0, wordSpan.Length - 4);
                    }
                }

                return word;
            }

            string FromPast(string word)
            {
                var wordSpan = word.AsSpan();

                if (wordSpan.EndsWith("pleted") || wordSpan.EndsWith("lenged") || wordSpan.EndsWith("hanged") || wordSpan.EndsWith("luenced"))
                {
                    return word.Substring(0, wordSpan.Length - 1);
                }

                if (wordSpan.EndsWithAny(ThreeCharacterPastEndings))
                {
                    return word.Substring(0, word.Length - 3);
                }

                if (wordSpan.EndsWith("ctored")
                 || wordSpan.EndsWith("dded")
                 || wordSpan.EndsWith("ered")
                 || wordSpan.EndsWith("itored")
                 || wordSpan.EndsWith("ivered")
                 || wordSpan.EndsWith("ixed")
                 || wordSpan.EndsWith("ltered")
                 || wordSpan.EndsWith("nged")
                 || wordSpan.EndsWith("ntered")
                 || wordSpan.EndsWith("peated")
                 || wordSpan.EndsWith("rked")
                 || wordSpan.EndsWith("shed")
                 || wordSpan.EndsWith("ssed")
                 || wordSpan.EndsWith("tched")
                 || wordSpan.EndsWith("zzed"))
                {
                    return word.Substring(0, word.Length - 2);
                }

                if (wordSpan.EndsWith("eed"))
                {
                    return word;
                }

                if (wordSpan.EndsWith("ied"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('y');
                }

                if (word.EndsWithAny(SpecialPastEndings))
                {
                    return word.Substring(0, word.Length - 1);
                }

                return word.Substring(0, word.Length - 2);
            }

            string FromIon(string word, in ReadOnlySpan<char> wordSpan)
            {
                if (wordSpan.EndsWith("ation"))
                {
                    if (wordSpan.EndsWith("cination") || wordSpan.EndsWith("peration") || wordSpan.EndsWith("reation"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("eration") || wordSpan.EndsWith("rmation"))
                    {
                        return word.Substring(0, wordSpan.Length - 5);
                    }

                    if (wordSpan.EndsWith("uation") || wordSpan.EndsWith("uration") || wordSpan.EndsWith("ination") || wordSpan.EndsWith("iration") || wordSpan.EndsWith("isation") || wordSpan.EndsWith("itation") || wordSpan.EndsWith("ization"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("istration"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 6).ConcatenatedWith("er");
                    }

                    if (wordSpan.EndsWithAny(ThreeCharacterInfiniteEndingsWithE))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("ellation"))
                    {
                        return word.Substring(0, wordSpan.Length - 6);
                    }

                    if (wordSpan.EndsWith("ndation") || wordSpan.EndsWith("lation") || wordSpan.EndsWith("tation"))
                    {
                        return word.Substring(0, wordSpan.Length - 5);
                    }

                    if (wordSpan.EndsWith("fication") || wordSpan.EndsWith("plication"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 7).ConcatenatedWith('y');
                    }

                    if (wordSpan.EndsWith("cupation"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith('y');
                    }

                    if (wordSpan.EndsWith("ocation"))
                    {
                        if (wordSpan.EndsWith("location", StringComparison.OrdinalIgnoreCase))
                        {
                            return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('e');
                        }

                        return wordSpan.Slice(0, wordSpan.Length - 6).ConcatenatedWith("ke");
                    }
                }
                else if (wordSpan.EndsWith("tion"))
                {
                    if (wordSpan.EndsWith("etion"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("duction"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("dition"))
                    {
                        return word.Substring(0, wordSpan.Length - 5);
                    }

                    if (wordSpan.EndsWith("quisition"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 6).ConcatenatedWith("re");
                    }

                    if (wordSpan.EndsWith("sition") || wordSpan.EndsWith("nition"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith('e');
                    }

                    if (wordSpan.EndsWith("iption"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith("be");
                    }

                    if (wordSpan.EndsWith("etition"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 6).ConcatenatedWith("at");
                    }
                }
                else if (wordSpan.EndsWith("sion"))
                {
                    if (wordSpan.EndsWith("nsion"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('d');
                    }

                    if (wordSpan.EndsWith("pulsion"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 6).ConcatenatedWith("el");
                    }

                    if (wordSpan.EndsWith("nversion"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('t');
                    }

                    if (wordSpan.EndsWith("rsion"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("ession"))
                    {
                        return word.Substring(0, wordSpan.Length - 3);
                    }

                    if (wordSpan.EndsWith("ission"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith('t');
                    }
                }

                return word.Substring(0, wordSpan.Length - 3);
            }

            string FromIng(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.Length is 4)
                {
                    // ignore short word such as "ping" or "thing"
                    return word;
                }

                if (wordSpan.EndsWith("thing", StringComparison.OrdinalIgnoreCase))
                {
                    return word;
                }

                if (wordSpan.EndsWithAny(FourCharacterGerundEndings))
                {
                    return word.Substring(0, wordSpan.Length - 4);
                }

                if (wordSpan.EndsWithAny(ThreeCharacterGerundEndings))
                {
                    return word.Substring(0, wordSpan.Length - 3);
                }

                if (wordSpan.EndsWithAny(ThreeCharacterGerundEndingsWithE))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('e');
                }

                return word.Substring(0, wordSpan.Length - 3);
            }

            string FromS(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("uccess"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("ed");
                }

                if (wordSpan.EndsWith("uccessors"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith("ed");
                }

                if (wordSpan.EndsWith("ss"))
                {
                    return word;
                }

                if (wordSpan.EndsWith("ies"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('y');
                }

                if (wordSpan.EndsWith("sis"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("ze");
                }

                if (wordSpan.EndsWith("ptors"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith("be");
                }

                if (wordSpan.EndsWith("bbers") || wordSpan.EndsWith("ppers"))
                {
                    return word.Substring(0, wordSpan.Length - 4);
                }

                if (wordSpan.EndsWith("oes") || wordSpan.EndsWith("zers") || wordSpan.EndsWith("shes") || wordSpan.EndsWith("sses") || wordSpan.EndsWith("tches") || wordSpan.EndsWith("zzes") || wordSpan.EndsWith("xes"))
                {
                    return word.Substring(0, wordSpan.Length - 2);
                }

                if (wordSpan.EndsWith("ions") || wordSpan.EndsWith("isons") || wordSpan.EndsWith("ments") || wordSpan.EndsWith("ers"))
                {
                    return MakeInfiniteVerb(word.Substring(0, wordSpan.Length - 1));
                }

                return word.Substring(0, wordSpan.Length - 1);
            }

            string FromT(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("et"))
                {
                    return word;
                }

                if (wordSpan.EndsWith("nquest"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith('r');
                }

                if (wordSpan.EndsWith("nt"))
                {
                    if (wordSpan.EndsWith("ant") || wordSpan.EndsWith("rnt"))
                    {
                        return word.Substring(0, wordSpan.Length - 1);
                    }

                    if (wordSpan.EndsWith("ment"))
                    {
                        if (wordSpan.EndsWith("ocument") || wordSpan.EndsWith("plement") || wordSpan.EndsWith("rgument"))
                        {
                            return word;
                        }

                        return word.Substring(0, wordSpan.Length - 4);
                    }

                    return word;
                }

                if (wordSpan.EndsWith("pt"))
                {
                    if (wordSpan.EndsWith("eapt"))
                    {
                        return word.Substring(0, wordSpan.Length - 1);
                    }

                    if (wordSpan.EndsWith("ept"))
                    {
                        if (wordSpan.EndsWith("cept"))
                        {
                            return word;
                        }

                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("ep");
                    }

                    if (wordSpan.EndsWith("ript"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("be");
                    }

                    return word;
                }

                if (wordSpan.EndsWith("lt"))
                {
                    if (wordSpan.EndsWith("elt"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('l');
                    }

                    if (wordSpan.EndsWith("alt"))
                    {
                        return word.Substring(0, wordSpan.Length - 1);
                    }

                    if (wordSpan.EndsWith("ilt"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('d');
                    }
                }

                if (wordSpan.EndsWith("amt"))
                {
                    return word.Substring(0, wordSpan.Length - 1);
                }

                return word;
            }
        }

        /// <summary>
        /// Converts the specified word to its past form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the past form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakePastTenseVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            return PastVerbs.GetOrAdd(value, CreatePastVerbWithAdd);

            string CreatePastVerbWithAdd(string word)
            {
                var pastVerb = CreatePastVerb(word);

                PastVerbsOnly.Add(pastVerb);

                return pastVerb;
            }

            string CreatePastVerb(string word)
            {
                var wordSpan = word.AsSpan();

                if (TryGetVerbForPrefixedVerb(wordSpan, MakePastTenseVerb, out string verb))
                {
                    return verb;
                }

                if (wordSpan.EndsWith("ed"))
                {
                    if (wordSpan.EndsWith("eed"))
                    {
                        return word + "ed";
                    }

                    return word;
                }

                if (wordSpan.EndsWith("eep"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("pt");
                }

                if (wordSpan.EndsWith("sis"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("zed");
                }

                if (IsGerundVerb(wordSpan)
                 || IsThirdPersonSingularVerb(wordSpan)
                 || wordSpan.EndsWith("on")
                 || wordSpan.EndsWith("er")
                 || wordSpan.EndsWith("ers")
                 || wordSpan.EndsWith("tor")
                 || wordSpan.EndsWith("tors")
                 || wordSpan.EndsWith("uest")
                 || wordSpan.EndsWith("uests")
                 || wordSpan.EndsWith("cript")
                 || wordSpan.EndsWith("ment")
                 || wordSpan.EndsWith("ence")
                 || wordSpan.EndsWith("nance")
                 || wordSpan.EndsWith("ure")
                 || wordSpan.EndsWith("pt"))
                {
                    word = MakeInfiniteVerb(word);

                    if (PastVerbs.TryGetValue(word, out var pastTense))
                    {
                        return pastTense;
                    }

                    wordSpan = word.AsSpan();
                }

                if (wordSpan.EndsWith('d'))
                {
                    if (wordSpan.EndsWith("ld"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('t');
                    }
                }
                else if (wordSpan.EndsWith('e'))
                {
                    if (wordSpan.EndsWith("ake"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("oke");
                    }

                    if (wordSpan.EndsWith("ive"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("ove");
                    }

                    if (wordSpan.EndsWith("voke"))
                    {
                        return wordSpan.ConcatenatedWith('d');
                    }

                    if (wordSpan.EndsWith("oke"))
                    {
                        return word;
                    }

                    return wordSpan.ConcatenatedWith('d');
                }
                else if (wordSpan.EndsWith('g'))
                {
                    if (wordSpan.EndsWith("ing"))
                    {
                        // could be a noun ending with 'ing'
                        if (wordSpan.EndsWith("lding"))
                        {
                            return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('t');
                        }

                        // could be a noun ending with 'ing'
                        if (wordSpan.EndsWith("melling"))
                        {
                            return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('t');
                        }

                        // could be a noun ending with 'ing'
                        if (wordSpan.EndsWith("tting"))
                        {
                            return MakePastTenseVerb(word.Substring(0, wordSpan.Length - 4));
                        }

                        return MakePastTenseVerb(word.Substring(0, wordSpan.Length - 3));
                    }
                }
                else if (wordSpan.EndsWith('n'))
                {
                    if (wordSpan.EndsWith("ean"))
                    {
                        return wordSpan.ConcatenatedWith('t');
                    }

                    if (wordSpan.EndsWith("earn"))
                    {
                        return wordSpan.ConcatenatedWith('t');
                    }
                }
                else if (wordSpan.EndsWith('p'))
                {
                    if (wordSpan.EndsWith("eap"))
                    {
                        return word + "t";
                    }
                }
                else if (wordSpan.EndsWith('r'))
                {
                    if (wordSpan.EndsWith("lor"))
                    {
                        return word + "ized";
                    }

                    if (wordSpan.EndsWith("uccessor"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith("eded");
                    }
                }
                else if (wordSpan.EndsWith('s'))
                {
                    if (wordSpan.EndsWith("uccess"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("eded");
                    }

                    if (wordSpan.EndsWith("uccessors"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith("eded");
                    }
                }
                else if (wordSpan.EndsWith('t'))
                {
                    if (wordSpan.EndsWith("lt"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("ept"))
                    {
                        if (wordSpan.EndsWith("cept"))
                        {
                            return word + "ed";
                        }

                        return word;
                    }

                    if (wordSpan.EndsWith("eant"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("earnt"))
                    {
                        return word;
                    }
                }
                else if (wordSpan.EndsWith('y'))
                {
                    if (wordSpan.EndsWith("ay") || wordSpan.EndsWith("ey"))
                    {
                        return word + "ed";
                    }

                    if (wordSpan.EndsWith("ery") || wordSpan.EndsWith("iry"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith("ed");
                    }

                    return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith("ied");
                }

                if (WantsDoubleConstants(wordSpan))
                {
                    // duplicate word's last consonant
                    return wordSpan.ConcatenatedWith(wordSpan[wordSpan.Length - 1], "ed");
                }

                return word + "ed";
            }

            bool WantsDoubleConstants(in ReadOnlySpan<char> word)
            {
                if (word.EndsWithAny(PastEndingsWithoutConsonantDoubling))
                {
                    return false;
                }

                if (word.EndsWithAny(PastEndingsWithConsonantDoubling))
                {
                    return true;
                }

                return word.EndsWithAny("gmpt") && word.Length > 1 && word[word.Length - 2].EqualsAny("aeiou");
            }
        }

        /// <summary>
        /// Converts the specified word to its third-person singular form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the third-person singular form of the word; or the original word if it is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        public static string MakeThirdPersonSingularVerb(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return value;
            }

            if (value.EqualsAny(ThirdPersonalSingularVerbExceptions, StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }

            if (value.EndsWithAny(SentenceEndingMarkers))
            {
                return ThirdPersonSingularVerbs.GetOrAdd(value, CreateThirdPersonSingularVerbForSentence);
            }

            return ThirdPersonSingularVerbs.GetOrAdd(value, CreateThirdPersonSingularVerb);

            string CreateThirdPersonSingularVerbForSentence(string sentenceEnding)
            {
                var word = sentenceEnding.TrimEnd(SentenceEndingMarkers);

                return CreateThirdPersonSingularVerb(word).ConcatenatedWith(sentenceEnding.AsSpan(word.Length));
            }

            string CreateThirdPersonSingularVerb(string word)
            {
                var wordSpan = word.AsSpan();

                if (TryGetVerbForPrefixedVerb(wordSpan, MakeThirdPersonSingularVerb, out string verb))
                {
                    return verb;
                }

                if (wordSpan.Length > 0)
                {
                    const char Apostrophe = '\'';
                    const char DoubleQuote = '\"';

                    var firstChar = wordSpan[0];
                    var lastChar = wordSpan[wordSpan.Length - 1];

                    switch (firstChar)
                    {
                        case Apostrophe when lastChar == Apostrophe:
                            return CreateThirdPersonSingularVerb(word.Trim(Apostrophe)).SurroundedWithApostrophe();

                        case DoubleQuote when lastChar == DoubleQuote:
                            return CreateThirdPersonSingularVerb(word.Trim(DoubleQuote)).SurroundedWithDoubleQuote();
                    }
                }

                if (wordSpan.EndsWith('d'))
                {
                    return FromD(wordSpan);
                }

                if (wordSpan.EndsWith('e'))
                {
                    return FromE(wordSpan);
                }

                if (wordSpan.EndsWith('g'))
                {
                    return FromG(wordSpan, word);
                }

                if (wordSpan.EndsWith('l'))
                {
                    return FromL(wordSpan);
                }

                if (wordSpan.EndsWith('n'))
                {
                    return FromN(wordSpan, word);
                }

                if (wordSpan.EndsWith('r'))
                {
                    return FromR(wordSpan, word);
                }

                if (wordSpan.EndsWith('s'))
                {
                    return FromS(wordSpan, word);
                }

                if (wordSpan.EndsWith('t'))
                {
                    return FromT(wordSpan, word);
                }

                if (wordSpan.EndsWith('y'))
                {
                    return FromY(wordSpan);
                }

                return AppendEndingS(wordSpan);
            }

            string FromPast(in ReadOnlySpan<char> wordSpan)
            {
                if (wordSpan.EndsWith("eed"))
                {
                    return wordSpan.ConcatenatedWith('s');
                }

                var difference = 2;

                if (wordSpan.EndsWithAny(ThreeCharacterPastEndings))
                {
                    difference = 3;
                }
                else if (wordSpan.EndsWithAny(TwoCharacterPastEndings))
                {
                    difference = 2;
                }
                else if (wordSpan.EndsWithAny(SpecialPastEndings))
                {
                    difference = 1;
                }

                return wordSpan.Slice(0, wordSpan.Length - difference).ConcatenatedWith('s');
            }

            string FromIng(in ReadOnlySpan<char> wordSpan, string word)
            {
                var infiniteVerb = MakeInfiniteVerb(word);

                if (infiniteVerb == word)
                {
                    // did not work, so just use this to avoid infinite recursion
                    return AppendEndingS(wordSpan);
                }

                return MakeThirdPersonSingularVerb(infiniteVerb);
            }

            string FromD(in ReadOnlySpan<char> wordSpan)
            {
                if (wordSpan.EndsWith("ed"))
                {
                    return FromPast(wordSpan);
                }

                return AppendEndingS(wordSpan);
            }

            string FromE(in ReadOnlySpan<char> wordSpan)
            {
                if (wordSpan.EndsWith("rrence"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 5).ConcatenatedWith('s');
                }

                if (wordSpan.EndsWith("rence"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('s');
                }

                if (wordSpan.EndsWith("osure"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("es");
                }

                if (wordSpan.EndsWith("oice"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("oses");
                }

                return wordSpan.ConcatenatedWith('s');
            }

            string FromG(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("ing"))
                {
                    return FromIng(wordSpan, word);
                }

                return AppendEndingS(wordSpan);
            }

            string FromL(in ReadOnlySpan<char> wordSpan)
            {
                return AppendEndingS(wordSpan);
            }

            string FromN(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("ion") || wordSpan.EndsWith("ison") || wordSpan.EndsWith("roken") || wordSpan.EndsWith("gan") || wordSpan.EndsWith("osen"))
                {
                    var infinite = MakeInfiniteVerb(word);

                    if (infinite != word)
                    {
                        return MakeThirdPersonSingularVerb(infinite);
                    }
                }

                return AppendEndingS(wordSpan);
            }

            string FromR(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("er"))
                {
                    if (wordSpan.EndsWith("gister"))
                    {
                        return wordSpan.ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("liver"))
                    {
                        return wordSpan.ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("iver"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("lter"))
                    {
                        return wordSpan.ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("ter"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("gger"))
                    {
                        return wordSpan.ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("bber") || wordSpan.EndsWith("pper"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("nger"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("ger") || wordSpan.EndsWith("ber") || wordSpan.EndsWith("zer") || wordSpan.EndsWith("encer"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('s');
                    }
                }

                if (wordSpan.EndsWith("or"))
                {
                    var infinite = MakeInfiniteVerb(word);

                    if (word != infinite)
                    {
                        return MakeThirdPersonSingularVerb(infinite);
                    }
                }

                return AppendEndingS(wordSpan);
            }

            string FromS(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("ss"))
                {
                    if (wordSpan.EndsWith("uccess"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("eds");
                    }

                    return word + "es";
                }

                if (wordSpan.EndsWith("sis"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith("zes");
                }

                if (wordSpan.EndsWith("oes") || wordSpan.EndsWith("shes"))
                {
                    return word;
                }

                if (wordSpan.EndsWith("ers"))
                {
                    if (wordSpan.EndsWith("ggers"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("bbers") || wordSpan.EndsWith("ppers"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 4).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("ngers"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("gers") || wordSpan.EndsWith("bers") || wordSpan.EndsWith("zers"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith('s');
                    }

                    if (wordSpan.EndsWith("gisters"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("lters"))
                    {
                        return word;
                    }

                    if (wordSpan.EndsWith("ters"))
                    {
                        return wordSpan.Slice(0, wordSpan.Length - 3).ConcatenatedWith('s');
                    }
                }

                if (wordSpan.EndsWith("ions") || wordSpan.EndsWith("ments") || wordSpan.EndsWith("ers") || wordSpan.EndsWith("ors"))
                {
                    var infinite = MakeInfiniteVerb(word);

                    if (word != infinite)
                    {
                        return MakeThirdPersonSingularVerb(infinite);
                    }
                }

                if (IsThirdPersonSingularVerb(wordSpan))
                {
                    return word;
                }

                return AppendEndingS(wordSpan);
            }

            string FromT(in ReadOnlySpan<char> wordSpan, string word)
            {
                if (wordSpan.EndsWith("et") || wordSpan.EndsWith("it") || wordSpan.EndsWith("cept") || wordSpan.EndsWith("dapt") || wordSpan.EndsWith("opt") || wordSpan.EndsWith("ort"))
                {
                    return wordSpan.ConcatenatedWith('s');
                }

                if (wordSpan.EndsWith("ept"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("eps");
                }

                if (wordSpan.EndsWith("eapt"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('s');
                }

                if (wordSpan.EndsWith("ipt"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 2).ConcatenatedWith("bes");
                }

                if (wordSpan.EndsWith("lt") || wordSpan.EndsWith("mt") || wordSpan.EndsWith("nt") || wordSpan.EndsWith("aught") || wordSpan.EndsWith("uest"))
                {
                    var infinite = MakeInfiniteVerb(word);

                    if (word != infinite)
                    {
                        return MakeThirdPersonSingularVerb(infinite);
                    }
                }

                return AppendEndingS(wordSpan);
            }

            string FromY(in ReadOnlySpan<char> wordSpan)
            {
                if (wordSpan.EndsWith("ay") || wordSpan.EndsWith("ey"))
                {
                    return wordSpan.ConcatenatedWith('s');
                }

                if (wordSpan.EndsWith("iry"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith("es");
                }

                if (wordSpan.EndsWith("ivery"))
                {
                    return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith('s');
                }

                return wordSpan.Slice(0, wordSpan.Length - 1).ConcatenatedWith("ies");
            }

            string AppendEndingS(in ReadOnlySpan<char> wordSpan)
            {
                var result = wordSpan.ConcatenatedWith('s');

                if (IsTwoCharacterEndingsWithS(result))
                {
                    return wordSpan.ConcatenatedWith("es");
                }

                return result;
            }
        }

        /// <summary>
        /// Attempts to convert the specified word to its verb form.
        /// </summary>
        /// <param name="value">
        /// The word to convert.
        /// </param>
        /// <param name="result">
        /// On successful return, contains the verb form of the word; otherwise the original word.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word was successfully converted to a verb form; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool TryMakeVerb(string value, out string result)
        {
            result = value;

            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }

            var span = value.AsSpan();

            if (HasAcceptableStartingPhrase(span))
            {
                return false;
            }

            if (HasAcceptableMiddlePhrase(value))
            {
                return false;
            }

            if (HasAcceptableEndingPhrase(span))
            {
                return false;
            }

            for (int index = 0, length = Endings.Length; index < length; index++)
            {
                var pair = Endings[index];

                if (span.EndsWith(pair.Key))
                {
                    result = span.Slice(0, span.Length - pair.Key.Length).ConcatenatedWith(pair.Value);

                    return result.Equals(value, StringComparison.Ordinal) is false;
                }
            }

            // TODO RKN: Call "MakeInfiniteVerb" here ?
            return false;
        }

        private static bool TryGetVerbForPrefixedVerb(in ReadOnlySpan<char> word, Func<string, string> callback, out string verb)
        {
            verb = null;

            foreach (var prefix in VerbPrefixes)
            {
                if (word.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    verb = GetVerbForPrefixedVerb(word, prefix, callback);

                    return true;
                }
            }

            return false;
        }

        private static string GetVerbForPrefixedVerb(in ReadOnlySpan<char> word, string prefix, Func<string, string> callback)
        {
            var wordWithoutPrefix = word.Slice(prefix.Length).ToString();

            // try to find form without prefix
            var verbWithoutPrefix = callback(wordWithoutPrefix);

            // now re-apply prefix
            var fixedVerb = word.Slice(0, prefix.Length).ConcatenatedWith(verbWithoutPrefix);

            return fixedVerb;
        }

        /// <summary>
        /// Gets the first word of the specified text and adjusts it based on the given adjustment.
        /// </summary>
        /// <param name="text">
        /// The text containing the word to return.
        /// </param>
        /// <param name="adjustment">
        /// A bitwise combination of the enumeration members that specifies the adjustment to apply to the first word's casing.
        /// The default is <see cref="FirstWordAdjustment.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> that contains the adjusted first word; or the <see cref="string.Empty"/> string ("") if the text is <see langword="null"/> or consists only of whitespace characters.
        /// </returns>
        private static string GetFirstWord(in ReadOnlySpan<char> text, in FirstWordAdjustment adjustment = FirstWordAdjustment.None)
        {
            var word = text.FirstWord();

            switch (adjustment)
            {
                case FirstWordAdjustment.StartLowerCase: return word.ToLowerCaseAt(0);
                case FirstWordAdjustment.StartUpperCase: return word.ToUpperCaseAt(0);
                default:
                    return word.ToString();
            }
        }

        /// <summary>
        /// Determines whether the specified word starts with an acceptable phrase.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word starts with an acceptable phrase; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasAcceptableStartingPhrase(in ReadOnlySpan<char> value)
        {
            for (int index = 0, length = StartingPhrases.Length; index < length; index++)
            {
                var phrase = StartingPhrases[index];

                if (value.StartsWith(phrase))
                {
                    var remaining = value.Slice(phrase.Length);

                    return remaining.Length is 0 || remaining[0].IsUpperCase();
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified word contains an acceptable phrase.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word contains an acceptable phrase; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasAcceptableMiddlePhrase(string value) => value.ContainsAny(MiddlePhrases);

        /// <summary>
        /// Determines whether the specified word ends with an acceptable phrase.
        /// </summary>
        /// <param name="value">
        /// The word to check.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the word ends with an acceptable phrase; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool HasAcceptableEndingPhrase(in ReadOnlySpan<char> value) => value.EndsWithAny(EndingPhrases);

        /// <summary>
        /// Creates a <see cref="KeyValuePair{TKey,TValue}"/> for the given key and value.
        /// </summary>
        /// <param name="key">
        /// The word to use as <see cref="KeyValuePair{TKey,TValue}.Key"/>.
        /// </param>
        /// <param name="value">
        /// The word to use as <see cref="KeyValuePair{TKey,TValue}.Value"/>.
        /// </param>
        /// <returns>
        /// The <see cref="KeyValuePair{TKey,TValue}"/> for the given key and value.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static KeyValuePair<string, string> Paired(string key, string value) => new KeyValuePair<string, string>(key, value);
    }
}