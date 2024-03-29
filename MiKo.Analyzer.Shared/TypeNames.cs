﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;

// ReSharper disable AssignNullToNotNullAttribute
namespace MiKoSolutions.Analyzers
{
    internal static class TypeNames
    {
        internal static readonly string CancellationToken = string.Intern(typeof(CancellationToken).FullName);
        internal static readonly string CancellationTokenSource = string.Intern(typeof(CancellationTokenSource).FullName);

        internal static readonly string Delegate = string.Intern(typeof(Delegate).FullName);

        internal static readonly string ArgumentException = string.Intern(typeof(ArgumentException).FullName);
        internal static readonly string ArgumentNullException = string.Intern(typeof(ArgumentNullException).FullName);
        internal static readonly string ArgumentOutOfRangeException = string.Intern(typeof(ArgumentOutOfRangeException).FullName);

        internal static readonly string InvalidOperationException = string.Intern(typeof(InvalidOperationException).FullName);
        internal static readonly string NotImplementedException = string.Intern(typeof(NotImplementedException).FullName);
        internal static readonly string NotSupportedException = string.Intern(typeof(NotSupportedException).FullName);

        internal static readonly string InvalidEnumArgumentException = string.Intern(typeof(InvalidEnumArgumentException).FullName);

        internal static readonly string PropertyChangingEventArgs = string.Intern(typeof(PropertyChangingEventArgs).FullName);
        internal static readonly string PropertyChangedEventArgs = string.Intern(typeof(PropertyChangedEventArgs).FullName);

        internal static readonly string SerializationInfo = string.Intern(typeof(SerializationInfo).FullName);
        internal static readonly string StreamingContext = string.Intern(typeof(StreamingContext).FullName);

        internal static readonly string TimeSpan = string.Intern(typeof(TimeSpan).FullName);
    }
}