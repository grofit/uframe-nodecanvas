using System;
using System.Linq;
using NodeCanvas;
using NodeCanvasAddons.uFrame.Components;
using UnityEditor;
using UnityEngine;
using NodeCanvasAddons.uFrame.Extensions;

namespace NodeCanvasAddons.uFrame.Editor
{
    [CustomEditor(typeof(SyncBlackboardWithViewModel))]
    public class SyncBlackboardWithViewModelEditor : UnityEditor.Editor
    {
        private ViewBase _view;
        private ViewModel _viewModel;
        private Blackboard _blackboard;

        void OnDisable()
        {
            RemovePropertiesFromBlackboard();
        }

        void OnEnable()
        {
            var component = target as SyncBlackboardWithViewModel;
            _view = component.GetComponent<ViewBase>();
            _viewModel = Activator.CreateInstance(_view.ViewModelType) as ViewModel;
            _blackboard = component.GetComponent<Blackboard>();

            SyncPropertiesToBlackboard();
        }

        private void DrawLocatedProperties()
        {
            foreach (var property in _viewModel.GetViewModelProperties().Select(x => x.Property))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(string.Format("{0}<{1}>", property.PropertyName, property.ValueType.Name));
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
        }

        private void SyncPropertiesToBlackboard()
        {
            Debug.Log("- Enabling " + _viewModel.GetViewModelProperties().Count + " Properties");
            foreach (var property in _viewModel.GetViewModelProperties().Select(x => x.Property))
            {
                if(!_blackboard.HasData(property.PropertyName))
                { _blackboard.SetDataValue(property.PropertyName, property.ObjectValue ?? "Empty"); }
            }
        }

        private void RemovePropertiesFromBlackboard()
        {
            Debug.Log("- Removing " + _viewModel.GetViewModelProperties().Count + " Properties");
            foreach (var property in _viewModel.GetViewModelProperties().Select(x => x.Property))
            {
                if (_blackboard.HasData(property.PropertyName))
                { _blackboard.DeleteData(property.PropertyName); }
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
