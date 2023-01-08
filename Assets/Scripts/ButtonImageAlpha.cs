using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageAlpha : MonoBehaviour
{
    private Image image;  

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        image.alphaHitTestMinimumThreshold = 0.99f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
