/// ###TL;DR..
/// 
/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;
using System.Collections.Generic;

namespace ComponentKit.Model {
    /// <summary>
    /// Provides methods for acquiring and getting rid of entities.
    /// </summary>
    public static class Entity {
        /// ###Manipulation
        
        /// <summary>
        /// Creates and registers an entity in the active registry.
        /// </summary>
        /// <remarks>
        /// > This *active registry* is going to be mentioned often. 
        /// But all it is, is a static property that can be changed and will always fall back to the default registry when null'ed. See `EntityRegistry` for more of that stuff.
        /// </remarks>
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

        /// ###Retrieval

        /// <summary>
        /// Attempts to find the entity that matches the specified name in the active registry.
        /// </summary>
        public static IEntityRecord Find(string name) {
            return Find(name, EntityRegistry.Current);
        }
        
        /// <summary>
        /// Attempts to find the entity that matches the specified name in a registry.
        /// </summary>
        public static IEntityRecord Find(string name, IEntityRecordCollection registry) {
            IEntityRecord record = new EntityRecord(name, registry);

            if (!registry.Contains(record)) {
                record = null;
            }

            return record;
        }

        /// ###Messaging

        /// <summary>
        /// Broadcasts a message to every single component that is currently attached to an entity in the active registry.
        /// </summary>
        public static void Broadcast<T>(string message, T data) {
            Broadcast(EntityRegistry.Current, message, data);
        }

        /// <summary>
        /// Broadcasts a message to every single component that is currently attached to an entity in the specified registry.
        /// </summary>
        public static void Broadcast<T>(IEntityRecordCollection registry, string message, T data) {
            if (registry == null) {
                return;
            }

            foreach (IEntityRecord record in registry) {
                record.Notify(message, data);
            }
        }
    }

    /// <summary>
    /// Convenient methods for dealing with entities.
    /// </summary>
    public static class EntityExtensions {
        /// ###Manipulation

        /// <summary>
        /// Attaches a component to this entity.
        /// </summary>
        public static void Add(this IEntityRecord entity, IComponent component) {
            if (entity.Registry != null) {
                entity.Registry.Add(entity, component);
            }
        }

        /// <summary>
        /// Dettaches a component from this entity.
        /// </summary>
        public static bool Remove(this IEntityRecord entity, IComponent component) {
            return entity.Registry != null ?
                entity.Registry.Remove(entity, component) :
                false;
        }

        /// <summary>
        /// Unregisters this entity from its registry.
        /// </summary>
        public static bool Drop(this IEntityRecord entity) {
            return entity.Registry != null ?
                entity.Registry.Drop(entity) :
                false;
        }

        /// ###Component retrieval

        /// <summary>
        /// Returns a component of the specified type if it is attached to this entity.
        /// </summary>
        public static T GetComponent<T>(this IEntityRecord entity) where T : IComponent {
            return entity.Registry == null ?
                default(T) :
                entity.Registry.GetComponent<T>(entity);
        }

        /// <summary>
        /// Returns a component of the specified type if it is attached to this entity.
        /// </summary>
        public static T GetComponent<T>(this IEntityRecord entity, T component) where T : IComponent {
            return entity.Registry == null ?
                default(T) :
                entity.Registry.GetComponent(entity, component);
        }

        /// <summary>
        /// If `allowingDerivedTypes` is `true`, returns any component that is either a subclass of, or is, the specified type if it is attached to this entity.
        /// </summary>
        public static T GetComponent<T>(this IEntityRecord entity, T component, bool allowingDerivedTypes) where T : IComponent {
            return entity.Registry == null ?
                default(T) :
                entity.Registry.GetComponent(entity, component, allowingDerivedTypes);
        }

        /// <summary>
        /// Gets all components currently attached to this entity.
        /// </summary>
        public static IEnumerable<IComponent> GetComponents(this IEntityRecord entity) {
            return entity.Registry == null ?
                null :
                entity.Registry.GetComponents(entity);
        }

        public static bool HasComponent<T>(this IEntityRecord entity) where T : IComponent {
            return GetComponent<T>(entity) != null;
        }

        public static bool HasComponent<T>(this IEntityRecord entity, T component) where T : IComponent {
            return GetComponent<T>(entity, component) != null;
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.