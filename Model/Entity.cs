using System;
using System.Linq;
using System.Collections.Generic;

namespace ComponentKit.Model {
    /// <summary>
    /// Provides methods for acquiring and getting rid of entities.
    /// </summary>
    public static class Entity {
        static readonly IEntityDefinitionCollection<string> _definitions = 
            new EntityDefinitions();

        /// ### Manipulation
        
        /// <summary>
        /// Creates and registers an entity in the active registry.
        /// </summary>
        public static IEntityRecord Create(string name) {
            return Create(name, EntityRegistry.Current);
        }

        /// <summary>
        /// Creates and registers an entity in the specified registry.
        /// </summary>
        public static IEntityRecord Create(string name, IEntityRecordCollection registry) {
            IEntityRecord record = new EntityRecord(name, registry);

            /// Ensure that the entity gets registered.
            record.Synchronize();

            return record;
        }

        /// <summary>
        /// Creates and registers an entity with the specified components attached to it. 
        /// A unique name is automatically generated.
        /// </summary>
        public static IEntityRecord Create(params IComponent[] components) {
            return Create(GetUniqueName(), components);
        }

        /// <summary>
        /// Returns a globally-unique identifier (GUID).
        /// </summary>
        /// <remarks>
        /// > This is useful when you don't care about naming your entities immediately.
        /// </remarks>
        static string GetUniqueName() {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates and registers an entity in the active registry. The specified components will also be attached to it.
        /// </summary>
        public static IEntityRecord Create(string name, params IComponent[] components) {
            return Create(name, EntityRegistry.Current, components);
        }

        /// <summary>
        /// Creates and registers a new entity in the specified registry. The specified components will also be attached to it.
        /// </summary>
        public static IEntityRecord Create(string name, IEntityRecordCollection registry, params IComponent[] components) {
            IEntityRecord record = Create(name, registry);

            foreach (IComponent component in components) {
                record.Add(component);
            }

            return record;
        }

        /// <summary>
        /// Returns true if an entity with the specified name was dropped from the active registry.
        /// </summary>
        public static bool Drop(string name) {
            return EntityRegistry.Current.Drop(
                new EntityRecord(name));
        }

        /// <summary>
        /// Defines a range of components by name. 
        /// The definition is only created if at least one valid component type is specified.
        /// </summary>
        public static void Define(string definition, params Type[] componentTypes) {
            _definitions.Define(definition, componentTypes);
        }

        /// <summary>
        /// Defines a range of components by name, with the option to include a previously defined range. 
        /// The definition is only established if at least one valid component type is specified.
        /// </summary>
        public static void Define(string definition, string inheritFromDefinition, params Type[] types) {
            _definitions.Define(definition, inheritFromDefinition, types);
        }

        /// <summary>
        /// Removes the component definition for the given name.
        /// </summary>
        public static bool Undefine(string definition) {
            return _definitions.Undefine(definition);
        }

        /// <summary>
        /// Creates an entity from the specified definition and registers it in the active registry.
        /// </summary>
        public static IEntityRecord CreateFromDefinition(string definition) {
            return _definitions.Make(definition);
        }

        public static IEntityRecord CreateFromDefinition(string definition, params IComponent[] components) {
            IEntityRecord entity = CreateFromDefinition(definition);

            foreach (IComponent component in components) {
                entity.Add(component);
            }

            return entity;
        }

        /// <summary>
        /// Creates an entity from the specified definition and registers it in the active registry.
        /// </summary>
        public static IEntityRecord CreateFromDefinition(string definition, string name) {
            return _definitions.Make(definition, Create(name));
        }

        public static IEntityRecord CreateFromDefinition(string definition, string name, params IComponent[] components) {
            IEntityRecord entity = _definitions.Make(definition, Create(name));

            foreach (IComponent component in components) {
                entity.Add(component);
            }

            return entity;
        }

        /// ### Retrieval

        /// <summary>
        /// Attempts to find the entity that matches the specified name in the active registry. Returns `null` if none was found.
        /// </summary>
        public static IEntityRecord Find(string name) {
            return Find(name, EntityRegistry.Current);
        }
        
        /// <summary>
        /// Attempts to find the entity that matches the specified name in a registry. Returns `null` if none was found.
        /// </summary>
        public static IEntityRecord Find(string name, IEntityRecordCollection registry) {
            IEntityRecord record = new EntityRecord(name, registry);

            if (!registry.Contains(record)) {
                record = null;
            }

            return record;
        }

        /// <summary>
        /// Finds all entities in the active registry that have a component of the specified type attached to them.
        /// </summary>
        public static IEnumerable<IEntityRecord> FindAllWithComponent<TComponent>() 
            where TComponent : class, IComponent {
            return FindAllWithComponent<TComponent>(EntityRegistry.Current);
        }

        /// <summary>
        /// Finds all entities in a registry that have a component of the specified type attached to them.
        /// </summary>
        public static IEnumerable<IEntityRecord> FindAllWithComponent<TComponent>(IEntityRecordCollection registry) 
            where TComponent : class, IComponent {
            return registry != null ? 
                registry.Where(entity => 
                    entity.HasComponent<TComponent>()) : 
                null;
        }

        /// <summary>
        /// Finds all entities in the active registry that have components of the specified types attached to them.
        /// </summary>
        public static IEnumerable<IEntityRecord> FindAllWithComponents<TComponentA, TComponentB>()
            where TComponentA : class, IComponent
            where TComponentB : class, IComponent {
            return FindAllWithComponents<TComponentA, TComponentB>(EntityRegistry.Current);
        }

        /// <summary>
        /// Finds all entities in a registry that have components of the specified types attached to them.
        /// </summary>
        public static IEnumerable<IEntityRecord> FindAllWithComponents<TComponentA, TComponentB>(IEntityRecordCollection registry)
            where TComponentA : class, IComponent
            where TComponentB : class, IComponent {
            return registry != null ?
                registry.Where(entity => 
                    entity.HasComponent<TComponentA>() && 
                    entity.HasComponent<TComponentB>()) :
                null;
        }

        /// ### Messaging

        /// <summary>
        /// Broadcasts a message to every single component that is currently attached to an entity in the active registry.
        /// </summary>
        public static void Broadcast<TData>(string message, TData data) {
            Broadcast(EntityRegistry.Current, message, data);
        }

        /// <summary>
        /// Broadcasts a message to every single component that is currently attached to an entity in the specified registry.
        /// </summary>
        public static void Broadcast<TData>(IEntityRecordCollection registry, string message, TData data) {
            if (registry == null) {
                return;
            }

            foreach (IEntityRecord record in registry) {
                record.Notify(message, data);
            }
        }
    }
}