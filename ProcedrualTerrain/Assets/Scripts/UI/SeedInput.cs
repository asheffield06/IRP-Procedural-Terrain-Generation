using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedInput : MonoBehaviour
{
    void Start()
    {
        iField = this.GetComponent<TMP_InputField>();
    }


    public TMP_InputField iField;
    public int newSeed;

    public void EditString()
    {
        newSeed = int.Parse(iField.text);

        TerrainGenerator.instance.seed = newSeed;

    }
}
