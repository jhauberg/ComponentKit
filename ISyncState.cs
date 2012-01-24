/// ###TL;DR..

/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;

namespace ComponentKit {
    /// <summary>
    /// Represents an object that can determine when it is out-of-sync.
    /// </summary>
    public interface ISyncState {
        /// <summary>
        /// Determines whether an object is in a desynchronized state.
        /// </summary>
        bool IsOutOfSync { get; }
    }
}
/// Copyright 2012 Jacob H. Hansen.