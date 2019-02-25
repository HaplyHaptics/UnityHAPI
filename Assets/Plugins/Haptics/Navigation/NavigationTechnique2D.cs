using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationTechnique2D : MonoBehaviour
{
    public Vector2 NavigationForce { get; set; }
    public Vector2 AvatarPosition { get; set; }
    public Vector2 AvatarVelocity { get; set; }
    public Vector2 ScenePosition { get; set; }
    public Vector2 SceneVelocity { get; set; }


    public abstract void CalculateAvatarPosition(Rigidbody2D rb_end_effector, Rigidbody2D rb_avatar);

    public abstract void CalculateAvatarVelocity(Rigidbody2D rb_end_effector, Rigidbody2D rb_avatar);

    public abstract void CalculateScenePosition(Rigidbody2D rb_end_effector, GameObject cameraView);

    public abstract void CalculateNavigationForce();







}
