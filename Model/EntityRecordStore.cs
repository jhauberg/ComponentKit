/// ###TL;DR..
/// 
/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a non-persistent collection of entities that uses triggers to intercept entity changes.
    /// </summary>
    internal class EntityRecordStore : IEntityRecordCollection {
        static readonly object _keyhole = new object();

        Dictionary<IEntityRecord, IDictionary<Type, IComponent>> _records =
            new Dictionary<IEntityRecord, IDictionary<Type, IComponent>>();

        /// <summary>
        /// Determines whether an entity is registered in the registry.
        /// </summary>
        public bool Contains(IEntityRecord entity) {
            return _records.ContainsKey(entity);
        }

        /// <summary>
        /// Registers an entity.
        /// </summary>
        public void Enter(IEntityRecord entity) {
            Add(entity, null);
        }

        /// <summary>
        /// Unregisters an entity and returns `true` if it was successfully dropped.
        /// </summary>
        public bool Drop(IEntityRecord entity) {
            if (entity == null) {
                return false;
            }

            bool entityWasDropped = false;

            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            lock (_keyhole) {
                if (components != null && components.Count > 0) {
                    ICollection<IComponent> values = components.Values;

                    for (int i = values.Count - 1; i > 0; i--) {
                        IComponent component = values.ElementAt(i);

                        Remove(entity, component);
                    }
                }

                entityWasDropped = _records.Remove(entity);
            }

            if (entityWasDropped) {
                OnRemoved(entity);
            }

            return entityWasDropped;
        }

        /// <summary>
        /// Attaches the specified component to an entity.
        /// 
        /// If a component of the same type is already attached to the entity, then nothing happens 
        /// and the method returns false.
        /// </summary>
        public bool Add(IEntityRecord entity, IComponent component) {
            bool componentSuccessfullyAttached = false;
            bool entityWasAlreadyRegistered = true;

            lock (_keyhole) {
                if (component != null) {
                    IEntityRecord previousRecord = component.Record;

                    if (previousRecord != null) {
                        if (!previousRecord.Equals(entity)) {
                            Remove(previousRecord, component);
                        }
                    }
                }

                IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

                if (components == null) {
                    components = new Dictionary<Type, IComponent>(1);

                    entityWasAlreadyRegistered = false;

                    _records[entity] = components;
                }

                if (component != null) {
                    Type key = component.GetType();

                    if (!entityWasAlreadyRegistered || !components.ContainsKey(key)) {
                        components.Add(key, component);

                        if (component.Record == null || !component.Record.Equals(entity)) {
                            component.Record = entity;
                        }

                        PrepareComponentForSynchronization(component);

                        componentSuccessfullyAttached = true;
                    }
                }
            }

            if (!entityWasAlreadyRegistered) {
                OnEntered(entity);
            }

            return componentSuccessfullyAttached;
        }

        /// <summary>
        /// Dettaches the specified component from an entity if possible, and returns `true` if it was successfully removed.
        /// </summary>
        public bool Remove(IEntityRecord entity, IComponent component) {
            if (entity == null ||
                component == null) {
                return false;
            }

            /// In the unusual case, where the passed component is not actually attached to the passed entity, we need to bail immediately.
            /// Otherwise, if the entity actually had the same type of component attached to it, it would unexpectedly get removed.
            if (component.Record != null && !component.Record.Equals(entity)) {
                return false;
            }

            bool componentWasRemoved = false;

            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            lock (_keyhole) {
                if (components != null && components.Count > 0) {
                    Type key = component.GetType();

                    if (components.ContainsKey(key)) {
                        componentWasRemoved = components.Remove(key);
                    }
                }

                if (component.Record != null) {
                    component.Record = null;
                }

                PrepareComponentForSynchronization(component);
            }

            return componentWasRemoved;
        }

        /// <summary>
        /// Occurs when a new entity is registered.
        /// </summary>
        public event EventHandler<EntityEventArgs> Entered;
        /// <summary>
        /// Occurs when an entity is unregistered.
        /// </summary>
        public event EventHandler<EntityEventArgs> Removed;

        void OnEntered(IEntityRecord entity) {
            if (Entered != null) {
                Entered(this, new EntityEventArgs(entity));
            }
        }

        void OnRemoved(IEntityRecord entity) {
            if (Removed != null) {
                Removed(this, new EntityEventArgs(entity));
            }
        }

        /// <summary>
        /// Returns a component of the specified type if it is attached to the entity.
        /// </summary>
        public TComponent GetComponent<TComponent>(IEntityRecord entity) 
            where TComponent : class, IComponent {
            TComponent component = default(TComponent);

            return GetComponent(entity, component);
        }

        /// <summary>
        /// Returns a component of the specified type if it is attached to the entity.
        /// </summary>
        public TComponent GetComponent<TComponent>(IEntityRecord entity, TComponent component) 
            where TComponent : class, IComponent {
            if (entity == null) {
                return default(TComponent);
            }

            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            TComponent result = default(TComponent);

            if (components != null && components.Count > 0) {
                Type componentType = null;

                if (component != null) {
                    componentType = component.GetType();
                } else {
                    componentType = typeof(TComponent);
                }

                if (components.ContainsKey(componentType)) {
                    result = (TComponent)components[componentType];
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a component of the specified type if it is attached to the entity.
        /// </summary>
        public IComponent GetComponent(IEntityRecord entity, Type componentType) {
            if (entity == null || componentType == null) {
                return null;
            }

            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            IComponent result = null;

            if (components != null && components.Count > 0) {
                if (components.ContainsKey(componentType)) {
                    result = components[componentType];
                }
            }

            return result;
        }

        /// <summary>
        /// If `allowingDerivedTypes` is `true`, returns any component that is either a subclass of, or is, the specified type if it is attached to the entity.
        /// </summary>
        public IComponent GetComponent(IEntityRecord entity, Type componentType, bool allowingDerivedTypes) {
            if (entity == null || componentType == null) {
                return null;
            }

            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            IComponent result = null;

            if (components != null && components.Count > 0) {
                foreach (IComponent otherComponent in components.Values) {
                    if (otherComponent.GetType().IsEquivalentTo(componentType)) {
                        result = otherComponent;

                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns any component that is either a subclass of, or is, the specified type if it is attached to the entity.
        /// </summary>
        public TComponent GetComponent<TComponent>(IEntityRecord entity, TComponent component, bool allowingDerivedTypes) 
            where TComponent : class, IComponent {
            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            TComponent result = default(TComponent);

            if (components != null && components.Count > 0) {
                foreach (IComponent otherComponent in components.Values) {
                    if (otherComponent is TComponent) {
                        result = (TComponent)otherComponent;

                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns all the components that are attached to the entity.
        /// </summary>
        public IEnumerable<IComponent> GetComponents(IEntityRecord entity) {
            IDictionary<Type, IComponent> components = GetComponentsForRecord(entity);

            return components != null ?
                components.Values :
                null;
        }

        IDictionary<Type, IComponent> GetComponentsForRecord(IEntityRecord record) {
            IDictionary<Type, IComponent> components = null;

            lock (_keyhole) {
                if (_records.ContainsKey(record)) {
                    components = _records[record];
                }
            }

            return components;
        }

        /// <summary>
        /// Returns all the components that are attached to the specified entity.
        /// </summary>
        public IEnumerable<IComponent> this[IEntityRecord entity] {
            get {
                return GetComponents(entity);
            }
        }

        public IEnumerator<IEntityRecord> GetEnumerator() {
            return _records.Keys.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        /// Stores all components that have changed sync state since last synchronization operation; i.e. components that have been added or removed.
        List<IComponent> _desynchronizedComponents =
            new List<IComponent>();

        /// <summary>
        /// This registry is considered out-of-sync as soon as there are components that have not yet been run through the triggers.
        /// </summary>
        public bool IsOutOfSync {
            get {
                return _desynchronizedComponents != null && _desynchronizedComponents.Count > 0;
            }
        }

        void PrepareComponentForSynchronization(IComponent component) {
            /// It is conceivable that a component might change sync state several times before a synchronization operation occurs, 
            /// so there's no reason to keep growing this list more than necessary.
            if (!_desynchronizedComponents.Contains(component)) {
                _desynchronizedComponents.Add(component);
            }
        }

        Dictionary<ComponentSyncTriggerPredicate, EventHandler<ComponentSyncEventArgs>> _triggers =
            new Dictionary<ComponentSyncTriggerPredicate, EventHandler<ComponentSyncEventArgs>>();

        /// <summary>
        /// Sets a trigger that fires when components matching the specified predicate are attached to entities.
        /// </summary>
        /// > Note that triggers are only run during synchronization operations.
        public void SetTrigger(ComponentSyncTriggerPredicate predicate, EventHandler<ComponentSyncEventArgs> handler) {
            lock (_keyhole) {
                _triggers[predicate] = handler;
            }
        }

        /// <summary>
        /// Clears out an existing trigger.
        /// </summary>
        public void ClearTrigger(ComponentSyncTriggerPredicate predicate) {
            lock (_keyhole) {
                if (_triggers.ContainsKey(predicate)) {
                    _triggers.Remove(predicate);
                }
            }
        }

        /// <summary>
        /// Runs the registered triggers for all components that have changed sync state since the previous synchronization operation.
        /// </summary>
        public void Synchronize() {
            if (_desynchronizedComponents.Count == 0) {
                return;
            }

            lock (_keyhole) {
                foreach (ComponentSyncTriggerPredicate trigger in _triggers.Keys) {
                    /// Discovers all components that matches the predicate described by the `trigger`
                    IEnumerable<IComponent> components =
                        _desynchronizedComponents
                            .Where(component => 
                                trigger(component));

                    if (components != null && 
                        components.Count() > 0) {
                        _triggers[trigger](this, 
                            new ComponentSyncEventArgs(components));
                    }
                }

                _desynchronizedComponents.Clear();
            }
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.