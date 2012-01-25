/// ###TL;DR..
/// 
/// The `DependencyComponent` is a `Component` that is aware of its dependencies, and will automatically attempt to inject them during synchronization.
/// 

/// ##Source
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ComponentKit.Model {
    /// <summary>
    /// Represents a component that can specify and inject dependencies.
    /// </summary>
    public abstract class DependencyComponent : Component {
        Dictionary<FieldInfo, RequireComponentAttribute> _dependencies =
            new Dictionary<FieldInfo, RequireComponentAttribute>();

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

            /// > It wouldn't be complete insanity to ditch the attributing entirely and just consider all `IComponent`-types as dependencies. 
            /// But, it *would* ultimately be an assumption, and it could very well lead to some *just what exactly is going on behind the scenes?*-confusion.
            FieldInfo[] fields = GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic);

            foreach (FieldInfo field in fields) {
                foreach (RequireComponentAttribute dependency in field.GetCustomAttributes(typeof(RequireComponentAttribute), false)) {
                    Type[] matchingInterfaces = field.FieldType.FindInterfaces(IsTypeEqualToName, "ComponentKit.IComponent");

                    if (matchingInterfaces == null || matchingInterfaces.Length == 0) {
                        throw new InvalidOperationException(
                            String.Format(CultureInfo.InvariantCulture, "This field can not be marked as a dependency because its type does not implement 'IComponent'.", 
                                field.DeclaringType.ToString() + "." + field.Name));
                    }

                    try {
                        _dependencies.Add(field, dependency);
                    } catch (ArgumentNullException) {
                        continue;
                    } catch (ArgumentException) {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the given string filter is equal to the name of the type.
        /// </summary>
        bool IsTypeEqualToName(Type m, object filterCriteria) {
            if (filterCriteria is string) {
                return m.Name == (string)filterCriteria;
            }

            return false;
        }

        /// <summary>
        /// Occurs when the component is attached to an entity. All dependencies are injected.
        /// </summary>
        protected override void OnAdded(ComponentStateEventArgs registrationArgs) {
            base.OnAdded(registrationArgs);

            InjectDependencies();
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
        /// Occurs when the component is dettached from an entity. All managed dependencies are null'ed.
        /// </summary>
        /// <remarks>
        /// > If you don't want the dependencies to get lost, then override this method and don't base.
        /// </remarks>
        protected override void OnRemoved(ComponentStateEventArgs registrationArgs) {
            base.OnRemoved(registrationArgs);

            ClearDependencies();
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