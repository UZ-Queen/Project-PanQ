using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up *speed );
    }
}
