using System;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas;
using NodeCanvas.Framework;
using uFrame.Kernel;
using uFrame.MVVM;
using UniRx;
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

        void Start()
        {
            _view = GetComponent<ViewBase>();
            _blackboard = GetComponent<Blackboard>();
            
            
            uFrameKernel.EventAggregator
                .GetEvent<SceneLoaderEvent>()
                .First(x => x.SceneRoot is ExampleScene && x.State == SceneState.Loaded)
                .Subscribe(OnSceneLoaded());
        }

        private Action<SceneLoaderEvent> OnSceneLoaded()
        {
            return x =>
            {
                _viewModel = _view.GetViewModel();
                RefreshBlackboardVarsFromView();
                GenerateSubscriptionManagementList();
                SetupBindings();
            };
        }

        void OnEnable()
        {
            if (_viewModel == null) { return; }
            SetupBindings();
        }

        void OnDisable()
        {
            if (_viewModel == null) { return; }
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
#if UNITY_EDITOR
            Debug.Log("Enabling " + _subscriptionManager.Count + " Bindings");
#endif
            foreach (var subscriptionManagementEntry in _subscriptionManager.Values)
            {
                subscriptionManagementEntry.SubscribeToView();
                subscriptionManagementEntry.SubscribeToBlackboard();
            }
        }

        private void ReleaseBindings()
        {
#if UNITY_EDITOR
            Debug.Log("Disabling " + _subscriptionManager.Count + " Bindings");
#endif
            foreach (var subscriptionManagementEntry in _subscriptionManager.Values)
            {
                subscriptionManagementEntry.UnsubscribeFromView();
                subscriptionManagementEntry.UnsubscribeFromBlackboard();
            }
        }

        private void CleanBlackboardProperties()
        {
            foreach (var property in _viewModel.Properties.Select(x => x.Property))
            { _blackboard.variables.Remove(property.PropertyName); }
        }

        private void RefreshBlackboardVarsFromView()
        {
            if(_subscriptionManager.Count > 0)
            { ReleaseBindings(); }

            foreach (var property in _viewModel.Properties.Select(x => x.Property))
            { _blackboard.variables[property.PropertyName].value = property.ObjectValue; }

            if (_subscriptionManager.Count > 0)
            { SetupBindings(); }
        }

        private void GenerateSubscriptionManagementList()
        {
#if UNITY_EDITOR
            Debug.Log("Generating Subscriptions For " + _viewModel.Properties.Count + " Properties");
#endif
            foreach (var property in _viewModel.Properties.Select(x => x.Property).ToList())
            {
                var scopedProperty = property;
                var subscriptionManagementEntity = new SubscriptionManagementEntity();
                var blackboardProperty = _blackboard.variables[scopedProperty.PropertyName];

                var subscriptionMethod = new Action<object>(x =>
                {
#if UNITY_EDITOR
                    Debug.Log(string.Format("View [{0}] changed from [{1} to {2}]", scopedProperty.PropertyName, blackboardProperty.value, x));
#endif
                    _subscriptionManager[scopedProperty.PropertyName].UnsubscribeFromBlackboard();
                    _blackboard.variables[scopedProperty.PropertyName].value = x;
                    _subscriptionManager[scopedProperty.PropertyName].SubscribeToBlackboard();
                });

                subscriptionManagementEntity.SubscribeToView = () => subscriptionManagementEntity.ViewSubscription = scopedProperty.ObserveEveryValueChanged(x => x.ObjectValue).Subscribe(subscriptionMethod);
                subscriptionManagementEntity.UnsubscribeFromView = () => subscriptionManagementEntity.ViewSubscription.Dispose();
                
                var blackboardCallbackAction = new Action<string, object>((varName, value) =>
                {
#if UNITY_EDITOR
                    Debug.Log(string.Format("Blackboard [{0}] changed from [{1} to {2}]", scopedProperty.PropertyName, scopedProperty.ObjectValue, value));
#endif
                    _subscriptionManager[scopedProperty.PropertyName].UnsubscribeFromView();
                    scopedProperty.ObjectValue = value;
                    _subscriptionManager[scopedProperty.PropertyName].SubscribeToView();
                });
                subscriptionManagementEntity.SubscribeToBlackboard = () => blackboardProperty.onValueChanged += blackboardCallbackAction;
                subscriptionManagementEntity.UnsubscribeFromBlackboard = () => blackboardProperty.onValueChanged -= blackboardCallbackAction;

                _subscriptionManager.Add(property.PropertyName, subscriptionManagementEntity);
            }
#if UNITY_EDITOR
            Debug.Log("Generated Subscriptions For " + _viewModel.Properties.Count + " Properties");
#endif
        }
    }
}
