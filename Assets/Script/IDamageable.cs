using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeHit(int damage, RaycastHit hit);
    void TakeDamage(int damage);
    event System.Action OnDeath;
}
