// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    private InputActionAsset asset;
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Debug"",
            ""id"": ""4e2663e4-9e72-433a-b00d-f234b38f81d2"",
            ""actions"": [
                {
                    ""name"": ""ChunkDebugActivate"",
                    ""type"": ""Button"",
                    ""id"": ""04ecada1-68b7-41c8-8e06-3e54d2637d9c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5bddf250-46a5-4078-a4f0-665767d5f6df"",
                    ""path"": ""<Keyboard>/#(T)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""ChunkDebugActivate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_ChunkDebugActivate = m_Debug.FindAction("ChunkDebugActivate", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_ChunkDebugActivate;
    public struct DebugActions
    {
        private @InputMaster m_Wrapper;
        public DebugActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @ChunkDebugActivate => m_Wrapper.m_Debug_ChunkDebugActivate;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                @ChunkDebugActivate.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnChunkDebugActivate;
                @ChunkDebugActivate.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnChunkDebugActivate;
                @ChunkDebugActivate.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnChunkDebugActivate;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ChunkDebugActivate.started += instance.OnChunkDebugActivate;
                @ChunkDebugActivate.performed += instance.OnChunkDebugActivate;
                @ChunkDebugActivate.canceled += instance.OnChunkDebugActivate;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IDebugActions
    {
        void OnChunkDebugActivate(InputAction.CallbackContext context);
    }
}
