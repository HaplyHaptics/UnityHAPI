using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : NavigationTechnique2D
{

    [SerializeField]
    private float velocityScalar= 100.0f;

    [SerializeField]
    private float k_nav= 0.01f;

    public override void CalculateAvatarPosition()
    {
        deviceController.rb_avatar.AddForce(velocityScalar * (deviceController.rb_end_effector.position - (Vector2) transform.position));
    }
    //rb_avatar.AddForce(-fscale* ForceVC);


    public override void CalculateNavigationForce()
    {
        this.NavigationForce = -k_nav * (deviceController.rb_end_effector.position - (Vector2)transform.position); 
    }

    public override void CalculateNavigationPosition()
    {
        deviceController.rb_avatar.AddForce(this.NavigationForce); 
    }

    public override void CalculateScenePosition()
    {
        throw new System.NotImplementedException();
    }
}
