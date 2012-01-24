/// ###TL;DR..
/// 
/// Throughout these documents, an `IEntityRecord` will be referred to as an *entity*. You can visualize it as a **row** in a database table, where each **column** holds a **component**. 

/// ####Similarities and differences
/// 
/// Just like in a regular database, it's not possible to have 2 identical rows. And, in most implementations, it's not possible to attach more 
/// than one column/component of the same type to a single entity - though I guess that actually makes it quite dissimilar.
/// 
/// But, since the components are just instances of real classes, they're not limited to being specific datatypes, and instead of simply 
/// providing data, they can have behavior too.

/// ##Source
using System;
using System.Collections.Generic;

namespace ComponentKit {
    /// <summary>
    /// Defines a single and uniquely identifiable entity.
    /// </summary>
    public interface IEntityRecord : ISynchronizable, IEquatable<IEntityRecord> {
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
        
        /// ###Messaging

        /// <summary>
        /// Notifies all attached components with a message containing arbitrary data.
        /// </summary>
        void Notify<T>(string message, T data);
    }
}
/// Copyright 2012 Jacob H. Hansen.