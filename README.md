# MiKo-Analyzers
Provides analyzers that are based on the .NET Compiler Platform (Roslyn).

## Build status
[![Build status](https://ci.appveyor.com/api/projects/status/qanrqn7r4q9frr9m/branch/master?svg=true)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/branch/master)
[![codecov](https://codecov.io/gh/RalfKoban/MiKo-Analyzers/branch/master/graph/badge.svg)](https://codecov.io/gh/RalfKoban/MiKo-Analyzers)

[![Build history](https://buildstats.info/appveyor/chart/RalfKoban/miko-analyzers)](https://ci.appveyor.com/project/RalfKoban/miko-analyzers/history)

## Project status
[![Waffle.io - Columns and their card count](https://badge.waffle.io/RalfKoban/MiKo-Analyzers.svg?columns=all)](https://waffle.io/RalfKoban/MiKo-Analyzers) 

[![Maintenance](https://img.shields.io/maintenance/yes/2018.svg)]()

## Available Rules
The following tables list all the rules that are currently provided by the analyzer.

### Metrics
|ID|Title|Enabled by default|
|:-|:----|:----------------:|
|MiKo_0001|Method is too long.|:white_check_mark:|
|MiKo_0002|Method is too complex.|:white_check_mark:|
|MiKo_0003|Type is too long.|:white_check_mark:|
|MiKo_0004|Methods should not have too many parameters.|:white_check_mark:|

### Naming
|ID|Title|Enabled by default|
|:-|:----|:----------------:|
|MiKo_1001|'System.EventArgs' parameters on methods should be named properly.|:white_check_mark:|
|MiKo_1002|Parameter names do not follow .NET Framework Guidelines for event handlers.|:white_check_mark:|
|MiKo_1003|Name of event handler does not follow the .NET Framework Best Practices.|:white_check_mark:|
|MiKo_1004|Events should not contain term 'Event' in their names.|:white_check_mark:|
|MiKo_1005|'System.EventArgs' variables should be named properly.|:white_check_mark:|
|MiKo_1010|Methods should not contain 'Do' in their names.|:white_check_mark:|
|MiKo_1011|Methods should not contain 'CanExecute' or 'Execute' in their names.|:white_check_mark:|
|MiKo_1012|Methods should not be named 'Fire'.|:white_check_mark:|
|MiKo_1013|Methods should not be named 'Notify' or 'OnNotify'.|:white_check_mark:|
|MiKo_1014|Methods should not be named 'Check'.|:white_check_mark:|
|MiKo_1015|Methods should not be named 'Init'.|:white_check_mark:|
|MiKo_1016|Factory methods should be named 'Create'.|:white_check_mark:|
|MiKo_1017|Methods should not be prefixed with 'Get' or 'Set' if followed by 'Is', 'Can' or 'Has'.|:white_check_mark:|
|MiKo_1018|Methods should not be suffixed with noun of a verb.|:white_check_mark:|
|MiKo_1020|Type names should be limited in length.|:white_large_square:|
|MiKo_1021|Method names should be limited in length.|:white_large_square:|
|MiKo_1022|Parameter names should be limited in length.|:white_large_square:|
|MiKo_1023|Field names should be limited in length.|:white_large_square:|
|MiKo_1024|Property names should be limited in length.|:white_large_square:|
|MiKo_1025|Event names should be limited in length.|:white_large_square:|
|MiKo_1026|Variable names should be limited in length.|:white_large_square:|
|MiKo_1030|Types should not have a 'Base' marker to indicate that they are base types.|:white_check_mark:|
|MiKo_1031|Entity types should not use a 'Model' suffix.|:white_check_mark:|
|MiKo_1032|Methods dealing with entities should not use a 'Model' marker.|:white_check_mark:|
|MiKo_1033|Parameters representing entities should not use a 'Model' suffix.|:white_check_mark:|
|MiKo_1034|Fields representing entities should not use a 'Model' suffix.|:white_check_mark:|
|MiKo_1035|Properties dealing with entities should not use a 'Model' marker.|:white_check_mark:|
|MiKo_1036|Events dealing with entities should not use a 'Model' marker.|:white_check_mark:|
|MiKo_1037|Types should not be suffixed with 'Enum'.|:white_check_mark:|
|MiKo_1038|Classes that contain extension methods should end with same suffix.|:white_check_mark:|
|MiKo_1039|Extension methods 'this' parameter should have default name.|:white_check_mark:|
|MiKo_1040|Parameters should not be suffixed with implementation details.|:white_check_mark:|
|MiKo_1041|Fields should not be suffixed with implementation details.|:white_check_mark:|
|MiKo_1042|'CancellationToken' parameters should have specific name.|:white_check_mark:|
|MiKo_1043|'CancellationToken' variables should have specific name.|:white_check_mark:|
|MiKo_1044|Commands should be suffixed with 'Command'.|:white_check_mark:|
|MiKo_1045|Methods that are invoked by commands should not be suffixed with 'Command'.|:white_check_mark:|
|MiKo_1046|Asynchronous methods should follow the Task-based Asynchronous Pattern (TAP).|:white_check_mark:|
|MiKo_1047|Methods not following the Task-based Asynchronous Pattern (TAP) should not lie about being asynchronous.|:white_check_mark:|
|MiKo_1048|To ease maintenance, the names of classes that are value converters should end with the same suffix.|:white_check_mark:|
|MiKo_1049|Do not use requirement terms such as 'Shall', 'Should', 'Must' or 'Need' for names.|:white_check_mark:|
|MiKo_1050|Return values should have descriptive names.|:white_check_mark:|
|MiKo_1101|Test classes should end with 'Tests'.|:white_check_mark:|
|MiKo_1102|Test methods should not contain 'Test'.|:white_check_mark:|
|MiKo_1103|Test initialization methods should be named 'PrepareTest'.|:white_check_mark:|
|MiKo_1104|Test cleanup methods should be named 'CleanupTest'.|:white_check_mark:|
|MiKo_1105|Test methods should not be in Pascal-casing.|:white_check_mark:|
|MiKo_1106|Do not name variables or parameters 'Mock' or 'Stub'.|:white_check_mark:|
|MiKo_1200|Name exceptions in catch blocks consistently.|:white_check_mark:|
|MiKo_1201|Name exceptions as parameters consistently.|:white_check_mark:|
|MiKo_1300|Unimportant identifiers in lambda statements should be named '_'.|:white_check_mark:|

### Documentation
|ID|Title|Enabled by default|
|:-|:----|:----------------:|
|MiKo_2000|Documentation should be valid XML.|:white_check_mark:|
|MiKo_2001|Events should be documented properly.|:white_check_mark:|
|MiKo_2002|EventArgs should be documented properly.|:white_check_mark:|
|MiKo_2003|Documentation of event handlers should have a default starting phrase.|:white_check_mark:|
|MiKo_2004|Documentation of parameter name does not follow .NET Framework Guidelines for event handlers.|:white_check_mark:|
|MiKo_2010|Sealed classes should document being sealed.|:white_check_mark:|
|MiKo_2011|Unsealed classes should not lie about sealing.|:white_check_mark:|
|MiKo_2012|&lt;summary&gt; documentation should describe their responsibility.|:white_check_mark:|
|MiKo_2013|&lt;summary&gt; documentation of Enums should have a default starting phrase.|:white_check_mark:|
|MiKo_2014|Dispose methods should be documented in the same way as they are documented by the .NET Framework.|:white_check_mark:|
|MiKo_2015|Documentation should use 'raise' or 'throw' instead of 'fire'.|:white_check_mark:|
|MiKo_2016|Documentation for asynchronous methods should start with specific phrase.|:white_check_mark:|
|MiKo_2017|Dependency properties should be documented as by the .NET Framework.|:white_check_mark:|
|MiKo_2018|Documentation should not use the ambiguous term 'Check'.|:white_check_mark:|
|MiKo_2019|&lt;summary&gt; documentation should start with a third person singular verb (for example "Provides ").|:white_check_mark:|
|MiKo_2020|Inherited documentation should be used with &lt;inheritdoc /&gt; marker.|:white_check_mark:|
|MiKo_2021|Documentation of parameter should have a default starting phrase.|:white_check_mark:|
|MiKo_2022|Documentation of [out] parameters should have a default starting phrase.|:white_check_mark:|
|MiKo_2023|Documentation of Boolean parameters should have a default starting phrase.|:white_check_mark:|
|MiKo_2024|Documentation of Enum parameters should have a default starting phrase.|:white_check_mark:|
|MiKo_2025|Documentation of 'CancellationToken' parameters should have a default starting phrase.|:white_check_mark:|
|MiKo_2030|Documentation of return value should have a default starting phrase.|:white_check_mark:|
|MiKo_2031|Documentation of Task return value should have a specific (starting) phrase.|:white_check_mark:|
|MiKo_2032|Documentation of Boolean return value should have a specific phrase.|:white_check_mark:|
|MiKo_2033|Documentation of String return value should have a default starting phrase.|:white_check_mark:|
|MiKo_2034|Documentation of Enum return value should have a default starting phrase.|:white_check_mark:|
|MiKo_2035|Documentation of collection return value should have a default starting phrase.|:white_check_mark:|
|MiKo_2036|Documentation of Boolean or Enum property shall describe the default value.|:white_check_mark:|
|MiKo_2037|&lt;summary&gt; documentation of command properties should have a default starting phrase.|:white_check_mark:|
|MiKo_2038|&lt;summary&gt; documentation of command should have a default starting phrase.|:white_check_mark:|
|MiKo_2039|&lt;summary&gt; documentation of classes that contain extension methods should have a default starting phrase.|:white_check_mark:|
|MiKo_2040|&lt;see langword="..."/&gt; should be used instead of &lt;c&gt;...&lt;/c&gt;.|:white_check_mark:|
|MiKo_2041|&lt;summary&gt; documentation should not contain other documentation tags.|:white_check_mark:|
|MiKo_2042|Documentation should use '&lt;para/&gt;' XML tags instead of '&lt;br/&gt;' HTML tags.|:white_check_mark:|
|MiKo_2043|&lt;summary&gt; documentation of custom delegates should have a default starting phrase.|:white_check_mark:|
|MiKo_2044|Documentation references method parameters correctly.|:white_check_mark:|
|MiKo_2045|&lt;summary&gt; documentation of public-visible read-only fields should have a default ending phrase.|:white_check_mark:|
|MiKo_2046|&lt;summary&gt; documentation of Attributes should have a  default starting phrase.|:white_check_mark:|
|MiKo_2047|&lt;summary&gt; documentation of value converters should have a  default starting phrase.|:white_check_mark:|
|MiKo_2048|Documentation should be more explicit and not use 'will be'.|:white_check_mark:|
|MiKo_2050|Exceptions should be documented following the .NET Framework.|:white_check_mark:|
|MiKo_2051|Thrown Exceptions should be documented as kind of a condition (such as '&lt;paramref name="xyz"/&gt; is &lt;c&gt;42&lt;/c&gt;').|:white_check_mark:|
|MiKo_2052|Throwing of ArgumentNullException should be documented using a default phrase.|:white_check_mark:|
|MiKo_2053|Throwing of ArgumentNullException should be documented only for reference type parameters.|:white_check_mark:|
|MiKo_2054|Throwing of ArgumentException should be documented using a default starting phrase.|:white_check_mark:|
|MiKo_2055|Throwing of ArgumentOutOfRangeException should be documented using a default starting phrase.|:white_check_mark:|
|MiKo_2056|Throwing of ObjectDisposedException should be documented using a default ending phrase.|:white_check_mark:|
|MiKo_2057|Types that are not disposable shall not throw an ObjectDisposedException.|:white_check_mark:|
|MiKo_2060|Factories should be documented in a uniform way.|:white_check_mark:|
|MiKo_2100|&lt;example&gt; documentation should start with descriptive default phrase.|:white_check_mark:|
|MiKo_2200|Use a capitalized letter to start the comment.|:white_check_mark:|

### Maintainability
|ID|Title|Enabled by default|
|:-|:----|:----------------:|
|MiKo_3001|Custom delegates should not be used.|:white_check_mark:|
|MiKo_3002|Classes should not have too many dependencies.|:white_check_mark:|
|MiKo_3003|Events should follow .NET Framework Guidelines for events.|:white_check_mark:|
|MiKo_3004|Property setters of EventArgs shall be private.|:white_check_mark:|
|MiKo_3005|Methods named 'Try' should follow the Trier-Doer-Pattern.|:white_check_mark:|
|MiKo_3006|'CancellationToken' parameter should be last method parameter.|:white_check_mark:|
|MiKo_3007|Do not use LINQ method and declarative query syntax in same method.|:white_check_mark:|
|MiKo_3008|Method should not return collections that can be changed from outside.|:white_check_mark:|
|MiKo_3009|Commands should invoke only named methods and no lambda expressions.|:white_check_mark:|
|MiKo_3010|Do not create or throw reserved exception types.|:white_check_mark:|
|MiKo_3011|Thrown ArgumentExceptions (or its subtypes) shall provide the correct parameter name.|:white_check_mark:|
|MiKo_3012|Thrown ArgumentOutOfRangeExceptions (or its subtypes) shall provide the actual value that causes the exception to be thrown.|:white_check_mark:|
|MiKo_3013|The 'default' clause in 'switch' statements should throw an ArgumentOutOfRangeException (or subtype), but no ArgumentException.|:white_check_mark:|
|MiKo_3014|InvalidOperationException, NotImplementedException and NotSupportedException should have a reason as message.|:white_check_mark:|
|MiKo_3015|Parameterless methods should throw InvalidOperationExceptions (instead of ArgumentExceptions or its subtypes) to indicate inappropriate states.|:white_check_mark:|
|MiKo_3020|Use 'Task.CompletedTask' instead of 'Task.FromResult'.|:white_check_mark:|
|MiKo_3021|Do not use 'Task.Run' in the implementation.|:white_check_mark:|
|MiKo_3030|Do not use object.Equals() on value types.|:white_check_mark:|
|MiKo_3031|Do not concatenate strings with += operator.|:white_check_mark:|
|MiKo_3040|Do not use Booleans unless you are absolutely sure there will never be a need for more than two values.|:white_check_mark:|
|MiKo_3047|Value converters shall be located in 'Converters' namespace.|:white_check_mark:|
|MiKo_3101|Test classes should contain tests.|:white_check_mark:|
|MiKo_3102|Test methods should not contain conditional statements such as 'if', 'switch', etc.|:white_check_mark:|
|MiKo_3103|Test methods should not use 'Guid.NewGuid()'.|:white_check_mark:|
