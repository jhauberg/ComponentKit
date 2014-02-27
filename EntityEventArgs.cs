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