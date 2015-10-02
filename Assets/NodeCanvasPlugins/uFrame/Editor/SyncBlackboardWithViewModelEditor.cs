using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.CSharp;
using NodeCanvas.Framework;
using NodeCanvasAddons.uFrame.Components;
using UnityEditor;
using UnityEngine;
using NodeCanvasAddons.uFrame.Extensions;
using uFrame.MVVM;

namespace NodeCanvasAddons.uFrame.Editor
{
    [CustomEditor(typeof(SyncBlackboardWithViewModel))]
    public class SyncBlackboardWithViewModelEditor : UnityEditor.Editor
    {
        private ViewBase _view;
        private Blackboard _blackboard;
        private List<PropertyInfo> _viewModelProperties;

        void OnEnable()
        {
            var component = target as SyncBlackboardWithViewModel;
            _view = component.GetComponent<ViewBase>();
            _blackboard = component.GetComponent<Blackboard>();

            var viewModelProperties = _view.ViewModelType.GetProperties();
            _viewModelProperties = viewModelProperties
                .Where(OnlyValidVMProperties())
                .OrderBy(x => x.Name)
                .ToList();
        }
        
        private void DrawLocatedProperties()
        {
            foreach (var property in _viewModelProperties)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0}<{1}>", property.Name, property.PropertyType.Name));
                GUILayout.EndHorizontal();
            }
        }

        private void DrawActionButtons()
        {
            if (GUILayout.Button("Refresh Bindings"))
            {
                RemovePropertiesFromBlackboard();
                SyncPropertiesToBlackboard();
            }

            if (GUILayout.Button("Clear All Bindings"))
            {
                RemovePropertiesFromBlackboard();
            }
        }

        private Variable CreateTypedVariable(Type valueType)
        {
            Type genericType = typeof(Variable<>).MakeGenericType(valueType);
            return Activator.CreateInstance(genericType) as Variable;
        }

        private void SyncPropertiesToBlackboard()
        {
            
            Debug.Log("- Enabling " + _viewModelProperties.Count + " Properties");
            foreach (var property in _viewModelProperties)
            {
                if (!_blackboard.HasData(property.Name))
                {
                    var variable = CreateTypedVariable(property.PropertyType);
                    variable.name = property.Name;
                    _blackboard.variables.Add(property.Name, variable);
                }
            }
        }

        private static Func<PropertyInfo, bool> OnlyValidVMProperties()
        {
            return x =>
            {
                var keywords = new[] { "Disposer", "Properties", "References", "Identifier", "Controller", "Bindings", "Item", "Dirty" };
                if(x.Name.Contains("Property")) {  return false; }
                if(keywords.Contains(x.Name)) {  return false; }
                return true;
            };
        }

        private void RemovePropertiesFromBlackboard()
        {
            Debug.Log("- Removing " + _viewModelProperties.Count() + " Properties");
            foreach (var property in _viewModelProperties)
            {
                if (_blackboard.variables.ContainsKey(property.Name))
                { _blackboard.variables.Remove(property.Name); }
            }
        }
       
        public override void OnInspectorGUI()
        {
            GUILayout.Space(10.0f);

            GUILayout.Label("Located Properties: ", new GUIStyle {fontStyle = FontStyle.Bold});
            DrawLocatedProperties();
            DrawActionButtons();
        }
    }
}
