/// ###TL;DR..
/// 
/// When a component switches entity, these are used.

/// ##Source
using System;

namespace ComponentKit.Model {
    /// <summary>
    /// Provides the entities involved in a change of state for a component.
    /// </summary>
    /// <remarks>
    /// > For example, when components are **added** or **removed** these `EventArgs` will be provided. 
    /// </remarks>
    public class ComponentStateEventArgs : EntityEventArgs {
        public ComponentStateEventArgs(IEntityRecord entity)
            : base(entity) { }

        public ComponentStateEventArgs(IEntityRecord entity, IEntityRecord previousEntity)
            : base(entity) {
            PreviousRecord = previousEntity;
        }

        /// <summary>
        /// Gets the entity that the component was previously attached to, or `null` if none.
        /// </summary>
        public IEntityRecord PreviousRecord {
            get;
            private set;
        }

        /// <summary>
        /// Gets the entity that the component is currently attached to, or `null` if it was just dettached.
        /// </summary>
        public override IEntityRecord Record {
            get {
                return base.Record;
            }
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.