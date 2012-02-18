/// ###TL;DR..
/// 
/// A component only has one really important attribute. The parent entity.

/// ##Source
using System;

namespace ComponentKit {
    /// <summary>
    /// Defines a component of an entity.
    /// </summary>
    public interface IComponent : ISynchronizable, IDisposable {
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

/// Copyright 2012 Jacob H. Hansen.