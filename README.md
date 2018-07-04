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
The following table shows which rules are currently provided by the analyzer.

|Category|Id|Title|Enabled by default|
|:-------|:-|:----|:----------------:|
|Metrics|MiKo_0001|Method is too long.|True|
|Metrics|MiKo_0002|Method is too complex.|True|
|Metrics|MiKo_0003|Type is too long.|True|
|Metrics|MiKo_0004|Methods should not have too many parameters.|True|
|Naming|MiKo_1001|'System.EventArgs' parameters on methods should be named properly.|True|
|Naming|MiKo_1002|Parameter names do not follow .NET Framework Guidelines for event handlers.|True|
|Naming|MiKo_1003|Name of event handler does not follow the .NET Framework Best Practices.|True|
|Naming|MiKo_1004|Events should not contain term 'Event' in their names.|True|
|Naming|MiKo_1005|'System.EventArgs' variables should be named properly.|True|
|Naming|MiKo_1010|Methods should not contain 'Do' in their names.|True|
|Naming|MiKo_1011|Methods should not contain 'CanExecute' or 'Execute' in their names.|True|
|Naming|MiKo_1012|Methods should not be named 'Fire'.|True|
|Naming|MiKo_1013|Methods should not be named 'Notify' or 'OnNotify'.|True|
|Naming|MiKo_1014|Methods should not be named 'Check'.|True|
|Naming|MiKo_1015|Methods should not be named 'Init'.|True|
|Naming|MiKo_1016|Factory methods should be named 'Create'.|True|
|Naming|MiKo_1017|Methods should not be prefixed with 'Get' or 'Set' if followed by 'Is', 'Can' or 'Has'.|True|
|Naming|MiKo_1020|Type names should be limited in length.|False|
|Naming|MiKo_1021|Method names should be limited in length.|False|
|Naming|MiKo_1022|Parameter names should be limited in length.|False|
|Naming|MiKo_1023|Field names should be limited in length.|False|
|Naming|MiKo_1024|Property names should be limited in length.|False|
|Naming|MiKo_1025|Event names should be limited in length.|False|
|Naming|MiKo_1026|Variable names should be limited in length.|False|
|Naming|MiKo_1030|Types should not have a 'Base' marker to indicate that they are base types.|True|
|Naming|MiKo_1031|Entity types should not use a 'Model' suffix.|True|
|Naming|MiKo_1032|Methods dealing with entities should not use a 'Model' marker.|True|
|Naming|MiKo_1033|Parameters representing entities should not use a 'Model' suffix.|True|
|Naming|MiKo_1034|Fields representing entities should not use a 'Model' suffix.|True|
|Naming|MiKo_1035|Properties dealing with entities should not use a 'Model' marker.|True|
|Naming|MiKo_1036|Events dealing with entities should not use a 'Model' marker.|True|
|Naming|MiKo_1037|Types should not be suffixed with 'Enum'.|True|
|Naming|MiKo_1038|Classes that contain extension methods should end with same suffix.|True|
|Naming|MiKo_1039|Extension methods 'this' parameter should have default name.|True|
|Naming|MiKo_1040|Parameters should not be suffixed with implementation details.|True|
|Naming|MiKo_1041|Fields should not be suffixed with implementation details.|True|
|Naming|MiKo_1042|'CancellationToken' parameters should have specific name.|True|
|Naming|MiKo_1043|'CancellationToken' variables should have specific name.|True|
|Naming|MiKo_1044|Commands should be suffixed with 'Command'.|True|
|Naming|MiKo_1045|Methods that are invoked by commands should not be suffixed with 'Command'.|True|
|Naming|MiKo_1046|Asynchronous methods should follow the Task-based Asynchronous Pattern (TAP).|True|
|Naming|MiKo_1047|Methods not following the Task-based Asynchronous Pattern (TAP) should not lie about being asynchronous.|True|
|Naming|MiKo_1048|To ease maintenance, the names of classes that are value converters should end with the same suffix.|True|
|Naming|MiKo_1049|Do not use requirement terms such as 'Shall', 'Should', 'Must' or 'Need' for names.|True|
|Naming|MiKo_1101|Test classes should end with 'Tests'.|True|
|Naming|MiKo_1102|Test methods should not contain 'Test'.|True|
|Naming|MiKo_1103|Test initialization methods should be named 'PrepareTest'.|True|
|Naming|MiKo_1104|Test cleanup methods should be named 'CleanupTest'.|True|
|Naming|MiKo_1105|Do not name variables or parameters 'Mock' or 'Stub'.|True|
|Naming|MiKo_1200|Name exceptions in catch blocks consistently.|True|
|Naming|MiKo_1201|Name exceptions as parameters consistently.|True|
|Naming|MiKo_1300|Unimportant identifiers in lambda statements should be named '_'.|True|
|Documentation|MiKo_2000|Documentation should be valid XML.|True|
|Documentation|MiKo_2001|Events should be documented properly.|True|
|Documentation|MiKo_2002|EventArgs should be documented properly.|True|
|Documentation|MiKo_2003|Documentation of event handlers should have a default starting phrase.|True|
|Documentation|MiKo_2004|Documentation of parameter name does not follow .NET Framework Guidelines for event handlers.|True|
|Documentation|MiKo_2010|Sealed classes should document being sealed.|True|
|Documentation|MiKo_2011|Unsealed classes should not lie about sealing.|True|
|Documentation|MiKo_2012|&lt;summary&gt; documentation should describe their responsibility.|True|
|Documentation|MiKo_2013|&lt;summary&gt; documentation of Enums should have a default starting phrase.|True|
|Documentation|MiKo_2014|Dispose methods should be documented in the same way as they are documented by the .NET Framework.|True|
|Documentation|MiKo_2015|Documentation should use 'raise' or 'throw' instead of 'fire'.|True|
|Documentation|MiKo_2016|Documentation for asynchronous methods should start with specific phrase.|True|
|Documentation|MiKo_2017|Dependency properties should be documented as by the .NET Framework.|True|
|Documentation|MiKo_2018|Documentation should not use the ambiguous term 'Check'.|True|
|Documentation|MiKo_2019|&lt;summary&gt; documentation should start with a third person singular verb (for example "Provides ").|True|
|Documentation|MiKo_2020|Inherited documentation should be used with &lt;inheritdoc /&gt; marker.|True|
|Documentation|MiKo_2021|Documentation of parameter should have a default starting phrase.|True|
|Documentation|MiKo_2022|Documentation of [out] parameters should have a default starting phrase.|True|
|Documentation|MiKo_2023|Documentation of Boolean parameters should have a default starting phrase.|True|
|Documentation|MiKo_2024|Documentation of Enum parameters should have a default starting phrase.|True|
|Documentation|MiKo_2025|Documentation of 'CancellationToken' parameters should have a default starting phrase.|True|
|Documentation|MiKo_2030|Documentation of return value should have a default starting phrase.|True|
|Documentation|MiKo_2031|Documentation of Task return value should have a specific (starting) phrase.|True|
|Documentation|MiKo_2032|Documentation of Boolean return value should have a specific phrase.|True|
|Documentation|MiKo_2033|Documentation of String return value should have a default starting phrase.|True|
|Documentation|MiKo_2034|Documentation of Enum return value should have a default starting phrase.|True|
|Documentation|MiKo_2035|Documentation of collection return value should have a default starting phrase.|True|
|Documentation|MiKo_2036|Documentation of Boolean or Enum property shall describe the default value.|True|
|Documentation|MiKo_2037|&lt;summary&gt; documentation of command properties should have a default starting phrase.|True|
|Documentation|MiKo_2038|&lt;summary&gt; documentation of command should have a default starting phrase.|True|
|Documentation|MiKo_2039|&lt;summary&gt; documentation of classes that contain extension methods should have a default starting phrase.|True|
|Documentation|MiKo_2040|&lt;see langword="..."/&gt; should be used instead of &lt;c&gt;...&lt;/c&gt;.|True|
|Documentation|MiKo_2041|&lt;summary&gt; documentation should not contain other documentation tags.|True|
|Documentation|MiKo_2042|Documentation should use '&lt;para/&gt;' XML tags instead of '&lt;br/&gt;' HTML tags.|True|
|Documentation|MiKo_2043|&lt;summary&gt; documentation of custom delegates should have a default starting phrase.|True|
|Documentation|MiKo_2044|Documentation references method parameters correctly.|True|
|Documentation|MiKo_2045|&lt;summary&gt; documentation of public-visible read-only fields should have a default ending phrase.|True|
|Documentation|MiKo_2046|&lt;summary&gt; documentation of Attributes should have a  default starting phrase.|True|
|Documentation|MiKo_2047|&lt;summary&gt; documentation of value converters should have a  default starting phrase.|True|
|Documentation|MiKo_2048|Documentation should be more explicit and not use 'will be'.|True|
|Documentation|MiKo_2050|Exceptions should be documented following the .NET Framework.|True|
|Documentation|MiKo_2051|Thrown Exceptions should be documented as kind of a condition (such as '&lt;paramref name="xyz"/&gt; is &lt;c&gt;42&lt;/c&gt;').|True|
|Documentation|MiKo_2052|Throwing of ArgumentNullException should be documented using a default phrase.|True|
|Documentation|MiKo_2053|Throwing of ArgumentNullException should be documented only for reference type parameters.|True|
|Documentation|MiKo_2054|Throwing of ArgumentException should be documented using a default starting phrase.|True|
|Documentation|MiKo_2055|Throwing of ArgumentOutOfRangeException should be documented using a default starting phrase.|True|
|Documentation|MiKo_2056|Throwing of ObjectDisposedException should be documented using a default ending phrase.|True|
|Documentation|MiKo_2057|Types that are not disposable shall not throw an ObjectDisposedException.|True|
|Documentation|MiKo_2060|Factories should be documented in a uniform way.|True|
|Documentation|MiKo_2100|&lt;example&gt; documentation should start with descriptive default phrase.|True|
|Documentation|MiKo_2200|Use a capitalized letter to start the comment.|True|
|Maintainability|MiKo_3001|Custom delegates should not be used.|True|
|Maintainability|MiKo_3002|Classes should not have too many dependencies.|True|
|Maintainability|MiKo_3003|Events should follow .NET Framework Guidelines for events.|True|
|Maintainability|MiKo_3004|Do not use object.Equals() on value types.|True|
|Maintainability|MiKo_3005|Methods named 'Try' should follow the Trier-Doer-Pattern.|True|
|Maintainability|MiKo_3006|'CancellationToken' parameter should be last method parameter.|True|
|Maintainability|MiKo_3007|Do not use LINQ method and declarative query syntax in same method.|True|
|Maintainability|MiKo_3008|Method should not return collections that can be changed from outside.|True|
|Maintainability|MiKo_3009|Commands should invoke only named methods and no lambda expressions.|True|
|Maintainability|MiKo_3010|Do not create or throw reserved exception types.|True|
|Maintainability|MiKo_3011|Thrown ArgumentExceptions (or its subtypes) shall provide the correct parameter name.|True|
|Maintainability|MiKo_3012|Thrown ArgumentOutOfRangeExceptions (or its subtypes) shall provide the actual value that causes the exception to be thrown.|True|
|Maintainability|MiKo_3013|The 'default' clause in 'switch' statements should throw an ArgumentOutOfRangeException (or subtype), but no ArgumentException.|True|
|Maintainability|MiKo_3014|InvalidOperationException, NotImplementedException and NotSupportedException should have a reason as message.|True|
|Maintainability|MiKo_3015|ArgumentExceptions (or its subtypes) should not be thrown by parameterless methods. Instead an InvalidOperationException should be thrown to indicate that the object is in an inappropriate state.|True|
|Maintainability|MiKo_3020|Use 'Task.CompletedTask' instead of 'Task.FromResult'.|True|
|Maintainability|MiKo_3021|Do not use 'Task.Run' in the implementation.|True|
|Maintainability|MiKo_3030|Do not concatenate strings with += operator.|True|
|Maintainability|MiKo_3040|Do not use Booleans unless you are absolutely sure there will never be a need for more than two values.|True|
|Maintainability|MiKo_3047|Value converters shall be located in 'Converters' namespace.|True|
|Maintainability|MiKo_3101|Test classes should contain tests.|True|
|Maintainability|MiKo_3102|Test methods should not contain conditional statements such as 'if', 'switch', etc.|True|
