using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodParticle2D : ContactModel2D
{
    [SerializeField]
    private float k_stiffness = 0.3f;
    [SerializeField]
    private float b_damping = 0.0005f;


    //ForceNavigation = ForceVC; 
    //ForceContacts = -k_stiffness * (PositionEE - rb_avatar.position) - b_damping * (VelocityEE - rb_avatar.velocity);

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("is triggering");


        if (other.tag != "Avatar")
        {
            //Debug.Log("is triggering");
            IsColliding = true;
            
           CollisionForce =  -k_stiffness * (deviceController.PositionEE - deviceController.rb_avatar.position) - b_damping * (deviceController.VelocityEE - deviceController.rb_avatar.velocity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Avatar")
        {
            //Debug.Log("is triggering");
            IsColliding = false; 
        }
    }




}
