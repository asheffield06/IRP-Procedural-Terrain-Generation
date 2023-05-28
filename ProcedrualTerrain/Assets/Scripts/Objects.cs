using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    public bool oneTimeSpawn = false;
    void Start()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
