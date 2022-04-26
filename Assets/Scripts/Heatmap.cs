using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap : MonoBehaviour
{
    Color color;
    Renderer renderer;
    // Start is called before the first frame update
    void Awake()
    {
     
        renderer = GetComponent<Renderer>();
        color = renderer.material.color;
    }

    public void IncrementColor()
    {
        if (color.r < 1)
            color.r += 1f/300;
        else if (color.g < 1)
            color.g += 1f / 300;
        else if (color.b < 1)
            color.b += 1f / 300;
        renderer.material.color = color;
    }
}
