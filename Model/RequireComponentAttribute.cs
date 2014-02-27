using System;

namespace ComponentKit.Model {
    /// <summary>
    /// Indicates that an `IComponent` field should be treated as a dependency.
    /// </summary>
    /// <remarks>
    /// When you want to reference a component automatically, you use this.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RequireComponentAttribute : Attribute {
        public RequireComponentAttribute() {
            /// Default to automatically injecting if possible.
            Automatically = true;
        }

        /// <summary>
        /// Gets or sets the entity that the dependency should be retrieved from.
        /// </summary>
        /// <remarks>
        /// If this is not specified, it defaults to the entity that the component is currently attached to.
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