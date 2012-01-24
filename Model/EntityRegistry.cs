/// ###TL;DR..
/// 
/// Too bad, because this part is missing right now ^_^

/// ##Source
using System;

namespace ComponentKit.Model {
    /// <summary>
    /// Provides factory methods for creating entity registries.
    /// </summary>
    public static class EntityRegistry {
        static readonly IEntityRecordCollection _defaultRegistry = Create();
        static IEntityRecordCollection _activeRegistry = _defaultRegistry;

        /// <summary>
        /// Gets the currently active registry.
        /// </summary>
        public static IEntityRecordCollection Current {
            get {
                return _activeRegistry;
            }
            
            set {
                /// You can set the active registry to something else..
                if (value != null) {
                    _activeRegistry = value;
                } else {
                    /// Or null it, and it will automatically fall back to the default one.
                    _activeRegistry = _defaultRegistry;
                }
            }
        }

        /// <summary>
        /// Creates a new entity registry.
        /// </summary>
        public static IEntityRecordCollection Create() {
            return new EntityRecordStore();
        }
    }
}

/// Copyright 2012 Jacob H. Hansen.