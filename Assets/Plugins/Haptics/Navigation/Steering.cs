using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : NavigationTechnique2D
{

    [SerializeField]
    private float velocityScalar;  

    public override void CalculateAvatarPosition(Rigidbody2D rb_end_effector, Rigidbody2D rb_avatar)
    {
        rb_avatar.AddForce(-velocityScalar * (rb_end_effector.position - rb_avatar.position)); 
    }

    public override void CalculateAvatarVelocity(Rigidbody2D rb_end_effector, Rigidbody2D rb_avatar)
    {
       
    }

    public override void CalculateNavigationForce()
    {
        this.NavigationForce = new Vector2(0, 0);
    }

    public override void CalculateScenePosition(Rigidbody2D rb_end_effector, GameObject cameraView)
    {
        throw new System.NotImplementedException();
    }
}
