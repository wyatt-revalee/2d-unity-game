using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IKnockbackable
{
    public void Knockback(float knockbackPwrX, float knockbackPwrY, Transform obj);
    
}
