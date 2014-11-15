using System;

namespace NodeCanvasAddons.uFrame.Extensions
{
    public static class ViewExtensions
    {
        /// <summary>
        /// Subscribes to the property and returns an action to unsubscribe.
        /// </summary>
        /// <typeparam name="TBindingType"></typeparam>
        /// <param name="modelProperty">The ViewModel Property to bind to.</param>
        /// <param name="onChange">When the property has changed.</param>
        /// <returns>An action to will unsubsribe.</returns>
        public static Action Subscribe(this IViewModelObserver behaviour, ModelPropertyBase modelProperty, Action<object> onChange)
        {
            var action = new ModelPropertyBase.PropertyChangedHandler(value => onChange(value));
            modelProperty.ValueChanged += action;
            return () => modelProperty.ValueChanged -= action;
        }
    }
}
