/// ###TL;DR..

/// Too bad, because this part is missing right now ^_^

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