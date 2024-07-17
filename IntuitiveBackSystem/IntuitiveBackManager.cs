using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Zenject;
using static UnityEngine.InputSystem.InputAction;

namespace IntuitiveBackSystem
{
    public class IntuitiveBackManager : IInitializable, ILateDisposable
    {
        public event Action<IBackHandler> OnRegister;
        public event Action<IBackHandler> OnUnregister;

        public IBackHandler CurrentBackHandler => BackHandlers.Count > 0 ? BackHandlers[0] : null;
        public List<IBackHandler> BackHandlers { get; } = new();

        private InputAction _backAction;
        
        public IntuitiveBackManager(InputAction backAction) => _backAction = backAction;

        public void Initialize()
        {
            _backAction.performed += OnBackPerformed;
        }
        
        public void LateDispose()
        {
            _backAction.performed -= OnBackPerformed;
            
            UnregisterAll();
        }

        private void OnBackPerformed(CallbackContext context) => 
            CurrentBackHandler?.OnBack();

        public void Register(IBackHandler backHandler)
        {
            BackHandlers.Insert(0, backHandler);

            if (BackHandlers.Count > 0)
                _backAction.Enable();
            
            OnRegister?.Invoke(backHandler);
        }
        
        public void Unregister(IBackHandler backHandler)
        {
            if (backHandler == null) return;
            if (!BackHandlers.Contains(backHandler)) return;
            
            BackHandlers.Remove(backHandler);
            
            if (BackHandlers.Count == 0)
                _backAction.Disable();
            
            OnUnregister?.Invoke(backHandler);
        }
        
        private void UnregisterAll()
        {
            for (var i = 0; i < BackHandlers.Count; i++) 
                Unregister(BackHandlers[i]);
        }
    }
}
