using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isometric : NavigationTechnique2D
{
    public override void CalculateAvatarPosition(Rigidbody2D rb_end_effector, Rigidbody2D rb_avatar)
    {
        this.AvatarPosition = rb_end_effector.position; 


    }

    public override void CalculateAvatarVelocity(Rigidbody2D rb_end_effector, Rigidbody2D rb_avatar)
    {
        throw new System.NotImplementedException();
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
