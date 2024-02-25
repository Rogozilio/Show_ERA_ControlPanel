using UnityEngine.InputSystem;
using Zenject;

public class InputSceneToolInstaller : MonoInstaller
{
    public InputActionAsset inputActionAsset;
    public override void InstallBindings()
    {
        Container.Bind<InputSceneTool>().AsSingle().WithArguments(inputActionAsset);
    }
}