using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar2D : MonoBehaviour
{

    private bool isCollision = false;
    public Vector2 collisionForce = Vector2.zero;

  

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Debug.Log("is triggering");


        if (other.collider.tag != "EndEffector")
        {
            //Debug.Log("is triggering");
            SetColliding(true);
            collisionForce = Vector2.zero; 

            foreach (ContactPoint2D contact in other.contacts)
            {
                print(contact.collider.name + " hit " + contact.otherCollider.name);
                // Visualize the contact point
                collisionForce = collisionForce + contact.normalImpulse * contact.normal;
                Debug.DrawRay(contact.point, collisionForce, Color.white);
                

            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        //Debug.Log("is triggering");
        SetColliding(true);
        collisionForce = 0.7f * collisionForce; 
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.tag != "EndEffector")
        {
            //Debug.Log("is triggering");
            SetColliding(false);
        }
    }

    public bool IsColliding()
    {
        return isCollision;
    }

    private void SetColliding(bool arg)
    {
        isCollision = arg;
    }


}
