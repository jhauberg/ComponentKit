/// ###TL;DR..

/// Components and entities needed a way to do whatever action necessary to become in sync.
///
/// ####More
/// 
/// Components and entities can be entered, altered or even removed as the app runs. 
/// But, depending on intent, these kinds of changes may cause a component to slip out of sync.
/// 
/// Calling `Synchronize()` causes any changes to be *committed*, so to speak.

/// ##Source
using System;

namespace ComponentKit {
    /// <summary>
    /// Defines methods for ensuring synchronicity.
    /// </summary>
    public interface ISynchronizable : ISyncState {
        /// <summary>
        /// Ensures that a potentially out-of-sync state becomes synchronized.
        /// </summary>
        void Synchronize();
    }
}

/// Copyright 2012 Jacob H. Hansen.