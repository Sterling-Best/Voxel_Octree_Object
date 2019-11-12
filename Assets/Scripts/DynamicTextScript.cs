using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicTextScript : MonoBehaviour
{

    public Text displayText;
    // Start is called before the first frame update
    void Start()
    {
        displayText.text = "Testing Sucessful";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
