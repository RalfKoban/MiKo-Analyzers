﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MiKoSolutions.Analyzers {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MiKoSolutions.Analyzers.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Methods should be simple..
        /// </summary>
        internal static string CyclomaticComplexityAnalyzer_Description {
            get {
                return ResourceManager.GetString("CyclomaticComplexityAnalyzer_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; has a Cyclomatic Complexity of {1} (allowed is {2}).
        /// </summary>
        internal static string CyclomaticComplexityAnalyzer_MessageFormat {
            get {
                return ResourceManager.GetString("CyclomaticComplexityAnalyzer_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method is too complex..
        /// </summary>
        internal static string CyclomaticComplexityAnalyzer_Title {
            get {
                return ResourceManager.GetString("CyclomaticComplexityAnalyzer_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameters of event handling methods should be named &apos;sender&apos; and &apos;e&apos;,  according to the .NET Framework Guidelines..
        /// </summary>
        internal static string EventHandlingMethodParametersAnalyzer_Description {
            get {
                return ResourceManager.GetString("EventHandlingMethodParametersAnalyzer_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; has wrong name for parameter &apos;{1}&apos; (should be named &apos;{2}&apos;).
        /// </summary>
        internal static string EventHandlingMethodParametersAnalyzer_MessageFormat {
            get {
                return ResourceManager.GetString("EventHandlingMethodParametersAnalyzer_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter names do not follow .NET Framework Guidelines for event handlers..
        /// </summary>
        internal static string EventHandlingMethodParametersAnalyzer_Title {
            get {
                return ResourceManager.GetString("EventHandlingMethodParametersAnalyzer_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Methods should be short..
        /// </summary>
        internal static string LinesOfCodeAnalyzer_Description {
            get {
                return ResourceManager.GetString("LinesOfCodeAnalyzer_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; has {1} Lines of Code (allowed are {2}).
        /// </summary>
        internal static string LinesOfCodeAnalyzer_MessageFormat {
            get {
                return ResourceManager.GetString("LinesOfCodeAnalyzer_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method is too long.
        /// </summary>
        internal static string LinesOfCodeAnalyzer_Title {
            get {
                return ResourceManager.GetString("LinesOfCodeAnalyzer_Title", resourceCulture);
            }
        }
    }
}
