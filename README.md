# MiKo-Analyzers
Provides analyzers that are based on the .NET Compiler Platform (Roslyn).

## Build / Project status
[![Maintenance](https://img.shields.io/maintenance/yes/2020.svg)](https://github.com/RalfKoban/MiKo-Analyzers)
[![Build status](https://ci.appveyor.com/api/projects/status/qanrqn7r4q9frr9m/branch/master?svg=true)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/branch/master)
[![codecov](https://codecov.io/gh/RalfKoban/MiKo-Analyzers/branch/master/graph/badge.svg)](https://codecov.io/gh/RalfKoban/MiKo-Analyzers)
[![Coverity Scan Build Status](https://img.shields.io/coverity/scan/18917.svg)](https://scan.coverity.com/projects/ralfkoban-miko-analyzers)

[![Build history](https://buildstats.info/appveyor/chart/RalfKoban/miko-analyzers)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/history)

## Available Rules
The following tables list all the 281 rules that are currently provided by the analyzer.

### Metrics
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_0001|Method is too big.|&#x2713;|\-|
|MiKo_0002|Method is too complex.|&#x2713;|\-|
|MiKo_0003|Type is too big.|&#x2713;|\-|
|MiKo_0004|Methods has too many parameters.|&#x2713;|\-|

### Naming
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_1001|'System.EventArgs' parameters on methods should be named properly.|&#x2713;|\-|
|MiKo_1002|Parameter names do not follow .NET Framework Guidelines for event handlers.|&#x2713;|&#x2713;|
|MiKo_1003|Event handling method name does not follow the .NET Framework Best Practices.|&#x2713;|&#x2713;|
|MiKo_1004|Events should not contain term 'Event' in their names.|&#x2713;|&#x2713;|
|MiKo_1005|'System.EventArgs' variables should be named properly.|&#x2713;|\-|
|MiKo_1006|Events shall use 'EventHandler&lt;T&gt;' with properly named 'EventArgs' as 'T'.|&#x2713;|\-|
|MiKo_1007|Events and 'EventArgs' types shall be located in the same namespace.|&#x2713;|\-|
|MiKo_1008|Parameter names do not follow .NET Framework Guidelines for DependencyProperty event handlers.|&#x2713;|&#x2713;|
|MiKo_1010|Methods should not contain 'CanExecute' or 'Execute' in their names.|&#x2713;|\-|
|MiKo_1011|Methods should not contain 'Do' in their names.|&#x2713;|\-|
|MiKo_1012|Methods should not be named 'Fire'.|&#x2713;|&#x2713;|
|MiKo_1013|Methods should not be named 'Notify' or 'OnNotify'.|&#x2713;|\-|
|MiKo_1014|Methods should not be named 'Check'.|&#x2713;|\-|
|MiKo_1015|Methods should not be named 'Init'.|&#x2713;|&#x2713;|
|MiKo_1016|Factory methods should be named 'Create'.|&#x2713;|\-|
|MiKo_1017|Methods should not be prefixed with 'Get' or 'Set' if followed by 'Is', 'Can' or 'Has'.|&#x2713;|\-|
|MiKo_1018|Methods should not be suffixed with noun of a verb.|&#x2713;|\-|
|MiKo_1019|'Clear' and 'Remove' methods should be named based on their number of parameters.|&#x2713;|&#x2713;|
|MiKo_1020|Type names should be limited in length.|&#x2713;|\-|
|MiKo_1021|Method names should be limited in length.|&#x2713;|\-|
|MiKo_1022|Parameter names should be limited in length.|&#x2713;|\-|
|MiKo_1023|Field names should be limited in length.|&#x2713;|\-|
|MiKo_1024|Property names should be limited in length.|&#x2713;|\-|
|MiKo_1025|Event names should be limited in length.|&#x2713;|\-|
|MiKo_1026|Variable names should be limited in length.|&#x2713;|\-|
|MiKo_1027|Variable names in loops should be limited in length.|&#x2713;|\-|
|MiKo_1030|Types should not have an 'Abstract' or 'Base' marker to indicate that they are base types.|&#x2713;|&#x2713;|
|MiKo_1031|Entity types should not use a 'Model' suffix.|&#x2713;|\-|
|MiKo_1032|Methods dealing with entities should not use a 'Model' marker.|&#x2713;|\-|
|MiKo_1033|Parameters representing entities should not use a 'Model' suffix.|&#x2713;|\-|
|MiKo_1034|Fields representing entities should not use a 'Model' suffix.|&#x2713;|\-|
|MiKo_1035|Properties dealing with entities should not use a 'Model' marker.|&#x2713;|\-|
|MiKo_1036|Events dealing with entities should not use a 'Model' marker.|&#x2713;|\-|
|MiKo_1037|Types should not be suffixed with 'Enum'.|&#x2713;|&#x2713;|
|MiKo_1038|Classes that contain extension methods should end with same suffix.|&#x2713;|&#x2713;|
|MiKo_1039|The 'this' parameter of extension methods should have a default name.|&#x2713;|\-|
|MiKo_1040|Parameters should not be suffixed with implementation details.|&#x2713;|\-|
|MiKo_1041|Fields should not be suffixed with implementation details.|&#x2713;|\-|
|MiKo_1042|'CancellationToken' parameters should have specific name.|&#x2713;|&#x2713;|
|MiKo_1043|'CancellationToken' variables should have specific name.|&#x2713;|&#x2713;|
|MiKo_1044|Commands should be suffixed with 'Command'.|&#x2713;|\-|
|MiKo_1045|Methods that are invoked by commands should not be suffixed with 'Command'.|&#x2713;|\-|
|MiKo_1046|Asynchronous methods should follow the Task-based Asynchronous Pattern (TAP).|&#x2713;|&#x2713;|
|MiKo_1047|Methods not following the Task-based Asynchronous Pattern (TAP) should not lie about being asynchronous.|&#x2713;|&#x2713;|
|MiKo_1048|To ease maintenance, the names of classes that are value converters should end with the same suffix.|&#x2713;|&#x2713;|
|MiKo_1049|Do not use requirement terms such as 'Shall', 'Should', 'Must' or 'Need' for names.|&#x2713;|\-|
|MiKo_1050|Return values should have descriptive names.|&#x2713;|\-|
|MiKo_1051|Do not suffix parameters with delegate types.|&#x2713;|\-|
|MiKo_1052|Do not suffix variables with delegate types.|&#x2713;|&#x2713;|
|MiKo_1053|Do not suffix fields with delegate types.|&#x2713;|\-|
|MiKo_1054|Do not name types 'Helper' or 'Utility'.|&#x2713;|\-|
|MiKo_1055|Dependency properties should be suffixed with 'Property' (as in the .NET Framework).|&#x2713;|\-|
|MiKo_1056|Dependency properties should be prefixed with property names (as in the .NET Framework).|&#x2713;|\-|
|MiKo_1057|Dependency property keys should be suffixed with 'Key' (as in the .NET Framework).|&#x2713;|\-|
|MiKo_1058|Dependency property keys should be prefixed with property names (as in the .NET Framework).|&#x2713;|\-|
|MiKo_1059|Do not name types 'Impl' or 'Implementation'.|&#x2713;|\-|
|MiKo_1060|Use '&lt;Entity&gt;NotFound' instead of 'Get&lt;Entity&gt;Failed' or '&lt;Entity&gt;Missing'.|&#x2713;|\-|
|MiKo_1061|The name of 'Try' method's [out] parameter should be specific.|&#x2713;|&#x2713;|
|MiKo_1062|'Can/Has/Contains' methods, properties or fields shall consist of only a few words.|&#x2713;|\-|
|MiKo_1063|Do not use abbreviations in names.|&#x2713;|\-|
|MiKo_1064|Parameter names reflect their meaning and not their type.|&#x2713;|\-|
|MiKo_1065|Parameter names do not follow .NET Framework Guidelines for operator overloads.|&#x2713;|&#x2713;|
|MiKo_1066|Parameters should not be suffixed with a number.|&#x2713;|\-|
|MiKo_1067|Methods should not contain 'Perform' in their names.|&#x2713;|\-|
|MiKo_1068|Workflow methods should be named 'CanRun' or 'Run'.|&#x2713;|\-|
|MiKo_1069|Property names reflect their meaning and not their type.|&#x2713;|\-|
|MiKo_1070|Local collection variables shall use plural name.|&#x2713;|\-|
|MiKo_1071|Local boolean variables should be named as statements and not as questions.|&#x2713;|\-|
|MiKo_1072|Boolean properties or methods should be named as statements and not as questions.|&#x2713;|\-|
|MiKo_1073|Boolean fields should be named as statements and not as questions.|&#x2713;|\-|
|MiKo_1080|Names should contain numbers instead of their spellings.|&#x2713;|\-|
|MiKo_1081|Methods should not be suffixed with a number.|&#x2713;|\-|
|MiKo_1082|Properties should not be suffixed with a number if their types have number suffixes.|&#x2713;|\-|
|MiKo_1083|Fields should not be suffixed with a number if their types have number suffixes.|&#x2713;|\-|
|MiKo_1084|Variables should not be suffixed with a number if their types have number suffixes.|&#x2713;|\-|
|MiKo_1085|Methods should not be named using numbers as slang.|&#x2713;|\-|
|MiKo_1090|Parameters should not be suffixed with specific types.|&#x2713;|\-|
|MiKo_1091|Variables should not be suffixed with specific types.|&#x2713;|\-|
|MiKo_1092|'Ability' Types should not be suffixed with redundant information.|&#x2713;|\-|
|MiKo_1093|Do not use the suffix 'Object' or 'Struct'.|&#x2713;|&#x2713;|
|MiKo_1094|Do not suffix types with passive namespace names.|&#x2713;|\-|
|MiKo_1100|Test classes should start with the name of the type under test.|&#x2713;|\-|
|MiKo_1101|Test classes should end with 'Tests'.|&#x2713;|&#x2713;|
|MiKo_1102|Test methods should not contain 'Test'.|&#x2713;|\-|
|MiKo_1103|Test initialization methods should be named 'PrepareTest'.|&#x2713;|&#x2713;|
|MiKo_1104|Test cleanup methods should be named 'CleanupTest'.|&#x2713;|&#x2713;|
|MiKo_1105|One-time test initialization methods should be named 'PrepareTestEnvironment'.|&#x2713;|&#x2713;|
|MiKo_1106|One-time test cleanup methods should be named 'CleanupTestEnvironment'.|&#x2713;|&#x2713;|
|MiKo_1107|Test methods should not be in Pascal-casing.|&#x2713;|\-|
|MiKo_1108|Do not name variables, parameters, fields and properties 'Mock' or 'Stub'.|&#x2713;|\-|
|MiKo_1109|Prefix testable types with 'Testable' instead of using the 'Ut' suffix.|&#x2713;|&#x2713;|
|MiKo_1110|Test methods with parameters should be suffixed with underscore.|&#x2713;|&#x2713;|
|MiKo_1111|Test methods should be named in a fluent way.|&#x2713;|\-|
|MiKo_1112|Do not name test data 'arbitrary'.|&#x2713;|&#x2713;|
|MiKo_1200|Name exceptions in catch blocks consistently.|&#x2713;|&#x2713;|
|MiKo_1201|Name exceptions as parameters consistently.|&#x2713;|\-|
|MiKo_1300|Unimportant identifiers in lambda statements should be named '_'.|&#x2713;|&#x2713;|
|MiKo_1400|Namespace names should be in plural.|&#x2713;|\-|
|MiKo_1401|Namespaces should not contain technical language names.|&#x2713;|\-|
|MiKo_1402|Namespaces should not be named after WPF specific design patterns.|&#x2713;|\-|
|MiKo_1403|Namespaces should not be named after any of their parent namespaces.|&#x2713;|\-|
|MiKo_1404|Namespaces should not contain unspecific names.|&#x2713;|\-|
|MiKo_1405|Namespaces should not contain 'Lib'.|&#x2713;|\-|
|MiKo_1406|Value converters should be placed in 'Converters' namespace.|&#x2713;|\-|
|MiKo_1407|Test namespaces should not contain 'Test'.|&#x2713;|\-|
|MiKo_1408|Extension methods should be placed in same namespace as the extended types.|&#x2713;|\-|

### Documentation
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_2000|Documentation should be valid XML.|&#x2713;|\-|
|MiKo_2001|Events should be documented properly.|&#x2713;|\-|
|MiKo_2002|EventArgs should be documented properly.|&#x2713;|\-|
|MiKo_2003|Documentation of event handlers should have a default starting phrase.|&#x2713;|\-|
|MiKo_2004|Documentation of parameter name does not follow .NET Framework Guidelines for event handlers.|&#x2713;|\-|
|MiKo_2005|Textual references to EventArgs should be documented properly.|&#x2713;|\-|
|MiKo_2006|Routed events should be documented as done by the .NET Framework.|&#x2713;|\-|
|MiKo_2010|Sealed classes should document being sealed.|&#x2713;|\-|
|MiKo_2011|Unsealed classes should not lie about sealing.|&#x2713;|\-|
|MiKo_2012|&lt;summary&gt; documentation should describe its responsibility.|&#x2713;|\-|
|MiKo_2013|&lt;summary&gt; documentation of Enums should have a default starting phrase.|&#x2713;|\-|
|MiKo_2014|Dispose methods should be documented as done by the .NET Framework.|&#x2713;|\-|
|MiKo_2015|Documentation should use 'raise' or 'throw' instead of 'fire'.|&#x2713;|\-|
|MiKo_2016|Documentation for asynchronous methods should start with specific phrase.|&#x2713;|\-|
|MiKo_2017|Dependency properties should be documented as done by the .NET Framework.|&#x2713;|\-|
|MiKo_2018|Documentation should not use the ambiguous term 'Check'.|&#x2713;|\-|
|MiKo_2019|&lt;summary&gt; documentation should start with a third person singular verb (for example "Provides ").|&#x2713;|\-|
|MiKo_2020|Inherited documentation should be used with &lt;inheritdoc /&gt; marker.|&#x2713;|\-|
|MiKo_2021|Documentation of parameter should have a default starting phrase.|&#x2713;|\-|
|MiKo_2022|Documentation of [out] parameters should have a default starting phrase.|&#x2713;|\-|
|MiKo_2023|Documentation of Boolean parameters should have a default starting phrase.|&#x2713;|\-|
|MiKo_2024|Documentation of Enum parameters should have a default starting phrase.|&#x2713;|\-|
|MiKo_2025|Documentation of 'CancellationToken' parameters should have a default starting phrase.|&#x2713;|\-|
|MiKo_2026|Used parameters should not be documented to be unused.|&#x2713;|\-|
|MiKo_2027|Serialization constructor parameters shall be documented with a specific phrase.|&#x2713;|\-|
|MiKo_2028|Documentation of parameter should not just contain the name of the parameter.|&#x2713;|\-|
|MiKo_2029|&lt;inheritdoc&gt; documentation should not use a 'cref' to itself.|&#x2713;|\-|
|MiKo_2030|Documentation of return value should have a default starting phrase.|&#x2713;|\-|
|MiKo_2031|Documentation of Task return value should have a specific (starting) phrase.|&#x2713;|\-|
|MiKo_2032|Documentation of Boolean return value should have a specific phrase.|&#x2713;|\-|
|MiKo_2033|Documentation of String return value should have a default starting phrase.|&#x2713;|\-|
|MiKo_2034|Documentation of Enum return value should have a default starting phrase.|&#x2713;|\-|
|MiKo_2035|Documentation of collection return value should have a default starting phrase.|&#x2713;|\-|
|MiKo_2036|Documentation of Boolean or Enum property shall describe the default value.|&#x2713;|\-|
|MiKo_2037|&lt;summary&gt; documentation of command properties should have a default starting phrase.|&#x2713;|\-|
|MiKo_2038|&lt;summary&gt; documentation of command should have a default starting phrase.|&#x2713;|\-|
|MiKo_2039|&lt;summary&gt; documentation of classes that contain extension methods should have a default starting phrase.|&#x2713;|\-|
|MiKo_2040|&lt;see langword="..."/&gt; should be used instead of &lt;c&gt;...&lt;/c&gt;.|&#x2713;|\-|
|MiKo_2041|&lt;summary&gt; documentation should not contain other documentation tags.|&#x2713;|\-|
|MiKo_2042|Documentation should use '&lt;para/&gt;' XML tags instead of '&lt;br/&gt;' HTML tags.|&#x2713;|\-|
|MiKo_2043|&lt;summary&gt; documentation of custom delegates should have a default starting phrase.|&#x2713;|\-|
|MiKo_2044|Documentation references method parameters correctly.|&#x2713;|\-|
|MiKo_2045|&lt;summary&gt; documentation should not reference parameters.|&#x2713;|\-|
|MiKo_2046|Documentation should reference type parameters correctly.|&#x2713;|\-|
|MiKo_2047|&lt;summary&gt; documentation of Attributes should have a  default starting phrase.|&#x2713;|\-|
|MiKo_2048|&lt;summary&gt; documentation of value converters should have a  default starting phrase.|&#x2713;|\-|
|MiKo_2049|Documentation should be more explicit and not use 'will be'.|&#x2713;|\-|
|MiKo_2050|Exceptions should be documented following the .NET Framework.|&#x2713;|\-|
|MiKo_2051|Thrown Exceptions should be documented as kind of a condition (such as '&lt;paramref name="xyz"/&gt; is &lt;c&gt;42&lt;/c&gt;').|&#x2713;|\-|
|MiKo_2052|Throwing of ArgumentNullException should be documented using a default phrase.|&#x2713;|\-|
|MiKo_2053|Throwing of ArgumentNullException should be documented only for reference type parameters.|&#x2713;|\-|
|MiKo_2054|Throwing of ArgumentException should be documented using a default starting phrase.|&#x2713;|\-|
|MiKo_2055|Throwing of ArgumentOutOfRangeException should be documented using a default starting phrase.|&#x2713;|\-|
|MiKo_2056|Throwing of ObjectDisposedException should be documented using a default ending phrase.|&#x2713;|\-|
|MiKo_2057|Types that are not disposable shall not throw an ObjectDisposedException.|&#x2713;|\-|
|MiKo_2060|Factories should be documented in a uniform way.|&#x2713;|\-|
|MiKo_2070|&lt;summary&gt; documentation should not start with 'Returns'.|&#x2713;|\-|
|MiKo_2071|&lt;summary&gt; documentation for methods that return Enum types should not contain phrase for boolean type.|&#x2713;|\-|
|MiKo_2072|&lt;summary&gt; documentation should not start with 'Try'.|&#x2713;|\-|
|MiKo_2073|&lt;summary&gt; documentation of 'Contains' methods should start with 'Determines '.|&#x2713;|\-|
|MiKo_2074|Documentation of parameter of 'Contains' method should have a default ending phrase.|&#x2713;|\-|
|MiKo_2080|&lt;summary&gt; documentation of fields should have a default starting phrase.|&#x2713;|\-|
|MiKo_2081|&lt;summary&gt; documentation of public-visible read-only fields should have a default ending phrase.|&#x2713;|\-|
|MiKo_2090|Documentation for equality operator shall have default phrase.|&#x2713;|\-|
|MiKo_2091|Documentation for inequality operator shall have default phrase.|&#x2713;|\-|
|MiKo_2100|&lt;example&gt; documentation should start with descriptive default phrase.|&#x2713;|\-|
|MiKo_2101|&lt;example&gt; documentation should show code example in &lt;code&gt; tags.|&#x2713;|\-|
|MiKo_2200|Use a capitalized letter to start the comment.|&#x2713;|\-|
|MiKo_2201|Use a capitalized letter to start the sentences in the comment.|&#x2713;|\-|
|MiKo_2202|Documentation should use the term 'identifier' instead of 'id'.|&#x2713;|\-|
|MiKo_2203|Documentation should use the term 'unique identifier' instead of 'guid'.|&#x2713;|\-|
|MiKo_2204|Documentation should use &lt;list&gt; for enumerations.|&#x2713;|\-|
|MiKo_2205|Documentation should use &lt;note&gt; for important information.|&#x2713;|\-|
|MiKo_2206|Documentation should not use the term 'flag'.|&#x2713;|\-|
|MiKo_2207|&lt;summary&gt; documentation shall be short.|&#x2713;|\-|
|MiKo_2208|Documentation should not use the term 'an instance of'.|&#x2713;|\-|
|MiKo_2209|Do not use double periods in documentation.|&#x2713;|\-|
|MiKo_2210|Documentation should use the term 'information' instead of 'info'.|&#x2713;|\-|
|MiKo_2211|Enum members should not have &lt;remarks&gt; sections.|&#x2713;|\-|
|MiKo_2212|Documentation should use the phrase 'failed' instead of the phrase 'was not sucessful'.|&#x2713;|\-|
|MiKo_2300|Comments should explain the 'Why' and not the 'How'.|&#x2713;|\-|
|MiKo_2301|Do not use obvious comments in AAA-Tests.|&#x2713;|\-|
|MiKo_2302|Do not keep code that is commented out.|&#x2713;|\-|
|MiKo_2303|Do not end comments with a period.|&#x2713;|\-|
|MiKo_2304|Do not formulate comments as questions.|&#x2713;|\-|
|MiKo_2305|Do not use double periods in comments.|&#x2713;|\-|
|MiKo_2306|Do end comments with a period.|&#x2713;|\-|
|MiKo_2307|Comments should use the phrase 'failed' instead of the phrase 'was not sucessful'.|&#x2713;|\-|

### Maintainability
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_3000|Do not use empty regions.|&#x2713;|\-|
|MiKo_3001|Custom delegates should not be used.|&#x2713;|\-|
|MiKo_3002|Classes should not have too many dependencies.|&#x2713;|\-|
|MiKo_3003|Events should follow .NET Framework Guidelines for events.|&#x2713;|\-|
|MiKo_3004|Property setters of EventArgs shall be private.|&#x2713;|\-|
|MiKo_3005|Methods named 'Try' should follow the Trier-Doer-Pattern.|&#x2713;|\-|
|MiKo_3006|'CancellationToken' parameter should be last method parameter.|&#x2713;|\-|
|MiKo_3007|Do not use LINQ method and declarative query syntax in same method.|&#x2713;|\-|
|MiKo_3008|Method should not return collections that can be changed from outside.|&#x2713;|\-|
|MiKo_3009|Commands should invoke only named methods and no lambda expressions.|&#x2713;|\-|
|MiKo_3010|Do not create or throw reserved exception types.|&#x2713;|\-|
|MiKo_3011|Thrown ArgumentExceptions (or its subtypes) shall provide the correct parameter name.|&#x2713;|\-|
|MiKo_3012|Thrown ArgumentOutOfRangeExceptions (or its subtypes) shall provide the actual value that causes the exception to be thrown.|&#x2713;|\-|
|MiKo_3013|The 'default' clause in 'switch' statements should throw an ArgumentOutOfRangeException (or subtype), but no ArgumentException.|&#x2713;|\-|
|MiKo_3014|InvalidOperationException, NotImplementedException and NotSupportedException should have a reason as message.|&#x2713;|\-|
|MiKo_3015|Parameterless methods should throw InvalidOperationExceptions (instead of ArgumentExceptions or its subtypes) to indicate inappropriate states.|&#x2713;|\-|
|MiKo_3020|Use 'Task.CompletedTask' instead of 'Task.FromResult'.|&#x2713;|\-|
|MiKo_3021|Do not use 'Task.Run' in the implementation.|&#x2713;|\-|
|MiKo_3022|Do not return Task&lt;IEnumerable&gt; or Task&lt;IEnumerable&lt;T&gt;&gt;.|&#x2713;|\-|
|MiKo_3023|Do not use 'CancellationTokenSource' as parameter.|&#x2713;|\-|
|MiKo_3024|Do not use the [ref] keyword on reference parameters.|&#x2713;|\-|
|MiKo_3025|Do not re-assign method parameters.|&#x2713;|\-|
|MiKo_3026|Unused parameters should be removed.|&#x2713;|\-|
|MiKo_3027|Parameters should not be marked to be reserved for future usage.|&#x2713;|\-|
|MiKo_3028|Do not assign null to lambda parameters.|&#x2713;|\-|
|MiKo_3031|ICloneable.Clone() should not be implemented.|&#x2713;|\-|
|MiKo_3032|Property names for created 'PropertyChangedEventArgs' instances shall be provided via 'nameof' operator instead of Cinch.|&#x2713;|\-|
|MiKo_3033|Property names for created 'PropertyChangingEventArgs' and 'PropertyChangedEventArgs' instances shall be provided via 'nameof' operator.|&#x2713;|\-|
|MiKo_3034|PropertyChanged event raiser shall use [CallerMemberName] attribute.|&#x2713;|\-|
|MiKo_3035|Do not invoke 'WaitOne' methods without timeouts.|&#x2713;|\-|
|MiKo_3036|Prefer to use 'TimeSpan' factory methods instead of constructors.|&#x2713;|\-|
|MiKo_3037|Do not use magic numbers for timeouts.|&#x2713;|\-|
|MiKo_3038|Properties should not use Linq or yield.|&#x2713;|\-|
|MiKo_3040|Do not use Booleans unless you are absolutely sure there will never be a need for more than 2 values.|&#x2713;|\-|
|MiKo_3041|EventArgs shall not use delegates.|&#x2713;|\-|
|MiKo_3042|EventArgs shall not implement interfaces.|&#x2713;|\-|
|MiKo_3047|Applied [ContentProperty] attributes shall use 'nameof'.|&#x2713;|\-|
|MiKo_3048|ValueConverters shall have the [ValueConversion] attribute applied.|&#x2713;|\-|
|MiKo_3049|Enum members shall have the [Description] attribute applied.|&#x2713;|\-|
|MiKo_3050|DependencyProperty fields should be 'public static readonly'.|&#x2713;|\-|
|MiKo_3051|DependencyProperty fields should be properly registered.|&#x2713;|\-|
|MiKo_3052|DependencyPropertyKey fields should be non-public 'static readonly'.|&#x2713;|\-|
|MiKo_3053|DependencyPropertyKey fields should be properly registered.|&#x2713;|\-|
|MiKo_3054|A read-only DependencyProperty should have an exposed DependencyProperty identifier.|&#x2713;|\-|
|MiKo_3060|Debug.Assert or Trace.Assert shall not be used.|&#x2713;|\-|
|MiKo_3061|Loggers shall use a proper log category.|&#x2713;|\-|
|MiKo_3070|Methods that return IEnumerable shall never return null.|&#x2713;|\-|
|MiKo_3071|Methods that return Task shall never return null.|&#x2713;|\-|
|MiKo_3072|Non-private methods should not return 'List&lt;&gt;' or 'Dictionary&lt;&gt;'.|&#x2713;|\-|
|MiKo_3073|Do not leave objects partially initialized.|&#x2713;|\-|
|MiKo_3081|Pattern matching is preferred over a logical NOT condition.|&#x2713;|\-|
|MiKo_3082|Pattern matching is preferred over a logical comparison with 'true' or 'false'.|&#x2713;|\-|
|MiKo_3083|Pattern matching is preferred for null checks.|&#x2713;|\-|
|MiKo_3084|Do not place constants on the left side for comparisons.|&#x2713;|\-|
|MiKo_3085|Conditional statements should be short.|&#x2713;|\-|
|MiKo_3086|Do not nest conditional statements.|&#x2713;|\-|
|MiKo_3090|Do not throw exceptions in finally blocks.|&#x2713;|\-|
|MiKo_3091|Do not raise events in finally blocks.|&#x2713;|\-|
|MiKo_3092|Do not raise events in locks.|&#x2713;|\-|
|MiKo_3093|Do not invoke delegates inside locks.|&#x2713;|\-|
|MiKo_3100|Test classes and types under test belong in same namespace.|&#x2713;|\-|
|MiKo_3101|Test classes should contain tests.|&#x2713;|\-|
|MiKo_3102|Test methods should not contain conditional statements such as 'if', 'switch', etc.|&#x2713;|\-|
|MiKo_3103|Test methods should not use 'Guid.NewGuid()'.|&#x2713;|\-|
|MiKo_3104|Use NUnit's [Combinatorial] attribute properly.|&#x2713;|\-|
|MiKo_3105|Test methods should use NUnit's fluent Assert approach.|&#x2713;|\-|
|MiKo_3106|Do not use equality or comparison operators in assertions.|&#x2713;|\-|
|MiKo_3107|Moq Mock condition matchers should be used on mocks only.|&#x2713;|\-|
|MiKo_3401|Namespace hierarchies should not be too deep.|&#x2713;|\-|

### Ordering
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_4001|Methods with same name should be ordered based on the number of their parameters.|&#x2713;|\-|
|MiKo_4002|Methods with same name and accessibility should be placed side-by-side.|&#x2713;|\-|
|MiKo_4003|Dispose methods should be placed directly after constructors and finalizers.|&#x2713;|\-|
|MiKo_4004|The interface that gives a type its name should be placed directly after the type's declaration.|&#x2713;|\-|
|MiKo_4101|Test initialization methods should be ordered directly after One-Time methods.|&#x2713;|\-|
|MiKo_4102|Test cleanup methods should be ordered before test methods.|&#x2713;|\-|
|MiKo_4103|One-Time test initialization methods should be ordered before all other methods.|&#x2713;|\-|
|MiKo_4104|One-Time test cleanup methods should be ordered directly after One-Time test initialization methods.|&#x2713;|\-|

### Performance
|ID|Title|Enabled by default|CodeFix available|
|:-|:----|:----------------:|:---------------:|
|MiKo_5001|'Debug' and 'DebugFormat' methods should be invoked only after 'IsDebugEnabled'.|&#x2713;|\-|
|MiKo_5002|'xxxFormat' methods should be invoked with multiple arguments only.|&#x2713;|\-|
|MiKo_5003|Correct Log methods should be invoked for exceptions.|&#x2713;|\-|
|MiKo_5010|Do not use 'object.Equals()' on value types.|&#x2713;|\-|
|MiKo_5011|Do not concatenate strings with += operator.|&#x2713;|\-|
|MiKo_5012|Do not use 'yield return' for recursively defined structures.|&#x2713;|\-|
