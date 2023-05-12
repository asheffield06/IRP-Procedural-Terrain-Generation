using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleSlider : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 75f;
        gameObject.GetComponent<Slider>().minValue = 25f;
        gameObject.GetComponent<Slider>().value = TerrainGenerator.instance.scale;
    }

    public void GetSliderValue()
    {
        TerrainGenerator.instance.scale = gameObject.GetComponent<Slider>().value;
    }
}
