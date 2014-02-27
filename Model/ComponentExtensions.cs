namespace ComponentKit.Model {
    public static class ComponentExtensions {
        public static bool Remove(this IComponent component) {
            if (component.Record != null) {
                return component.Record.Remove(component);
            }

            return false;
        }
    }
}