using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class 작아져라얍 : MonoBehaviour
{

    RectTransform rT;
    // Start is called before the first frame update
     private void Awake() {
        rT = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        Debug.Log(gameObject.name+ " : 아 상쾌해!");
        rT.localScale = Vector3.one;
    }
        

    // Update is called once per frame
    void Update()
    {
        
    }
}
