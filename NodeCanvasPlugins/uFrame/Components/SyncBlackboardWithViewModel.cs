using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas;
using UniRx;
using UniRx.UI;
using UnityEngine;

namespace NodeCanvasAddons.uFrame.Components
{
    public class SubscriptionManagementEntity
    {
        public IDisposable ViewSubscription;
        public Action SubscribeToView { get; set; }
        public Action SubscribeToBlackboard { get; set; }
        public Action UnsubscribeFromView { get; set; }
        public Action UnsubscribeFromBlackboard { get; set; }
    }

    [RequireComponent(typeof(Blackboard))]
    [RequireComponent(typeof(ViewBase))]
    public class SyncBlackboardWithViewModel : MonoBehaviour
    {
        private ViewBase _view;
        private ViewModel _viewModel;
        private Blackboard _blackboard;

        private readonly Dictionary<string, SubscriptionManagementEntity> _subscriptionManager = new Dictionary<string, SubscriptionManagementEntity>();

        void Awake()
        {
            _view = GetComponent<ViewBase>();
            _blackboard = GetComponent<Blackboard>();
            _viewModel = _view.ViewModelObject;
            RefreshBlackboardVarsFromView();
            GenerateSubscriptionManagementList();
        }

        void OnEnable()
        {
            SetupBindings();
        }

        void OnDisable()
        {
            ReleaseBindings();
        }

        void OnDestroy()
        {
            ReleaseBindings();
            CleanBlackboardProperties();
            _subscriptionManager.Clear();
        }

        private void SetupBindings()
        {
            Debug.Log("Enabling " + _subscriptionManager.Count + " Bindings");
            foreach (var subscriptionManagementEntry in _subscriptionManager.Values)
            {
                subscriptionManagementEntry.SubscribeToView();
                subscriptionManagementEntry.SubscribeToBlackboard();
            }
        }

        private void ReleaseBindings()
        {
            Debug.Log("Disabling " + _subscriptionManager.Count + " Bindings");
            foreach (var subscriptionManagementEntry in _subscriptionManager.Values)
            {
                subscriptionManagementEntry.UnsubscribeFromView();
                subscriptionManagementEntry.UnsubscribeFromBlackboard();
            }
        }

        private void CleanBlackboardProperties()
        {
            foreach (var property in _viewModel.Properties.Select(x => x.Property))
            { _blackboard.DeleteData(property.PropertyName); }
        }

        private void RefreshBlackboardVarsFromView()
        {
            if(_subscriptionManager.Count > 0)
            { ReleaseBindings(); }

            foreach (var property in _viewModel.Properties.Select(x => x.Property))
            { _blackboard.SetDataValue(property.PropertyName, property.ObjectValue); }

            if (_subscriptionManager.Count > 0)
            { SetupBindings(); }
        }

        private void GenerateSubscriptionManagementList()
        {
            Debug.Log("Generating Subscriptions For " + _viewModel.Properties.Count + " Properties");
            foreach (var property in _viewModel.Properties.Select(x => x.Property))
            {
                var scopedProperty = property;
                var subscriptionManagementEntity = new SubscriptionManagementEntity();
                var internalPropertyName = string.Format("_{0}Property", scopedProperty.PropertyName);
                var blackboardProperty = _blackboard.GetAllData().Single(x => x.name == scopedProperty.PropertyName);

                var subscriptionMethod = new Action<object>(x =>
                {
                    Debug.Log(string.Format("View [{0}] changed from [{1} to {2}]", scopedProperty.PropertyName, blackboardProperty.objectValue, x));
                    _subscriptionManager[scopedProperty.PropertyName].UnsubscribeFromBlackboard();
                    _blackboard.SetDataValue(scopedProperty.PropertyName, x);
                    _subscriptionManager[scopedProperty.PropertyName].SubscribeToBlackboard();
                });

                subscriptionManagementEntity.SubscribeToView = () => subscriptionManagementEntity.ViewSubscription = scopedProperty.ObserveEveryValueChanged(x => x.ObjectValue).Subscribe(subscriptionMethod);
                subscriptionManagementEntity.UnsubscribeFromView = () => subscriptionManagementEntity.ViewSubscription.Dispose();
                
                var blackboardCallbackAction = new Action<string, object>((varName, value) =>
                {
                    Debug.Log(string.Format("Blackboard [{0}] changed from [{1} to {2}]", scopedProperty.PropertyName, _viewModel[internalPropertyName].Property.ObjectValue, value));
                    _subscriptionManager[scopedProperty.PropertyName].UnsubscribeFromView();
                    _viewModel[internalPropertyName].Property.ObjectValue = value;
                    _subscriptionManager[scopedProperty.PropertyName].SubscribeToView();
                });
                subscriptionManagementEntity.SubscribeToBlackboard = () => blackboardProperty.onValueChanged += blackboardCallbackAction;
                subscriptionManagementEntity.UnsubscribeFromBlackboard = () => blackboardProperty.onValueChanged -= blackboardCallbackAction;

                _subscriptionManager.Add(property.PropertyName, subscriptionManagementEntity);
            }
            Debug.Log("Generated Subscriptions For " + _viewModel.Properties.Count + " Properties");
        }
    }
}
