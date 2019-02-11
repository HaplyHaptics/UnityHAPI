using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEffector2D : MonoBehaviour
{

    private bool isCollision = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log("is triggering");


        if (other.tag != "Avatar")
        {
            //Debug.Log("is triggering");
            SetColliding(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
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


}
