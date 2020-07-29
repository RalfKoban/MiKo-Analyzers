﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using NUnit.Framework;

using TestHelper;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [TestFixture]
    public sealed class MiKo_1054_HelperClassNameAnalyzerTests : CodeFixVerifier
    {
        private static readonly string[] WrongNames = CreateWrongNames();

        [Test]
        public void No_issue_is_reported_for_correctly_named_class() => No_issue_is_reported_for(@"
public class TestMe
{
}
");

        [Test]
        public void An_issue_is_reported_for_wrong_name_([ValueSource(nameof(WrongNames))] string name) => An_issue_is_reported_for(@"
public class " + name + @"
{
}
");

        protected override string GetDiagnosticId() => MiKo_1054_HelperClassNameAnalyzer.Id;

        protected override DiagnosticAnalyzer GetObjectUnderTest() => new MiKo_1054_HelperClassNameAnalyzer();

        [ExcludeFromCodeCoverage]
        private static string[] CreateWrongNames()
        {
            var names = new[]
                            {
                                "Helper",
                                "Helpers",
                                "Util",
                                "Utils",
                                "Utility",
                                "Utilities",
                            };

            var allNames = new HashSet<string>(names);
            foreach (var s in names)
            {
                var lower = s.ToLowerInvariant();
                var upper = s.ToUpperInvariant();

                foreach (var name in new[] { s, lower, upper })
                {
                    allNames.Add(name);
                    allNames.Add("Some" + name);
                    allNames.Add(name + "Stuff");
                    allNames.Add("Some" + name + "Stuff");
                }
            }

            return allNames.OrderBy(_ => _).ToArray();
        }
    }
}