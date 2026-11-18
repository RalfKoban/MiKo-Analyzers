//// ncrunch: rdi off
//// ncrunch: no coverage start
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace System.Threading
{
    /// <summary>
    /// Provides a set of <see langword="static"/> methods for <see cref="ReaderWriterLockSlim"/>s.
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        /// <summary>
        /// Enters the read lock and returns an object that exits the read lock when disposed.
        /// </summary>
        /// <param name="value">
        /// The lock to enter in read mode.
        /// </param>
        /// <returns>
        /// A <see cref="ReadLock"/> that exits the read lock when disposed.
        /// </returns>
        /// <remarks>
        /// This method is intended to be used in a <see langword="using"/> statement.
        /// </remarks>
        internal static ReadLock InReadMode(this ReaderWriterLockSlim value) => new ReadLock(value);

        /// <summary>
        /// Enters the write lock and returns an object that exits the write lock when disposed.
        /// </summary>
        /// <param name="value">
        /// The lock to enter in write mode.
        /// </param>
        /// <returns>
        /// A <see cref="WriteLock"/> that exits the write lock when disposed.
        /// </returns>
        /// <remarks>
        /// This method is intended to be used in a <see langword="using"/> statement.
        /// </remarks>
        internal static WriteLock InWriteMode(this ReaderWriterLockSlim value) => new WriteLock(value);

        /// <summary>
        /// Represents an acquired read lock that is released on disposal.
        /// </summary>
        /// <remarks>
        /// This type is a <see langword="struct"/> to avoid a heap allocation each time the lock is used.
        /// </remarks>
        internal readonly struct ReadLock : IDisposable
        {
            /// <summary>
            /// The lock to enter in read mode.
            /// </summary>
            private readonly ReaderWriterLockSlim m_lock;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadLock"/> struct and enters the read lock.
            /// </summary>
            /// <param name="lock">
            /// The lock to enter in read mode.
            /// </param>
            public ReadLock(ReaderWriterLockSlim @lock)
            {
                m_lock = @lock;
                m_lock.EnterReadLock();
            }

            /// <inheritdoc/>
            /// <remarks>
            /// Exits the read lock.
            /// </remarks>
            public void Dispose() => m_lock.ExitReadLock();
        }

        /// <summary>
        /// Represents an acquired write lock that is released on disposal.
        /// </summary>
        /// <remarks>
        /// This type is a <see langword="struct"/> to avoid a heap allocation each time the lock is used.
        /// </remarks>
        internal readonly struct WriteLock : IDisposable
        {
            /// <summary>
            /// The lock to enter in write mode.
            /// </summary>
            private readonly ReaderWriterLockSlim m_lock;

            /// <summary>
            /// Initializes a new instance of the <see cref="WriteLock"/> struct and enters the write lock.
            /// </summary>
            /// <param name="lock">
            /// The lock to enter in write mode.
            /// </param>
            public WriteLock(ReaderWriterLockSlim @lock)
            {
                m_lock = @lock;
                m_lock.EnterWriteLock();
            }

            /// <inheritdoc/>
            /// <remarks>
            /// Exits the write lock.
            /// </remarks>
            public void Dispose() => m_lock.ExitWriteLock();
        }
    }
}