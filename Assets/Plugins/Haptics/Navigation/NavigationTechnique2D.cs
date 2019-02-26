using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationTechnique2D : MonoBehaviour
{
   
    public DeviceController2D deviceController; 

    public Vector2 NavigationForce { get; set; }
    public Vector2 AvatarPosition { get; set; }
    public Vector2 AvatarVelocity { get; set; }
    public Vector2 ScenePosition { get; set; }
    public Vector2 SceneVelocity { get; set; }
    public Vector2 NavigationPosition { get; set; }
    public Vector2 NavigationVelocity { get; set; }


    public abstract void CalculateAvatarPosition();

    public abstract void CalculateScenePosition();

    public abstract void CalculateNavigationPosition();

    public abstract void CalculateNavigationForce();







}
