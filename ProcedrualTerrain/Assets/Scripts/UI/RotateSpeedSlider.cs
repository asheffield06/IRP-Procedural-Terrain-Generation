using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateSpeedSlider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Slider>().maxValue = 1.5f;
        gameObject.GetComponent<Slider>().minValue = 0f;
        gameObject.GetComponent<Slider>().value = RotateScript.instance.speed;
    }

    public void GetSliderValue()
    {
        RotateScript.instance.speed = gameObject.GetComponent<Slider>().value;
    }
}
