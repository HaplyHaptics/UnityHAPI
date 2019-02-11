using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEffector3D : MonoBehaviour
{

    private bool isCollision = false;

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("is triggering");


        if (other.tag != "Avatar")
        {
            //Debug.Log("is triggering");
            SetColliding(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Avatar")
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


    public void FixedUpdate()
    {
        //SetColliding(false);
    }

}
