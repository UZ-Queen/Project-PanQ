using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeHit(int damage, Vector3 hitPoint, Vector3 normal);
    void TakeDamage(int damage);
    event System.Action OnDeath;
}
