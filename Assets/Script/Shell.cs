using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour
{
    Rigidbody rb;

    
    [SerializeField]    float minSpeed;
    [SerializeField]    float maxSpeed;

    float lifeTime = 4f;
    float fadeTime = 2f;

    
    // [SerializeField]    float minSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();



        float speed = Random.Range(minSpeed, maxSpeed);

        rb.AddForce(transform.right * speed, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * speed);

        StartCoroutine(Fade());
    }



    IEnumerator Fade(){
        yield return new WaitForSeconds(lifeTime);


        Material mat = GetComponent<Renderer>().material;
        Color originalColor = mat.color;

        float percent =0f;
        float fadeSpeed = 1/fadeTime;
        while(percent <= 1){
            percent += fadeSpeed * Time.deltaTime;
            mat.color = Color.Lerp(originalColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}
