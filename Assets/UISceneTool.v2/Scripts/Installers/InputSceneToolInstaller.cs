using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[Serializable]
public class InputSceneToolInstaller : MonoInstaller
{
    public InputActionAsset inputActionAsset;
    public List<string> actionNames = new List<string>();
    public override void InstallBindings()
    {
        Container.Bind<InputSceneTool>().AsSingle().WithArguments(inputActionAsset, actionNames);
    }
}