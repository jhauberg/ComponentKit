/// ###TL;DR..
/// 
/// The `Component` is used to build custom behavior.

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

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a single component of an entity.
    /// </summary>
    public abstract class Component : IComponent {
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
                if (!Record.HasComponent(this)) {
                    Record.Add(this);

                    OnAdded(new ComponentStateEventArgs(Record, _previousRecord));
                }
            } else {
                if (_previousRecord != null) {
                    if (_previousRecord.Remove(this)) {
                        OnRemoved(new ComponentStateEventArgs(Record, _previousRecord));

                        _previousRecord = null;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the component is attached to an entity.
        /// </summary>
        protected virtual void OnAdded(ComponentStateEventArgs registrationArgs) { }

        /// <summary>
        /// Occurs when the component is dettached from an entity.
        /// </summary>
        protected virtual void OnRemoved(ComponentStateEventArgs registrationArgs) { }

        /// <summary>
        /// Receives a message from a containing arbitrary data.
        /// </summary>
        public virtual void Receive<T>(string message, T data) { }
    }
}
/// Copyright 2012 Jacob H. Hansen.