using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring2D : ContactModel2D
{
    // Start is called before the first frame update
    [SerializeField]
    private float springStiffness = 1.0f;
    public GameObject launchLocation;
    [SerializeField]
    private float springHeight = 0.0f;
    [SerializeField]
    private float springRadius = 1.5f; 
    public bool SpringActivated { get; private set;  }

    public float SpringDot { get; private set; } 



    private void FixedUpdate()
    {

        Debug.DrawLine(launchLocation.transform.position, GetComponent<Transform>().position);
        Debug.DrawLine(launchLocation.transform.position, launchLocation.transform.up);

        Vector2 springDir = launchLocation.GetComponent<Transform>().position - GetComponent<Transform>().position;
        float springDis = (launchLocation.GetComponent<Transform>().position - GetComponent<Transform>().position).magnitude;

        float springProjection= Vector3.Project(springDir, launchLocation.transform.up).magnitude;

        //Debug.Log(springProjection);
        

        SpringDot = Vector2.Dot(springDir, launchLocation.transform.up);

        if (springProjection > springHeight && SpringDot > 0 )
        {
            SpringActivated = true;
            IsColliding = true;
        }
        else
        {
            IsColliding = false;
            SpringActivated = false;
        }


        if( SpringActivated)
        {
            float dist = ((launchLocation.GetComponent<Transform>().position - GetComponent<Transform>().position).magnitude - springRadius); 
            this.CollisionForce = springStiffness  * Mathf.Pow(dist,3) * (launchLocation.GetComponent<Transform>().position - GetComponent<Transform>().position).normalized;
        }
        else
        {
            this.CollisionForce = Vector2.zero; 
        }
    }



}
