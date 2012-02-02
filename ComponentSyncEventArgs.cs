/// ###TL;DR..
/// 
/// When a bunch of components requires processing from **trigger handlers**, these are used.

/// ##Source
using System;
using System.Linq;
using System.Collections.Generic;

namespace ComponentKit {
    /// <summary>
    /// Encapsulates a method that determines whether a component enables a trigger.
    /// </summary>
    public delegate bool ComponentSyncTriggerPredicate(IComponent component);

    /// <summary>
    /// Provides the components that enabled a trigger during a synchronization operation.
    /// </summary>
    public sealed class ComponentSyncEventArgs : EventArgs {
        public ComponentSyncEventArgs(IEnumerable<IComponent> components) {
            Components = components;
        }

        /// <summary>
        /// Gets the components that enabled a trigger.
        /// </summary>
        public IEnumerable<IComponent> Components {
            get;
            private set;
        }

        /// <summary>
        /// Retrieves all components that have been dettached.
        /// </summary>
        public IEnumerable<IComponent> DettachedComponents {
            get {
                return Components.Where(c => c.IsOutOfSync);
            }
        }

        /// <summary>
        /// Retrieves all components that have been attached.
        /// </summary>
        public IEnumerable<IComponent> AttachedComponents {
            get {
                return Components.Where(c => !c.IsOutOfSync);
            }
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.