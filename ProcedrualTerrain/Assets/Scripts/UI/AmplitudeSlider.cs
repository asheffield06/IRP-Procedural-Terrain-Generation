using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmplitudeSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 25f;
        gameObject.GetComponent<Slider>().minValue = 10f;
        gameObject.GetComponent<Slider>().value = TerrainGenerator.instance.amplitude;
    }

    public void GetSliderValue()
    {
        TerrainGenerator.instance.amplitude = gameObject.GetComponent<Slider>().value;
    }
}
