using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public abstract class Item
{
    public abstract string Name{get;protected set;}

    public virtual void Update(){

    }
    public virtual void OnHit(){
        
    }

}
