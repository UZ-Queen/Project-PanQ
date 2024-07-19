using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Vector3 velocity = Vector3.zero;
    Rigidbody rb;


    public void Move(Vector3 velocity){
        this.velocity = velocity;
    }

    public void LookAt(Vector3 point){
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectedPoint);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        
    }

    void FixedUpdate(){
        rb.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
    }
}
