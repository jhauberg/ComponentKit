/// ###TL;DR..

/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;

namespace ComponentKit {
    /// <summary>
    /// Defines a component of an entity.
    /// </summary>
    public interface IComponent : ISynchronizable {
        /// <summary>
        /// Gets or sets the entity that this component is attached to.
        /// </summary>
        IEntityRecord Record { get; set; }
        /// <summary>
        /// Receives a message containing arbitrary data.
        /// </summary>
        void Receive<T>(string message, T data);
    }
}
/// Copyright 2012 Jacob H. Hansen.