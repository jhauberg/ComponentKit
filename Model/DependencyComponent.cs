/// ###TL;DR..
/// 
/// The `DependencyComponent` can be sub-classed to build custom behavior. It is aware of its dependencies, and will automatically inject them during synchronization.

/// ####Processing
/// 
/// Triggers can be used to intercept when components are attached/dettached from entities, which makes it easy to add special processing of specific types of components.
/// 
/// ####Staying n'sync
/// 
/// A component becomes out-of-sync when it is no longer attached to an `IEntityRecord`. Triggers, however, are not guaranteed to be notified of this immediately. 
/// The default implementation of `IEntityRecordCollection` only runs the triggers during a synchronization operation.

/// ##Source
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a single component of an entity. It uses dependency injection where applicable.
    /// </summary>
    public abstract class DependencyComponent : IComponent {
        Dictionary<FieldInfo, RequireComponentAttribute> _dependencies =
            new Dictionary<FieldInfo, RequireComponentAttribute>();

        /// It can only have one immediate parent.
        IEntityRecord _record;

        /// It does keep track of its previous parent though, if any.
        /// > Note that this is an implementation detail and is not exposed through the interface.
        IEntityRecord _previousRecord;

        /// <summary>
        /// Gets or sets the entity that this component is currently attached to.
        /// </summary>
        public IEntityRecord Record {
            /// > The component is in an inconsistent state when the **Record** is not `null` but does not have the component attached to it.
            get {
                return _record;
            }
            
            set {
                bool requiresSynchronization =
                    _previousRecord != null && 
                    _previousRecord.HasComponent(this);

                if (!requiresSynchronization) {
                    if (_record == null || !_record.Equals(value)) {
                        _previousRecord = _record;
                        _record = value;

                        Synchronize();
                    }
                } else {
                    throw new InvalidOperationException("Component has to be synchronized before further changes can happen.");
                }
            }
        }
        
        /// <summary>
        /// The component is considered out-of-sync if it is not attached to any entity.
        /// </summary>
        public bool IsOutOfSync {
            get {
                return
                    Record == null ||
                    !Record.HasComponent(this);
            }
        }
  
        /// <summary>
        /// Ensures that the component becomes synchronized by establishing the appropriate relation to its parent entity.
        /// </summary>
        public void Synchronize() {
            if (Record != null) {
                Record.Add(this);

                InjectDependencies();

                OnAdded(new ComponentStateEventArgs(Record, _previousRecord));
            } else {
                if (_previousRecord != null) {
                    if (_previousRecord.Remove(this)) {
                        OnRemoved(new ComponentStateEventArgs(Record, _previousRecord));

                        ClearDependencies();

                        _previousRecord = null;
                    }
                }
            }
        }

        protected DependencyComponent() {
            /// Since the base constructor is guaranteed to be called (even if implemented in a subclass without calling `base()`), this opportunity is used to immediately discover the required dependencies.
            FindDependencies();
            /// > Note that for dependency injection to work, all components must be instantiable and have an empty constructor. However, it is not necessary to implement one.
        }

        /// <summary>
        /// Discovers which member fields are explicitly marked as dependencies.
        /// </summary>
        void FindDependencies() {
            _dependencies.Clear();

            /// > It wouldn't be complete insanity to ditch the attributing entirely and just consider all `Component`-types as dependencies. 
            /// But, it *would* ultimately be an assumption, and it could very well lead to some *just what exactly is going on behind the scenes?*-confusion.
            FieldInfo[] fields = GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (FieldInfo field in fields) {
                foreach (RequireComponentAttribute dependency in field.GetCustomAttributes(typeof(RequireComponentAttribute), false)) {
                    if (!field.FieldType.IsSubclassOf(typeof(DependencyComponent))) {
                        throw new InvalidOperationException(
                            String.Format(CultureInfo.InvariantCulture, "This field can not be marked as a dependency because its type is not a subclass of 'Component'.", 
                                field.DeclaringType.ToString() + "." + field.Name));
                    }

                    try {
                        _dependencies.Add(field, dependency);
                    } catch (ArgumentNullException ane) {
                        Console.Error.WriteLine(ane.ToString());
                    } catch (ArgumentException ae) {
                        Console.Error.WriteLine(ae.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Goes through all discovered dependencies and injects them one by one.
        /// </summary>
        void InjectDependencies() {
            if (Record == null) {
                return;
            }

            foreach (KeyValuePair<FieldInfo, RequireComponentAttribute> pair in _dependencies) {
                FieldInfo field = pair.Key;
                RequireComponentAttribute dependency = pair.Value;

                /// Determines which entity the dependency should be grabbed from. 
                IEntityRecord record =
                    (dependency.FromRecordNamed != null) ?
                        /// The dependency will **not** be injected if the specified entity is not registered at the time.
                        Entity.Find(dependency.FromRecordNamed, Record.Registry) :
                        Record;

                if (record == null) {
                    continue;
                }

                /// Immediately attempt injecting the component.
                InjectDependency(field, record, allowingDerivedTypes: dependency.AllowDerivedTypes);
            }
        }

        /// <summary>
        /// Attempts to inject a component into a field, and adds the component to the specified entity.
        /// </summary>
        /// <remarks>
        /// > Note that the dependency will remain, even if it becomes dettached from its entity.
        /// </remarks>
        void InjectDependency(FieldInfo field, IEntityRecord record, bool allowingDerivedTypes) {
            if (field == null || record == null) {
                return;
            }

            Type componentType = field.FieldType;
            IComponent dependency = record.Registry.GetComponent(record, componentType, allowingDerivedTypes);

            if (dependency == null) {
                dependency = Create(componentType);

                record.Add(dependency);
            }

            if (dependency != null) {
                field.SetValue(this, dependency);
            }
        }

        /// <summary>
        /// Clears out all managed dependencies.
        /// </summary>
        void ClearDependencies() {
            foreach (KeyValuePair<FieldInfo, RequireComponentAttribute> pair in _dependencies) {
                FieldInfo field = pair.Key;

                field.SetValue(this, null);
            }
        }

        /// <summary>
        /// Occurs when this component is attached to an entity.
        /// </summary>
        protected virtual void OnAdded(ComponentStateEventArgs registrationArgs) { }

        /// <summary>
        /// Occurs when this component is dettached from an entity.
        /// </summary>
        protected virtual void OnRemoved(ComponentStateEventArgs registrationArgs) { }
        /// > Not going to place anything important in those two, because *some people* forget to call base when overriding :(

        /// <summary>
        /// Receives a message from a containing arbitrary data.
        /// </summary>
        public virtual void Receive<T>(string message, T data) { }

        /// <summary>
        /// Helper method to create an instance of a specified type, and casting it to `IComponent`
        /// </summary>
        static IComponent Create(Type type) {
            IComponent result = null;

            try {
                result = Activator.CreateInstance(type) as IComponent;
            } catch (MissingMethodException) {
                throw new MissingMethodException(String.Format(CultureInfo.CurrentCulture,
                    "The component type '{0}' does not implement an empty constructor.", type.ToString()));
            }

            return result;
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.
