/// ###TL;DR..

/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;

namespace ComponentKit {
    /// <summary>
    /// Provides the entity for which an event has occurred.
    /// </summary>
    /// <remarks>
    /// > For example, when entities are **added** or **removed** these `EventArgs` will be provided.
    /// </remarks>
    public class EntityEventArgs : EventArgs {
        public EntityEventArgs(IEntityRecord entity) {
            Record = entity;
        }

        /// <summary>
        /// Gets the entity for which the event occurred.
        /// </summary>
        public virtual IEntityRecord Record {
            get;
            private set;
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.