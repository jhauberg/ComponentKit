/// ###TL;DR..
/// 
/// When you want to reference a component automatically, you use this.
/// 
/// This attribute is not very practical when reach goes beyond referencing components 
/// from the same entity; you see, in order to retrieve the component from somewhere
/// else, you first need to be able to identify the target. But, in a dynamic scenario 
/// it's unlikely that the target is known at compile time.

/// ##Source
using System;

namespace ComponentKit.Model {
    /// <summary>
    /// Indicates that an `IComponent` field should be treated as a dependency.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RequireComponentAttribute : Attribute {
        public RequireComponentAttribute() {
            /// > Default to automatically injecting if possible.
            Automatically = true;
        }

        /// <summary>
        /// Gets or sets the entity that the dependency should be retrieved from.
        /// </summary>
        /// <remarks>
        /// > If this is not specified, it defaults to the entity that the component is currently attached to.
        /// </remarks>
        public string FromRecordNamed {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the dependency should be automatically injected if possible.
        /// </summary>
        public bool Automatically {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the dependency can be a subclass of the specified component type.
        /// </summary>
        public bool AllowDerivedTypes {
            get;
            set;
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.