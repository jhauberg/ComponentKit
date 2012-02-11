/// ###TL;DR..

/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;
using System.Collections.Generic;

namespace ComponentKit {
    /// <summary>
    /// Defines a collection of unique entities.
    /// </summary>
    public interface IEntityRecordCollection : ISynchronizable, IEnumerable<IEntityRecord> {
        /// <summary>
        /// Determines whether an entity is registered in the registry.
        /// </summary>
        bool Contains(IEntityRecord entity);

        /// ###Manipulation
        
        /// <summary>
        /// Registers an entity.
        /// </summary>
        void Enter(IEntityRecord entity);
        /// <summary>
        /// Unregisters an entity and returns `true` if it was successfully dropped.
        /// </summary>
        bool Drop(IEntityRecord entity);
        /// <summary>
        /// Attaches the specified component to an entity.
        /// </summary>
        bool Add(IEntityRecord entity, IComponent component);
        /// <summary>
        /// Dettaches the specified component from an entity if possible, and returns `true` if it was successfully removed.
        /// </summary>
        bool Remove(IEntityRecord entity, IComponent component);

        /// ###Events

        /// <summary>
        /// Occurs when a new entity is registered.
        /// </summary>
        event EventHandler<EntityEventArgs> Entered;
        /// <summary>
        /// Occurs when an entity is unregistered.
        /// </summary>
        event EventHandler<EntityEventArgs> Removed;

        /// ###Triggers

        /// Triggers allow you to intercept components that have changed state during a synchronization operation.

        /// <summary>
        /// Adds a trigger that fires on components that matches a predicate.
        /// </summary>
        void SetTrigger(ComponentSyncTriggerPredicate predicate, EventHandler<ComponentSyncEventArgs> handler);
        /// <summary>
        /// Clears out an existing trigger.
        /// </summary>
        void ClearTrigger(ComponentSyncTriggerPredicate predicate);

        /// ###Component retrieval

        /// <summary>
        /// Returns a component of the specified type if it is attached to the entity.
        /// </summary>
        T GetComponent<T>(IEntityRecord entity) where T : class, IComponent;
        /// <summary>
        /// Returns a component of the specified type if it is attached to the entity.
        /// </summary>
        T GetComponent<T>(IEntityRecord entity, T component) where T : class, IComponent;
        /// <summary>
        /// Returns any component that is either a subclass of, or is, the specified type if it is attached to the entity.
        /// </summary>
        T GetComponent<T>(IEntityRecord entity, T component, bool allowingDerivedTypes) where T : class, IComponent;

        /// <summary>
        /// Returns a component of the specified type if it is attached to the entity.
        /// </summary>
        IComponent GetComponent(IEntityRecord entity, Type componentType);
        /// <summary>
        /// If `allowingDerivedTypes` is `true`, returns any component that is either a subclass of, or is, the specified type if it is attached to the entity.
        /// </summary>
        IComponent GetComponent(IEntityRecord entity, Type componentType, bool allowingDerivedTypes);

        /// <summary>
        /// Returns all the components that are attached to the entity.
        /// </summary>
        IEnumerable<IComponent> GetComponents(IEntityRecord entity);
        /// <summary>
        /// Returns all the components that are attached to the specified entity.
        /// </summary>
        IEnumerable<IComponent> this[IEntityRecord entity] { get; }
    }
}

/// Copyright 2012 Jacob H. Hansen.