using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public bool oneTimeSpawn = false;

    public bool hitTerrain = false;
    void Start()
    {
        RaycastHit ObjectPosition;
        Ray castedRay = new Ray(transform.position, -transform.up);
        
        if (Physics.Raycast(castedRay, out ObjectPosition))
        {
            hitTerrain = true;
            gameObject.transform.position = new Vector3(ObjectPosition.point.x, ObjectPosition.point.y, ObjectPosition.point.z);
            if(gameObject.transform.position.y < 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
