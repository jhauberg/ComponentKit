using System;
using System.Collections.Generic;

namespace ComponentKit {
    /// <summary>
    /// Defines a single and uniquely identifiable entity.
    /// </summary>
    public interface IEntityRecord : ISynchronizable, IEquatable<IEntityRecord>, IEnumerable<IComponent> {
        /// <summary>
        /// Gets the unique name identifier for the entity.
        /// </summary>
        /// <remarks>
        /// > It is debatable whether this should be considered an implementation detail, and not be exposed at all.
        /// </remarks>
        string Name {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the registry that the entity is registered to (when in a synchronized state).
        /// </summary>
        IEntityRecordCollection Registry {
            get;
            set;
        }
        
        /// <summary>
        /// Gets a component that matches the specified type name, if it is attached to the entity.
        /// </summary>
        IComponent this[string componentNameOrType] { get; }
        
        /// ### Messaging

        /// <summary>
        /// Notifies all attached components with a message containing arbitrary data.
        /// </summary>
        void Notify<TData>(string message, TData data);
    }
}