using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Zenject;
using static UnityEngine.InputSystem.InputAction;

namespace IntuitiveBackSystem
{
    /// <summary>
    /// Manager that handles the back action and calls the current IIntuitiveBackHandler.
    /// It will call the OnBack method of the current IIntuitiveBackHandler when the back action is performed.
    /// </summary>
    public class IntuitiveBackManager : IInitializable, ILateDisposable
    {
        /// <summary>
        /// Event that is called when a new IIntuitiveBackHandler is registered.
        /// </summary>
        public event Action<IIntuitiveBackHandler> OnRegister;
        /// <summary>
        /// Event that is called when an IIntuitiveBackHandler is unregistered.
        /// </summary>
        public event Action<IIntuitiveBackHandler> OnUnregister;
        
        /// <summary>
        /// The current IIntuitiveBackHandler that is registered.
        /// </summary>
        public IIntuitiveBackHandler CurrentIntuitiveBackHandler => BackHandlers.Count > 0 ? BackHandlers[0] : null;
        /// <summary>
        /// A list of all registered IIntuitiveBackHandler (First in, first out)
        /// </summary>
        public List<IIntuitiveBackHandler> BackHandlers { get; } = new();

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
            CurrentIntuitiveBackHandler?.OnBack();

        /// <summary>
        /// Register a new IIntuitiveBackHandler.
        /// </summary>
        /// <param name="intuitiveBackHandler">The IIntuitiveBackHandler to register.</param>
        public void Register(IIntuitiveBackHandler intuitiveBackHandler)
        {
            BackHandlers.Insert(0, intuitiveBackHandler);

            if (BackHandlers.Count > 0)
                _backAction.Enable();
            
            OnRegister?.Invoke(intuitiveBackHandler);
        }
        
        /// <summary>
        /// Unregister an IIntuitiveBackHandler.
        /// </summary>
        /// <param name="intuitiveBackHandler">The IIntuitiveBackHandler to unregister.</param>
        public void Unregister(IIntuitiveBackHandler intuitiveBackHandler)
        {
            if (intuitiveBackHandler == null) return;
            if (!BackHandlers.Contains(intuitiveBackHandler)) return;
            
            BackHandlers.Remove(intuitiveBackHandler);
            
            if (BackHandlers.Count == 0)
                _backAction.Disable();
            
            OnUnregister?.Invoke(intuitiveBackHandler);
        }
        
        private void UnregisterAll()
        {
            for (var i = 0; i < BackHandlers.Count; i++) 
                Unregister(BackHandlers[i]);
        }
    }
}
