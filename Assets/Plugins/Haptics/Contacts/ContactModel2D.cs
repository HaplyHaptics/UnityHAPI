using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContactModel2D : MonoBehaviour
{
    public DeviceController2D deviceController;

    public bool IsColliding {get; protected set;}
    public Vector2 CollisionForce {  get; protected set; }



}
