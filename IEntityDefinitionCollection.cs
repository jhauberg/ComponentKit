/// ###TL;DR..
/// 
/// Lets you group components together, so you can later spawn them all in one line.

/// ##Source
using System;
using System.Collections.Generic;

namespace ComponentKit {
    /// <summary>
    /// Describes a set of unique entity compositions, each defined by `TEntityDefinition`.
    /// </summary>
    internal interface IEntityDefinitionCollection<TEntityDefinition> 
        : IEnumerable<TEntityDefinition> {
        /// <summary>
        /// Defines a range of components by name.
        /// </summary>
        void Define(TEntityDefinition definition, params Type[] types);
        /// <summary>
        /// Defines a range of components by name, with the option to include a previously defined range.
        /// </summary>
        void Define(string definition, string inheritFromDefinition, params Type[] types);
        /// <summary>
        /// Removes the component definition for the given name.
        /// </summary>
        bool Undefine(TEntityDefinition definition);
        /// <summary>
        /// Removes all definitions.
        /// </summary>
        void Clear();
        /// <summary>
        /// Creates a new entity from the specified definition.
        /// </summary>
        IEntityRecord Make(TEntityDefinition definition);
        /// <summary>
        /// Realizes the specified definition for a given entity.
        /// </summary>
        IEntityRecord Make(TEntityDefinition definition, IEntityRecord entity);
    }
}

/// Copyright 2012 Jacob H. Hansen.
