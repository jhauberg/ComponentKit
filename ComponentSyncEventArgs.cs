/// ###TL;DR..

/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;
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
    }
}

/// Copyright 2012 Jacob H. Hansen.