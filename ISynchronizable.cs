using System;

namespace ComponentKit {
    /// <summary>
    /// Defines methods for ensuring synchronicity.
    /// </summary>
    /// <remarks>
    /// Components and entities needed a way to do whatever action necessary to become in sync.
    /// 
    /// Components and entities can be entered, altered or even removed as the app runs. 
    /// But, depending on intent, these kinds of changes may cause a component to slip out of sync.
    /// </remarks>
    public interface ISynchronizable : ISyncState {
        /// <summary>
        /// Ensures that a potentially out-of-sync state becomes synchronized.
        /// </summary>
        void Synchronize();
    }
}