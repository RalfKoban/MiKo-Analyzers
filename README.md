# MiKo-Analyzers
Provides analyzers that are based on the .NET Compiler Platform (Roslyn) and can be used inside Visual Studio 2019 (v16.11) or 2022 (v17.11).

How to install an Roslyn analyzer is described [here](https://learn.microsoft.com/en-us/visualstudio/code-quality/install-roslyn-analyzers?view=vs-2022).

Screenshots on how to use such analyzers can be found [here](https://learn.microsoft.com/en-us/visualstudio/code-quality/use-roslyn-analyzers?view=vs-2022).


## Build / Project status
[![Maintenance](https://img.shields.io/maintenance/yes/2024.svg)](https://github.com/RalfKoban/MiKo-Analyzers)
[![Build status](https://ci.appveyor.com/api/projects/status/qanrqn7r4q9frr9m/branch/master?svg=true)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/branch/master)
[![codecov](https://codecov.io/gh/RalfKoban/MiKo-Analyzers/branch/master/graph/badge.svg)](https://codecov.io/gh/RalfKoban/MiKo-Analyzers)
[![Coverity Scan Build Status](https://img.shields.io/coverity/scan/18917.svg)](https://scan.coverity.com/projects/ralfkoban-miko-analyzers)

[![Build history](https://buildstats.info/appveyor/chart/RalfKoban/miko-analyzers)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/history)

## Available Rules
The following tables lists all the 470 rules that are currently provided by the analyzer.

### Metrics
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_0001|Method is too big|&#x2713;|\-|
|MiKo_0002|Method is too complex|&#x2713;|\-|
|MiKo_0003|Type is too big|&#x2713;|\-|
|MiKo_0004|Method has too many parameters|&#x2713;|\-|
|MiKo_0005|Local function is too big|&#x2713;|\-|
|MiKo_0006|Local function is too complex|&#x2713;|\-|
|MiKo_0007|Local function has too many parameters|&#x2713;|\-|

### Naming
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_1000|'System.EventArgs' types should be suffixed with 'EventArgs'|&#x2713;|&#x2713;|
|MiKo_1001|'System.EventArgs' parameters should be named 'e'|&#x2713;|&#x2713;|
|MiKo_1002|Parameters should be named according the .NET Framework Design Guidelines for event handlers|&#x2713;|&#x2713;|
|MiKo_1003|Event handling method names should follow the .NET Framework Design Guidelines|&#x2713;|&#x2713;|
|MiKo_1004|Events should not contain term 'Event' in their names|&#x2713;|&#x2713;|
|MiKo_1005|'System.EventArgs' variables should be named properly|&#x2713;|&#x2713;|
|MiKo_1006|Events should use 'EventHandler&lt;T&gt;' with 'EventArgs' which are named after the event|&#x2713;|\-|
|MiKo_1007|Events and their corresponding 'EventArgs' types should be located in the same namespace|&#x2713;|\-|
|MiKo_1008|Parameters should be named according the .NET Framework Design Guidelines for DependencyProperty event handlers|&#x2713;|&#x2713;|
|MiKo_1009|'System.EventHandler' variables should be named properly|&#x2713;|&#x2713;|
|MiKo_1010|Methods should not contain 'CanExecute' or 'Execute' in their names|&#x2713;|&#x2713;|
|MiKo_1011|Methods should not contain 'Do' in their names|&#x2713;|&#x2713;|
|MiKo_1012|Methods should be named 'Raise' instead of 'Fire'|&#x2713;|&#x2713;|
|MiKo_1013|Methods should not be named 'Notify' or 'OnNotify'|&#x2713;|&#x2713;|
|MiKo_1014|Methods should not be named with ambiguous 'Check'|&#x2713;|&#x2713;|
|MiKo_1015|Methods should be named 'Initialize' instead of 'Init'|&#x2713;|&#x2713;|
|MiKo_1016|Factory methods should be named 'Create'|&#x2713;|&#x2713;|
|MiKo_1017|Methods should not be prefixed with 'Get' or 'Set' if followed by 'Is', 'Can' or 'Has'|&#x2713;|&#x2713;|
|MiKo_1018|Methods should not be suffixed with noun of a verb|&#x2713;|&#x2713;|
|MiKo_1019|'Clear' and 'Remove' methods should be named based on their number of parameters|&#x2713;|&#x2713;|
|MiKo_1020|Type names should be limited in length|\-|\-|
|MiKo_1021|Method names should be limited in length|\-|\-|
|MiKo_1022|Parameter names should be limited in length|\-|\-|
|MiKo_1023|Field names should be limited in length|\-|\-|
|MiKo_1024|Property names should be limited in length|\-|\-|
|MiKo_1025|Event names should be limited in length|\-|\-|
|MiKo_1026|Variable names should be limited in length|\-|\-|
|MiKo_1027|Variable names in loops should be limited in length|\-|\-|
|MiKo_1028|Local function names should be limited in length|\-|\-|
|MiKo_1030|Types should not have an 'Abstract' or 'Base' marker to indicate that they are base types|&#x2713;|&#x2713;|
|MiKo_1031|Entity types should not use a 'Model' suffix|&#x2713;|&#x2713;|
|MiKo_1032|Methods dealing with entities should not use a 'Model' as marker|&#x2713;|&#x2713;|
|MiKo_1033|Parameters representing entities should not use a 'Model' suffix|&#x2713;|&#x2713;|
|MiKo_1034|Fields representing entities should not use a 'Model' suffix|&#x2713;|&#x2713;|
|MiKo_1035|Properties dealing with entities should not use a 'Model' marker|&#x2713;|&#x2713;|
|MiKo_1036|Events dealing with entities should not use a 'Model' marker|&#x2713;|&#x2713;|
|MiKo_1037|Types should not be suffixed with 'Type', 'Interface', 'Class', 'Struct', 'Record' or 'Enum'|&#x2713;|&#x2713;|
|MiKo_1038|Classes that contain extension methods should end with same suffix|&#x2713;|&#x2713;|
|MiKo_1039|The 'this' parameter of extension methods should have a default name|&#x2713;|&#x2713;|
|MiKo_1040|Parameters should not be suffixed with implementation details|&#x2713;|\-|
|MiKo_1041|Fields should not be suffixed with implementation details|&#x2713;|\-|
|MiKo_1042|'CancellationToken' parameters should have specific name|&#x2713;|&#x2713;|
|MiKo_1043|'CancellationToken' variables should have specific name|&#x2713;|&#x2713;|
|MiKo_1044|Commands should be suffixed with 'Command'|&#x2713;|&#x2713;|
|MiKo_1045|Methods that are invoked by commands should not be suffixed with 'Command'|&#x2713;|&#x2713;|
|MiKo_1046|Asynchronous methods should follow the Task-based Asynchronous Pattern (TAP)|&#x2713;|&#x2713;|
|MiKo_1047|Methods not following the Task-based Asynchronous Pattern (TAP) should not lie about being asynchronous|&#x2713;|&#x2713;|
|MiKo_1048|Classes that are value converters should end with a specific suffix|&#x2713;|&#x2713;|
|MiKo_1049|Do not use requirement terms such as 'Shall', 'Should', 'Must' or 'Need' for names|&#x2713;|&#x2713;|
|MiKo_1050|Return values should have descriptive names|&#x2713;|&#x2713;|
|MiKo_1051|Do not suffix parameters with delegate types|&#x2713;|&#x2713;|
|MiKo_1052|Do not suffix variables with delegate types|&#x2713;|&#x2713;|
|MiKo_1053|Do not suffix fields with delegate types|&#x2713;|&#x2713;|
|MiKo_1054|Do not name types 'Helper' or 'Utility'|&#x2713;|&#x2713;|
|MiKo_1055|Dependency properties should be suffixed with 'Property' (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1056|Dependency properties should be prefixed with property names (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1057|Dependency property keys should be suffixed with 'Key' (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1058|Dependency property keys should be prefixed with property names (as in the .NET Framework)|&#x2713;|&#x2713;|
|MiKo_1059|Do not name types 'Impl' or 'Implementation'|&#x2713;|&#x2713;|
|MiKo_1060|Use '&lt;Entity&gt;NotFound' instead of 'Get&lt;Entity&gt;Failed' or '&lt;Entity&gt;Missing'|&#x2713;|&#x2713;|
|MiKo_1061|The name of 'Try' method's [out] parameter should be specific|&#x2713;|&#x2713;|
|MiKo_1062|'Can/Has/Contains' methods, properties or fields shall consist of only a few words|&#x2713;|\-|
|MiKo_1063|Do not use abbreviations in names|&#x2713;|&#x2713;|
|MiKo_1064|Parameter names reflect their meaning and not their type|&#x2713;|\-|
|MiKo_1065|Operator parameters should be named according the .NET Framework Design Guidelines for operator overloads|&#x2713;|&#x2713;|
|MiKo_1066|Constructor parameters that are assigned to a property should be named after the property|&#x2713;|&#x2713;|
|MiKo_1067|Methods should not contain 'Perform' in their names|&#x2713;|&#x2713;|
|MiKo_1068|Workflow methods should be named 'CanRun' or 'Run'|&#x2713;|\-|
|MiKo_1069|Property names reflect their meaning and not their type|&#x2713;|\-|
|MiKo_1070|Local collection variables shall use plural name|&#x2713;|&#x2713;|
|MiKo_1071|Local boolean variables should be named as statements and not as questions|&#x2713;|\-|
|MiKo_1072|Boolean properties or methods should be named as statements and not as questions|&#x2713;|\-|
|MiKo_1073|Boolean fields should be named as statements and not as questions|&#x2713;|\-|
|MiKo_1074|Objects used to lock on should be suffixed with 'Lock'|&#x2713;|\-|
|MiKo_1075|Non-'System.EventArgs' types should not be suffixed with 'EventArgs'|&#x2713;|&#x2713;|
|MiKo_1076|Prism event types should be suffixed with 'Event'|&#x2713;|&#x2713;|
|MiKo_1077|Enum members should not be suffixed with 'Enum'|&#x2713;|&#x2713;|
|MiKo_1078|Builder method names should start with 'Build'|&#x2713;|&#x2713;|
|MiKo_1080|Names should contain numbers instead of their spellings|&#x2713;|\-|
|MiKo_1081|Methods should not be suffixed with a number|&#x2713;|&#x2713;|
|MiKo_1082|Properties should not be suffixed with a number if their types have number suffixes|&#x2713;|&#x2713;|
|MiKo_1083|Fields should not be suffixed with a number if their types have number suffixes|&#x2713;|&#x2713;|
|MiKo_1084|Variables should not be suffixed with a number if their types have number suffixes|&#x2713;|&#x2713;|
|MiKo_1085|Parameters should not be suffixed with a number|&#x2713;|&#x2713;|
|MiKo_1086|Methods should not be named using numbers as slang|&#x2713;|\-|
|MiKo_1087|Name constructor parameters after their counterparts in the base class|&#x2713;|&#x2713;|
|MiKo_1088|Singleton instances should be named 'Instance'|&#x2713;|\-|
|MiKo_1090|Parameters should not be suffixed with specific types|&#x2713;|&#x2713;|
|MiKo_1091|Variables should not be suffixed with specific types|&#x2713;|&#x2713;|
|MiKo_1092|'Ability' Types should not be suffixed with redundant information|&#x2713;|&#x2713;|
|MiKo_1093|Do not use the suffix 'Object' or 'Struct'|&#x2713;|&#x2713;|
|MiKo_1094|Do not suffix types with passive namespace names|&#x2713;|\-|
|MiKo_1095|Do not use 'Delete' and 'Remove' both in names and documentation|&#x2713;|\-|
|MiKo_1096|Names should use 'Failed' instead of 'NotSuccessful'|&#x2713;|\-|
|MiKo_1097|Parameter names should not follow the naming scheme for fields|&#x2713;|&#x2713;|
|MiKo_1098|Type names should reflect the business interface(s) they implement|&#x2713;|\-|
|MiKo_1099|Matching parameters on method overloads should have identical names|&#x2713;|&#x2713;|
|MiKo_1100|Test classes should start with the name of the type under test|&#x2713;|\-|
|MiKo_1101|Test classes should end with 'Tests'|&#x2713;|&#x2713;|
|MiKo_1102|Test methods should not contain 'Test' in their names|&#x2713;|&#x2713;|
|MiKo_1103|Test initialization methods should be named 'PrepareTest'|&#x2713;|&#x2713;|
|MiKo_1104|Test cleanup methods should be named 'CleanupTest'|&#x2713;|&#x2713;|
|MiKo_1105|One-time test initialization methods should be named 'PrepareTestEnvironment'|&#x2713;|&#x2713;|
|MiKo_1106|One-time test cleanup methods should be named 'CleanupTestEnvironment'|&#x2713;|&#x2713;|
|MiKo_1107|Test methods should not be in Pascal-casing|&#x2713;|&#x2713;|
|MiKo_1108|Do not name variables, parameters, fields and properties 'Mock', 'Stub', 'Fake' or 'Shim'|&#x2713;|&#x2713;|
|MiKo_1109|Prefix testable types with 'Testable' instead of using the 'Ut' suffix|&#x2713;|&#x2713;|
|MiKo_1110|Test methods with parameters should be suffixed with underscore|&#x2713;|&#x2713;|
|MiKo_1111|Test methods without parameters should not be suffixed with underscore|&#x2713;|&#x2713;|
|MiKo_1112|Do not name test data 'arbitrary'|&#x2713;|&#x2713;|
|MiKo_1113|Test methods should not be named according BDD style|&#x2713;|\-|
|MiKo_1114|Test methods should not be named 'HappyPath' or 'BadPath'|&#x2713;|\-|
|MiKo_1115|Test methods should be named in a fluent way|&#x2713;|&#x2713;|
|MiKo_1200|Name exceptions in catch blocks consistently|&#x2713;|&#x2713;|
|MiKo_1201|Name exceptions as parameters consistently|&#x2713;|&#x2713;|
|MiKo_1300|Unimportant identifiers in lambda statements should be named '_'|&#x2713;|&#x2713;|
|MiKo_1400|Namespace names should be in plural|&#x2713;|\-|
|MiKo_1401|Namespaces should not contain technical language names|&#x2713;|\-|
|MiKo_1402|Namespaces should not be named after WPF-specific design patterns|&#x2713;|\-|
|MiKo_1403|Namespaces should not be named after any of their parent namespaces|&#x2713;|\-|
|MiKo_1404|Namespaces should not contain unspecific names|&#x2713;|\-|
|MiKo_1405|Namespaces should not contain 'Lib'|&#x2713;|\-|
|MiKo_1406|Value converters should be placed in 'Converters' namespace|&#x2713;|\-|
|MiKo_1407|Test namespaces should not contain 'Test'|&#x2713;|\-|
|MiKo_1408|Extension methods should be placed in same namespace as the extended types|&#x2713;|\-|
|MiKo_1409|Do not prefix or suffix namespaces with underscores|&#x2713;|\-|

### Documentation
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_2000|Documentation should be valid XML|&#x2713;|&#x2713;|
|MiKo_2001|Events should be documented properly|&#x2713;|&#x2713;|
|MiKo_2002|EventArgs should be documented properly|&#x2713;|&#x2713;|
|MiKo_2003|Documentation of event handlers should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2004|Documentation of event handler parameter names should follow .NET Framework Design Guidelines for event handlers|&#x2713;|&#x2713;|
|MiKo_2005|Textual references to EventArgs should be documented properly|&#x2713;|\-|
|MiKo_2006|Routed events should be documented as done by the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2010|Sealed classes should document being sealed|&#x2713;|&#x2713;|
|MiKo_2011|Unsealed classes should not lie about sealing|&#x2713;|&#x2713;|
|MiKo_2012|&lt;summary&gt; documentation should describe the type's responsibility|&#x2713;|&#x2713;|
|MiKo_2013|&lt;summary&gt; documentation of Enums should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2014|Dispose methods should be documented as done by the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2015|Documentation should use 'raise' or 'throw' instead of 'fire'|&#x2713;|&#x2713;|
|MiKo_2016|Documentation for asynchronous methods should start with specific phrase|&#x2713;|&#x2713;|
|MiKo_2017|Dependency properties should be documented as done by the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2018|Documentation should not use the ambiguous terms 'Check' or 'Test'|&#x2713;|&#x2713;|
|MiKo_2019|&lt;summary&gt; documentation should start with a third person singular verb (for example "Provides ")|&#x2713;|\-|
|MiKo_2020|Inherited documentation should be used with &lt;inheritdoc /&gt; marker|&#x2713;|&#x2713;|
|MiKo_2021|Documentation of parameter should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2022|Documentation of [out] parameters should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2023|Documentation of Boolean parameters should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2024|Documentation of Enum parameters should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2025|Documentation of 'CancellationToken' parameters should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2026|Used parameters should not be documented to be unused|&#x2713;|\-|
|MiKo_2027|Serialization constructor parameters shall be documented with a specific phrase|&#x2713;|&#x2713;|
|MiKo_2028|Documentation of parameter should not just contain the name of the parameter|&#x2713;|\-|
|MiKo_2029|&lt;inheritdoc&gt; documentation should not use a 'cref' to itself|&#x2713;|&#x2713;|
|MiKo_2030|Documentation of return value should have a default starting phrase|&#x2713;|\-|
|MiKo_2031|Documentation of Task return value should have a specific (starting) phrase|&#x2713;|&#x2713;|
|MiKo_2032|Documentation of Boolean return value should have a specific phrase|&#x2713;|&#x2713;|
|MiKo_2033|Documentation of String return value should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2034|Documentation of Enum return value should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2035|Documentation of collection return value should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2036|Documentation of Boolean or Enum property shall describe the default value|&#x2713;|&#x2713;|
|MiKo_2037|&lt;summary&gt; documentation of command properties should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2038|&lt;summary&gt; documentation of command should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2039|&lt;summary&gt; documentation of classes that contain extension methods should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2040|&lt;see langword="..."/&gt; should be used instead of &lt;c&gt;...&lt;/c&gt;|&#x2713;|&#x2713;|
|MiKo_2041|&lt;summary&gt; documentation should not contain other documentation tags|&#x2713;|&#x2713;|
|MiKo_2042|Documentation should use '&lt;para/&gt;' XML tags instead of '&lt;br/&gt;' HTML tags|&#x2713;|&#x2713;|
|MiKo_2043|&lt;summary&gt; documentation of custom delegates should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2044|Documentation references method parameters correctly|&#x2713;|&#x2713;|
|MiKo_2045|&lt;summary&gt; documentation should not reference parameters|&#x2713;|&#x2713;|
|MiKo_2046|Documentation should reference type parameters correctly|&#x2713;|&#x2713;|
|MiKo_2047|&lt;summary&gt; documentation of Attributes should have a default starting phrase|&#x2713;|\-|
|MiKo_2048|&lt;summary&gt; documentation of value converters should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2049|Documentation should be more explicit and not use 'will be'|&#x2713;|&#x2713;|
|MiKo_2050|Exceptions should be documented following the .NET Framework|&#x2713;|&#x2713;|
|MiKo_2051|Thrown Exceptions should be documented as kind of a condition (such as '&lt;paramref name="xyz"/&gt; is &lt;c&gt;42&lt;/c&gt;')|&#x2713;|&#x2713;|
|MiKo_2052|Throwing of ArgumentNullException should be documented using a default phrase|&#x2713;|&#x2713;|
|MiKo_2053|Throwing of ArgumentNullException should be documented only for reference type parameters|&#x2713;|\-|
|MiKo_2054|Throwing of ArgumentException should be documented using a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2055|Throwing of ArgumentOutOfRangeException should be documented using a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2056|Throwing of ObjectDisposedException should be documented using a default ending phrase|&#x2713;|&#x2713;|
|MiKo_2057|Types that are not disposable shall not throw an ObjectDisposedException|&#x2713;|&#x2713;|
|MiKo_2059|Multiple documentation of same exception should be consolidated into one|&#x2713;|&#x2713;|
|MiKo_2060|Factories should be documented in an uniform way|&#x2713;|&#x2713;|
|MiKo_2070|&lt;summary&gt; documentation should not start with 'Returns'|&#x2713;|&#x2713;|
|MiKo_2071|&lt;summary&gt; documentation for methods that return Enum types should not contain phrase for boolean type|&#x2713;|\-|
|MiKo_2072|&lt;summary&gt; documentation should not start with 'Try'|&#x2713;|&#x2713;|
|MiKo_2073|&lt;summary&gt; documentation of 'Contains' methods should start with 'Determines whether '|&#x2713;|&#x2713;|
|MiKo_2074|Documentation of parameter of 'Contains' method should have a default ending phrase|&#x2713;|&#x2713;|
|MiKo_2075|Documentation should use the term 'callback' instead of 'action', 'func' or 'function'|&#x2713;|&#x2713;|
|MiKo_2076|Documentation should document default values of optional parameters|&#x2713;|&#x2713;|
|MiKo_2077|&lt;summary&gt; documentation should not contain &lt;code&gt;|&#x2713;|\-|
|MiKo_2078|&lt;code&gt; documentation should not contain XML tags|&#x2713;|\-|
|MiKo_2079|&lt;summary&gt; documentation of properties should not have obvious text|&#x2713;|&#x2713;|
|MiKo_2080|&lt;summary&gt; documentation of fields should have a default starting phrase|&#x2713;|&#x2713;|
|MiKo_2081|&lt;summary&gt; documentation of public-visible read-only fields should have a default ending phrase|&#x2713;|&#x2713;|
|MiKo_2082|&lt;summary&gt; documentation of Enum members should not start with default starting phrases of Enum &lt;summary&gt; documentation|&#x2713;|&#x2713;|
|MiKo_2090|Documentation for equality operator shall have default phrase|&#x2713;|&#x2713;|
|MiKo_2091|Documentation for inequality operator shall have default phrase|&#x2713;|&#x2713;|
|MiKo_2100|&lt;example&gt; documentation should start with descriptive default phrase|&#x2713;|&#x2713;|
|MiKo_2101|&lt;example&gt; documentation should show code example in &lt;code&gt; tags|&#x2713;|&#x2713;|
|MiKo_2200|Use a capitalized letter to start the comment|&#x2713;|&#x2713;|
|MiKo_2201|Use a capitalized letter to start the sentences in the comment|&#x2713;|\-|
|MiKo_2202|Documentation should use the term 'identifier' instead of 'id'|&#x2713;|&#x2713;|
|MiKo_2203|Documentation should use the term 'unique identifier' instead of 'guid'|&#x2713;|&#x2713;|
|MiKo_2204|Documentation should use &lt;list&gt; for enumerations|&#x2713;|&#x2713;|
|MiKo_2205|Documentation should use &lt;note&gt; for important information|&#x2713;|\-|
|MiKo_2206|Documentation should not use the term 'flag'|&#x2713;|\-|
|MiKo_2207|&lt;summary&gt; documentation shall be short|&#x2713;|\-|
|MiKo_2208|Documentation should not use the term 'an instance of'|&#x2713;|&#x2713;|
|MiKo_2209|Do not use double periods in documentation|&#x2713;|&#x2713;|
|MiKo_2210|Documentation should use the term 'information' instead of 'info'|&#x2713;|&#x2713;|
|MiKo_2211|Enum members should not have &lt;remarks&gt; sections|&#x2713;|&#x2713;|
|MiKo_2212|Documentation should use the phrase 'failed' instead of 'was not successful'|&#x2713;|&#x2713;|
|MiKo_2213|Documentation should not use the contraction "n't"|&#x2713;|&#x2713;|
|MiKo_2214|Documentation should not contain empty lines|&#x2713;|&#x2713;|
|MiKo_2215|Sentences in documentation shall be short|&#x2713;|\-|
|MiKo_2216|Use &lt;paramref&gt; instead of &lt;param&gt; to reference parameters|&#x2713;|&#x2713;|
|MiKo_2217|&lt;list&gt; documentation is done properly|&#x2713;|&#x2713;|
|MiKo_2218|Documentation should use shorter terms instead of longer term 'used to/in/by'|&#x2713;|&#x2713;|
|MiKo_2219|Do not use question or explamation marks in documentation|&#x2713;|\-|
|MiKo_2220|Documentation should use 'to seek' instead of 'to look for', 'to inspect for' or 'to test for'|&#x2713;|&#x2713;|
|MiKo_2221|Documentation should not use empty XML tags|&#x2713;|\-|
|MiKo_2222|Documentation should use the term 'identification' instead of 'ident'|&#x2713;|&#x2713;|
|MiKo_2223|Documentation links references via &lt;see cref="..."/&gt;|&#x2713;|\-|
|MiKo_2224|Documentation should have XML tags and texts placed on separate lines|&#x2713;|&#x2713;|
|MiKo_2225|Code marked with &lt;c&gt; tags should be placed on single line|&#x2713;|&#x2713;|
|MiKo_2226|Documentation should explain the 'Why' and not the 'That'|&#x2713;|\-|
|MiKo_2227|Documentation should not contain ReSharper suppressions|&#x2713;|\-|
|MiKo_2228|Documentation should use positive wording instead of negative|&#x2713;|\-|
|MiKo_2229|Documentation should not contain left-over XML fragments|&#x2713;|&#x2713;|
|MiKo_2231|Documentation of overridden 'GetHashCode()' methods shall use '&lt;inheritdoc /&gt;' marker|&#x2713;|&#x2713;|
|MiKo_2232|&lt;summary&gt; documentation should not be empty|&#x2713;|&#x2713;|
|MiKo_2300|Comments should explain the 'Why' and not the 'How'|&#x2713;|\-|
|MiKo_2301|Do not use obvious comments in AAA-Tests|&#x2713;|&#x2713;|
|MiKo_2302|Do not keep code that is commented out|&#x2713;|\-|
|MiKo_2303|Do not end comments with a period|&#x2713;|&#x2713;|
|MiKo_2304|Do not formulate comments as questions|&#x2713;|\-|
|MiKo_2305|Do not use double periods in comments|&#x2713;|&#x2713;|
|MiKo_2306|End comments with a period|\-|\-|
|MiKo_2307|Comments should use the phrase 'failed' instead of 'was not successful'|&#x2713;|&#x2713;|
|MiKo_2308|Do not place comment on single line before closing brace but after code|&#x2713;|&#x2713;|
|MiKo_2309|Comments should not use the contraction "n't"|&#x2713;|&#x2713;|
|MiKo_2310|Comments should explain the 'Why' and not the 'That'|&#x2713;|\-|
|MiKo_2311|Do not use separator comments|&#x2713;|&#x2713;|

### Maintainability
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_3000|Do not use empty regions|&#x2713;|\-|
|MiKo_3001|Custom delegates should not be used|&#x2713;|\-|
|MiKo_3002|Classes should not have too many dependencies|&#x2713;|\-|
|MiKo_3003|Events should follow .NET Framework Design Guidelines for events|&#x2713;|\-|
|MiKo_3004|Property setters of EventArgs shall be private|&#x2713;|\-|
|MiKo_3005|Methods named 'Try' should follow the Trier-Doer-Pattern|&#x2713;|\-|
|MiKo_3006|'CancellationToken' parameter should be last method parameter|&#x2713;|\-|
|MiKo_3007|Do not use LINQ method and declarative query syntax in same method|&#x2713;|\-|
|MiKo_3008|Method should not return collections that can be changed from outside|&#x2713;|\-|
|MiKo_3009|Commands should invoke only named methods and no lambda expressions|&#x2713;|\-|
|MiKo_3010|Do not create or throw reserved exception types|&#x2713;|\-|
|MiKo_3011|Thrown ArgumentExceptions (or its subtypes) shall provide the correct parameter name|&#x2713;|&#x2713;|
|MiKo_3012|Thrown ArgumentOutOfRangeExceptions (or its subtypes) shall provide the actual value that causes the exception to be thrown|&#x2713;|&#x2713;|
|MiKo_3013|The 'default' clause in 'switch' statements should throw an ArgumentOutOfRangeException (or subtype), but no ArgumentException|&#x2713;|&#x2713;|
|MiKo_3014|InvalidOperationException, NotImplementedException and NotSupportedException should have a reason as message|&#x2713;|&#x2713;|
|MiKo_3015|Throw InvalidOperationExceptions (instead of ArgumentExceptions or its subtypes) to indicate inappropriate states of parameterless methods|&#x2713;|&#x2713;|
|MiKo_3016|Do not throw ArgumentNullException for inappropriate states of property return values|&#x2713;|&#x2713;|
|MiKo_3017|Do not swallow exceptions when throwing new exceptions|&#x2713;|&#x2713;|
|MiKo_3018|Throw ObjectDisposedExceptions on publicly visible methods of disposable types|&#x2713;|\-|
|MiKo_3020|Use 'Task.CompletedTask' instead of 'Task.FromResult'|&#x2713;|&#x2713;|
|MiKo_3021|Do not use 'Task.Run' in the implementation|&#x2713;|\-|
|MiKo_3022|Do not return Task&lt;IEnumerable&gt; or Task&lt;IEnumerable&lt;T&gt;&gt;|&#x2713;|\-|
|MiKo_3023|Do not use 'CancellationTokenSource' as parameter|&#x2713;|\-|
|MiKo_3024|Do not use the [ref] keyword on reference parameters|&#x2713;|\-|
|MiKo_3025|Do not re-assign method parameters|&#x2713;|\-|
|MiKo_3026|Unused parameters should be removed|&#x2713;|\-|
|MiKo_3027|Parameters should not be marked to be reserved for future usage|&#x2713;|\-|
|MiKo_3028|Do not assign null to lambda parameters|&#x2713;|\-|
|MiKo_3029|Event registrations should not cause memory leaks|&#x2713;|\-|
|MiKo_3030|Methods should follow the Law of Demeter|\-|\-|
|MiKo_3031|ICloneable.Clone() should not be implemented|&#x2713;|\-|
|MiKo_3032|Use 'nameof' instead of Cinch for names of properties for created 'PropertyChangedEventArgs' instances|&#x2713;|&#x2713;|
|MiKo_3033|Use 'nameof' for names of properties for created 'PropertyChangingEventArgs' and 'PropertyChangedEventArgs' instances|&#x2713;|&#x2713;|
|MiKo_3034|PropertyChanged event raiser shall use [CallerMemberName] attribute|&#x2713;|&#x2713;|
|MiKo_3035|Do not invoke 'WaitOne' methods without timeouts|&#x2713;|\-|
|MiKo_3036|Prefer to use 'TimeSpan' factory methods instead of constructors|&#x2713;|&#x2713;|
|MiKo_3037|Do not use magic numbers for timeouts|&#x2713;|\-|
|MiKo_3038|Do not use magic numbers|&#x2713;|\-|
|MiKo_3039|Properties should not use Linq or yield|&#x2713;|\-|
|MiKo_3040|Do not use Booleans unless you are absolutely sure that you will never ever need more than 2 values|&#x2713;|\-|
|MiKo_3041|EventArgs shall not use delegates|&#x2713;|\-|
|MiKo_3042|EventArgs shall not implement interfaces|&#x2713;|\-|
|MiKo_3043|Use 'nameof' for WeakEventManager event (de-)registrations|&#x2713;|&#x2713;|
|MiKo_3044|Use 'nameof' to compare property names of 'PropertyChangingEventArgs' and 'PropertyChangedEventArgs'|&#x2713;|&#x2713;|
|MiKo_3045|Use 'nameof' for EventManager event registrations|&#x2713;|&#x2713;|
|MiKo_3046|Use 'nameof' for property names of property raising methods|&#x2713;|&#x2713;|
|MiKo_3047|Use 'nameof' for applied [ContentProperty] attributes|&#x2713;|&#x2713;|
|MiKo_3048|ValueConverters shall have the [ValueConversion] attribute applied|&#x2713;|\-|
|MiKo_3049|Enum members shall have the [Description] attribute applied|&#x2713;|\-|
|MiKo_3050|DependencyProperty fields should be 'public static readonly'|&#x2713;|&#x2713;|
|MiKo_3051|DependencyProperty fields should be properly registered|&#x2713;|&#x2713;|
|MiKo_3052|DependencyPropertyKey fields should be non-public 'static readonly'|&#x2713;|&#x2713;|
|MiKo_3053|DependencyPropertyKey fields should be properly registered|&#x2713;|\-|
|MiKo_3054|A read-only DependencyProperty should have an exposed DependencyProperty identifier|&#x2713;|&#x2713;|
|MiKo_3055|ViewModels should implement INotifyPropertyChanged|&#x2713;|\-|
|MiKo_3060|Debug.Assert or Trace.Assert shall not be used|&#x2713;|&#x2713;|
|MiKo_3061|Loggers shall use a proper log category|&#x2713;|\-|
|MiKo_3062|End log messages for exceptions with a colon|&#x2713;|&#x2713;|
|MiKo_3063|End non-exceptional log messages with a dot|&#x2713;|&#x2713;|
|MiKo_3064|Log messages should not use the contraction "n't"|&#x2713;|&#x2713;|
|MiKo_3065|Microsoft Logging calls should not use interpolated strings|&#x2713;|&#x2713;|
|MiKo_3070|Do not return null for an IEnumerable|&#x2713;|\-|
|MiKo_3071|Do not return null for a Task|&#x2713;|\-|
|MiKo_3072|Non-private methods should not return 'List&lt;&gt;' or 'Dictionary&lt;&gt;'|&#x2713;|\-|
|MiKo_3073|Do not leave objects partially initialized|&#x2713;|\-|
|MiKo_3074|Do not define 'ref' or 'out' parameters on constructors|&#x2713;|\-|
|MiKo_3075|Internal and private types should be either static or sealed unless derivation from them is required|&#x2713;|&#x2713;|
|MiKo_3076|Do not initialize static member with static member below or in other type part|&#x2713;|\-|
|MiKo_3077|Properties that return an Enum should have a default value|&#x2713;|&#x2713;|
|MiKo_3078|Enum members should have a default value|&#x2713;|&#x2713;|
|MiKo_3079|HResults should be written in hexadecimal|&#x2713;|&#x2713;|
|MiKo_3080|Use 'switch ... return' instead of 'switch ... break' when assigning variables|&#x2713;|\-|
|MiKo_3081|Prefer pattern matching over a logical NOT condition|&#x2713;|&#x2713;|
|MiKo_3082|Prefer pattern matching over a logical comparison with 'true' or 'false'|&#x2713;|&#x2713;|
|MiKo_3083|Prefer pattern matching for null checks|&#x2713;|&#x2713;|
|MiKo_3084|Do not place constants on the left side for comparisons|&#x2713;|&#x2713;|
|MiKo_3085|Conditional statements should be short|&#x2713;|\-|
|MiKo_3086|Do not nest conditional statements|&#x2713;|\-|
|MiKo_3087|Do not use negative complex conditions|&#x2713;|\-|
|MiKo_3088|Prefer pattern matching for not-null checks|&#x2713;|&#x2713;|
|MiKo_3089|Do not use simple constant property patterns as conditions of 'if' statements|&#x2713;|&#x2713;|
|MiKo_3090|Do not throw exceptions in finally blocks|&#x2713;|\-|
|MiKo_3091|Do not raise events in finally blocks|&#x2713;|\-|
|MiKo_3092|Do not raise events in locks|&#x2713;|\-|
|MiKo_3093|Do not invoke delegates inside locks|&#x2713;|\-|
|MiKo_3094|Do not invoke methods or properties of parameters inside locks|&#x2713;|\-|
|MiKo_3095|Code blocks should not be empty|&#x2713;|\-|
|MiKo_3096|Use dictionaries instead of large switch statements|&#x2713;|\-|
|MiKo_3097|Do not cast to type and return object|&#x2713;|\-|
|MiKo_3098|Justifications of suppressed messages shall explain|&#x2713;|\-|
|MiKo_3099|Do not compare enum values with null|&#x2713;|&#x2713;|
|MiKo_3100|Test classes and types under test belong in same namespace|&#x2713;|\-|
|MiKo_3101|Test classes should contain tests|&#x2713;|\-|
|MiKo_3102|Test methods should not contain conditional statements (such as 'if', 'switch', etc.)|&#x2713;|\-|
|MiKo_3103|Test methods should not use 'Guid.NewGuid()'|&#x2713;|&#x2713;|
|MiKo_3104|Use NUnit's [Combinatorial] attribute properly|&#x2713;|&#x2713;|
|MiKo_3105|Test methods should use NUnit's fluent Assert approach|&#x2713;|&#x2713;|
|MiKo_3106|Assertions should not use equality or comparison operators|&#x2713;|\-|
|MiKo_3107|Moq Mock condition matchers should be used on mocks only|&#x2713;|&#x2713;|
|MiKo_3108|Test methods should use assertions|&#x2713;|\-|
|MiKo_3109|Multiple assertions shall use assertion messages|&#x2713;|&#x2713;|
|MiKo_3110|Assertions should not use 'Count' or 'Length'|&#x2713;|&#x2713;|
|MiKo_3111|Assertions should use 'Is.Zero' instead of 'Is.EqualTo(0)'|&#x2713;|&#x2713;|
|MiKo_3112|Assertions should use 'Is.Empty' instead of 'Has.Count.Zero'|&#x2713;|&#x2713;|
|MiKo_3113|Do not use FluentAssertions|&#x2713;|&#x2713;|
|MiKo_3114|Use 'Mock.Of&lt;T&gt;()' instead of 'new Mock&lt;T&gt;().Object'|&#x2713;|&#x2713;|
|MiKo_3115|Test methods should contain code|&#x2713;|\-|
|MiKo_3116|Test initialization methods should contain code|&#x2713;|\-|
|MiKo_3117|Test cleanup methods should contain code|&#x2713;|\-|
|MiKo_3118|Test methods should not use ambiguous Linq calls|&#x2713;|\-|
|MiKo_3119|Test methods should not simply return completed task|&#x2713;|&#x2713;|
|MiKo_3120|Moq mocks should use values instead of 'It.Is&lt;&gt;(...)' condition matcher to verify exact values|&#x2713;|&#x2713;|
|MiKo_3121|Tests should test concrete implementations and no interfaces|&#x2713;|\-|
|MiKo_3122|Test methods should not use more than 2 parameters|&#x2713;|\-|
|MiKo_3201|If statements can be inverted in short methods|&#x2713;|&#x2713;|
|MiKo_3202|Use positive conditions when returning in all paths|&#x2713;|&#x2713;|
|MiKo_3203|If-continue statements can be inverted when followed by single line|&#x2713;|&#x2713;|
|MiKo_3204|Negative If statements can be inverted when they have an else clause|&#x2713;|&#x2713;|
|MiKo_3210|Only the longest overloads should be virtual or abstract|&#x2713;|\-|
|MiKo_3211|Public types should not have finalizers|&#x2713;|\-|
|MiKo_3212|Do not confuse developers by providing other Dispose methods|&#x2713;|\-|
|MiKo_3213|Parameterless Dispose method follows Basic Dispose pattern|&#x2713;|\-|
|MiKo_3214|Interfaces do not contain 'Begin/End' or 'Enter/Exit' scope-defining methods|&#x2713;|\-|
|MiKo_3215|Callbacks should be 'Func&lt;T, bool&gt;' instead of 'Predicate&lt;bool&gt;'|&#x2713;|&#x2713;|
|MiKo_3216|Static fields with initializers should be read-only|&#x2713;|&#x2713;|
|MiKo_3217|Do not use generic types that have other generic types as type arguments|&#x2713;|\-|
|MiKo_3218|Do not define extension methods in unexpected places|&#x2713;|\-|
|MiKo_3219|Public members should not be 'virtual'|&#x2713;|\-|
|MiKo_3220|Logical '&amp;&amp;' or '&#124;&#124;' conditions using 'true' or 'false' should be simplified|&#x2713;|&#x2713;|
|MiKo_3221|GetHashCode overrides should use 'HashCode.Combine'|&#x2713;|&#x2713;|
|MiKo_3222|String comparisons can be simplified|&#x2713;|&#x2713;|
|MiKo_3223|Reference comparisons can be simplified|&#x2713;|&#x2713;|
|MiKo_3224|Value comparisons can be simplified|&#x2713;|&#x2713;|
|MiKo_3225|Redundant comparisons can be simplified|&#x2713;|&#x2713;|
|MiKo_3301|Favor lambda expression bodies instead of parenthesized lambda expression blocks for single statements|&#x2713;|&#x2713;|
|MiKo_3302|Favor simple lambda expression bodies instead of parenthesized lambda expression bodies for single parameters|&#x2713;|&#x2713;|
|MiKo_3401|Namespace hierarchies should not be too deep|&#x2713;|\-|
|MiKo_3501|Do not suppress nullable warnings on Null-conditional operators|&#x2713;|&#x2713;|
|MiKo_3502|Do not suppress nullable warnings on Linq calls|&#x2713;|&#x2713;|

### Ordering
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_4001|Methods with same name should be ordered based on the number of their parameters|&#x2713;|&#x2713;|
|MiKo_4002|Methods with same name and accessibility should be placed side-by-side|&#x2713;|&#x2713;|
|MiKo_4003|Dispose methods should be placed directly after constructors and finalizers|&#x2713;|&#x2713;|
|MiKo_4004|Dispose methods should be placed before all other methods of the same accessibility|&#x2713;|&#x2713;|
|MiKo_4005|The interface that gives a type its name should be placed directly after the type's declaration|&#x2713;|&#x2713;|
|MiKo_4007|Operators should be placed before methods|&#x2713;|&#x2713;|
|MiKo_4008|GetHashCode methods should be placed directly after Equals methods|&#x2713;|&#x2713;|
|MiKo_4101|Test initialization methods should be ordered directly after One-Time methods|&#x2713;|&#x2713;|
|MiKo_4102|Test cleanup methods should be ordered after test initialization methods and before test methods|&#x2713;|&#x2713;|
|MiKo_4103|One-Time test initialization methods should be ordered before all other methods|&#x2713;|&#x2713;|
|MiKo_4104|One-Time test cleanup methods should be ordered directly after One-Time test initialization methods|&#x2713;|&#x2713;|

### Performance
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_5001|'Debug' and 'DebugFormat' methods should be invoked only after 'IsDebugEnabled'|&#x2713;|&#x2713;|
|MiKo_5002|'xxxFormat' methods should be invoked with multiple arguments only|&#x2713;|&#x2713;|
|MiKo_5003|Correct Log methods should be invoked for exceptions|&#x2713;|\-|
|MiKo_5010|Do not use 'object.Equals()' on value types|&#x2713;|&#x2713;|
|MiKo_5011|Do not concatenate strings with += operator|&#x2713;|\-|
|MiKo_5012|Do not use 'yield return' for recursively defined structures|&#x2713;|\-|
|MiKo_5013|Do not create empty arrays|&#x2713;|&#x2713;|
|MiKo_5014|Do not create empty lists if the return value is read-only|&#x2713;|&#x2713;|
|MiKo_5015|Do not intern string literals|&#x2713;|&#x2713;|
|MiKo_5016|Use a HashSet for lookups in 'List.RemoveAll'|&#x2713;|\-|
|MiKo_5017|Fields or variables assigned with string literals should be constant|&#x2713;|&#x2713;|
|MiKo_5018|Value comparisons should be performed before reference comparisons|&#x2713;|&#x2713;|

### Spacing
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_6001|Log statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6002|Assertion statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6003|Local variable statements should be preceded by blank lines|&#x2713;|&#x2713;|
|MiKo_6004|Variable assignment statements should be preceded by blank lines|&#x2713;|&#x2713;|
|MiKo_6005|Return statements should be preceded by blank lines|&#x2713;|&#x2713;|
|MiKo_6006|Awaited statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6007|Test statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6008|Using directives should be preceded by blank lines|&#x2713;|&#x2713;|
|MiKo_6009|Try statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6010|If statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6011|Lock statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6012|foreach loops should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6013|for loops should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6014|while loops should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6015|do/while loops should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6016|using statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6017|switch statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6018|break statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6019|continue statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6020|throw statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6021|ArgumentNullException.ThrowIfNull statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6022|ArgumentException.ThrowIfNullOrEmpty statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6023|ArgumentOutOfRangeException.ThrowIf statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6024|ObjectDisposedException.ThrowIf statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6030|Open braces of initializers should be placed directly below the corresponding type definition|&#x2713;|&#x2713;|
|MiKo_6031|Question and colon tokens of ternary operators should be placed directly below the corresponding condition|&#x2713;|&#x2713;|
|MiKo_6032|Multi-line parameters are positioned outdented at end of method|&#x2713;|&#x2713;|
|MiKo_6033|Braces of blocks below case sections should be placed directly below the corresponding case keyword|&#x2713;|&#x2713;|
|MiKo_6034|Dots should be placed on same line(s) as invoked members|&#x2713;|&#x2713;|
|MiKo_6035|Open parenthesis should be placed on same line(s) as invoked methods|&#x2713;|&#x2713;|
|MiKo_6036|Lambda blocks should be placed directly below the corresponding arrow(s)|&#x2713;|&#x2713;|
|MiKo_6037|Single arguments should be placed on same line(s) as invoked methods|&#x2713;|&#x2713;|
|MiKo_6038|Casts should be placed on same line(s)|&#x2713;|&#x2713;|
|MiKo_6039|Return values should be placed on same line(s) as return keywords|&#x2713;|&#x2713;|
|MiKo_6040|Consecutive invocations spaning multiple lines should be aligned by their dots|&#x2713;|&#x2713;|
|MiKo_6041|Assignments should be placed on same line(s)|&#x2713;|&#x2713;|
|MiKo_6042|'new' keywords should be placed on same line(s) as the types|&#x2713;|&#x2713;|
|MiKo_6043|Expression bodies of lambdas should be placed on same line as lambda itself when fitting|&#x2713;|&#x2713;|
|MiKo_6044|Operators such as '&amp;&amp;' or '&#124;&#124;' should be placed on same line(s) as their (right) operands|&#x2713;|&#x2713;|
|MiKo_6045|Comparisons using operators such as '==' or '!=' should be placed on same line(s)|&#x2713;|&#x2713;|
|MiKo_6046|Calculations using operators such as '+' or '%' should be placed on same line(s)|&#x2713;|&#x2713;|
|MiKo_6047|Braces of switch expressions should be placed directly below the corresponding switch keyword|&#x2713;|&#x2713;|
|MiKo_6048|Logical conditions should be placed on a single line|&#x2713;|&#x2713;|
|MiKo_6049|Event (un-)registrations should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6050|Multi-line arguments are positioned outdented at end of method call|&#x2713;|&#x2713;|
|MiKo_6051|Colon of constructor call shall be placed on same line as constructor call|&#x2713;|&#x2713;|
|MiKo_6052|Colon of list of base types shall be placed on same line as first base type|&#x2713;|&#x2713;|
|MiKo_6053|Single-line arguments shall be placed on single line|&#x2713;|&#x2713;|
|MiKo_6054|Lambda arrows shall be placed on same line as the parameter(s) of the lambda|&#x2713;|&#x2713;|
|MiKo_6055|Assignment statements should be surrounded by blank lines|&#x2713;|&#x2713;|
|MiKo_6056|Brackets of collection expressions should be placed directly at the same place collection initializer braces would be positioned|&#x2713;|&#x2713;|
|MiKo_6057|Type parameter constraint clauses should be aligned vertically|&#x2713;|&#x2713;|
|MiKo_6058|Type parameter constraint clauses should be indented below parameter list|&#x2713;|&#x2713;|
|MiKo_6059|Multi-line conditions are positioned outdented below associated calls|&#x2713;|&#x2713;|
|MiKo_6060|Switch case labels should be placed on same line|&#x2713;|&#x2713;|
|MiKo_6061|Switch expression arms should be placed on same line|&#x2713;|&#x2713;|
|MiKo_6070|Console statements should be surrounded by blank lines|&#x2713;|&#x2713;|
