using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrameRate : MonoBehaviour
{

    public TextMeshProUGUI textMeshPro;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = "FPS: " + 1.0f / Time.deltaTime;
    }
}
