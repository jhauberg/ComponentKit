using System;
using System.Collections.Generic;

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a set of definitions, each describing the composition of an entity.
    /// </summary>
    /// <remarks>
    /// Definitions are like prefabs, or blueprints, except without default properties.
    /// </remarks>
    internal class EntityDefinitions : IEntityDefinitionCollection<string> {
        IDictionary<string, IList<Type>> _definitions =
            new Dictionary<string, IList<Type>>();

        /// <summary>
        /// Defines a range of components by name. 
        /// The definition is only established if at least one valid component type is specified.
        /// </summary>
        public void Define(string definition, params Type[] types) {
            Define(definition, null, types);
        }

        /// <summary>
        /// Defines a range of components by name, with the option to include a previously defined range. 
        /// The definition is only established if at least one valid component type is specified.
        /// </summary>
        public void Define(string definition, string inheritFromDefinition, params Type[] types) {
            if (definition == null || types == null || types.Length == 0) {
                return;
            }

            List<Type> componentTypes = 
                new List<Type>();

            if (inheritFromDefinition != null && _definitions.ContainsKey(inheritFromDefinition)) {
                componentTypes.AddRange(_definitions[inheritFromDefinition]);
            }

            foreach (Type type in types) {
                bool adheresToConstraints = 
                    Component.CanCreate(type) && 
                    type.GetConstructor(Type.EmptyTypes) != null &&
                    !componentTypes.Contains(type);

                if (adheresToConstraints) {
                    componentTypes.Add(type);
                }
            }

            if (componentTypes.Count > 0) {
                _definitions[definition] = componentTypes;
            } else {
                /// If there's no valid component types, then assume that the definition should be cleared.
                Undefine(definition);
            }
        }

        /// <summary>
        /// Removes the component definition for the given name.
        /// </summary>
        public bool Undefine(string definition) {
            if (!_definitions.ContainsKey(definition)) {
                return false;
            }

            return _definitions.Remove(definition);
        }

        /// <summary>
        /// Removes all definitions.
        /// </summary>
        public void Clear() {
            _definitions.Clear();
        }

        /// <summary>
        /// Creates a new entity from the specified definition.
        /// </summary>
        public IEntityRecord Make(string definition) {
            /// Note that `Entity.Create()` automatically picks a unique name for the entity.
            return Make(definition, Entity.Create());
        }
        
        /// <summary>
        /// Realizes the specified definition for a given entity.
        /// </summary>
        public IEntityRecord Make(string definition, IEntityRecord entity) {
            if (!_definitions.ContainsKey(definition)) {
                return null;
            }

            IList<Type> componentTypes = _definitions[definition];

            foreach (Type componentType in componentTypes) {
                try {
                    entity.Add(Component.Create(componentType));
                } catch (InvalidOperationException) {
                    /// A component that doesn't provide a parameter-less constructor will fail instantiation. 
                    /// If that is the case, skip it.
                    continue;
                }
            }

            return entity;
        }
        
        public IEnumerator<string> GetEnumerator() {
            return _definitions.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}