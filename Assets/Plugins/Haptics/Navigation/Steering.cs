using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering : NavigationTechnique2D
{

    [SerializeField]
    private float velocityScalar;  

    public override void CalculateAvatarPosition()
    {
        deviceController.rb_avatar.AddForce(-velocityScalar * (deviceController.rb_end_effector.position - deviceController.rb_avatar.position));
    }



    public override void CalculateNavigationForce()
    {
        this.NavigationForce = new Vector2(0, 0);
    }

    public override void CalculateNavigationPosition()
    {
        throw new System.NotImplementedException();
    }

    public override void CalculateScenePosition()
    {
        throw new System.NotImplementedException();
    }
}
