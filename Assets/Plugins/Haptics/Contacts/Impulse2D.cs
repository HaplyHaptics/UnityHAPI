using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impulse2D : ContactModel2D
{

    [SerializeField]
    private float impulseDecayConstant = 0.7f;


    private void FixedUpdate()
    {
        CollisionForce = impulseDecayConstant * CollisionForce;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("is triggering");


        if (other.collider.tag != "EndEffector")
        {
            //Debug.Log("is triggering");
            this.IsColliding = true;
            CollisionForce = Vector2.zero;

            foreach (ContactPoint2D contact in other.contacts)
            {
                //print(contact.collider.name + " hit " + contact.otherCollider.name);
                // Visualize the contact point
                CollisionForce = CollisionForce + Mathf.Pow(contact.normalImpulse, 1) * contact.normal;
                //Debug.DrawRay(contact.point, CollisionForce, Color.white);


            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        //Debug.Log("is triggering");
        this.IsColliding = true;
        CollisionForce =Vector2.zero; 
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.tag != "EndEffector")
        {
            //Debug.Log("is triggering");
            this.IsColliding = false;
            CollisionForce = Vector2.zero;
        }
    }




}
