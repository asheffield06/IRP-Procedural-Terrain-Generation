using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrequencySlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 2.5f;
        gameObject.GetComponent<Slider>().minValue = 0f;
        gameObject.GetComponent<Slider>().value = TerrainGenerator.instance.frequency;
    }

    public void GetSliderValue()
    {
        TerrainGenerator.instance.frequency = gameObject.GetComponent<Slider>().value;
    }
}
