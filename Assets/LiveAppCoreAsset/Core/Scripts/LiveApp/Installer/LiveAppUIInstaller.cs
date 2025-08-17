using LiveAppUI.Presenter;
using LiveAppUI.View;
using UnityEngine;
using Zenject;

namespace LiveAppCore.Installer
{
    public class LiveAppUIInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuView _mainMenuView = null;

        public override void InstallBindings()
        {
            Container
                .Bind<IMainMenuView>()
                .FromInstance( _mainMenuView )
                .AsSingle();
        }
    }
}
