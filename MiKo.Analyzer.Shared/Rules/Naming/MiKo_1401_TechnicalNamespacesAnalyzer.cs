﻿using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1401_TechnicalNamespacesAnalyzer : NamingNamespaceAnalyzer
    {
        public const string Id = "MiKo_1401";

        private static readonly HashSet<string> TechnicalNamespaces = new HashSet<string>
                                                                          {
                                                                              "API", "Api", "APIs", "Apis",
                                                                              "Base", "Class", "Classes",
                                                                              "Controller", "Controllers",
                                                                              "Dto", "DTO", "DTOs", "Dtos",
                                                                              "Enum", "Enums", "Enumeration", "Enumerations",
                                                                              "Exception", "Exceptions",
                                                                              "Func", "Funcs", "Function", "Functions",
                                                                              "Imp", "Impl", "Implementation", "Implementations",
                                                                              "Interface", "Interfaces", "Itf", "Itfs",
                                                                              "Platform",
                                                                              "Proxies", "Proxy", "ServiceProxies", "ServiceProxy",
                                                                              "Record",
                                                                              "Struct", "Structs", "Action", "Actions",
                                                                          };

        public MiKo_1401_TechnicalNamespacesAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeNamespaceName(IEnumerable<SyntaxToken> names)
        {
            foreach (var name in names)
            {
                if (TechnicalNamespaces.Contains(name.ValueText))
                {
                    yield return Issue(name);
                }
            }
        }
    }
}