using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]SpriteRenderer dot;
    [SerializeField]float rotateSpeed = 60f;
    [SerializeField]bool rotateClockwise;

    [SerializeField]Color normalColor = Color.blue;
    [SerializeField]Color targetDetectedColor = Color.red;
    [SerializeField]LayerMask layerMask;
    
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateAngle = Vector3.up * rotateSpeed * Time.deltaTime;
        rotateAngle *= rotateClockwise ? 1 : -1;
        transform.Rotate(rotateAngle);
    }

    public void DetectTarget(Ray ray){
        if(Physics.Raycast(ray, 1000, layerMask)){
            dot.color = targetDetectedColor;
        }
        else{
            dot.color = normalColor;
        }
    }
}
