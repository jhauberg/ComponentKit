using System;

namespace ComponentKit {
    /// <summary>
    /// Represents an object that can determine when it is out-of-sync.
    /// </summary>
    /// <remarks>
    /// Components and entities needed a way to communicate whether they had changed and should be processed again.
    /// </remarks>
    public interface ISyncState {
        /// <summary>
        /// Determines whether an object is in a desynchronized state.
        /// </summary>
        bool IsOutOfSync { get; }
    }
}