using System.Collections.Generic;

namespace ComponentKit.Model {
    /// <summary>
    /// Convenient methods for dealing with entities.
    /// </summary>
    public static class EntityRecordExtensions {
        /// ###Manipulation

        /// <summary>
        /// Attaches a component to this entity.
        /// </summary>
        public static bool Add(this IEntityRecord entity, IComponent component) {
            if (entity.Registry != null) {
                return entity.Registry.Add(entity, component);
            }

            return false;
        }

        /// <summary>
        /// Dettaches a component from this entity.
        /// </summary>
        public static bool Remove(this IEntityRecord entity, IComponent component) {
            if (entity.Registry != null) {
                return entity.Registry.Remove(entity, component);
            }

            return false;
        }

        /// <summary>
        /// Unregisters this entity from its registry.
        /// </summary>
        public static bool Drop(this IEntityRecord entity) {
            if (entity.Registry != null) {
                return entity.Registry.Drop(entity);
            }

            return false;
        }

        /// ###Component retrieval

        /// <summary>
        /// Returns a component of the specified type if it is attached to this entity.
        /// </summary>
        public static TComponent GetComponent<TComponent>(this IEntityRecord entity)
            where TComponent : class, IComponent {
            if (entity.Registry != null) {
                return entity.Registry.GetComponent<TComponent>(entity);
            }

            return default(TComponent);
        }

        /// <summary>
        /// Returns a component of the specified type if it is attached to this entity.
        /// </summary>
        public static TComponent GetComponent<TComponent>(this IEntityRecord entity, TComponent component)
            where TComponent : class, IComponent {
            if (entity.Registry != null) {
                return entity.Registry.GetComponent(entity, component);
            }

            return default(TComponent);
        }

        /// <summary>
        /// If `allowingDerivedTypes` is `true`, returns any component that is either a subclass of, or is, the specified type if it is attached to this entity.
        /// </summary>
        public static TComponent GetComponent<TComponent>(this IEntityRecord entity, TComponent component, bool allowingDerivedTypes)
            where TComponent : class, IComponent {
            if (entity.Registry != null) {
                return entity.Registry.GetComponent(entity, component, allowingDerivedTypes);
            }

            return default(TComponent);
        }

        /// <summary>
        /// Gets all components currently attached to this entity.
        /// </summary>
        public static IEnumerable<IComponent> GetComponents(this IEntityRecord entity) {
            if (entity.Registry != null) {
                return entity.Registry.GetComponents(entity);
            }

            return null;
        }

        public static bool HasComponent<TComponent>(this IEntityRecord entity)
            where TComponent : class, IComponent {
            return GetComponent<TComponent>(entity) != null;
        }

        public static bool HasComponent<TComponent>(this IEntityRecord entity, TComponent component)
            where TComponent : class, IComponent {
            return GetComponent<TComponent>(entity, component) != null;
        }
    }
}
