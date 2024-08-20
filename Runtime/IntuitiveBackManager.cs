using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Zenject;

namespace IntuitiveBackSystem.Runtime{
    /// <summary>
    /// Manager that handles the back action and calls the current IIntuitiveBackHandler.
    /// It will call the OnBack method of the current IIntuitiveBackHandler when the back action is performed.
    /// </summary>
    public class IntuitiveBackManager : IInitializable, ILateDisposable{
        /// <summary>
        /// Event that is called when a new IIntuitiveBackHandler is registered.
        /// </summary>
        public event Action<IIntuitiveBackHandler> onRegister;

        /// <summary>
        /// Event that is called when an IIntuitiveBackHandler is unregistered.
        /// </summary>
        public event Action<IIntuitiveBackHandler> onUnregister;

        /// <summary>
        /// The current IIntuitiveBackHandler that is registered.
        /// </summary>
        public IIntuitiveBackHandler currentIntuitiveBackHandler => backHandlers.Count > 0 ? backHandlers[0] : null;

        /// <summary>
        /// A list of all registered IIntuitiveBackHandler (First in, first out)
        /// </summary>
        public List<IIntuitiveBackHandler> backHandlers{ get; } = new();

        private InputAction _backAction;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="backAction">The back action to listen to.</param>
        public IntuitiveBackManager(InputAction backAction) => _backAction = backAction;

        /// <summary>
        /// Initialize the IntuitiveBackManager.
        /// </summary>
        public void Initialize(){
            _backAction.performed += OnBackPerformed;
        }

        /// <summary>
        /// Dispose the IntuitiveBackManager.
        /// </summary>
        public void LateDispose(){
            _backAction.performed -= OnBackPerformed;
            UnregisterAll();
        }

        private void OnBackPerformed(InputAction.CallbackContext context) =>
            currentIntuitiveBackHandler?.OnBack();

        /// <summary>
        /// Register a new IIntuitiveBackHandler.
        /// </summary>
        /// <param name="intuitiveBackHandler">The IIntuitiveBackHandler to register.</param>
        public void Register(IIntuitiveBackHandler intuitiveBackHandler){
            backHandlers.Insert(0, intuitiveBackHandler);

            if (backHandlers.Count > 0)
                _backAction.Enable();

            onRegister?.Invoke(intuitiveBackHandler);
        }

        /// <summary>
        /// Unregister an IIntuitiveBackHandler.
        /// </summary>
        /// <param name="intuitiveBackHandler">The IIntuitiveBackHandler to unregister.</param>
        public void Unregister(IIntuitiveBackHandler intuitiveBackHandler){
            if (intuitiveBackHandler == null) return;
            if (!backHandlers.Contains(intuitiveBackHandler)) return;

            backHandlers.Remove(intuitiveBackHandler);

            if (backHandlers.Count == 0)
                _backAction.Disable();

            onUnregister?.Invoke(intuitiveBackHandler);
        }

        private void UnregisterAll(){
            for (var i = 0; i < backHandlers.Count; i++)
                Unregister(backHandlers[i]);
        }
    }
}
