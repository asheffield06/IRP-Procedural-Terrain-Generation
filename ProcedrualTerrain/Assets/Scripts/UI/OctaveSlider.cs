using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OctaveSlider : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 6;
        gameObject.GetComponent<Slider>().minValue = 0;
        gameObject.GetComponent<Slider>().value = TerrainGenerator.instance.octaves;
    }

    public void GetSliderValue(float value)
    {
        TerrainGenerator.instance.octaves = (int)gameObject.GetComponent<Slider>().value;
    }
}
