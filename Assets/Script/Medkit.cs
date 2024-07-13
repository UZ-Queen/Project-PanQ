using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour, ICollectable
{
    public void OnPickUp(LivingEntity entity)
    {
        entity.Heal(entity.MaxHealth);
        Destroy(gameObject);
    }

}
