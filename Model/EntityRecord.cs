/// ###TL;DR..

/// This concrete implementation of `IEntityRecord` is a **struct**, and uses its **Name** property to determine equality.
/// 
/// The fact that it is a struct means that instances can be created on the fly pretty much without penalty.

/// ##Source
using System;
using System.Dynamic;

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a concrete entity that can be entered into a registry.
    /// </summary>
    internal struct EntityRecord : IEntityRecord {
        string _name;

        /// This implementation uses the `Name` property to determine its identity.
        public override int GetHashCode() {
            return Name == null ?
                0 : Name.GetHashCode();
        }

        public bool Equals(IEntityRecord other) {
            if (other == null) {
                return false;
            }

            /// Which is used to determine equality between entities.
            return GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object other) {
            if (other == null) {
                return false;
            }

            if (!(other is IEntityRecord)) {
                return false;
            }

            return Equals((EntityRecord)other);
        }

        /// <summary>
        /// Gets the unique name identifier for this entity.
        /// </summary>
        public string Name {
            get {
                return _name;
            }

            /// The name is only allowed to be set once. This is necessary, because since `EntityRecord` is a struct, it will always have an empty constructor.
            /// And when this is used, the instance becomes useless without being able to ever have a **Name**.
            set {
                if (_name == null) {
                    _name = value;
                } else {
                    /// > If you want to rename an entity, it's usually done by simply moving the components into a new one.
                    /// Since the name is used for determining identity, being able to mutate it would cause leaks in the `Dictionaries` containing the entities.
                    throw new InvalidOperationException();
                }
            }
        }

        IEntityRecordCollection _registry;

        /// <summary>
        /// Gets the registry that this entity is registered to (when in a synchronized state).
        /// </summary>
        public IEntityRecordCollection Registry {
            get {
                return _registry;
            }

            set {
                /// When set to a different registry, this entity will automatically be registered into the new one when synchronized.
                if (_registry != value) {
                    /// And of course, it is also unregistered from the previous registry if necessary..
                    if (_registry != null || (value == null && _registry != null)) {
                        /// > Note that by switching registry, all attached components will be lost.
                        _registry.Drop(this);
                    }
                    
                    _registry = value;
                }
            }
        }

        /// <summary>
        /// Creates a new entity with the specified name.
        /// </summary>
        /// <remarks>
        /// > The entity will be out-of-sync until it has a registry, so this constructor is usually only used when you need to query the
        /// registry for a certain entity (since its name == its identity).
        /// </remarks>
        public EntityRecord(string name)
            : this(name, null) { }

        /// <summary>
        /// Creates a new entity with the specified name and registers it to the registry.
        /// </summary>
        public EntityRecord(string name, IEntityRecordCollection registry)
            : this() {
            Name = name;
            Registry = registry;
        }
        
        /// <summary>
        /// The entity is considered out-of-sync when it is not registered in a registry, and/or has no name.
        /// </summary>
        public bool IsOutOfSync {
            get {
                return
                    Name == null ||
                    Registry == null ||
                    !Registry.Contains(this);
            }
        }

        /// <summary>
        /// Ensures that the entity is registered to its registry.
        /// </summary>
        public void Synchronize() {
            if (Name == null) {
                throw new InvalidOperationException();
            }

            if (_registry != null) {
                if (!_registry.Contains(this)) {
                    _registry.Enter(this);
                }
            }
        }

        /// <summary>
        /// Gets a component that matches the specified type name, if it is attached to this entity.
        /// </summary>
        public IComponent this[string componentNameOrType] {
            get {
                return Registry == null ?
                    null :
                    Registry.GetComponent(this, Type.GetType(componentNameOrType));
            }
        }
        
        /// <summary>
        /// Broadcasts a message containing arbitrary data to all components attached to this entity.
        /// </summary>
        public void Notify<T>(string message, T data) {
            if (Registry == null) {
                return;
            }
            
            foreach (IComponent component in (this as IEntityRecord).GetComponents()) {
                component.Receive(message, data);
            }
        }

        /// Unfortunately, overloading the operators will not work for comparing 2 instances of `IEntityRecord` (which is the exposed interface). 
        /// It sucks, but because this is a compile-time feature, it will only work when comparing instances that have been cast to `EntityRecord`. This is no good.
        public static bool operator ==(EntityRecord left, EntityRecord right) {
            if (Object.ReferenceEquals(left, right)) {
                return true;
            }

            return left.Equals(right);
        }

        public static bool operator !=(EntityRecord left, EntityRecord right) {
            return !(left == right);
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.