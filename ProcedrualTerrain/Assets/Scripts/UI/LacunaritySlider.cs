using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LacunaritySlider : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 3f;
        gameObject.GetComponent<Slider>().minValue = 1.5f;
        gameObject.GetComponent<Slider>().value = TerrainGenerator.instance.lacunarity;
    }

    public void GetSliderValue()
    {
        TerrainGenerator.instance.lacunarity = gameObject.GetComponent<Slider>().value;
    }
}
