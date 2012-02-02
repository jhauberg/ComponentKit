/// ###TL;DR..

/// Components and entities needed a way to communicate whether they had 
/// changed and should be processed again.

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