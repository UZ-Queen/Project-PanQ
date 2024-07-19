using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[[RequireComponent(typeof(Box))]]
public class DeadZone : MonoBehaviour
{
    [SerializeField] int damage = 0;
    //   Collider collider;
    // Start is called before the first frame update
    void Start()
    {
        // collider = GetComponent<typeof>()
        if(damage == 0){
            damage = int.MaxValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {

        IDamageable damageable = other.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
    }
}
