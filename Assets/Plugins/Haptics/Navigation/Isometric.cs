using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Isometric : NavigationTechnique2D
{
    public float distance_threshold = 10.0f;
    [SerializeField]
    private float k_vc = 5000.0f;
    [SerializeField]
    private float b_vc = 2.0f;
    [SerializeField]
    private float vc_scalar = 2000.0f; 



    public override void CalculateAvatarPosition()
    {
         
        deviceController.rb_avatar.AddForce(k_vc * (deviceController.PositionEE - deviceController.rb_avatar.position) + b_vc * (deviceController.VelocityEE - deviceController.rb_avatar.velocity));


        if ((this.deviceController.rb_avatar.position - this.deviceController.rb_end_effector.position).magnitude > distance_threshold)
        {

            deviceController.rb_avatar.position = deviceController.rb_end_effector.position;

        }

    }

    public override void CalculateNavigationForce()
    {
        


    }

    public override void CalculateNavigationPosition()
    {
       
    }

    public override void CalculateScenePosition()
    {
        
    }
}
