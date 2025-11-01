# MiKo-Analyzers
Provides analyzers that are based on the .NET Compiler Platform (Roslyn) and can be used inside Visual Studio 2019 (v16.11) or 2022 (v17.14).

How to install an Roslyn analyzer is described [here](https://learn.microsoft.com/en-us/visualstudio/code-quality/install-roslyn-analyzers?view=vs-2022).

Screenshots on how to use such analyzers can be found [here](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022).


## Build / Project status
[![Maintenance](https://img.shields.io/maintenance/yes/2025.svg)](https://github.com/RalfKoban/MiKo-Analyzers)
[![Build status](https://ci.appveyor.com/api/projects/status/qanrqn7r4q9frr9m/branch/master?svg=true)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/branch/master)
[![codecov](https://codecov.io/gh/RalfKoban/MiKo-Analyzers/branch/master/graph/badge.svg)](https://codecov.io/gh/RalfKoban/MiKo-Analyzers)
[![Coverity Scan Build Status](https://img.shields.io/coverity/scan/18917.svg)](https://scan.coverity.com/projects/ralfkoban-miko-analyzers)

## Available Rules
The following tables lists all the 528 rules that are currently provided by the analyzer.

### Metrics
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_0001|Keep methods small|&#x2713;|\-|
|MiKo_0002|Simplify complex methods|&#x2713;|\-|
|MiKo_0003|Keep types small|&#x2713;|\-|
|MiKo_0004|Limit method parameters|&#x2713;|\-|
|MiKo_0005|Keep local functions small|&#x2713;|\-|
|MiKo_0006|Simplify complex local functions|&#x2713;|\-|
|MiKo_0007|Limit local function parameters|&#x2713;|\-|

### Naming
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_1000|Suffix 'System.EventArgs' types with 'EventArgs'|&#x2713;|&#x2713;|
|MiKo_1001|Name 'System.EventArgs' parameters 'e'|&#x2713;|&#x2713;|
|MiKo_1002|Follow .NET Framework Design Guidelines for event handler parameter names|&#x2713;|&#x2713;|
|MiKo_1003|Follow .NET Framework Design Guidelines for event handling method names|&#x2713;|&#x2713;|
|MiKo_1004|Remove term 'Event' from event names|&#x2713;|&#x2713;|
|MiKo_1005|Name 'System.EventArgs' variables properly|&#x2713;|&#x2713;|
|MiKo_1006|Use 'EventHandler&lt;T&gt;' with 'EventArgs' named after the event|&#x2713;|\-|
|MiKo_1007|Place events and their 'EventArgs' types in the same namespace|&#x2713;|\-|
|MiKo_1008|Follow .NET Framework Design Guidelines for DependencyProperty event handler parameter names|&#x2713;|&#x2713;|
|MiKo_1009|Name 'System.EventHandler' variables properly|&#x2713;|&#x2713;|
|MiKo_1010|Do not include 'CanExecute' or 'Execute' in method names|&#x2713;|&#x2713;|
|MiKo_1011|Do not include 'Do' in method names|&#x2713;|&#x2713;|
|MiKo_1012|Use 'Raise' instead of 'Fire' in method names|&#x2713;|&#x2713;|
|MiKo_1013|Do not name methods 'Notify' or 'OnNotify'|&#x2713;|&#x2713;|
|MiKo_1014|Do not use ambiguous 'Check' in method names|&#x2713;|&#x2713;|
|MiKo_1015|Use 'Initialize' instead of 'Init' in method names|&#x2713;|&#x2713;|
|MiKo_1016|Name factory methods 'Create'|&#x2713;|&#x2713;|
|MiKo_1017|Do not prefix methods with 'Get' or 'Set' when followed by 'Is', 'Can' or 'Has'|&#x2713;|&#x2713;|
|MiKo_1018|Do not suffix methods with noun of a verb|&#x2713;|&#x2713;|
|MiKo_1019|Name 'Clear' and 'Remove' methods based on their number of parameters|&#x2713;|&#x2713;|
|MiKo_1020|Limit type name length|\-|\-|
|MiKo_1021|Limit method name length|\-|\-|
|MiKo_1022|Limit parameter name length|\-|\-|
|MiKo_1023|Limit field name length|\-|\-|
|MiKo_1024|Limit property name length|\-|\-|
|MiKo_1025|Limit event name length|\-|\-|
|MiKo_1026|Limit variable name length|\-|\-|
|MiKo_1027|Limit loop variable name length|\-|\-|
|MiKo_1028|Limit local function name length|\-|\-|
|MiKo_1030|Do not mark base types with 'Abstract' or 'Base'|&#x2713;|&#x2713;|
|MiKo_1031|Do not suffix entity types with 'Model'|&#x2713;|&#x2713;|
|MiKo_1032|Do not use 'Model' as marker in methods dealing with entities|&#x2713;|&#x2713;|
|MiKo_1033|Do not suffix entity parameters with 'Model'|&#x2713;|&#x2713;|
|MiKo_1034|Do not suffix entity fields with 'Model'|&#x2713;|&#x2713;|
|MiKo_1035|Do not use 'Model' marker in properties dealing with entities|&#x2713;|&#x2713;|
|MiKo_1036|Do not use 'Model' marker in events dealing with entities|&#x2713;|&#x2713;|
|MiKo_1037|Do not suffix types with 'Type', 'Interface', 'Class', 'Struct', 'Record' or 'Enum'|&#x2713;|&#x2713;|
|MiKo_1038|Use consistent suffix for extension method container classes|&#x2713;|&#x2713;|
|MiKo_1039|Use default name for 'this' parameter of extension methods|&#x2713;|&#x2713;|
|MiKo_1040|Do not suffix parameters with implementation details|&#x2713;|&#x2713;|
|MiKo_1041|Do not suffix fields with implementation details|&#x2713;|&#x2713;|
|MiKo_1042|Use specific name for 'CancellationToken' parameters|&#x2713;|&#x2713;|
|MiKo_1043|Use specific name for 'CancellationToken' variables|&#x2713;|&#x2713;|
|MiKo_1044|Suffix commands with 'Command'|&#x2713;|&#x2713;|
|MiKo_1045|Do not suffix command-invoked methods with 'Command'|&#x2713;|&#x2713;|
|MiKo_1046|Follow Task-based Asynchronous Pattern (TAP) for asynchronous methods|&#x2713;|&#x2713;|
|MiKo_1047|Do not falsely indicate asynchronous behavior for methods not following Task-based Asynchronous Pattern (TAP)|&#x2713;|&#x2713;|
|MiKo_1048|End value converter classes with a specific suffix|&#x2713;|&#x2713;|
|MiKo_1049|Do not use requirement terms such as 'Shall', 'Should', 'Must' or 'Need' for names|&#x2713;|&#x2713;|
|MiKo_1050|Use descriptive names for return values|&#x2713;|&#x2713;|
|MiKo_1051|Do not suffix parameters with delegate types|&#x2713;|&#x2713;|
|MiKo_1052|Do not suffix variables with delegate types|&#x2713;|&#x2713;|
|MiKo_1053|Do not suffix fields with delegate types|&#x2713;|&#x2713;|
|MiKo_1054|Do not name types 'Helper' or 'Utility'|&#x2713;|&#x2713;|
|MiKo_1055|Suffix dependency properties with 'Property' (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1056|Prefix dependency properties with property names (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1057|Suffix dependency property keys with 'Key' (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1058|Prefix dependency property keys with property names (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1059|Do not name types 'Impl' or 'Implementation'|&#x2713;|&#x2713;|
|MiKo_1060|Use '&lt;Entity&gt;NotFound' instead of 'Get&lt;Entity&gt;Failed' or '&lt;Entity&gt;Missing'|&#x2713;|&#x2713;|
|MiKo_1061|Use specific name for 'Try' method's [out] parameters|&#x2713;|&#x2713;|
|MiKo_1062|Keep 'Can/Has/Contains' methods, properties or fields to a few words|&#x2713;|\-|
|MiKo_1063|Do not use abbreviations in names|&#x2713;|&#x2713;|
|MiKo_1064|Make parameter names reflect their meaning, not their type|&#x2713;|\-|
|MiKo_1065|Follow .NET Framework Design Guidelines for operator overload parameter names|&#x2713;|&#x2713;|
|MiKo_1066|Name constructor parameters after the property they're assigned to|&#x2713;|&#x2713;|
|MiKo_1067|Do not include 'Perform' in method names|&#x2713;|&#x2713;|
|MiKo_1068|Name workflow methods 'CanRun' or 'Run'|&#x2713;|\-|
|MiKo_1069|Make property names reflect their meaning, not their type|&#x2713;|\-|
|MiKo_1070|Use plural names for local collection variables|&#x2713;|&#x2713;|
|MiKo_1071|Name local boolean variables as statements, not questions|&#x2713;|\-|
|MiKo_1072|Name boolean properties or methods as statements, not questions|&#x2713;|\-|
|MiKo_1073|Name boolean fields as statements, not questions|&#x2713;|\-|
|MiKo_1074|Suffix lock objects with 'Lock'|&#x2713;|\-|
|MiKo_1075|Do not suffix non-'System.EventArgs' types with 'EventArgs'|&#x2713;|&#x2713;|
|MiKo_1076|Suffix Prism event types with 'Event'|&#x2713;|&#x2713;|
|MiKo_1077|Do not suffix enum members with 'Enum'|&#x2713;|&#x2713;|
|MiKo_1078|Start builder method names with 'Build'|&#x2713;|&#x2713;|
|MiKo_1079|Do not suffix repositories with 'Repository'|&#x2713;|&#x2713;|
|MiKo_1080|Use numbers instead of their spellings in names|&#x2713;|\-|
|MiKo_1081|Do not suffix methods with a number|&#x2713;|&#x2713;|
|MiKo_1082|Do not suffix properties with a number if their types have number suffixes|&#x2713;|&#x2713;|
|MiKo_1083|Do not suffix fields with a number if their types have number suffixes|&#x2713;|&#x2713;|
|MiKo_1084|Do not suffix variables with a number if their types have number suffixes|&#x2713;|&#x2713;|
|MiKo_1085|Do not suffix parameters with a number|&#x2713;|&#x2713;|
|MiKo_1086|Do not use numbers as slang in method names|&#x2713;|\-|
|MiKo_1087|Name constructor parameters after their base class counterparts|&#x2713;|&#x2713;|
|MiKo_1088|Name singleton instances 'Instance'|&#x2713;|\-|
|MiKo_1089|Do not prefix methods with 'Get'|&#x2713;|&#x2713;|
|MiKo_1090|Do not suffix parameters with specific types|&#x2713;|&#x2713;|
|MiKo_1091|Do not suffix variables with specific types|&#x2713;|&#x2713;|
|MiKo_1092|Do not suffix 'Ability' types with redundant information|&#x2713;|&#x2713;|
|MiKo_1093|Do not use the suffix 'Object' or 'Struct'|&#x2713;|&#x2713;|
|MiKo_1094|Do not suffix types with passive namespace names|&#x2713;|\-|
|MiKo_1095|Do not use 'Delete' and 'Remove' both in names and documentation|&#x2713;|\-|
|MiKo_1096|Use 'Failed' instead of 'NotSuccessful' in names|&#x2713;|\-|
|MiKo_1097|Do not use field naming schemes for parameter names|&#x2713;|&#x2713;|
|MiKo_1098|Reflect implemented business interface(s) in type names|&#x2713;|\-|
|MiKo_1099|Use identical names for matching parameters on method overloads|&#x2713;|&#x2713;|
|MiKo_1100|Start test class names with the name of the type under test|&#x2713;|\-|
|MiKo_1101|End test class names with 'Tests'|&#x2713;|&#x2713;|
|MiKo_1102|Do not include 'Test' in test method names|&#x2713;|&#x2713;|
|MiKo_1103|Name test initialization methods 'PrepareTest'|&#x2713;|&#x2713;|
|MiKo_1104|Name test cleanup methods 'CleanupTest'|&#x2713;|&#x2713;|
|MiKo_1105|Name one-time test initialization methods 'PrepareTestEnvironment'|&#x2713;|&#x2713;|
|MiKo_1106|Name one-time test cleanup methods 'CleanupTestEnvironment'|&#x2713;|&#x2713;|
|MiKo_1107|Do not use Pascal-casing for test methods|&#x2713;|&#x2713;|
|MiKo_1108|Do not name variables, parameters, fields and properties 'Mock', 'Stub', 'Fake' or 'Shim'|&#x2713;|&#x2713;|
|MiKo_1109|Prefix testable types with 'Testable' instead of using the 'Ut' suffix|&#x2713;|&#x2713;|
|MiKo_1110|Suffix test methods with parameters with underscore|&#x2713;|&#x2713;|
|MiKo_1111|Do not suffix parameterless test methods with underscore|&#x2713;|&#x2713;|
|MiKo_1112|Do not name test data 'arbitrary'|&#x2713;|&#x2713;|
|MiKo_1113|Do not use BDD style naming for test methods|&#x2713;|\-|
|MiKo_1114|Do not name test methods 'HappyPath' or 'BadPath'|&#x2713;|\-|
|MiKo_1115|Name test methods in a fluent way|&#x2713;|&#x2713;|
|MiKo_1116|Use present tense for test method names|&#x2713;|&#x2713;|
|MiKo_1117|Make test method names more precise|&#x2713;|\-|
|MiKo_1118|Do not end test method names with 'Async'|&#x2713;|&#x2713;|
|MiKo_1200|Name catch block exceptions consistently|&#x2713;|&#x2713;|
|MiKo_1201|Name exception parameters consistently|&#x2713;|&#x2713;|
|MiKo_1300|Name unimportant lambda statement identifiers '_'|&#x2713;|&#x2713;|
|MiKo_1400|Use plural for namespace names|&#x2713;|\-|
|MiKo_1401|Do not include technical language names in namespaces|&#x2713;|\-|
|MiKo_1402|Do not name namespaces after WPF-specific design patterns|&#x2713;|\-|
|MiKo_1403|Do not name namespaces after any of their parent namespaces|&#x2713;|\-|
|MiKo_1404|Do not use unspecific names for namespaces|&#x2713;|\-|
|MiKo_1405|Do not include 'Lib' in namespaces|&#x2713;|\-|
|MiKo_1406|Place value converters in 'Converters' namespace|&#x2713;|\-|
|MiKo_1407|Do not include 'Test' in test namespaces|&#x2713;|\-|
|MiKo_1408|Place extension methods in same namespace as the extended types|&#x2713;|\-|
|MiKo_1409|Do not prefix or suffix namespaces with underscores|&#x2713;|\-|
|MiKo_1501|Do not use 'Filter' in names|&#x2713;|\-|
|MiKo_1502|Do not use 'Process' in names|&#x2713;|\-|
|MiKo_1503|Do not suffix methods with 'Counter'|&#x2713;|&#x2713;|
|MiKo_1504|Do not suffix properties with 'Counter'|&#x2713;|&#x2713;|
|MiKo_1505|Do not suffix fields with 'Counter'|&#x2713;|&#x2713;|
|MiKo_1506|Do not suffix local variables with 'Counter'|&#x2713;|&#x2713;|
|MiKo_1507|Do not suffix parameters with 'Counter'|&#x2713;|&#x2713;|
|MiKo_1508|Do not suffix local variables with pattern names|&#x2713;|&#x2713;|
|MiKo_1509|Do not suffix parameters with pattern names|&#x2713;|&#x2713;|
|MiKo_1510|Do not suffix fields with pattern names|&#x2713;|&#x2713;|
|MiKo_1511|Do not prefix or suffix local variables with 'proxy'|&#x2713;|&#x2713;|
|MiKo_1512|Do not prefix or suffix parameters with 'proxy'|&#x2713;|&#x2713;|
|MiKo_1513|Do not suffix types with 'Advanced', 'Complex', 'Enhanced', 'Extended', 'Simple' or 'Simplified'|&#x2713;|&#x2713;|
|MiKo_1514|Do not suffix types with 'Info'|&#x2713;|\-|
|MiKo_1515|Express binary conditions clearly in boolean property names|&#x2713;|&#x2713;|
|MiKo_1516|Express binary conditions clearly in boolean parameter names|&#x2713;|&#x2713;|
|MiKo_1517|Express binary conditions clearly in boolean field names|&#x2713;|&#x2713;|
|MiKo_1518|Do not prefix or suffix local variables with 'reference'|&#x2713;|&#x2713;|
|MiKo_1519|Do not prefix or suffix parameters with 'reference'|&#x2713;|&#x2713;|
|MiKo_1520|Do not prefix or suffix local variables with 'toCopy'|&#x2713;|&#x2713;|
|MiKo_1521|Do not prefix or suffix parameters with 'toCopy'|&#x2713;|&#x2713;|

### Documentation
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_2000|Write valid XML documentation|&#x2713;|&#x2713;|
|MiKo_2001|Document events properly|&#x2713;|&#x2713;|
|MiKo_2002|Document EventArgs properly|&#x2713;|&#x2713;|
|MiKo_2003|Start event handler documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2004|Follow .NET Framework Design Guidelines for event handler parameter names in documentation|&#x2713;|&#x2713;|
|MiKo_2005|Document textual references to EventArgs properly|&#x2713;|\-|
|MiKo_2006|Document routed events as done by the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2010|Document sealed classes as being sealed|&#x2713;|&#x2713;|
|MiKo_2011|Do not falsely document unsealed classes as sealed|&#x2713;|&#x2713;|
|MiKo_2012|Describe the type's responsibility in &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2013|Start Enum &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2014|Document Dispose methods as done by the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2015|Use 'raise' or 'throw' instead of 'fire' in documentation|&#x2713;|&#x2713;|
|MiKo_2016|Start documentation for asynchronous methods with specific phrase|&#x2713;|&#x2713;|
|MiKo_2017|Document dependency properties as done by the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2018|Do not use ambiguous terms 'Check' or 'Test' in documentation|&#x2713;|&#x2713;|
|MiKo_2019|Start &lt;summary&gt; documentation with third person singular verb (e.g., "Provides")|&#x2713;|&#x2713;|
|MiKo_2020|Use &lt;inheritdoc /&gt; marker for inherited documentation|&#x2713;|&#x2713;|
|MiKo_2021|Start parameter documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2022|Start [out] parameter documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2023|Start Boolean parameter documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2024|Start Enum parameter documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2025|Start 'CancellationToken' parameter documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2026|Do not document used parameters as unused|&#x2713;|\-|
|MiKo_2027|Document serialization constructor parameters with specific phrase|&#x2713;|&#x2713;|
|MiKo_2028|Provide more information than just parameter name in documentation|&#x2713;|\-|
|MiKo_2029|Do not use self-referencing 'cref' in &lt;inheritdoc&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2030|Start return value documentation with default phrase|&#x2713;|\-|
|MiKo_2031|Use specific (starting) phrase for Task return value documentation|&#x2713;|&#x2713;|
|MiKo_2032|Use specific phrase for Boolean return value documentation|&#x2713;|&#x2713;|
|MiKo_2033|Start String return value documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2034|Start Enum return value documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2035|Start collection return value documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2036|Describe default values in Boolean or Enum property documentation|&#x2713;|&#x2713;|
|MiKo_2037|Start command property &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2038|Start command &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2039|Start extension method class &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2040|Use &lt;see langword="..."/&gt; instead of &lt;c&gt;...&lt;/c&gt;|&#x2713;|&#x2713;|
|MiKo_2041|Do not include other documentation tags in &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2042|Use '&lt;para&gt;' XML tags instead of '&lt;p&gt;' HTML tags in documentation|&#x2713;|&#x2713;|
|MiKo_2043|Start custom delegate &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2044|Reference method parameters correctly in documentation|&#x2713;|&#x2713;|
|MiKo_2045|Do not reference parameters in &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2046|Reference type parameters correctly in documentation|&#x2713;|&#x2713;|
|MiKo_2047|Start Attribute &lt;summary&gt; documentation with default phrase|&#x2713;|\-|
|MiKo_2048|Start value converter &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2049|Use explicit wording instead of 'will be' in documentation|&#x2713;|&#x2713;|
|MiKo_2050|Follow .NET Framework conventions for exception documentation|&#x2713;|&#x2713;|
|MiKo_2051|Document thrown exceptions as conditions (e.g., '&lt;paramref name="xyz"/&gt; is &lt;c&gt;42&lt;/c&gt;')|&#x2713;|&#x2713;|
|MiKo_2052|Use default phrase for ArgumentNullException documentation|&#x2713;|&#x2713;|
|MiKo_2053|Document ArgumentNullException only for reference type parameters|&#x2713;|\-|
|MiKo_2054|Start ArgumentException documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2055|Start ArgumentOutOfRangeException documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2056|End ObjectDisposedException documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2057|Do not throw ObjectDisposedException from non-disposable types|&#x2713;|&#x2713;|
|MiKo_2059|Consolidate multiple documentations of same exception into one|&#x2713;|&#x2713;|
|MiKo_2060|Document factories uniformly|&#x2713;|&#x2713;|
|MiKo_2070|Do not start &lt;summary&gt; documentation with 'Returns'|&#x2713;|&#x2713;|
|MiKo_2071|Do not use boolean phrases in Enum return type &lt;summary&gt; documentation|&#x2713;|\-|
|MiKo_2072|Do not start &lt;summary&gt; documentation with 'Try'|&#x2713;|&#x2713;|
|MiKo_2073|Start 'Contains' method &lt;summary&gt; documentation with 'Determines whether'|&#x2713;|&#x2713;|
|MiKo_2074|End 'Contains' method parameter documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2075|Use 'callback' instead of 'action', 'func' or 'function' in documentation|&#x2713;|&#x2713;|
|MiKo_2076|Document default values of optional parameters|&#x2713;|&#x2713;|
|MiKo_2077|Do not include &lt;code&gt; in &lt;summary&gt; documentation|&#x2713;|\-|
|MiKo_2078|Do not include XML tags in &lt;code&gt; documentation|&#x2713;|\-|
|MiKo_2079|Do not include obvious text in property &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2080|Start field &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2081|End public-visible read-only field &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|MiKo_2082|Use distinct phrases for Enum member &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2083|Do not falsely document writable fields as read-only|&#x2713;|&#x2713;|
|MiKo_2090|Use default phrase for equality operator documentation|&#x2713;|&#x2713;|
|MiKo_2091|Use default phrase for inequality operator documentation|&#x2713;|&#x2713;|
|MiKo_2100|Start &lt;example&gt; documentation with descriptive default phrase|&#x2713;|&#x2713;|
|MiKo_2101|Show code examples within &lt;code&gt; tags in &lt;example&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2200|Start comments with a capitalized letter|&#x2713;|&#x2713;|
|MiKo_2201|Start sentences in comments with a capitalized letter|&#x2713;|\-|
|MiKo_2202|Use term 'identifier' instead of 'id' in documentation|&#x2713;|&#x2713;|
|MiKo_2203|Use term 'unique identifier' instead of 'guid' in documentation|&#x2713;|&#x2713;|
|MiKo_2204|Use &lt;list&gt; for enumerations in documentation|&#x2713;|&#x2713;|
|MiKo_2205|Use &lt;note&gt; for important information in documentation|&#x2713;|\-|
|MiKo_2206|Do not use term 'flag' in documentation|&#x2713;|\-|
|MiKo_2207|Keep &lt;summary&gt; documentation short|&#x2713;|\-|
|MiKo_2208|Do not use term 'an instance of' in documentation|&#x2713;|&#x2713;|
|MiKo_2209|Do not use double periods in documentation|&#x2713;|&#x2713;|
|MiKo_2210|Use term 'information' instead of 'info' in documentation|&#x2713;|&#x2713;|
|MiKo_2211|Do not use &lt;remarks&gt; sections for enum members|&#x2713;|&#x2713;|
|MiKo_2212|Use phrase 'failed' instead of 'was not successful' in documentation|&#x2713;|&#x2713;|
|MiKo_2213|Do not use contraction "n't" in documentation|&#x2713;|&#x2713;|
|MiKo_2214|Remove empty lines from documentation|&#x2713;|&#x2713;|
|MiKo_2215|Keep documentation sentences short|&#x2713;|\-|
|MiKo_2216|Use &lt;paramref&gt; instead of &lt;param&gt; to reference parameters|&#x2713;|&#x2713;|
|MiKo_2217|Format &lt;list&gt; documentation properly|&#x2713;|&#x2713;|
|MiKo_2218|Use shorter terms instead of 'used to/in/by' in documentation|&#x2713;|&#x2713;|
|MiKo_2219|Do not use question or exclamation marks in documentation|&#x2713;|\-|
|MiKo_2220|Use 'to seek' instead of 'to look for', 'to inspect for' or 'to test for' in documentation|&#x2713;|&#x2713;|
|MiKo_2221|Do not use empty XML tags in documentation|&#x2713;|\-|
|MiKo_2222|Use term 'identification' instead of 'ident' in documentation|&#x2713;|&#x2713;|
|MiKo_2223|Link references via &lt;see cref="..."/&gt; in documentation|&#x2713;|&#x2713;|
|MiKo_2224|Place XML tags and texts on separate lines in documentation|&#x2713;|&#x2713;|
|MiKo_2225|Place code marked with &lt;c&gt; tags on single line|&#x2713;|&#x2713;|
|MiKo_2226|Explain the 'Why' instead of the 'That' in documentation|&#x2713;|\-|
|MiKo_2227|Remove ReSharper suppressions from documentation|&#x2713;|\-|
|MiKo_2228|Use positive wording instead of negative in documentation|&#x2713;|\-|
|MiKo_2229|Remove left-over XML fragments from documentation|&#x2713;|&#x2713;|
|MiKo_2230|Use &lt;list&gt; for return values with specific meanings in documentation|&#x2713;|&#x2713;|
|MiKo_2231|Use '&lt;inheritdoc /&gt;' marker for overridden 'GetHashCode()' methods documentation|&#x2713;|&#x2713;|
|MiKo_2232|Do not leave &lt;summary&gt; documentation empty|&#x2713;|&#x2713;|
|MiKo_2233|Place XML tags on single line|&#x2713;|&#x2713;|
|MiKo_2234|Use 'to' instead of 'that is to' or 'which is to' in documentation|&#x2713;|&#x2713;|
|MiKo_2235|Use 'will' instead of 'going to' in documentation|&#x2713;|&#x2713;|
|MiKo_2236|Use 'for example' instead of abbreviation 'e.g.' in documentation|&#x2713;|&#x2713;|
|MiKo_2237|Do not separate documentation with empty lines|&#x2713;|&#x2713;|
|MiKo_2238|Do not start &lt;summary&gt; documentation with 'Make sure to call this'|&#x2713;|\-|
|MiKo_2239|Use '///' instead of '/** */' for documentation|&#x2713;|&#x2713;|
|MiKo_2240|Do not start &lt;response&gt; documentation with 'Returns'|&#x2713;|&#x2713;|
|MiKo_2241|Do not use 'empty string' in documentation|&#x2713;|&#x2713;|
|MiKo_2244|Use &lt;list&gt; instead of &lt;ul&gt; or &lt;ol&gt; in documentation|&#x2713;|&#x2713;|
|MiKo_2245|Wrap numbers with &lt;c&gt; in documentation|&#x2713;|&#x2713;|
|MiKo_2300|Explain the 'Why' instead of the 'How' in comments|&#x2713;|\-|
|MiKo_2301|Do not use obvious comments in AAA-Tests|&#x2713;|&#x2713;|
|MiKo_2302|Remove commented-out code|&#x2713;|\-|
|MiKo_2303|Do not end comments with a period|&#x2713;|&#x2713;|
|MiKo_2304|Do not formulate comments as questions|&#x2713;|\-|
|MiKo_2305|Do not use double periods in comments|&#x2713;|&#x2713;|
|MiKo_2306|End comments with a period|\-|\-|
|MiKo_2307|Use phrase 'failed' instead of 'was not successful' in comments|&#x2713;|&#x2713;|
|MiKo_2308|Place comments after code instead of on single line before closing brace|&#x2713;|&#x2713;|
|MiKo_2309|Do not use contraction "n't" in comments|&#x2713;|&#x2713;|
|MiKo_2310|Explain the 'Why' instead of the 'That' in comments|&#x2713;|\-|
|MiKo_2311|Do not use separator comments|&#x2713;|&#x2713;|
|MiKo_2312|Use 'to' instead of 'that is to' or 'which is to' in comments|&#x2713;|&#x2713;|
|MiKo_2313|Format plain documentation comments as XML documentation|&#x2713;|&#x2713;|

### Maintainability
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_3000|Do not use empty regions|&#x2713;|\-|
|MiKo_3001|Do not use custom delegates|&#x2713;|\-|
|MiKo_3002|Limit class dependencies|&#x2713;|\-|
|MiKo_3003|Follow .NET Framework Design Guidelines for events|&#x2713;|\-|
|MiKo_3004|Make EventArgs property setters private|&#x2713;|\-|
|MiKo_3005|Follow Trier-Doer-Pattern for methods named 'Try'|&#x2713;|\-|
|MiKo_3006|Place 'CancellationToken' parameter last in method parameters|&#x2713;|\-|
|MiKo_3007|Do not mix LINQ method and declarative query syntax in same method|&#x2713;|\-|
|MiKo_3008|Return immutable collections|&#x2713;|\-|
|MiKo_3009|Use named methods instead of lambda expressions with commands|&#x2713;|\-|
|MiKo_3010|Do not create or throw reserved exception types|&#x2713;|\-|
|MiKo_3011|Provide correct parameter name for ArgumentExceptions|&#x2713;|&#x2713;|
|MiKo_3012|Provide actual value when throwing ArgumentOutOfRangeExceptions|&#x2713;|&#x2713;|
|MiKo_3013|Throw ArgumentOutOfRangeException (not ArgumentException) in 'switch' default clauses|&#x2713;|&#x2713;|
|MiKo_3014|Include reason in InvalidOperationException, NotImplementedException and NotSupportedException messages|&#x2713;|&#x2713;|
|MiKo_3015|Use InvalidOperationExceptions for inappropriate states of parameterless methods|&#x2713;|&#x2713;|
|MiKo_3016|Do not throw ArgumentNullException for property return values|&#x2713;|&#x2713;|
|MiKo_3017|Include original exception when throwing new exceptions|&#x2713;|&#x2713;|
|MiKo_3018|Throw ObjectDisposedExceptions on public methods of disposable types|&#x2713;|\-|
|MiKo_3020|Use 'Task.CompletedTask' instead of 'Task.FromResult'|&#x2713;|&#x2713;|
|MiKo_3021|Do not use 'Task.Run' in implementation|&#x2713;|\-|
|MiKo_3022|Do not return Task&lt;IEnumerable&gt; or Task&lt;IEnumerable&lt;T&gt;&gt;|&#x2713;|\-|
|MiKo_3023|Do not use 'CancellationTokenSource' as parameter|&#x2713;|\-|
|MiKo_3024|Do not use [ref] keyword on reference parameters|&#x2713;|\-|
|MiKo_3025|Do not re-assign method parameters|&#x2713;|\-|
|MiKo_3026|Remove unused parameters|&#x2713;|\-|
|MiKo_3027|Do not reserve parameters for future use|&#x2713;|\-|
|MiKo_3028|Do not assign null to lambda parameters|&#x2713;|\-|
|MiKo_3029|Prevent memory leaks in event registrations|&#x2713;|\-|
|MiKo_3030|Follow Law of Demeter in methods|\-|\-|
|MiKo_3031|Do not implement ICloneable.Clone()|&#x2713;|\-|
|MiKo_3032|Use 'nameof' instead of Cinch for PropertyChangedEventArgs property names|&#x2713;|&#x2713;|
|MiKo_3033|Use 'nameof' for property names in PropertyChangingEventArgs and PropertyChangedEventArgs|&#x2713;|&#x2713;|
|MiKo_3034|Use [CallerMemberName] attribute for PropertyChanged event raisers|&#x2713;|&#x2713;|
|MiKo_3035|Always specify timeouts with 'WaitOne' methods|&#x2713;|\-|
|MiKo_3036|Use 'TimeSpan' factory methods instead of constructors|&#x2713;|&#x2713;|
|MiKo_3037|Do not use magic numbers for timeouts|&#x2713;|\-|
|MiKo_3038|Do not use magic numbers|&#x2713;|\-|
|MiKo_3039|Do not use Linq or yield in properties|&#x2713;|\-|
|MiKo_3040|Use enums instead of booleans when more than 2 values might be needed|&#x2713;|\-|
|MiKo_3041|Do not use delegates in EventArgs|&#x2713;|\-|
|MiKo_3042|Do not implement interfaces in EventArgs|&#x2713;|\-|
|MiKo_3043|Use 'nameof' for WeakEventManager event (de-)registrations|&#x2713;|&#x2713;|
|MiKo_3044|Use 'nameof' to compare property names of PropertyChangingEventArgs and PropertyChangedEventArgs|&#x2713;|&#x2713;|
|MiKo_3045|Use 'nameof' for EventManager event registrations|&#x2713;|&#x2713;|
|MiKo_3046|Use 'nameof' for property names in property raising methods|&#x2713;|&#x2713;|
|MiKo_3047|Use 'nameof' for applied [ContentProperty] attributes|&#x2713;|&#x2713;|
|MiKo_3048|Apply [ValueConversion] attribute to ValueConverters|&#x2713;|\-|
|MiKo_3049|Apply [Description] attribute to enum members|&#x2713;|\-|
|MiKo_3050|Declare DependencyProperty fields as 'public static readonly'|&#x2713;|&#x2713;|
|MiKo_3051|Register DependencyProperty fields properly|&#x2713;|&#x2713;|
|MiKo_3052|Declare DependencyPropertyKey fields as non-public 'static readonly'|&#x2713;|&#x2713;|
|MiKo_3053|Register DependencyPropertyKey fields properly|&#x2713;|\-|
|MiKo_3054|Expose DependencyProperty identifier for read-only DependencyProperties|&#x2713;|&#x2713;|
|MiKo_3055|Implement INotifyPropertyChanged in ViewModels|&#x2713;|\-|
|MiKo_3060|Do not use Debug.Assert or Trace.Assert|&#x2713;|&#x2713;|
|MiKo_3061|Use proper log categories with loggers|&#x2713;|\-|
|MiKo_3062|End exception log messages with a colon|&#x2713;|&#x2713;|
|MiKo_3063|End non-exceptional log messages with a dot|&#x2713;|&#x2713;|
|MiKo_3064|Do not use contraction "n't" in log messages|&#x2713;|&#x2713;|
|MiKo_3065|Do not use interpolated strings with Microsoft Logging calls|&#x2713;|&#x2713;|
|MiKo_3070|Do not return null for IEnumerable|&#x2713;|\-|
|MiKo_3071|Do not return null for Task|&#x2713;|\-|
|MiKo_3072|Do not return 'List&lt;&gt;' or 'Dictionary&lt;&gt;' from non-private methods|&#x2713;|\-|
|MiKo_3073|Fully initialize objects|&#x2713;|\-|
|MiKo_3074|Do not define 'ref' or 'out' parameters on constructors|&#x2713;|\-|
|MiKo_3075|Mark internal and private types as static or sealed unless derivation is needed|&#x2713;|&#x2713;|
|MiKo_3076|Do not initialize static members with static members below or in other type parts|&#x2713;|\-|
|MiKo_3077|Provide default values for Enum-returning properties|&#x2713;|&#x2713;|
|MiKo_3078|Provide default values for enum members|&#x2713;|&#x2713;|
|MiKo_3079|Write HResults in hexadecimal|&#x2713;|&#x2713;|
|MiKo_3080|Use 'switch ... return' instead of 'switch ... break' for variable assignments|&#x2713;|\-|
|MiKo_3081|Use pattern matching instead of logical NOT conditions|&#x2713;|&#x2713;|
|MiKo_3082|Use pattern matching instead of comparing with 'true' or 'false'|&#x2713;|&#x2713;|
|MiKo_3083|Use pattern matching for null checks|&#x2713;|&#x2713;|
|MiKo_3084|Place variables, not constants, on the left side of comparisons|&#x2713;|&#x2713;|
|MiKo_3085|Keep conditional statements short|&#x2713;|&#x2713;|
|MiKo_3086|Do not nest conditional statements|&#x2713;|\-|
|MiKo_3087|Do not use negative complex conditions|&#x2713;|\-|
|MiKo_3088|Use pattern matching for not-null checks|&#x2713;|&#x2713;|
|MiKo_3089|Do not use simple constant property patterns as conditions in 'if' statements|&#x2713;|&#x2713;|
|MiKo_3090|Do not throw exceptions in finally blocks|&#x2713;|\-|
|MiKo_3091|Do not raise events in finally blocks|&#x2713;|\-|
|MiKo_3092|Do not raise events in locks|&#x2713;|\-|
|MiKo_3093|Do not invoke delegates inside locks|&#x2713;|\-|
|MiKo_3094|Do not invoke methods or properties of parameters inside locks|&#x2713;|\-|
|MiKo_3095|Do not use empty code blocks|&#x2713;|\-|
|MiKo_3096|Use dictionaries instead of large switch statements|&#x2713;|\-|
|MiKo_3097|Do not cast to type and return object|&#x2713;|\-|
|MiKo_3098|Provide meaningful explanations for suppressed messages|&#x2713;|\-|
|MiKo_3099|Do not compare enum values with null|&#x2713;|&#x2713;|
|MiKo_3100|Place test classes in same namespace as types under test|&#x2713;|\-|
|MiKo_3101|Include tests in test classes|&#x2713;|\-|
|MiKo_3102|Do not use conditional statements in test methods|&#x2713;|\-|
|MiKo_3103|Do not use 'Guid.NewGuid()' in test methods|&#x2713;|&#x2713;|
|MiKo_3104|Apply NUnit's [Combinatorial] attribute properly|&#x2713;|&#x2713;|
|MiKo_3105|Use NUnit's fluent Assert approach in test methods|&#x2713;|&#x2713;|
|MiKo_3106|Do not use equality or comparison operators in assertions|&#x2713;|\-|
|MiKo_3107|Use Moq Mock condition matchers only on mocks|&#x2713;|&#x2713;|
|MiKo_3108|Include assertions in test methods|&#x2713;|\-|
|MiKo_3109|Include assertion messages with multiple assertions|&#x2713;|&#x2713;|
|MiKo_3110|Do not use 'Count' or 'Length' in assertions|&#x2713;|&#x2713;|
|MiKo_3111|Use 'Is.Zero' instead of 'Is.EqualTo(0)' in assertions|&#x2713;|&#x2713;|
|MiKo_3112|Use 'Is.Empty' instead of 'Has.Count.Zero' in assertions|&#x2713;|&#x2713;|
|MiKo_3113|Do not use FluentAssertions|&#x2713;|&#x2713;|
|MiKo_3114|Use 'Mock.Of&lt;T&gt;()' instead of 'new Mock&lt;T&gt;().Object'|&#x2713;|&#x2713;|
|MiKo_3115|Include code in test methods|&#x2713;|\-|
|MiKo_3116|Include code in test initialization methods|&#x2713;|\-|
|MiKo_3117|Include code in test cleanup methods|&#x2713;|\-|
|MiKo_3118|Do not use ambiguous Linq calls in test methods|&#x2713;|\-|
|MiKo_3119|Do not return only completed task in test methods|&#x2713;|&#x2713;|
|MiKo_3120|Use direct values instead of 'It.Is&lt;&gt;(...)' condition matcher to verify exact values in Moq mocks|&#x2713;|&#x2713;|
|MiKo_3121|Test concrete implementations instead of interfaces|&#x2713;|\-|
|MiKo_3122|Limit test method parameters to 2 or fewer|&#x2713;|\-|
|MiKo_3123|Do not catch exceptions in test methods|&#x2713;|&#x2713;|
|MiKo_3124|Do not use assertions in finally blocks in test methods|&#x2713;|&#x2713;|
|MiKo_3201|Invert if statements in short methods|&#x2713;|&#x2713;|
|MiKo_3202|Use positive conditions when returning in all paths|&#x2713;|&#x2713;|
|MiKo_3203|Invert if-continue statements when followed by single line|&#x2713;|&#x2713;|
|MiKo_3204|Invert negative if statements when they have an else clause|&#x2713;|&#x2713;|
|MiKo_3210|Make only the longest overloads virtual or abstract|&#x2713;|\-|
|MiKo_3211|Do not use finalizers in public types|&#x2713;|\-|
|MiKo_3212|Follow standard Dispose pattern without adding other Dispose methods|&#x2713;|\-|
|MiKo_3213|Implement parameterless Dispose method using Basic Dispose pattern|&#x2713;|\-|
|MiKo_3214|Remove 'Begin/End' or 'Enter/Exit' scope-defining methods from interfaces|&#x2713;|\-|
|MiKo_3215|Use 'Func&lt;T, bool&gt;' instead of 'Predicate&lt;bool&gt;' for callbacks|&#x2713;|&#x2713;|
|MiKo_3216|Mark static fields with initializers as read-only|&#x2713;|&#x2713;|
|MiKo_3217|Do not use generic types that have other generic types as type arguments|&#x2713;|\-|
|MiKo_3218|Define extension methods in expected places|&#x2713;|\-|
|MiKo_3219|Do not mark public members as 'virtual'|&#x2713;|\-|
|MiKo_3220|Simplify logical '&amp;&amp;' or '&#124;&#124;' conditions using 'true' or 'false'|&#x2713;|&#x2713;|
|MiKo_3221|Use 'HashCode.Combine' in GetHashCode overrides|&#x2713;|&#x2713;|
|MiKo_3222|Simplify string comparisons|&#x2713;|&#x2713;|
|MiKo_3223|Simplify reference comparisons|&#x2713;|&#x2713;|
|MiKo_3224|Simplify value comparisons|&#x2713;|&#x2713;|
|MiKo_3225|Simplify redundant comparisons|&#x2713;|&#x2713;|
|MiKo_3226|Make read-only fields with initializers const|&#x2713;|&#x2713;|
|MiKo_3227|Use pattern matching for equality checks|&#x2713;|&#x2713;|
|MiKo_3228|Use pattern matching for inequality checks|&#x2713;|&#x2713;|
|MiKo_3229|Use 'KeyValuePair.Create' instead of constructors|&#x2713;|&#x2713;|
|MiKo_3230|Do not use 'Guid' as type for identifiers|&#x2713;|\-|
|MiKo_3231|Use pattern matching for ordinal string comparison equality checks|&#x2713;|&#x2713;|
|MiKo_3301|Use lambda expression bodies instead of parenthesized lambda expression blocks for single statements|&#x2713;|&#x2713;|
|MiKo_3302|Use simple lambda expression bodies instead of parenthesized lambda expression bodies for single parameters|&#x2713;|&#x2713;|
|MiKo_3401|Keep namespace hierarchies from becoming too deep|&#x2713;|\-|
|MiKo_3501|Do not suppress nullable warnings on Null-conditional operators|&#x2713;|&#x2713;|
|MiKo_3502|Do not suppress nullable warnings on Linq calls|&#x2713;|&#x2713;|
|MiKo_3503|Do not assign variables in try-catch blocks that are returned directly outside|&#x2713;|&#x2713;|

### Ordering
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_4001|Order methods with same name based on their parameter count|&#x2713;|&#x2713;|
|MiKo_4002|Place methods with same name and accessibility side-by-side|&#x2713;|&#x2713;|
|MiKo_4003|Place Dispose methods directly after constructors and finalizers|&#x2713;|&#x2713;|
|MiKo_4004|Place Dispose methods before all other methods of the same accessibility|&#x2713;|&#x2713;|
|MiKo_4005|Place the interface that gives a type its name directly after the type's declaration|&#x2713;|&#x2713;|
|MiKo_4007|Place operators before methods|&#x2713;|&#x2713;|
|MiKo_4008|Place GetHashCode methods directly after Equals methods|&#x2713;|&#x2713;|
|MiKo_4101|Place test initialization methods directly after One-Time methods|&#x2713;|&#x2713;|
|MiKo_4102|Place test cleanup methods after test initialization methods and before test methods|&#x2713;|&#x2713;|
|MiKo_4103|Place One-Time test initialization methods before all other methods|&#x2713;|&#x2713;|
|MiKo_4104|Place One-Time test cleanup methods directly after One-Time test initialization methods|&#x2713;|&#x2713;|
|MiKo_4105|Place object under test fields before all other fields|&#x2713;|&#x2713;|

### Performance
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_5001|Invoke 'Debug' and 'DebugFormat' methods only after checking 'IsDebugEnabled'|&#x2713;|&#x2713;|
|MiKo_5002|Use 'xxxFormat' methods only with multiple arguments|&#x2713;|&#x2713;|
|MiKo_5003|Use appropriate Log methods for exceptions|&#x2713;|\-|
|MiKo_5010|Do not use 'object.Equals()' on value types|&#x2713;|&#x2713;|
|MiKo_5011|Do not concatenate strings with += operator|&#x2713;|\-|
|MiKo_5012|Do not use 'yield return' for recursively defined structures|&#x2713;|\-|
|MiKo_5013|Do not create empty arrays|&#x2713;|&#x2713;|
|MiKo_5014|Do not create empty lists when return value is read-only|&#x2713;|&#x2713;|
|MiKo_5015|Do not intern string literals|&#x2713;|&#x2713;|
|MiKo_5016|Use HashSet for lookups in 'List.RemoveAll'|&#x2713;|\-|
|MiKo_5017|Make fields or variables assigned with string literals constant|&#x2713;|&#x2713;|
|MiKo_5018|Perform value comparisons before reference comparisons|&#x2713;|&#x2713;|
|MiKo_5019|Add [in] modifier to read-only struct parameters|&#x2713;|&#x2713;|

### Spacing
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_6001|Surround log statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6002|Surround assertion statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6003|Precede local variable statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6004|Precede variable assignment statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6005|Precede return statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6006|Surround awaited statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6007|Surround test statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6008|Precede using directives with blank lines|&#x2713;|&#x2713;|
|MiKo_6009|Surround try statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6010|Surround if statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6011|Surround lock statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6012|Surround foreach loops with blank lines|&#x2713;|&#x2713;|
|MiKo_6013|Surround for loops with blank lines|&#x2713;|&#x2713;|
|MiKo_6014|Surround while loops with blank lines|&#x2713;|&#x2713;|
|MiKo_6015|Surround do/while loops with blank lines|&#x2713;|&#x2713;|
|MiKo_6016|Surround using statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6017|Surround switch statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6018|Surround break statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6019|Surround continue statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6020|Surround throw statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6021|Surround ArgumentNullException.ThrowIfNull statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6022|Surround ArgumentException.ThrowIfNullOrEmpty statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6023|Surround ArgumentOutOfRangeException.ThrowIf statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6024|Surround ObjectDisposedException.ThrowIf statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6030|Place open braces of initializers directly below the corresponding type definition|&#x2713;|&#x2713;|
|MiKo_6031|Place question and colon tokens of ternary operators directly below the corresponding condition|&#x2713;|&#x2713;|
|MiKo_6032|Position multi-line parameters outdented at end of method|&#x2713;|&#x2713;|
|MiKo_6033|Place braces of blocks below case sections directly below the corresponding case keyword|&#x2713;|&#x2713;|
|MiKo_6034|Place dots on same line(s) as invoked members|&#x2713;|&#x2713;|
|MiKo_6035|Place open parenthesis on same line(s) as invoked methods|&#x2713;|&#x2713;|
|MiKo_6036|Place lambda blocks directly below the corresponding arrow(s)|&#x2713;|&#x2713;|
|MiKo_6037|Place single arguments on same line(s) as invoked methods|&#x2713;|&#x2713;|
|MiKo_6038|Place casts on same line(s)|&#x2713;|&#x2713;|
|MiKo_6039|Place return values on same line(s) as return keywords|&#x2713;|&#x2713;|
|MiKo_6040|Align consecutive multi-line invocations by their dots|&#x2713;|&#x2713;|
|MiKo_6041|Place assignments on same line(s)|&#x2713;|&#x2713;|
|MiKo_6042|Place 'new' keywords on same line(s) as the types|&#x2713;|&#x2713;|
|MiKo_6043|Place expression bodies of lambdas on same line as lambda when fitting|&#x2713;|&#x2713;|
|MiKo_6044|Place operators such as '&amp;&amp;' or '&#124;&#124;' on same line(s) as their (right) operands|&#x2713;|&#x2713;|
|MiKo_6045|Place comparisons using operators such as '==' or '!=' on same line(s)|&#x2713;|&#x2713;|
|MiKo_6046|Place calculations using operators such as '+' or '%' on same line(s)|&#x2713;|&#x2713;|
|MiKo_6047|Place braces of switch expressions directly below the corresponding switch keyword|&#x2713;|&#x2713;|
|MiKo_6048|Place logical conditions on a single line|&#x2713;|&#x2713;|
|MiKo_6049|Surround event (un-)registrations with blank lines|&#x2713;|&#x2713;|
|MiKo_6050|Position multi-line arguments outdented at end of method call|&#x2713;|&#x2713;|
|MiKo_6051|Place colon of constructor call on same line as constructor call|&#x2713;|&#x2713;|
|MiKo_6052|Place colon of list of base types on same line as first base type|&#x2713;|&#x2713;|
|MiKo_6053|Place single-line arguments on single line|&#x2713;|&#x2713;|
|MiKo_6054|Place lambda arrows on same line as the parameter(s) of the lambda|&#x2713;|&#x2713;|
|MiKo_6055|Surround assignment statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6056|Place brackets of collection expressions at the same position as collection initializer braces|&#x2713;|&#x2713;|
|MiKo_6057|Align type parameter constraint clauses vertically|&#x2713;|&#x2713;|
|MiKo_6058|Indent type parameter constraint clauses below parameter list|&#x2713;|&#x2713;|
|MiKo_6059|Position multi-line conditions outdented below associated calls|&#x2713;|&#x2713;|
|MiKo_6060|Place switch case labels on same line|&#x2713;|&#x2713;|
|MiKo_6061|Place switch expression arms on same line|&#x2713;|&#x2713;|
|MiKo_6062|Place expressions within complex initializer expressions beside open brace|&#x2713;|&#x2713;|
|MiKo_6063|Place invocations on same line|&#x2713;|&#x2713;|
|MiKo_6064|Place identifier invocations on same line|&#x2713;|&#x2713;|
|MiKo_6065|Indent rather than outdent consecutive invocations spanning multiple lines|&#x2713;|&#x2713;|
|MiKo_6066|Indent rather than outdent collection expression elements|&#x2713;|&#x2713;|
|MiKo_6067|Place ternary operators on same lines as their respective expressions|&#x2713;|&#x2713;|
|MiKo_6070|Surround Console statements with blank lines|&#x2713;|&#x2713;|
|MiKo_6071|Surround local using statements with blank lines|&#x2713;|&#x2713;|
