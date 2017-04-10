// Sharp Essentials
// Copyright 2017 Matthew Hamilton - matthamilton@live.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace SharpEssentials.Controls.Localization
{
    /// <summary>
    /// Defines a base class for markup extensions which are managed by a central 
    /// <see cref="MarkupExtensionManager"/>. This allows the associated markup targets to be
    /// updated via the manager.
    /// </summary>
    /// <remarks>
    /// The ManagedMarkupExtension holds a weak reference to the target object to allow it to update 
    /// the target.  A weak reference is used to avoid a circular dependency which would prevent the
    /// target being garbage collected.  
    /// </remarks>
    public abstract class ManagedMarkupExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of a markup extension.
        /// </summary>
        protected ManagedMarkupExtension(MarkupExtensionManager manager)
        {
            manager.RegisterExtension(this);
        }

        /// <summary>
        /// Returns the value for this instance of the Markup Extension.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <returns>The value of the element</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            RegisterTarget(serviceProvider);

            // When used in a template the _targetProperty may be null - in this case
            // return this.
            return TargetProperty != null ? GetValue() : this;
        }

        /// <summary>
        /// Called by <see cref="ProvideValue(IServiceProvider)"/> to register the target and object
        /// using the extension.   
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        protected virtual void RegisterTarget(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
            object target = provideValueTarget.TargetObject;

            // Check if the target is a SharedDp which indicates the target is a template
            // In this case we don't register the target and ProvideValue returns this
            // allowing the extension to be evaluated for each instance of the template.
            if (target != null && target.GetType().FullName != "System.Windows.SharedDp")
            {
                TargetProperty = provideValueTarget.TargetProperty;
                TargetObjects.Add(new WeakReference(target));
            }
        }

        /// <summary>
        /// Called by <see cref="UpdateTargets"/> to update each target referenced by the extension
        /// </summary>
        /// <param name="target">The target to update</param>
        protected virtual void UpdateTarget(object target)
        {
            switch (TargetProperty)
            {
                case DependencyProperty dependencyProperty when target is DependencyObject dependencyObject:
                    dependencyObject.SetValue(dependencyProperty, GetValue());
                    break;

                case PropertyInfo property:
                    property.SetValue(target, GetValue(), null);
                    break;
            }
        }

        /// <summary>
        /// Update the associated targets
        /// </summary>
        public void UpdateTargets()
        {
            foreach (var reference in TargetObjects)
            {
                if (reference.IsAlive)
                {
                    UpdateTarget(reference.Target);
                }
            }
        }

        /// <summary>
        /// Checks whether the given object is the target for the extension.
        /// </summary>
        /// <param name="target">The target to check</param>
        /// <returns>True if the object is one of the targets for this extension</returns>
        public bool IsTarget(object target) => TargetObjects.Any(reference => reference.IsAlive &&
                                                                              reference.Target == target);

        /// <summary>
        /// Is an associated target still alive, ie. not garbage collected.
        /// </summary>
        public bool IsTargetAlive
        {
            get 
            {
                // For normal elements the TargetObjects.Count will always be 1
                // for templates the Count may be zero if this method is called
                // in the middle of window elaboration after the template has been
                // instantiated but before the elements that use it have been. In
                // this case return true so that we don't unhook the extension
                // prematurely.
                if (!TargetObjects.Any())
                    return true;
                
                // Otherwise, just check whether the referenced target(s) are alive.
                return TargetObjects.Any(reference => reference.IsAlive);
            } 
        }

        /// <summary>
        /// Returns true if a target attached to this extension is in design mode.
        /// </summary>
        public bool IsInDesignMode =>
            TargetObjects
                .Select(reference => reference.Target)
                .OfType<DependencyObject>()
                .Any(element => element != null && DesignerProperties.GetIsInDesignMode(element));

        /// <summary>
        /// Return the target objects the extension is associated with.
        /// </summary>
        /// <remarks>
        /// For normal elements their will be a single target. For templates
        /// their may be zero or more targets
        /// </remarks>
        protected ICollection<WeakReference> TargetObjects { get; } = new List<WeakReference>();

        /// <summary>
        /// Return the Target Property the extension is associated with
        /// </summary>
        /// <remarks>
        /// Can either be a <see cref="DependencyProperty"/> or <see cref="PropertyInfo"/>
        /// </remarks>
        protected object TargetProperty { get; private set; }

        /// <summary>
        /// The type of the Target Property.
        /// </summary>
        protected Type TargetPropertyType
        {
            get
            {
                switch (TargetProperty)
                {
                    case DependencyProperty dependencyProperty:
                        return dependencyProperty.PropertyType;
                    case PropertyInfo property:
                        return property.PropertyType;
                    default:
                        return TargetProperty?.GetType();
                }
            }
        }

        /// <summary>
        /// Returns the value associated with the key from the resource manager.
        /// </summary>
        /// <returns>The value from the resources if possible otherwise the default value</returns>
        protected abstract object GetValue();
    }
}
