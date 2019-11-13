using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class DynamicTextScript : MonoBehaviour
{
    public InputMaster controls;
    // public TextElement displayText;
    public GameObject ChunkDebugPan;
    

    void Awake()
    {
        controls = new InputMaster();
        controls.Enable();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        this.controls.Debug.ChunkDebugActivate.performed += _ => this.HideDebugMenu();
    }

    // Update is called once per frame
    void Update()
    {

            

    }

    void HideDebugMenu()
    {
        ChunkDebugPan.SetActive(!ChunkDebugPan.activeSelf);
        Debug.Log("Enable DebugMenu!");
        //stateChunkPan = ChunkDebugPan.
    }
}
