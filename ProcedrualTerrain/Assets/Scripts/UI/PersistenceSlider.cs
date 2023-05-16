using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistenceSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 0.75f;
        gameObject.GetComponent<Slider>().minValue = 0f;
        gameObject.GetComponent<Slider>().value = TerrainGenerator.instance.persistence;
    }

    public void GetSliderValue()
    {
        TerrainGenerator.instance.persistence = gameObject.GetComponent<Slider>().value;
    }
}
