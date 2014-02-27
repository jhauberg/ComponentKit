using System;

namespace ComponentKit {
    /// <summary>
    /// Defines a component of an entity.
    /// </summary>
    public interface IComponent : ISyncState, IDisposable {
        /// <summary>
        /// Gets or sets the entity that this component is attached to.
        /// </summary>
        IEntityRecord Record { get; set; }
        /// <summary>
        /// Receives a message containing arbitrary data.
        /// </summary>
        void Receive<TData>(string message, TData data);
    }
}