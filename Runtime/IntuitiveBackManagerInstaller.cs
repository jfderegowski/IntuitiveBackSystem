using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace IntuitiveBackSystem.Runtime{
    public class IntuitiveBackManagerInstaller : MonoInstaller{
        [SerializeField] private InputActionReference _inputActionReference;

        public override void InstallBindings(){
            var intuitiveBackManager = new IntuitiveBackManager(_inputActionReference.action);

            Container.BindInterfacesAndSelfTo<IntuitiveBackManager>()
                .FromInstance(intuitiveBackManager).AsSingle().NonLazy();
        }
    }
}