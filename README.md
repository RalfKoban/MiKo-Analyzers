# MiKo-Analyzers

Provides analyzers that are based on the .NET Compiler Platform (Roslyn) and can be used inside Visual Studio 2019 (v16.11) or 2022 (v17.14).

How to install an Roslyn analyzer is described [here](https://learn.microsoft.com/en-us/visualstudio/code-quality/install-roslyn-analyzers?view=vs-2022).

Screenshots on how to use such analyzers can be found [here](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022).

## Build / Project status

[![Maintenance](https://img.shields.io/maintenance/yes/2026.svg)](https://github.com/RalfKoban/MiKo-Analyzers)
[![Build status](https://ci.appveyor.com/api/projects/status/qanrqn7r4q9frr9m/branch/master?svg=true)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/branch/master)
[![codecov](https://codecov.io/gh/RalfKoban/MiKo-Analyzers/branch/master/graph/badge.svg)](https://codecov.io/gh/RalfKoban/MiKo-Analyzers)
[![Coverity Scan Build Status](https://img.shields.io/coverity/scan/18917.svg)](https://scan.coverity.com/projects/ralfkoban-miko-analyzers)

## Available Rules

The following tables lists all the 537 rules that are currently provided by the analyzer.

### Metrics

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_0001](/Documentation/MiKo_0001.md)|Keep methods small|&#x2713;|\-|
|[MiKo_0002](/Documentation/MiKo_0002.md)|Simplify complex methods|&#x2713;|\-|
|[MiKo_0003](/Documentation/MiKo_0003.md)|Keep types small|&#x2713;|\-|
|[MiKo_0004](/Documentation/MiKo_0004.md)|Limit method parameters|&#x2713;|\-|
|[MiKo_0005](/Documentation/MiKo_0005.md)|Keep local functions small|&#x2713;|\-|
|[MiKo_0006](/Documentation/MiKo_0006.md)|Simplify complex local functions|&#x2713;|\-|
|[MiKo_0007](/Documentation/MiKo_0007.md)|Limit local function parameters|&#x2713;|\-|

### Naming

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_1000](/Documentation/MiKo_1000.md)|Suffix 'System.EventArgs' types with 'EventArgs'|&#x2713;|&#x2713;|
|[MiKo_1001](/Documentation/MiKo_1001.md)|Name 'System.EventArgs' parameters 'e'|&#x2713;|&#x2713;|
|[MiKo_1002](/Documentation/MiKo_1002.md)|Follow .NET Framework Design Guidelines for event handler parameter names|&#x2713;|&#x2713;|
|[MiKo_1003](/Documentation/MiKo_1003.md)|Follow .NET Framework Design Guidelines for event handling method names|&#x2713;|&#x2713;|
|[MiKo_1004](/Documentation/MiKo_1004.md)|Remove term 'Event' from event names|&#x2713;|&#x2713;|
|[MiKo_1005](/Documentation/MiKo_1005.md)|Name 'System.EventArgs' variables properly|&#x2713;|&#x2713;|
|[MiKo_1006](/Documentation/MiKo_1006.md)|Use 'EventHandler&lt;T&gt;' with 'EventArgs' named after the event|&#x2713;|\-|
|[MiKo_1007](/Documentation/MiKo_1007.md)|Place events and their 'EventArgs' types in the same namespace|&#x2713;|\-|
|[MiKo_1008](/Documentation/MiKo_1008.md)|Follow .NET Framework Design Guidelines for DependencyProperty event handler parameter names|&#x2713;|&#x2713;|
|[MiKo_1009](/Documentation/MiKo_1009.md)|Name 'System.EventHandler' variables properly|&#x2713;|&#x2713;|
|[MiKo_1010](/Documentation/MiKo_1010.md)|Do not include 'CanExecute' or 'Execute' in method names|&#x2713;|&#x2713;|
|[MiKo_1011](/Documentation/MiKo_1011.md)|Do not include 'Do' in method names|&#x2713;|&#x2713;|
|[MiKo_1012](/Documentation/MiKo_1012.md)|Use 'Raise' instead of 'Fire' in method names|&#x2713;|&#x2713;|
|[MiKo_1013](/Documentation/MiKo_1013.md)|Do not name methods 'Notify' or 'OnNotify'|&#x2713;|&#x2713;|
|[MiKo_1014](/Documentation/MiKo_1014.md)|Do not use ambiguous 'Check' in method names|&#x2713;|&#x2713;|
|[MiKo_1015](/Documentation/MiKo_1015.md)|Use 'Initialize' instead of 'Init' in method names|&#x2713;|&#x2713;|
|[MiKo_1016](/Documentation/MiKo_1016.md)|Name factory methods 'Create'|&#x2713;|&#x2713;|
|[MiKo_1017](/Documentation/MiKo_1017.md)|Do not prefix methods with 'Get' or 'Set' when followed by 'Is', 'Can' or 'Has'|&#x2713;|&#x2713;|
|[MiKo_1018](/Documentation/MiKo_1018.md)|Do not suffix methods with noun of a verb|&#x2713;|&#x2713;|
|[MiKo_1019](/Documentation/MiKo_1019.md)|Name 'Clear' and 'Remove' methods based on their number of parameters|&#x2713;|&#x2713;|
|[MiKo_1020](/Documentation/MiKo_1020.md)|Limit type name length|\-|\-|
|[MiKo_1021](/Documentation/MiKo_1021.md)|Limit method name length|\-|\-|
|[MiKo_1022](/Documentation/MiKo_1022.md)|Limit parameter name length|\-|\-|
|[MiKo_1023](/Documentation/MiKo_1023.md)|Limit field name length|\-|\-|
|[MiKo_1024](/Documentation/MiKo_1024.md)|Limit property name length|\-|\-|
|[MiKo_1025](/Documentation/MiKo_1025.md)|Limit event name length|\-|\-|
|[MiKo_1026](/Documentation/MiKo_1026.md)|Limit variable name length|\-|\-|
|[MiKo_1027](/Documentation/MiKo_1027.md)|Limit loop variable name length|\-|\-|
|[MiKo_1028](/Documentation/MiKo_1028.md)|Limit local function name length|\-|\-|
|[MiKo_1030](/Documentation/MiKo_1030.md)|Do not mark base types with 'Abstract' or 'Base'|&#x2713;|&#x2713;|
|[MiKo_1031](/Documentation/MiKo_1031.md)|Do not suffix entity types with 'Model'|&#x2713;|&#x2713;|
|[MiKo_1032](/Documentation/MiKo_1032.md)|Do not use 'Model' as marker in methods dealing with entities|&#x2713;|&#x2713;|
|[MiKo_1033](/Documentation/MiKo_1033.md)|Do not suffix entity parameters with 'Model'|&#x2713;|&#x2713;|
|[MiKo_1034](/Documentation/MiKo_1034.md)|Do not suffix entity fields with 'Model'|&#x2713;|&#x2713;|
|[MiKo_1035](/Documentation/MiKo_1035.md)|Do not use 'Model' marker in properties dealing with entities|&#x2713;|&#x2713;|
|[MiKo_1036](/Documentation/MiKo_1036.md)|Do not use 'Model' marker in events dealing with entities|&#x2713;|&#x2713;|
|[MiKo_1037](/Documentation/MiKo_1037.md)|Do not suffix types with 'Type', 'Interface', 'Class', 'Struct', 'Record' or 'Enum'|&#x2713;|&#x2713;|
|[MiKo_1038](/Documentation/MiKo_1038.md)|Use consistent suffix for extension method container classes|&#x2713;|&#x2713;|
|[MiKo_1039](/Documentation/MiKo_1039.md)|Use default name for 'this' parameter of extension methods|&#x2713;|&#x2713;|
|[MiKo_1040](/Documentation/MiKo_1040.md)|Do not suffix parameters with implementation details|&#x2713;|&#x2713;|
|[MiKo_1041](/Documentation/MiKo_1041.md)|Do not suffix fields with implementation details|&#x2713;|&#x2713;|
|[MiKo_1042](/Documentation/MiKo_1042.md)|Use specific name for 'CancellationToken' parameters|&#x2713;|&#x2713;|
|[MiKo_1043](/Documentation/MiKo_1043.md)|Use specific name for 'CancellationToken' variables|&#x2713;|&#x2713;|
|[MiKo_1044](/Documentation/MiKo_1044.md)|Suffix commands with 'Command'|&#x2713;|&#x2713;|
|[MiKo_1045](/Documentation/MiKo_1045.md)|Do not suffix command-invoked methods with 'Command'|&#x2713;|&#x2713;|
|[MiKo_1046](/Documentation/MiKo_1046.md)|Follow Task-based Asynchronous Pattern (TAP) for asynchronous methods|&#x2713;|&#x2713;|
|[MiKo_1047](/Documentation/MiKo_1047.md)|Do not falsely indicate asynchronous behavior for methods not following Task-based Asynchronous Pattern (TAP)|&#x2713;|&#x2713;|
|[MiKo_1048](/Documentation/MiKo_1048.md)|End value converter classes with a specific suffix|&#x2713;|&#x2713;|
|[MiKo_1049](/Documentation/MiKo_1049.md)|Do not use requirement terms such as 'Shall', 'Should', 'Must' or 'Need' for names|&#x2713;|&#x2713;|
|[MiKo_1050](/Documentation/MiKo_1050.md)|Use descriptive names for return values|&#x2713;|&#x2713;|
|[MiKo_1051](/Documentation/MiKo_1051.md)|Do not suffix parameters with delegate types|&#x2713;|&#x2713;|
|[MiKo_1052](/Documentation/MiKo_1052.md)|Do not suffix variables with delegate types|&#x2713;|&#x2713;|
|[MiKo_1053](/Documentation/MiKo_1053.md)|Do not suffix fields with delegate types|&#x2713;|&#x2713;|
|[MiKo_1054](/Documentation/MiKo_1054.md)|Do not name types 'Helper' or 'Utility'|&#x2713;|&#x2713;|
|[MiKo_1055](/Documentation/MiKo_1055.md)|Suffix dependency properties with 'Property' (as in the .NET Framework)|&#x2713;|&#x2713;|
|[MiKo_1056](/Documentation/MiKo_1056.md)|Prefix dependency properties with property names (as in the .NET Framework)|&#x2713;|&#x2713;|
|[MiKo_1057](/Documentation/MiKo_1057.md)|Suffix dependency property keys with 'Key' (as in the .NET Framework)|&#x2713;|&#x2713;|
|[MiKo_1058](/Documentation/MiKo_1058.md)|Prefix dependency property keys with property names (as in the .NET Framework)|&#x2713;|&#x2713;|
|[MiKo_1059](/Documentation/MiKo_1059.md)|Do not name types 'Impl' or 'Implementation'|&#x2713;|&#x2713;|
|[MiKo_1060](/Documentation/MiKo_1060.md)|Use '&lt;Entity&gt;NotFound' instead of 'Get&lt;Entity&gt;Failed' or '&lt;Entity&gt;Missing'|&#x2713;|&#x2713;|
|[MiKo_1061](/Documentation/MiKo_1061.md)|Use specific name for 'Try' method's [out] parameters|&#x2713;|&#x2713;|
|[MiKo_1062](/Documentation/MiKo_1062.md)|Keep 'Can/Has/Contains' methods, properties or fields to a few words|&#x2713;|\-|
|[MiKo_1063](/Documentation/MiKo_1063.md)|Do not use abbreviations in names|&#x2713;|&#x2713;|
|[MiKo_1064](/Documentation/MiKo_1064.md)|Make parameter names reflect their meaning, not their type|&#x2713;|\-|
|[MiKo_1065](/Documentation/MiKo_1065.md)|Follow .NET Framework Design Guidelines for operator overload parameter names|&#x2713;|&#x2713;|
|[MiKo_1066](/Documentation/MiKo_1066.md)|Name constructor parameters after the property they're assigned to|&#x2713;|&#x2713;|
|[MiKo_1067](/Documentation/MiKo_1067.md)|Do not include 'Perform' in method names|&#x2713;|&#x2713;|
|[MiKo_1068](/Documentation/MiKo_1068.md)|Name workflow methods 'CanRun' or 'Run'|&#x2713;|\-|
|[MiKo_1069](/Documentation/MiKo_1069.md)|Make property names reflect their meaning, not their type|&#x2713;|\-|
|[MiKo_1070](/Documentation/MiKo_1070.md)|Use plural names for local collection variables|&#x2713;|&#x2713;|
|[MiKo_1071](/Documentation/MiKo_1071.md)|Name local boolean variables as statements, not questions|&#x2713;|\-|
|[MiKo_1072](/Documentation/MiKo_1072.md)|Name boolean properties or methods as statements, not questions|&#x2713;|\-|
|[MiKo_1073](/Documentation/MiKo_1073.md)|Name boolean fields as statements, not questions|&#x2713;|\-|
|[MiKo_1074](/Documentation/MiKo_1074.md)|Suffix lock objects with 'Lock'|&#x2713;|\-|
|[MiKo_1075](/Documentation/MiKo_1075.md)|Do not suffix non-'System.EventArgs' types with 'EventArgs'|&#x2713;|&#x2713;|
|[MiKo_1076](/Documentation/MiKo_1076.md)|Suffix Prism event types with 'Event'|&#x2713;|&#x2713;|
|[MiKo_1077](/Documentation/MiKo_1077.md)|Do not suffix enum members with 'Enum'|&#x2713;|&#x2713;|
|[MiKo_1078](/Documentation/MiKo_1078.md)|Start builder method names with 'Build'|&#x2713;|&#x2713;|
|[MiKo_1079](/Documentation/MiKo_1079.md)|Do not suffix repositories with 'Repository'|&#x2713;|&#x2713;|
|[MiKo_1080](/Documentation/MiKo_1080.md)|Use numbers instead of their spellings in names|&#x2713;|\-|
|[MiKo_1081](/Documentation/MiKo_1081.md)|Do not suffix methods with a number|&#x2713;|&#x2713;|
|[MiKo_1082](/Documentation/MiKo_1082.md)|Do not suffix properties with a number if their types have number suffixes|&#x2713;|&#x2713;|
|[MiKo_1083](/Documentation/MiKo_1083.md)|Do not suffix fields with a number if their types have number suffixes|&#x2713;|&#x2713;|
|[MiKo_1084](/Documentation/MiKo_1084.md)|Do not suffix variables with a number if their types have number suffixes|&#x2713;|&#x2713;|
|[MiKo_1085](/Documentation/MiKo_1085.md)|Do not suffix parameters with a number|&#x2713;|&#x2713;|
|[MiKo_1086](/Documentation/MiKo_1086.md)|Do not use numbers as slang in method names|&#x2713;|\-|
|[MiKo_1087](/Documentation/MiKo_1087.md)|Name constructor parameters after their base class counterparts|&#x2713;|&#x2713;|
|[MiKo_1088](/Documentation/MiKo_1088.md)|Name singleton instances 'Instance'|&#x2713;|\-|
|[MiKo_1089](/Documentation/MiKo_1089.md)|Do not prefix methods with 'Get'|&#x2713;|&#x2713;|
|[MiKo_1090](/Documentation/MiKo_1090.md)|Do not suffix parameters with specific types|&#x2713;|&#x2713;|
|[MiKo_1091](/Documentation/MiKo_1091.md)|Do not suffix variables with specific types|&#x2713;|&#x2713;|
|[MiKo_1092](/Documentation/MiKo_1092.md)|Do not suffix 'Ability' types with redundant information|&#x2713;|&#x2713;|
|[MiKo_1093](/Documentation/MiKo_1093.md)|Do not use the suffix 'Object' or 'Struct'|&#x2713;|&#x2713;|
|[MiKo_1094](/Documentation/MiKo_1094.md)|Do not suffix types with passive namespace names|&#x2713;|\-|
|[MiKo_1095](/Documentation/MiKo_1095.md)|Do not use 'Delete' and 'Remove' both in names and documentation|&#x2713;|\-|
|[MiKo_1096](/Documentation/MiKo_1096.md)|Use 'Failed' instead of 'NotSuccessful' in names|&#x2713;|\-|
|[MiKo_1097](/Documentation/MiKo_1097.md)|Do not use field naming schemes for parameter names|&#x2713;|&#x2713;|
|[MiKo_1098](/Documentation/MiKo_1098.md)|Reflect implemented business interface(s) in type names|&#x2713;|\-|
|[MiKo_1099](/Documentation/MiKo_1099.md)|Use identical names for matching parameters on method overloads|&#x2713;|&#x2713;|
|[MiKo_1100](/Documentation/MiKo_1100.md)|Start test class names with the name of the type under test|&#x2713;|\-|
|[MiKo_1101](/Documentation/MiKo_1101.md)|End test class names with 'Tests'|&#x2713;|&#x2713;|
|[MiKo_1102](/Documentation/MiKo_1102.md)|Do not include 'Test' in test method names|&#x2713;|&#x2713;|
|[MiKo_1103](/Documentation/MiKo_1103.md)|Name test initialization methods 'PrepareTest'|&#x2713;|&#x2713;|
|[MiKo_1104](/Documentation/MiKo_1104.md)|Name test cleanup methods 'CleanupTest'|&#x2713;|&#x2713;|
|[MiKo_1105](/Documentation/MiKo_1105.md)|Name one-time test initialization methods 'PrepareTestEnvironment'|&#x2713;|&#x2713;|
|[MiKo_1106](/Documentation/MiKo_1106.md)|Name one-time test cleanup methods 'CleanupTestEnvironment'|&#x2713;|&#x2713;|
|[MiKo_1107](/Documentation/MiKo_1107.md)|Do not use Pascal-casing for test methods|&#x2713;|&#x2713;|
|[MiKo_1108](/Documentation/MiKo_1108.md)|Do not name variables, parameters, fields and properties 'Mock', 'Stub', 'Fake' or 'Shim'|&#x2713;|&#x2713;|
|[MiKo_1109](/Documentation/MiKo_1109.md)|Prefix testable types with 'Testable' instead of using the 'Ut' suffix|&#x2713;|&#x2713;|
|[MiKo_1110](/Documentation/MiKo_1110.md)|Suffix test methods with parameters with underscore|&#x2713;|&#x2713;|
|[MiKo_1111](/Documentation/MiKo_1111.md)|Do not suffix parameterless test methods with underscore|&#x2713;|&#x2713;|
|[MiKo_1112](/Documentation/MiKo_1112.md)|Do not name test data 'arbitrary'|&#x2713;|&#x2713;|
|[MiKo_1113](/Documentation/MiKo_1113.md)|Do not use BDD style naming for test methods|&#x2713;|\-|
|[MiKo_1114](/Documentation/MiKo_1114.md)|Do not name test methods 'HappyPath' or 'BadPath'|&#x2713;|\-|
|[MiKo_1115](/Documentation/MiKo_1115.md)|Name test methods in a fluent way|&#x2713;|&#x2713;|
|[MiKo_1116](/Documentation/MiKo_1116.md)|Use present tense for test method names|&#x2713;|&#x2713;|
|[MiKo_1117](/Documentation/MiKo_1117.md)|Make test method names more precise|&#x2713;|\-|
|[MiKo_1118](/Documentation/MiKo_1118.md)|Do not end test method names with 'Async'|&#x2713;|&#x2713;|
|[MiKo_1119](/Documentation/MiKo_1119.md)|Do not include 'when_present' in test method names|&#x2713;|\-|
|[MiKo_1200](/Documentation/MiKo_1200.md)|Name catch block exceptions consistently|&#x2713;|&#x2713;|
|[MiKo_1201](/Documentation/MiKo_1201.md)|Name exception parameters consistently|&#x2713;|&#x2713;|
|[MiKo_1300](/Documentation/MiKo_1300.md)|Name unimportant lambda parameters '_'|&#x2713;|&#x2713;|
|[MiKo_1400](/Documentation/MiKo_1400.md)|Use plural for namespace names|&#x2713;|\-|
|[MiKo_1401](/Documentation/MiKo_1401.md)|Do not include technical language names in namespaces|&#x2713;|\-|
|[MiKo_1402](/Documentation/MiKo_1402.md)|Do not name namespaces after WPF-specific design patterns|&#x2713;|\-|
|[MiKo_1403](/Documentation/MiKo_1403.md)|Do not name namespaces after any of their parent namespaces|&#x2713;|\-|
|[MiKo_1404](/Documentation/MiKo_1404.md)|Do not use unspecific names for namespaces|&#x2713;|\-|
|[MiKo_1405](/Documentation/MiKo_1405.md)|Do not include 'Lib' in namespaces|&#x2713;|\-|
|[MiKo_1406](/Documentation/MiKo_1406.md)|Place value converters in 'Converters' namespace|&#x2713;|\-|
|[MiKo_1407](/Documentation/MiKo_1407.md)|Do not include 'Test' in test namespaces|&#x2713;|\-|
|[MiKo_1408](/Documentation/MiKo_1408.md)|Place extension methods in same namespace as the extended types|&#x2713;|\-|
|[MiKo_1409](/Documentation/MiKo_1409.md)|Do not prefix or suffix namespaces with underscores|&#x2713;|\-|
|[MiKo_1501](/Documentation/MiKo_1501.md)|Do not use 'Filter' in names|&#x2713;|\-|
|[MiKo_1502](/Documentation/MiKo_1502.md)|Do not use 'Process' in names|&#x2713;|\-|
|[MiKo_1503](/Documentation/MiKo_1503.md)|Do not suffix methods with 'Counter'|&#x2713;|&#x2713;|
|[MiKo_1504](/Documentation/MiKo_1504.md)|Do not suffix properties with 'Counter'|&#x2713;|&#x2713;|
|[MiKo_1505](/Documentation/MiKo_1505.md)|Do not suffix fields with 'Counter'|&#x2713;|&#x2713;|
|[MiKo_1506](/Documentation/MiKo_1506.md)|Do not suffix local variables with 'Counter'|&#x2713;|&#x2713;|
|[MiKo_1507](/Documentation/MiKo_1507.md)|Do not suffix parameters with 'Counter'|&#x2713;|&#x2713;|
|[MiKo_1508](/Documentation/MiKo_1508.md)|Do not suffix local variables with pattern names|&#x2713;|&#x2713;|
|[MiKo_1509](/Documentation/MiKo_1509.md)|Do not suffix parameters with pattern names|&#x2713;|&#x2713;|
|[MiKo_1510](/Documentation/MiKo_1510.md)|Do not suffix fields with pattern names|&#x2713;|&#x2713;|
|[MiKo_1511](/Documentation/MiKo_1511.md)|Do not prefix or suffix local variables with 'proxy'|&#x2713;|&#x2713;|
|[MiKo_1512](/Documentation/MiKo_1512.md)|Do not prefix or suffix parameters with 'proxy'|&#x2713;|&#x2713;|
|[MiKo_1513](/Documentation/MiKo_1513.md)|Do not suffix types with 'Advanced', 'Complex', 'Enhanced', 'Extended', 'Simple' or 'Simplified'|&#x2713;|&#x2713;|
|[MiKo_1514](/Documentation/MiKo_1514.md)|Do not suffix types with 'Info'|&#x2713;|\-|
|[MiKo_1515](/Documentation/MiKo_1515.md)|Express binary conditions clearly in boolean property names|&#x2713;|&#x2713;|
|[MiKo_1516](/Documentation/MiKo_1516.md)|Express binary conditions clearly in boolean parameter names|&#x2713;|&#x2713;|
|[MiKo_1517](/Documentation/MiKo_1517.md)|Express binary conditions clearly in boolean field names|&#x2713;|&#x2713;|
|[MiKo_1518](/Documentation/MiKo_1518.md)|Do not prefix or suffix local variables with 'reference'|&#x2713;|&#x2713;|
|[MiKo_1519](/Documentation/MiKo_1519.md)|Do not prefix or suffix parameters with 'reference'|&#x2713;|&#x2713;|
|[MiKo_1520](/Documentation/MiKo_1520.md)|Do not prefix or suffix local variables with 'toCopy'|&#x2713;|&#x2713;|
|[MiKo_1521](/Documentation/MiKo_1521.md)|Do not prefix or suffix parameters with 'toCopy'|&#x2713;|&#x2713;|
|[MiKo_1522](/Documentation/MiKo_1522.md)|Do not start void methods with 'Get'|&#x2713;|\-|
|[MiKo_1523](/Documentation/MiKo_1523.md)|Do not name methods 'Helper'|&#x2713;|\-|

### Documentation

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_2000](/Documentation/MiKo_2000.md)|Write valid XML documentation|&#x2713;|&#x2713;|
|[MiKo_2001](/Documentation/MiKo_2001.md)|Document events properly|&#x2713;|&#x2713;|
|[MiKo_2002](/Documentation/MiKo_2002.md)|Document EventArgs properly|&#x2713;|&#x2713;|
|[MiKo_2003](/Documentation/MiKo_2003.md)|Start event handler documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2004](/Documentation/MiKo_2004.md)|Follow .NET Framework Design Guidelines for event handler parameter names in documentation|&#x2713;|&#x2713;|
|[MiKo_2005](/Documentation/MiKo_2005.md)|Document textual references to EventArgs properly|&#x2713;|\-|
|[MiKo_2006](/Documentation/MiKo_2006.md)|Document routed events as done by the .NET Framework|&#x2713;|&#x2713;|
|[MiKo_2010](/Documentation/MiKo_2010.md)|Document sealed classes as being sealed|&#x2713;|&#x2713;|
|[MiKo_2011](/Documentation/MiKo_2011.md)|Do not falsely document unsealed classes as sealed|&#x2713;|&#x2713;|
|[MiKo_2012](/Documentation/MiKo_2012.md)|Describe the responsibility in &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2013](/Documentation/MiKo_2013.md)|Start Enum &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2014](/Documentation/MiKo_2014.md)|Document Dispose methods as done by the .NET Framework|&#x2713;|&#x2713;|
|[MiKo_2015](/Documentation/MiKo_2015.md)|Use 'raise' or 'throw' instead of 'fire' in documentation|&#x2713;|&#x2713;|
|[MiKo_2016](/Documentation/MiKo_2016.md)|Start documentation for asynchronous methods with specific phrase|&#x2713;|&#x2713;|
|[MiKo_2017](/Documentation/MiKo_2017.md)|Document dependency properties as done by the .NET Framework|&#x2713;|&#x2713;|
|[MiKo_2018](/Documentation/MiKo_2018.md)|Do not use ambiguous terms 'Check' or 'Test' in documentation|&#x2713;|&#x2713;|
|[MiKo_2019](/Documentation/MiKo_2019.md)|Start &lt;summary&gt; documentation with third person singular verb (e.g., "Provides")|&#x2713;|&#x2713;|
|[MiKo_2020](/Documentation/MiKo_2020.md)|Use &lt;inheritdoc /&gt; marker for inherited documentation|&#x2713;|&#x2713;|
|[MiKo_2021](/Documentation/MiKo_2021.md)|Start parameter documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2022](/Documentation/MiKo_2022.md)|Start [out] parameter documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2023](/Documentation/MiKo_2023.md)|Start Boolean parameter documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2024](/Documentation/MiKo_2024.md)|Start Enum parameter documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2025](/Documentation/MiKo_2025.md)|Start 'CancellationToken' parameter documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2026](/Documentation/MiKo_2026.md)|Do not document used parameters as unused|&#x2713;|\-|
|[MiKo_2027](/Documentation/MiKo_2027.md)|Document serialization constructor parameters with specific phrase|&#x2713;|&#x2713;|
|[MiKo_2028](/Documentation/MiKo_2028.md)|Provide more information than just parameter name in documentation|&#x2713;|\-|
|[MiKo_2029](/Documentation/MiKo_2029.md)|Do not use self-referencing 'cref' in &lt;inheritdoc&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2030](/Documentation/MiKo_2030.md)|Start return value documentation with default phrase|&#x2713;|\-|
|[MiKo_2031](/Documentation/MiKo_2031.md)|Use specific (starting) phrase for Task return value documentation|&#x2713;|&#x2713;|
|[MiKo_2032](/Documentation/MiKo_2032.md)|Use specific phrase for Boolean return value documentation|&#x2713;|&#x2713;|
|[MiKo_2033](/Documentation/MiKo_2033.md)|Start String return value documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2034](/Documentation/MiKo_2034.md)|Start Enum return value documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2035](/Documentation/MiKo_2035.md)|Start collection return value documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2036](/Documentation/MiKo_2036.md)|Describe default values in Boolean or Enum property documentation|&#x2713;|&#x2713;|
|[MiKo_2037](/Documentation/MiKo_2037.md)|Start command property &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2038](/Documentation/MiKo_2038.md)|Start command &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2039](/Documentation/MiKo_2039.md)|Start extension method class &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2040](/Documentation/MiKo_2040.md)|Use &lt;see langword="..."/&gt; instead of &lt;c&gt;...&lt;/c&gt;|&#x2713;|&#x2713;|
|[MiKo_2041](/Documentation/MiKo_2041.md)|Do not include other documentation tags in &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2042](/Documentation/MiKo_2042.md)|Use '&lt;para&gt;' XML tags instead of '&lt;p&gt;' HTML tags in documentation|&#x2713;|&#x2713;|
|[MiKo_2043](/Documentation/MiKo_2043.md)|Start custom delegate &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2044](/Documentation/MiKo_2044.md)|Reference method parameters correctly in documentation|&#x2713;|&#x2713;|
|[MiKo_2045](/Documentation/MiKo_2045.md)|Do not reference parameters in &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2046](/Documentation/MiKo_2046.md)|Reference type parameters correctly in documentation|&#x2713;|&#x2713;|
|[MiKo_2047](/Documentation/MiKo_2047.md)|Start Attribute &lt;summary&gt; documentation with default phrase|&#x2713;|\-|
|[MiKo_2048](/Documentation/MiKo_2048.md)|Start value converter &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2049](/Documentation/MiKo_2049.md)|Use explicit wording instead of 'will be' in documentation|&#x2713;|&#x2713;|
|[MiKo_2050](/Documentation/MiKo_2050.md)|Follow .NET Framework conventions for exception documentation|&#x2713;|&#x2713;|
|[MiKo_2051](/Documentation/MiKo_2051.md)|Document thrown exceptions as conditions (e.g., '&lt;paramref name="xyz"/&gt; is &lt;c&gt;42&lt;/c&gt;')|&#x2713;|&#x2713;|
|[MiKo_2052](/Documentation/MiKo_2052.md)|Use default phrase for ArgumentNullException documentation|&#x2713;|&#x2713;|
|[MiKo_2053](/Documentation/MiKo_2053.md)|Document ArgumentNullException only for reference type parameters|&#x2713;|\-|
|[MiKo_2054](/Documentation/MiKo_2054.md)|Start ArgumentException documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2055](/Documentation/MiKo_2055.md)|Start ArgumentOutOfRangeException documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2056](/Documentation/MiKo_2056.md)|End ObjectDisposedException documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2057](/Documentation/MiKo_2057.md)|Do not throw ObjectDisposedException from non-disposable types|&#x2713;|&#x2713;|
|[MiKo_2059](/Documentation/MiKo_2059.md)|Consolidate multiple documentations of same exception into one|&#x2713;|&#x2713;|
|[MiKo_2060](/Documentation/MiKo_2060.md)|Document factories uniformly|&#x2713;|&#x2713;|
|[MiKo_2070](/Documentation/MiKo_2070.md)|Do not start &lt;summary&gt; documentation with 'Returns'|&#x2713;|&#x2713;|
|[MiKo_2071](/Documentation/MiKo_2071.md)|Do not use boolean phrases in Enum return type &lt;summary&gt; documentation|&#x2713;|\-|
|[MiKo_2072](/Documentation/MiKo_2072.md)|Do not start &lt;summary&gt; documentation with 'Try'|&#x2713;|&#x2713;|
|[MiKo_2073](/Documentation/MiKo_2073.md)|Start 'Contains' method &lt;summary&gt; documentation with 'Determines whether'|&#x2713;|&#x2713;|
|[MiKo_2074](/Documentation/MiKo_2074.md)|End 'Contains' method parameter documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2075](/Documentation/MiKo_2075.md)|Use 'callback' instead of 'action', 'func' or 'function' in documentation|&#x2713;|&#x2713;|
|[MiKo_2076](/Documentation/MiKo_2076.md)|Document default values of optional parameters|&#x2713;|&#x2713;|
|[MiKo_2077](/Documentation/MiKo_2077.md)|Do not include &lt;code&gt; in &lt;summary&gt; documentation|&#x2713;|\-|
|[MiKo_2078](/Documentation/MiKo_2078.md)|Do not include XML tags in &lt;code&gt; documentation|&#x2713;|\-|
|[MiKo_2079](/Documentation/MiKo_2079.md)|Do not include obvious text in property &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2080](/Documentation/MiKo_2080.md)|Start field &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2081](/Documentation/MiKo_2081.md)|End public-visible read-only field &lt;summary&gt; documentation with default phrase|&#x2713;|&#x2713;|
|[MiKo_2082](/Documentation/MiKo_2082.md)|Use distinct phrases for Enum member &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2083](/Documentation/MiKo_2083.md)|Do not falsely document writable fields as read-only|&#x2713;|&#x2713;|
|[MiKo_2090](/Documentation/MiKo_2090.md)|Use default phrase for equality operator documentation|&#x2713;|&#x2713;|
|[MiKo_2091](/Documentation/MiKo_2091.md)|Use default phrase for inequality operator documentation|&#x2713;|&#x2713;|
|[MiKo_2100](/Documentation/MiKo_2100.md)|Start &lt;example&gt; documentation with descriptive default phrase|&#x2713;|&#x2713;|
|[MiKo_2101](/Documentation/MiKo_2101.md)|Show code examples within &lt;code&gt; tags in &lt;example&gt; documentation|&#x2713;|&#x2713;|
|[MiKo_2200](/Documentation/MiKo_2200.md)|Start comments with a capitalized letter|&#x2713;|&#x2713;|
|[MiKo_2201](/Documentation/MiKo_2201.md)|Start sentences in comments with a capitalized letter|&#x2713;|\-|
|[MiKo_2202](/Documentation/MiKo_2202.md)|Use term 'identifier' instead of 'id' in documentation|&#x2713;|&#x2713;|
|[MiKo_2203](/Documentation/MiKo_2203.md)|Use term 'unique identifier' instead of 'guid' in documentation|&#x2713;|&#x2713;|
|[MiKo_2204](/Documentation/MiKo_2204.md)|Use &lt;list&gt; for enumerations in documentation|&#x2713;|&#x2713;|
|[MiKo_2205](/Documentation/MiKo_2205.md)|Use &lt;note&gt; for important information in documentation|&#x2713;|\-|
|[MiKo_2206](/Documentation/MiKo_2206.md)|Do not use term 'flag' in documentation|&#x2713;|\-|
|[MiKo_2207](/Documentation/MiKo_2207.md)|Keep &lt;summary&gt; documentation short|&#x2713;|\-|
|[MiKo_2208](/Documentation/MiKo_2208.md)|Do not use term 'an instance of' in documentation|&#x2713;|&#x2713;|
|[MiKo_2209](/Documentation/MiKo_2209.md)|Do not use double periods in documentation|&#x2713;|&#x2713;|
|[MiKo_2210](/Documentation/MiKo_2210.md)|Use term 'information' instead of 'info' in documentation|&#x2713;|&#x2713;|
|[MiKo_2211](/Documentation/MiKo_2211.md)|Do not use &lt;remarks&gt; sections for enum members|&#x2713;|&#x2713;|
|[MiKo_2212](/Documentation/MiKo_2212.md)|Use phrase 'failed' instead of 'was not successful' in documentation|&#x2713;|&#x2713;|
|[MiKo_2213](/Documentation/MiKo_2213.md)|Do not use contraction "n't" in documentation|&#x2713;|&#x2713;|
|[MiKo_2214](/Documentation/MiKo_2214.md)|Remove empty lines from documentation|&#x2713;|&#x2713;|
|[MiKo_2215](/Documentation/MiKo_2215.md)|Keep documentation sentences short|&#x2713;|\-|
|[MiKo_2216](/Documentation/MiKo_2216.md)|Use &lt;paramref&gt; instead of &lt;param&gt; to reference parameters|&#x2713;|&#x2713;|
|[MiKo_2217](/Documentation/MiKo_2217.md)|Format &lt;list&gt; documentation properly|&#x2713;|&#x2713;|
|[MiKo_2218](/Documentation/MiKo_2218.md)|Use shorter terms instead of 'used to/in/by' in documentation|&#x2713;|&#x2713;|
|[MiKo_2219](/Documentation/MiKo_2219.md)|Do not use question or exclamation marks in documentation|&#x2713;|\-|
|[MiKo_2220](/Documentation/MiKo_2220.md)|Use 'to seek' instead of 'to look for', 'to inspect for' or 'to test for' in documentation|&#x2713;|&#x2713;|
|[MiKo_2221](/Documentation/MiKo_2221.md)|Do not use empty XML tags in documentation|&#x2713;|\-|
|[MiKo_2222](/Documentation/MiKo_2222.md)|Use term 'identification' instead of 'ident' in documentation|&#x2713;|&#x2713;|
|[MiKo_2223](/Documentation/MiKo_2223.md)|Link references via &lt;see cref="..."/&gt; in documentation|&#x2713;|&#x2713;|
|[MiKo_2224](/Documentation/MiKo_2224.md)|Place XML tags and texts on separate lines in documentation|&#x2713;|&#x2713;|
|[MiKo_2225](/Documentation/MiKo_2225.md)|Place code marked with &lt;c&gt; tags on single line|&#x2713;|&#x2713;|
|[MiKo_2226](/Documentation/MiKo_2226.md)|Explain the 'Why' instead of the 'That' in documentation|&#x2713;|\-|
|[MiKo_2227](/Documentation/MiKo_2227.md)|Remove ReSharper suppressions from documentation|&#x2713;|\-|
|[MiKo_2228](/Documentation/MiKo_2228.md)|Use positive wording instead of negative in documentation|&#x2713;|\-|
|[MiKo_2229](/Documentation/MiKo_2229.md)|Remove left-over XML fragments from documentation|&#x2713;|&#x2713;|
|[MiKo_2230](/Documentation/MiKo_2230.md)|Use &lt;list&gt; for return values with specific meanings in documentation|&#x2713;|&#x2713;|
|[MiKo_2231](/Documentation/MiKo_2231.md)|Use '&lt;inheritdoc /&gt;' marker for overridden 'GetHashCode()' methods documentation|&#x2713;|&#x2713;|
|[MiKo_2232](/Documentation/MiKo_2232.md)|Do not leave &lt;summary&gt; documentation empty|&#x2713;|&#x2713;|
|[MiKo_2233](/Documentation/MiKo_2233.md)|Place XML tags on single line|&#x2713;|&#x2713;|
|[MiKo_2234](/Documentation/MiKo_2234.md)|Use 'to' instead of 'that is to' or 'which is to' in documentation|&#x2713;|&#x2713;|
|[MiKo_2235](/Documentation/MiKo_2235.md)|Use 'will' instead of 'going to' in documentation|&#x2713;|&#x2713;|
|[MiKo_2236](/Documentation/MiKo_2236.md)|Use 'for example' instead of abbreviation 'e.g.' in documentation|&#x2713;|&#x2713;|
|[MiKo_2237](/Documentation/MiKo_2237.md)|Do not separate documentation with empty lines|&#x2713;|&#x2713;|
|[MiKo_2238](/Documentation/MiKo_2238.md)|Do not start &lt;summary&gt; documentation with 'Make sure to call this'|&#x2713;|\-|
|[MiKo_2239](/Documentation/MiKo_2239.md)|Use '///' instead of '/** */' for documentation|&#x2713;|&#x2713;|
|[MiKo_2240](/Documentation/MiKo_2240.md)|Do not start &lt;response&gt; documentation with 'Returns'|&#x2713;|&#x2713;|
|[MiKo_2241](/Documentation/MiKo_2241.md)|Do not use 'empty string' in documentation|&#x2713;|&#x2713;|
|[MiKo_2242](/Documentation/MiKo_2242.md)|Use 'textual representation' instead of 'string representation' in documentation|&#x2713;|&#x2713;|
|[MiKo_2244](/Documentation/MiKo_2244.md)|Use &lt;list&gt; instead of &lt;ul&gt; or &lt;ol&gt; in documentation|&#x2713;|&#x2713;|
|[MiKo_2245](/Documentation/MiKo_2245.md)|Wrap numbers with &lt;c&gt; in documentation|&#x2713;|&#x2713;|
|[MiKo_2300](/Documentation/MiKo_2300.md)|Explain the 'Why' instead of the 'How' in comments|&#x2713;|\-|
|[MiKo_2301](/Documentation/MiKo_2301.md)|Do not use obvious comments in AAA-Tests|&#x2713;|&#x2713;|
|[MiKo_2302](/Documentation/MiKo_2302.md)|Remove commented-out code|&#x2713;|\-|
|[MiKo_2303](/Documentation/MiKo_2303.md)|Do not end comments with a period|&#x2713;|&#x2713;|
|[MiKo_2304](/Documentation/MiKo_2304.md)|Do not formulate comments as questions|&#x2713;|\-|
|[MiKo_2305](/Documentation/MiKo_2305.md)|Do not use double periods in comments|&#x2713;|&#x2713;|
|[MiKo_2306](/Documentation/MiKo_2306.md)|End comments with a period|\-|\-|
|[MiKo_2307](/Documentation/MiKo_2307.md)|Use phrase 'failed' instead of 'was not successful' in comments|&#x2713;|&#x2713;|
|[MiKo_2308](/Documentation/MiKo_2308.md)|Place comments after code instead of on single line before closing brace|&#x2713;|&#x2713;|
|[MiKo_2309](/Documentation/MiKo_2309.md)|Do not use contraction "n't" in comments|&#x2713;|&#x2713;|
|[MiKo_2310](/Documentation/MiKo_2310.md)|Explain the 'Why' instead of the 'That' in comments|&#x2713;|\-|
|[MiKo_2311](/Documentation/MiKo_2311.md)|Do not use separator comments|&#x2713;|&#x2713;|
|[MiKo_2312](/Documentation/MiKo_2312.md)|Use 'to' instead of 'that is to' or 'which is to' in comments|&#x2713;|&#x2713;|
|[MiKo_2313](/Documentation/MiKo_2313.md)|Format plain documentation comments as XML documentation|&#x2713;|&#x2713;|

### Maintainability

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_3000](/Documentation/MiKo_3000.md)|Do not use empty regions|&#x2713;|\-|
|[MiKo_3001](/Documentation/MiKo_3001.md)|Do not use custom delegates|&#x2713;|\-|
|[MiKo_3002](/Documentation/MiKo_3002.md)|Limit class dependencies|&#x2713;|\-|
|[MiKo_3003](/Documentation/MiKo_3003.md)|Follow .NET Framework Design Guidelines for events|&#x2713;|\-|
|[MiKo_3004](/Documentation/MiKo_3004.md)|Make EventArgs property setters private|&#x2713;|\-|
|[MiKo_3005](/Documentation/MiKo_3005.md)|Follow Trier-Doer-Pattern for methods named 'Try'|&#x2713;|\-|
|[MiKo_3006](/Documentation/MiKo_3006.md)|Place 'CancellationToken' parameter last in method parameters|&#x2713;|\-|
|[MiKo_3007](/Documentation/MiKo_3007.md)|Do not mix LINQ method and declarative query syntax in same method|&#x2713;|\-|
|[MiKo_3008](/Documentation/MiKo_3008.md)|Return immutable collections|&#x2713;|\-|
|[MiKo_3009](/Documentation/MiKo_3009.md)|Use named methods instead of lambda expressions with commands|&#x2713;|\-|
|[MiKo_3010](/Documentation/MiKo_3010.md)|Do not create or throw reserved exception types|&#x2713;|\-|
|[MiKo_3011](/Documentation/MiKo_3011.md)|Provide correct parameter name for ArgumentExceptions|&#x2713;|&#x2713;|
|[MiKo_3012](/Documentation/MiKo_3012.md)|Provide actual value when throwing ArgumentOutOfRangeExceptions|&#x2713;|&#x2713;|
|[MiKo_3013](/Documentation/MiKo_3013.md)|Throw ArgumentOutOfRangeException (not ArgumentException) in 'switch' default clauses|&#x2713;|&#x2713;|
|[MiKo_3014](/Documentation/MiKo_3014.md)|Include reason in InvalidOperationException, NotImplementedException and NotSupportedException messages|&#x2713;|&#x2713;|
|[MiKo_3015](/Documentation/MiKo_3015.md)|Use InvalidOperationExceptions for inappropriate states of parameterless methods|&#x2713;|&#x2713;|
|[MiKo_3016](/Documentation/MiKo_3016.md)|Do not throw ArgumentNullException for property return values|&#x2713;|&#x2713;|
|[MiKo_3017](/Documentation/MiKo_3017.md)|Include original exception when throwing new exceptions|&#x2713;|&#x2713;|
|[MiKo_3018](/Documentation/MiKo_3018.md)|Throw ObjectDisposedExceptions on public methods of disposable types|&#x2713;|\-|
|[MiKo_3020](/Documentation/MiKo_3020.md)|Use 'Task.CompletedTask' instead of 'Task.FromResult'|&#x2713;|&#x2713;|
|[MiKo_3021](/Documentation/MiKo_3021.md)|Do not use 'Task.Run' in implementation|&#x2713;|\-|
|[MiKo_3022](/Documentation/MiKo_3022.md)|Do not return Task&lt;IEnumerable&gt; or Task&lt;IEnumerable&lt;T&gt;&gt;|&#x2713;|\-|
|[MiKo_3023](/Documentation/MiKo_3023.md)|Do not use 'CancellationTokenSource' as parameter|&#x2713;|\-|
|[MiKo_3024](/Documentation/MiKo_3024.md)|Do not use [ref] keyword on reference parameters|&#x2713;|\-|
|[MiKo_3025](/Documentation/MiKo_3025.md)|Do not re-assign method parameters|&#x2713;|\-|
|[MiKo_3026](/Documentation/MiKo_3026.md)|Remove unused parameters|&#x2713;|\-|
|[MiKo_3027](/Documentation/MiKo_3027.md)|Do not reserve parameters for future use|&#x2713;|\-|
|[MiKo_3028](/Documentation/MiKo_3028.md)|Do not assign null to lambda parameters|&#x2713;|\-|
|[MiKo_3029](/Documentation/MiKo_3029.md)|Prevent memory leaks in event registrations|&#x2713;|\-|
|[MiKo_3030](/Documentation/MiKo_3030.md)|Follow Law of Demeter in methods|\-|\-|
|[MiKo_3031](/Documentation/MiKo_3031.md)|Do not implement ICloneable.Clone()|&#x2713;|\-|
|[MiKo_3032](/Documentation/MiKo_3032.md)|Use 'nameof' instead of Cinch for PropertyChangedEventArgs property names|&#x2713;|&#x2713;|
|[MiKo_3033](/Documentation/MiKo_3033.md)|Use 'nameof' for property names in PropertyChangingEventArgs and PropertyChangedEventArgs|&#x2713;|&#x2713;|
|[MiKo_3034](/Documentation/MiKo_3034.md)|Use [CallerMemberName] attribute for PropertyChanged event raisers|&#x2713;|&#x2713;|
|[MiKo_3035](/Documentation/MiKo_3035.md)|Always specify timeouts with 'WaitOne' methods|&#x2713;|\-|
|[MiKo_3036](/Documentation/MiKo_3036.md)|Use 'TimeSpan' factory methods instead of constructors|&#x2713;|&#x2713;|
|[MiKo_3037](/Documentation/MiKo_3037.md)|Do not use magic numbers for timeouts|&#x2713;|\-|
|[MiKo_3038](/Documentation/MiKo_3038.md)|Do not use magic numbers|&#x2713;|\-|
|[MiKo_3039](/Documentation/MiKo_3039.md)|Do not use Linq or yield in properties|&#x2713;|\-|
|[MiKo_3040](/Documentation/MiKo_3040.md)|Use enums instead of booleans when more than 2 values might be needed|&#x2713;|\-|
|[MiKo_3041](/Documentation/MiKo_3041.md)|Do not use delegates in EventArgs|&#x2713;|\-|
|[MiKo_3042](/Documentation/MiKo_3042.md)|Do not implement interfaces in EventArgs|&#x2713;|\-|
|[MiKo_3043](/Documentation/MiKo_3043.md)|Use 'nameof' for WeakEventManager event (de-)registrations|&#x2713;|&#x2713;|
|[MiKo_3044](/Documentation/MiKo_3044.md)|Use 'nameof' to compare property names of PropertyChangingEventArgs and PropertyChangedEventArgs|&#x2713;|&#x2713;|
|[MiKo_3045](/Documentation/MiKo_3045.md)|Use 'nameof' for EventManager event registrations|&#x2713;|&#x2713;|
|[MiKo_3046](/Documentation/MiKo_3046.md)|Use 'nameof' for property names in property raising methods|&#x2713;|&#x2713;|
|[MiKo_3047](/Documentation/MiKo_3047.md)|Use 'nameof' for applied [ContentProperty] attributes|&#x2713;|&#x2713;|
|[MiKo_3048](/Documentation/MiKo_3048.md)|Apply [ValueConversion] attribute to ValueConverters|&#x2713;|\-|
|[MiKo_3049](/Documentation/MiKo_3049.md)|Apply [Description] attribute to enum members|&#x2713;|\-|
|[MiKo_3050](/Documentation/MiKo_3050.md)|Declare DependencyProperty fields as 'public static readonly'|&#x2713;|&#x2713;|
|[MiKo_3051](/Documentation/MiKo_3051.md)|Register DependencyProperty fields properly|&#x2713;|&#x2713;|
|[MiKo_3052](/Documentation/MiKo_3052.md)|Declare DependencyPropertyKey fields as non-public 'static readonly'|&#x2713;|&#x2713;|
|[MiKo_3053](/Documentation/MiKo_3053.md)|Register DependencyPropertyKey fields properly|&#x2713;|\-|
|[MiKo_3054](/Documentation/MiKo_3054.md)|Expose DependencyProperty identifier for read-only DependencyProperties|&#x2713;|&#x2713;|
|[MiKo_3055](/Documentation/MiKo_3055.md)|Implement INotifyPropertyChanged in ViewModels|&#x2713;|\-|
|[MiKo_3060](/Documentation/MiKo_3060.md)|Do not use Debug.Assert or Trace.Assert|&#x2713;|&#x2713;|
|[MiKo_3061](/Documentation/MiKo_3061.md)|Use proper log categories with loggers|&#x2713;|\-|
|[MiKo_3062](/Documentation/MiKo_3062.md)|End exception log messages with a colon|&#x2713;|&#x2713;|
|[MiKo_3063](/Documentation/MiKo_3063.md)|End non-exceptional log messages with a dot|&#x2713;|&#x2713;|
|[MiKo_3064](/Documentation/MiKo_3064.md)|Do not use contraction "n't" in log messages|&#x2713;|&#x2713;|
|[MiKo_3065](/Documentation/MiKo_3065.md)|Do not use interpolated strings with Microsoft Logging calls|&#x2713;|&#x2713;|
|[MiKo_3070](/Documentation/MiKo_3070.md)|Do not return null for IEnumerable|&#x2713;|\-|
|[MiKo_3071](/Documentation/MiKo_3071.md)|Do not return null for Task|&#x2713;|\-|
|[MiKo_3072](/Documentation/MiKo_3072.md)|Do not return 'List&lt;&gt;' or 'Dictionary&lt;&gt;' from non-private methods|&#x2713;|\-|
|[MiKo_3073](/Documentation/MiKo_3073.md)|Fully initialize objects|&#x2713;|\-|
|[MiKo_3074](/Documentation/MiKo_3074.md)|Do not define 'ref' or 'out' parameters on constructors|&#x2713;|\-|
|[MiKo_3075](/Documentation/MiKo_3075.md)|Mark internal and private types as static or sealed unless derivation is needed|&#x2713;|&#x2713;|
|[MiKo_3076](/Documentation/MiKo_3076.md)|Do not initialize static members with static members below or in other type parts|&#x2713;|\-|
|[MiKo_3077](/Documentation/MiKo_3077.md)|Provide default values for Enum-returning properties|&#x2713;|&#x2713;|
|[MiKo_3078](/Documentation/MiKo_3078.md)|Provide default values for enum members|&#x2713;|&#x2713;|
|[MiKo_3079](/Documentation/MiKo_3079.md)|Write HResults in hexadecimal|&#x2713;|&#x2713;|
|[MiKo_3080](/Documentation/MiKo_3080.md)|Use 'switch ... return' instead of 'switch ... break' for variable assignments|&#x2713;|\-|
|[MiKo_3081](/Documentation/MiKo_3081.md)|Use pattern matching instead of logical NOT conditions|&#x2713;|&#x2713;|
|[MiKo_3082](/Documentation/MiKo_3082.md)|Use pattern matching instead of comparing with 'true' or 'false'|&#x2713;|&#x2713;|
|[MiKo_3083](/Documentation/MiKo_3083.md)|Use pattern matching for null checks|&#x2713;|&#x2713;|
|[MiKo_3084](/Documentation/MiKo_3084.md)|Place variables, not constants, on the left side of comparisons|&#x2713;|&#x2713;|
|[MiKo_3085](/Documentation/MiKo_3085.md)|Keep conditional statements short|&#x2713;|&#x2713;|
|[MiKo_3086](/Documentation/MiKo_3086.md)|Do not nest conditional statements|&#x2713;|\-|
|[MiKo_3087](/Documentation/MiKo_3087.md)|Do not use negative complex conditions|&#x2713;|\-|
|[MiKo_3088](/Documentation/MiKo_3088.md)|Use pattern matching for not-null checks|&#x2713;|&#x2713;|
|[MiKo_3089](/Documentation/MiKo_3089.md)|Do not use simple constant property patterns as conditions in 'if' statements|&#x2713;|&#x2713;|
|[MiKo_3090](/Documentation/MiKo_3090.md)|Do not throw exceptions in finally blocks|&#x2713;|\-|
|[MiKo_3091](/Documentation/MiKo_3091.md)|Do not raise events in finally blocks|&#x2713;|\-|
|[MiKo_3092](/Documentation/MiKo_3092.md)|Do not raise events in locks|&#x2713;|\-|
|[MiKo_3093](/Documentation/MiKo_3093.md)|Do not invoke delegates inside locks|&#x2713;|\-|
|[MiKo_3094](/Documentation/MiKo_3094.md)|Do not invoke methods or properties of parameters inside locks|&#x2713;|\-|
|[MiKo_3095](/Documentation/MiKo_3095.md)|Do not use empty code blocks|&#x2713;|\-|
|[MiKo_3096](/Documentation/MiKo_3096.md)|Use dictionaries instead of large switch statements|&#x2713;|\-|
|[MiKo_3097](/Documentation/MiKo_3097.md)|Do not cast to type and return object|&#x2713;|\-|
|[MiKo_3098](/Documentation/MiKo_3098.md)|Provide meaningful explanations for suppressed messages|&#x2713;|\-|
|[MiKo_3099](/Documentation/MiKo_3099.md)|Do not compare enum values with null|&#x2713;|&#x2713;|
|[MiKo_3100](/Documentation/MiKo_3100.md)|Place test classes in same namespace as types under test|&#x2713;|\-|
|[MiKo_3101](/Documentation/MiKo_3101.md)|Include tests in test classes|&#x2713;|\-|
|[MiKo_3102](/Documentation/MiKo_3102.md)|Do not use conditional statements in test methods|&#x2713;|\-|
|[MiKo_3103](/Documentation/MiKo_3103.md)|Do not use 'Guid.NewGuid()' in test methods|&#x2713;|&#x2713;|
|[MiKo_3104](/Documentation/MiKo_3104.md)|Apply NUnit's [Combinatorial] attribute properly|&#x2713;|&#x2713;|
|[MiKo_3105](/Documentation/MiKo_3105.md)|Use NUnit's fluent Assert approach in test methods|&#x2713;|&#x2713;|
|[MiKo_3106](/Documentation/MiKo_3106.md)|Do not use equality or comparison operators in assertions|&#x2713;|\-|
|[MiKo_3107](/Documentation/MiKo_3107.md)|Use Moq Mock condition matchers only on mocks|&#x2713;|&#x2713;|
|[MiKo_3108](/Documentation/MiKo_3108.md)|Include assertions in test methods|&#x2713;|\-|
|[MiKo_3109](/Documentation/MiKo_3109.md)|Include assertion messages with multiple assertions|&#x2713;|&#x2713;|
|[MiKo_3110](/Documentation/MiKo_3110.md)|Do not use 'Count' or 'Length' in assertions|&#x2713;|&#x2713;|
|[MiKo_3111](/Documentation/MiKo_3111.md)|Use 'Is.Zero' instead of 'Is.EqualTo(0)' in assertions|&#x2713;|&#x2713;|
|[MiKo_3112](/Documentation/MiKo_3112.md)|Use 'Is.Empty' instead of 'Has.Count.Zero' in assertions|&#x2713;|&#x2713;|
|[MiKo_3113](/Documentation/MiKo_3113.md)|Do not use FluentAssertions|&#x2713;|&#x2713;|
|[MiKo_3114](/Documentation/MiKo_3114.md)|Use 'Mock.Of&lt;T&gt;()' instead of 'new Mock&lt;T&gt;().Object'|&#x2713;|&#x2713;|
|[MiKo_3115](/Documentation/MiKo_3115.md)|Include code in test methods|&#x2713;|\-|
|[MiKo_3116](/Documentation/MiKo_3116.md)|Include code in test initialization methods|&#x2713;|\-|
|[MiKo_3117](/Documentation/MiKo_3117.md)|Include code in test cleanup methods|&#x2713;|\-|
|[MiKo_3118](/Documentation/MiKo_3118.md)|Do not use ambiguous Linq calls in test methods|&#x2713;|\-|
|[MiKo_3119](/Documentation/MiKo_3119.md)|Do not return only completed task in test methods|&#x2713;|&#x2713;|
|[MiKo_3120](/Documentation/MiKo_3120.md)|Use direct values instead of 'It.Is&lt;&gt;(...)' condition matcher to verify exact values in Moq mocks|&#x2713;|&#x2713;|
|[MiKo_3121](/Documentation/MiKo_3121.md)|Test concrete implementations instead of interfaces|&#x2713;|\-|
|[MiKo_3122](/Documentation/MiKo_3122.md)|Limit test method parameters to 2 or fewer|&#x2713;|\-|
|[MiKo_3123](/Documentation/MiKo_3123.md)|Do not catch exceptions in test methods|&#x2713;|&#x2713;|
|[MiKo_3124](/Documentation/MiKo_3124.md)|Do not use assertions in finally blocks in test methods|&#x2713;|&#x2713;|
|[MiKo_3201](/Documentation/MiKo_3201.md)|Invert if statements in short methods|&#x2713;|&#x2713;|
|[MiKo_3202](/Documentation/MiKo_3202.md)|Use positive conditions when returning in all paths|&#x2713;|&#x2713;|
|[MiKo_3203](/Documentation/MiKo_3203.md)|Invert if-continue statements when followed by single line|&#x2713;|&#x2713;|
|[MiKo_3204](/Documentation/MiKo_3204.md)|Invert negative if statements when they have an else clause|&#x2713;|&#x2713;|
|[MiKo_3210](/Documentation/MiKo_3210.md)|Make only the longest overloads virtual or abstract|&#x2713;|\-|
|[MiKo_3211](/Documentation/MiKo_3211.md)|Do not use finalizers in public types|&#x2713;|\-|
|[MiKo_3212](/Documentation/MiKo_3212.md)|Follow standard Dispose pattern without adding other Dispose methods|&#x2713;|\-|
|[MiKo_3213](/Documentation/MiKo_3213.md)|Implement parameterless Dispose method using Basic Dispose pattern|&#x2713;|\-|
|[MiKo_3214](/Documentation/MiKo_3214.md)|Remove 'Begin/End' or 'Enter/Exit' scope-defining methods from interfaces|&#x2713;|\-|
|[MiKo_3215](/Documentation/MiKo_3215.md)|Use 'Func&lt;T, bool&gt;' instead of 'Predicate&lt;bool&gt;' for callbacks|&#x2713;|&#x2713;|
|[MiKo_3216](/Documentation/MiKo_3216.md)|Mark static fields with initializers as read-only|&#x2713;|&#x2713;|
|[MiKo_3217](/Documentation/MiKo_3217.md)|Do not use generic types that have other generic types as type arguments|&#x2713;|\-|
|[MiKo_3218](/Documentation/MiKo_3218.md)|Define extension methods in expected places|&#x2713;|\-|
|[MiKo_3219](/Documentation/MiKo_3219.md)|Do not mark public members as 'virtual'|&#x2713;|\-|
|[MiKo_3220](/Documentation/MiKo_3220.md)|Simplify logical '&amp;&amp;' or '&#124;&#124;' conditions using 'true' or 'false'|&#x2713;|&#x2713;|
|[MiKo_3221](/Documentation/MiKo_3221.md)|Use 'HashCode.Combine' in GetHashCode overrides|&#x2713;|&#x2713;|
|[MiKo_3222](/Documentation/MiKo_3222.md)|Simplify string comparisons|&#x2713;|&#x2713;|
|[MiKo_3223](/Documentation/MiKo_3223.md)|Simplify reference comparisons|&#x2713;|&#x2713;|
|[MiKo_3224](/Documentation/MiKo_3224.md)|Simplify value comparisons|&#x2713;|&#x2713;|
|[MiKo_3225](/Documentation/MiKo_3225.md)|Remove duplicate logical conditions|&#x2713;|&#x2713;|
|[MiKo_3226](/Documentation/MiKo_3226.md)|Make read-only fields with initializers const|&#x2713;|&#x2713;|
|[MiKo_3227](/Documentation/MiKo_3227.md)|Use pattern matching for equality checks|&#x2713;|&#x2713;|
|[MiKo_3228](/Documentation/MiKo_3228.md)|Use pattern matching for inequality checks|&#x2713;|&#x2713;|
|[MiKo_3229](/Documentation/MiKo_3229.md)|Use 'KeyValuePair.Create' instead of constructors|&#x2713;|&#x2713;|
|[MiKo_3230](/Documentation/MiKo_3230.md)|Do not use 'Guid' as type for identifiers|&#x2713;|\-|
|[MiKo_3231](/Documentation/MiKo_3231.md)|Use pattern matching for ordinal string comparison equality checks|&#x2713;|&#x2713;|
|[MiKo_3232](/Documentation/MiKo_3232.md)|Use null checks instead of empty property pattern|&#x2713;|\-|
|[MiKo_3301](/Documentation/MiKo_3301.md)|Use lambda expression bodies instead of parenthesized lambda expression blocks for single statements|&#x2713;|&#x2713;|
|[MiKo_3302](/Documentation/MiKo_3302.md)|Use simple lambda expression bodies instead of parenthesized lambda expression bodies for single parameters|&#x2713;|&#x2713;|
|[MiKo_3401](/Documentation/MiKo_3401.md)|Keep namespace hierarchies from becoming too deep|&#x2713;|\-|
|[MiKo_3501](/Documentation/MiKo_3501.md)|Do not suppress nullable warnings on Null-conditional operators|&#x2713;|&#x2713;|
|[MiKo_3502](/Documentation/MiKo_3502.md)|Do not suppress nullable warnings on Linq calls|&#x2713;|&#x2713;|
|[MiKo_3503](/Documentation/MiKo_3503.md)|Do not assign variables in try-catch blocks that are returned directly outside|&#x2713;|&#x2713;|

### Ordering

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_4001](/Documentation/MiKo_4001.md)|Order methods with same name based on their parameter count|&#x2713;|&#x2713;|
|[MiKo_4002](/Documentation/MiKo_4002.md)|Place methods with same name and accessibility side-by-side|&#x2713;|&#x2713;|
|[MiKo_4003](/Documentation/MiKo_4003.md)|Place Dispose methods directly after constructors and finalizers|&#x2713;|&#x2713;|
|[MiKo_4004](/Documentation/MiKo_4004.md)|Place Dispose methods before all other methods of the same accessibility|&#x2713;|&#x2713;|
|[MiKo_4005](/Documentation/MiKo_4005.md)|Place the interface that gives a type its name directly after the type's declaration|&#x2713;|&#x2713;|
|[MiKo_4007](/Documentation/MiKo_4007.md)|Place operators before methods|&#x2713;|&#x2713;|
|[MiKo_4008](/Documentation/MiKo_4008.md)|Place GetHashCode methods directly after Equals methods|&#x2713;|&#x2713;|
|[MiKo_4101](/Documentation/MiKo_4101.md)|Place test initialization methods directly after one-time methods|&#x2713;|&#x2713;|
|[MiKo_4102](/Documentation/MiKo_4102.md)|Place test cleanup methods after test initialization methods and before test methods|&#x2713;|&#x2713;|
|[MiKo_4103](/Documentation/MiKo_4103.md)|Place one-time test initialization methods after assembly-wide test lifecycle methods and before all other methods|&#x2713;|&#x2713;|
|[MiKo_4104](/Documentation/MiKo_4104.md)|Place one-time test cleanup methods directly after one-time test initialization methods|&#x2713;|&#x2713;|
|[MiKo_4105](/Documentation/MiKo_4105.md)|Place object under test fields before all other fields|&#x2713;|&#x2713;|
|[MiKo_4106](/Documentation/MiKo_4106.md)|Place assembly-wide test initialization methods before all other methods|&#x2713;|&#x2713;|
|[MiKo_4107](/Documentation/MiKo_4107.md)|Place assembly-wide test cleanup methods directly after assembly-wide test initialization methods|&#x2713;|&#x2713;|

### Performance

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_5001](/Documentation/MiKo_5001.md)|Invoke 'Debug' and 'DebugFormat' methods only after checking 'IsDebugEnabled'|&#x2713;|&#x2713;|
|[MiKo_5002](/Documentation/MiKo_5002.md)|Use 'xxxFormat' methods only with multiple arguments|&#x2713;|&#x2713;|
|[MiKo_5003](/Documentation/MiKo_5003.md)|Use appropriate Log methods for exceptions|&#x2713;|\-|
|[MiKo_5010](/Documentation/MiKo_5010.md)|Do not use 'object.Equals()' on value types|&#x2713;|&#x2713;|
|[MiKo_5011](/Documentation/MiKo_5011.md)|Do not concatenate strings with += operator|&#x2713;|\-|
|[MiKo_5012](/Documentation/MiKo_5012.md)|Do not use 'yield return' for recursively defined structures|&#x2713;|\-|
|[MiKo_5013](/Documentation/MiKo_5013.md)|Do not create empty arrays|&#x2713;|&#x2713;|
|[MiKo_5014](/Documentation/MiKo_5014.md)|Do not create empty lists when return value is read-only|&#x2713;|&#x2713;|
|[MiKo_5015](/Documentation/MiKo_5015.md)|Do not intern string literals|&#x2713;|&#x2713;|
|[MiKo_5016](/Documentation/MiKo_5016.md)|Use HashSet for lookups in 'List.RemoveAll'|&#x2713;|\-|
|[MiKo_5017](/Documentation/MiKo_5017.md)|Make fields or variables assigned with string literals constant|&#x2713;|&#x2713;|
|[MiKo_5018](/Documentation/MiKo_5018.md)|Perform value comparisons before reference comparisons|&#x2713;|&#x2713;|
|[MiKo_5019](/Documentation/MiKo_5019.md)|Add [in] modifier to read-only struct parameters|&#x2713;|&#x2713;|

### Spacing

|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|[MiKo_6001](/Documentation/MiKo_6001.md)|Surround log statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6002](/Documentation/MiKo_6002.md)|Surround assertion statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6003](/Documentation/MiKo_6003.md)|Precede local variable statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6004](/Documentation/MiKo_6004.md)|Precede variable assignment statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6005](/Documentation/MiKo_6005.md)|Precede return statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6006](/Documentation/MiKo_6006.md)|Surround awaited statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6007](/Documentation/MiKo_6007.md)|Surround test statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6008](/Documentation/MiKo_6008.md)|Precede using directives with blank lines|&#x2713;|&#x2713;|
|[MiKo_6009](/Documentation/MiKo_6009.md)|Surround try statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6010](/Documentation/MiKo_6010.md)|Surround if statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6011](/Documentation/MiKo_6011.md)|Surround lock statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6012](/Documentation/MiKo_6012.md)|Surround foreach loops with blank lines|&#x2713;|&#x2713;|
|[MiKo_6013](/Documentation/MiKo_6013.md)|Surround for loops with blank lines|&#x2713;|&#x2713;|
|[MiKo_6014](/Documentation/MiKo_6014.md)|Surround while loops with blank lines|&#x2713;|&#x2713;|
|[MiKo_6015](/Documentation/MiKo_6015.md)|Surround do/while loops with blank lines|&#x2713;|&#x2713;|
|[MiKo_6016](/Documentation/MiKo_6016.md)|Surround using statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6017](/Documentation/MiKo_6017.md)|Surround switch statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6018](/Documentation/MiKo_6018.md)|Surround break statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6019](/Documentation/MiKo_6019.md)|Surround continue statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6020](/Documentation/MiKo_6020.md)|Surround throw statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6021](/Documentation/MiKo_6021.md)|Surround ArgumentNullException.ThrowIfNull statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6022](/Documentation/MiKo_6022.md)|Surround ArgumentException.ThrowIfNullOrEmpty statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6023](/Documentation/MiKo_6023.md)|Surround ArgumentOutOfRangeException.ThrowIf statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6024](/Documentation/MiKo_6024.md)|Surround ObjectDisposedException.ThrowIf statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6030](/Documentation/MiKo_6030.md)|Place open braces of initializers directly below the corresponding type definition|&#x2713;|&#x2713;|
|[MiKo_6031](/Documentation/MiKo_6031.md)|Place question and colon tokens of ternary operators directly below the corresponding condition|&#x2713;|&#x2713;|
|[MiKo_6032](/Documentation/MiKo_6032.md)|Position multi-line parameters outdented at end of method|&#x2713;|&#x2713;|
|[MiKo_6033](/Documentation/MiKo_6033.md)|Place braces of blocks below case sections directly below the corresponding case keyword|&#x2713;|&#x2713;|
|[MiKo_6034](/Documentation/MiKo_6034.md)|Place dots on same line(s) as invoked members|&#x2713;|&#x2713;|
|[MiKo_6035](/Documentation/MiKo_6035.md)|Place open parenthesis on same line(s) as invoked methods|&#x2713;|&#x2713;|
|[MiKo_6036](/Documentation/MiKo_6036.md)|Place lambda blocks directly below the corresponding arrow(s)|&#x2713;|&#x2713;|
|[MiKo_6037](/Documentation/MiKo_6037.md)|Place single arguments on same line(s) as invoked methods|&#x2713;|&#x2713;|
|[MiKo_6038](/Documentation/MiKo_6038.md)|Place casts on same line(s)|&#x2713;|&#x2713;|
|[MiKo_6039](/Documentation/MiKo_6039.md)|Place return values on same line(s) as return keywords|&#x2713;|&#x2713;|
|[MiKo_6040](/Documentation/MiKo_6040.md)|Align consecutive multi-line invocations by their dots|&#x2713;|&#x2713;|
|[MiKo_6041](/Documentation/MiKo_6041.md)|Place assignments on same line(s)|&#x2713;|&#x2713;|
|[MiKo_6042](/Documentation/MiKo_6042.md)|Place 'new' keywords on same line(s) as the types|&#x2713;|&#x2713;|
|[MiKo_6043](/Documentation/MiKo_6043.md)|Place expression bodies of lambdas on same line as lambda when fitting|&#x2713;|&#x2713;|
|[MiKo_6044](/Documentation/MiKo_6044.md)|Place operators such as '&amp;&amp;' or '&#124;&#124;' on same line(s) as their (right) operands|&#x2713;|&#x2713;|
|[MiKo_6045](/Documentation/MiKo_6045.md)|Place comparisons using operators such as '==' or '!=' on same line(s)|&#x2713;|&#x2713;|
|[MiKo_6046](/Documentation/MiKo_6046.md)|Place calculations using operators such as '+' or '%' on same line(s)|&#x2713;|&#x2713;|
|[MiKo_6047](/Documentation/MiKo_6047.md)|Place braces of switch expressions directly below the corresponding switch keyword|&#x2713;|&#x2713;|
|[MiKo_6048](/Documentation/MiKo_6048.md)|Place logical conditions on a single line|&#x2713;|&#x2713;|
|[MiKo_6049](/Documentation/MiKo_6049.md)|Surround event (un-)registrations with blank lines|&#x2713;|&#x2713;|
|[MiKo_6050](/Documentation/MiKo_6050.md)|Position multi-line arguments outdented at end of method call|&#x2713;|&#x2713;|
|[MiKo_6051](/Documentation/MiKo_6051.md)|Place colon of constructor call on same line as constructor call|&#x2713;|&#x2713;|
|[MiKo_6052](/Documentation/MiKo_6052.md)|Place colon of list of base types on same line as first base type|&#x2713;|&#x2713;|
|[MiKo_6053](/Documentation/MiKo_6053.md)|Place single-line arguments on single line|&#x2713;|&#x2713;|
|[MiKo_6054](/Documentation/MiKo_6054.md)|Place lambda arrows on same line as the parameter(s) of the lambda|&#x2713;|&#x2713;|
|[MiKo_6055](/Documentation/MiKo_6055.md)|Surround assignment statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6056](/Documentation/MiKo_6056.md)|Place brackets of collection expressions at the same position as collection initializer braces|&#x2713;|&#x2713;|
|[MiKo_6057](/Documentation/MiKo_6057.md)|Align type parameter constraint clauses vertically|&#x2713;|&#x2713;|
|[MiKo_6058](/Documentation/MiKo_6058.md)|Indent type parameter constraint clauses below parameter list|&#x2713;|&#x2713;|
|[MiKo_6059](/Documentation/MiKo_6059.md)|Position multi-line conditions outdented below associated calls|&#x2713;|&#x2713;|
|[MiKo_6060](/Documentation/MiKo_6060.md)|Place switch case labels on same line|&#x2713;|&#x2713;|
|[MiKo_6061](/Documentation/MiKo_6061.md)|Place switch expression arms on same line|&#x2713;|&#x2713;|
|[MiKo_6062](/Documentation/MiKo_6062.md)|Place expressions within complex initializer expressions beside open brace|&#x2713;|&#x2713;|
|[MiKo_6063](/Documentation/MiKo_6063.md)|Place invocations on same line|&#x2713;|&#x2713;|
|[MiKo_6064](/Documentation/MiKo_6064.md)|Place identifier invocations on same line|&#x2713;|&#x2713;|
|[MiKo_6065](/Documentation/MiKo_6065.md)|Indent rather than outdent consecutive invocations spanning multiple lines|&#x2713;|&#x2713;|
|[MiKo_6066](/Documentation/MiKo_6066.md)|Indent rather than outdent collection expression elements|&#x2713;|&#x2713;|
|[MiKo_6067](/Documentation/MiKo_6067.md)|Place ternary operators on same lines as their respective expressions|&#x2713;|&#x2713;|
|[MiKo_6068](/Documentation/MiKo_6068.md)|Place property patterns inside 'if' conditions on same line|&#x2713;|&#x2713;|
|[MiKo_6070](/Documentation/MiKo_6070.md)|Surround Console statements with blank lines|&#x2713;|&#x2713;|
|[MiKo_6071](/Documentation/MiKo_6071.md)|Surround local using statements with blank lines|&#x2713;|&#x2713;|
