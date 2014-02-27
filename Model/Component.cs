using System;

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a single component of an entity.
    /// </summary>
    /// <remarks>
    /// The `Component` is sub-classed to build custom behavior.
    /// </remarks>
    public abstract class Component : IComponent {
        /// It can only have one immediate parent.
        IEntityRecord _record;

        /// It does keep track of its previous parent though, if any.
        /// Note that this is an implementation detail and is not exposed through the interface.
        IEntityRecord _previousRecord;

        /// <summary>
        /// Gets or sets the entity that this component is currently attached to.
        /// </summary>
        public IEntityRecord Record {
            /// The component is in an inconsistent state when the **Record** is *not* `null` 
            /// but does not have the component attached to it.
            get {
                return _record;
            }

            set {
                bool requiresSynchronization =
                    _previousRecord != null &&
                    _previousRecord.HasComponent(this);

                if (requiresSynchronization) {
                    throw new InvalidOperationException(
                        "Component has to be synchronized before further changes can happen.");
                } else {
                    if (_record == null || !_record.Equals(value)) {
                        _previousRecord = _record;
                        _record = value;

                        ComponentStateEventArgs stateChange = 
                            new ComponentStateEventArgs(_record, _previousRecord);

                        if (_record != null) {
                            OnAdded(stateChange);
                        } else {
                            OnRemoved(stateChange);
                        }
                    }
                }
            }
        }

        public override string ToString() {
            return String.Format(
                "{0}{1}", 
                GetType().GetPrettyName(),
                IsOutOfSync ? "*" : string.Empty);
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
        
        public event EventHandler<ComponentStateEventArgs> Added;
        public event EventHandler<ComponentStateEventArgs> Removed;
        
        /// <summary>
        /// Occurs when the component is attached to an entity.
        /// </summary>
        protected virtual void OnAdded(ComponentStateEventArgs registrationArgs) {
            if (Added != null) {
                Added(this, registrationArgs);
            }
        }

        /// <summary>
        /// Occurs when the component is dettached from an entity.
        /// </summary>
        protected virtual void OnRemoved(ComponentStateEventArgs registrationArgs) {
            if (Removed != null) {
                Removed(this, registrationArgs);
            }
        }

        /// <summary>
        /// Receives a message from a containing arbitrary data.
        /// </summary>
        public virtual void Receive<TData>(string message, TData data) { }

        /// `IDisposable` pattern implementation as described in
        /// [Game Engine Toolset Development](http://www.amazon.com/Engine-Toolset-Development-Graham-Wihlidal/dp/1592009638) by Graham Wihlidal.
        
        ~Component() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            /// Preventing any multi-threading issues by locking this instance.
            /// Sub-classes should also lock, and should call `base.Dispose(disposing)` last, within the lock context.
            lock (this) {
                if (disposing) {
                    
                }
            }
        }

        /// <summary>
        /// Helper method to create an instance of a specified type, and casting it to `IComponent`
        /// </summary>
        public static IComponent Create(Type type) {
            IComponent result = null;

            try {
                result = Activator.CreateInstance(type) as IComponent;
            } catch (MissingMethodException) {
                throw new MissingMethodException(String.Format(
                    "The component type '{0}' does not provide a parameter-less constructor.", type.ToString()));
            }

            return result;
        }

        /// <summary>
        /// Determines whether the given type implements the `IComponent` interface.
        /// </summary>
        public static bool CanCreate(Type type) {
          return typeof(IComponent).IsAssignableFrom(type);
        }
    }
}