using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] Vector3 offset = new Vector3(0,2,0);
    [SerializeField] Vector2 randomOffset = new Vector2(2,0);
    [SerializeField] float localScaleThresholdToBeDestroyed = 0.15f;
    [SerializeField] float lifeTime = 2f;
    // Start is called before the first frame update
    void Start()
    {
        //System.Random prng = new System.Random();
        Vector3 randomPosition = new Vector3(Random.Range(-randomOffset.x, +randomOffset.x), 0, Random.Range(-randomOffset.y, +randomOffset.y));

        //transform.SetParent(Camera.main.transform, true);
        transform.Rotate(60f, 0,0);
        transform.Translate(offset+randomPosition);
        Destroy(gameObject, lifeTime);
       
    }

    // Update is called once per frame
    void Update()
    {
        
        // transform.LookAt(Camera.main.transform);
        // transform.Rotate(new Vector3(0,180,0));          // 카메라를 보면 뒤집어지는데..

        if(transform.localScale.x < localScaleThresholdToBeDestroyed)
            GameObject.Destroy(gameObject);

    }
}
