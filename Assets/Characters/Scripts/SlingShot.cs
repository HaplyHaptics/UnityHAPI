using UnityEngine;

public class SlingShot : MonoBehaviour
{
   public LineRenderer[] HandlesLineRenderers;
   public Transform[] HandleAnchorTrnsforms;
   public DragHandle DragHandle;
   public Transform ReleasePointTransform;
   public Transform ProjectileSpawnTransform;
   public Transform AimerTransform;
   public GameObject ProjectilePrefab;
   public float StartPower = 0;
    [SerializeField]
    private float PowerScale =3.0f;
    [SerializeField]
    private float TrajectoryScale = 3.0f;

    [SerializeField]
    private float lineFat; 
   [SerializeField]
    private float lineThin; 

   private float[] LineLengths;

    public float flip; 

   public float GetVelocity()
   {
        //return Vector3.Distance(DragHandle.transform.position, ReleasePointTransform.transform.position) * PowerScale ;
        return Vector2.Distance(DragHandle.transform.position, ReleasePointTransform.transform.position) * PowerScale*TrajectoryScale;
    }

   public float GetDistance(float Vinit)
   {
        //var g = Physics.gravity.y;
        var g = Physics2D.gravity.y;
        var Vvert = Vinit * (Mathf.Sin(GetAngle() * Mathf.Deg2Rad));
        var Vhor = Vinit * (Mathf.Cos(GetAngle() * Mathf.Deg2Rad));

        //var Tvert = (0 - Vvert) / (flip*g);
        var Tvert = 0.2f;
        //Debug.Log(Tvert);
        var Thor = 2 * Tvert;
        var distance = Vhor * Thor;
       
        return distance;

        //var g_x = Physics2D.gravity.x;
        //var g_y = Physics2D.gravity.y; 

        //var Vvert = Vinit * ()


   }

   public float GetHeight(float Vinit, int amountPoints, int pointIndex)
   {
      //var g = Physics.gravity.y;
      var g = Physics2D.gravity.y;

      var Vvert = Vinit * (Mathf.Sin(GetAngle() * Mathf.Deg2Rad));
      var Vhor = Vinit * (Mathf.Cos(GetAngle() * Mathf.Deg2Rad));
      //var Tvert = (0 - Vvert) / (flip*g);
        var Tvert = 0.2f;
        var Thor = 2 * Tvert;
      var Dtot = Vhor * Thor;
      var Dp = (Dtot / (amountPoints)) * pointIndex;
      var T2 = Dp / Vhor;
      var height = ((Vvert * Dp) / Vhor) + 0.5f * g * Mathf.Pow(T2, 2);

      return height;
   }

   public void MakeShot()
   {
      var _projectile = Instantiate(ProjectilePrefab, ProjectileSpawnTransform.position, Quaternion.identity) as GameObject;
        //_projectile.GetComponent<Rigidbody>().AddForce(GetShotDirection() * StartPower * PowerScale, ForceMode.Impulse);
        //_projectile.GetComponent<Rigidbody2D>().AddForce(GetShotDirection() * StartPower * PowerScale, ForceMode2D.Impulse); 
        _projectile.GetComponent<Rigidbody2D>().AddForce(DragHandle.LaunchVelocity* PowerScale, ForceMode2D.Impulse);

      

        Destroy(_projectile, 4.0f);
   }

   public float GetAngle()

   {

        //Debug.DrawLine(Vector3.zero, Vector3.right);
        //Debug.DrawLine(Vector3.zero, (ReleasePointTransform.transform.position - DragHandle.transform.position).normalized); 

      //var angle = Vector3.Angle( Vector3.right, (ReleasePointTransform.transform.position - DragHandle.transform.position).normalized);
      var angle = Mathf.Rad2Deg*Mathf.Atan2((ReleasePointTransform.transform.position - DragHandle.transform.position).normalized.y, (ReleasePointTransform.transform.position - DragHandle.transform.position).normalized.x); 
        //var angle = Vector3.Angle(Vector3.right, (DragHandle.transform.position).normalized - ReleasePointTransform.transform.position);

        // just need to make the right angle depending on the quadrant

        ////TODO quadrant rectification 
        ///


        if (DragHandle.transform.position.y > AimerTransform.position.y)
      {
            angle = angle + 360.0f;
            flip = -1.0f; 
            
        }
        else
        {
            flip = 1.0f; 
        }

        return angle;
   }

   private void Start()
   {

        
      LineLengths = new float[2];
      AimerTransform.position = new Vector3(1, 1, 0);

      for (var i = 0; i < HandlesLineRenderers.Length; i++)
      {
         HandlesLineRenderers[i].SetPosition(0, HandleAnchorTrnsforms[i].position);
         HandlesLineRenderers[i].SetPosition(1, DragHandle.transform.position);
            HandlesLineRenderers[i].startWidth = lineFat;
            HandlesLineRenderers[i].endWidth = lineThin; 
      }
   }

   private void OnEnable()
   {
      DragHandle.OnDragHandleReleaseEvent += DragHandle_OnDragHandleReleaseEvent;
   }

   private void OnDisable()
   {
      DragHandle.OnDragHandleReleaseEvent -= DragHandle_OnDragHandleReleaseEvent;
   }

   private void OnDestroy()
   {
      DragHandle.OnDragHandleReleaseEvent -= DragHandle_OnDragHandleReleaseEvent;
   }

   private void DragHandle_OnDragHandleReleaseEvent()
   {
      MakeShot();
   }

   private void Update()
   {
      UpdateLines();
      UpdateAim();
      GetHeight(GetVelocity(), 3, 1);
        StartPower = Vector3.Distance(DragHandle.transform.position, ReleasePointTransform.transform.position);
       


    }

   private void UpdateLines()
   {
      for (var i = 0; i < HandlesLineRenderers.Length; i++)
      {
         HandlesLineRenderers[i].SetPosition(1, DragHandle.transform.position);
         HandlesLineRenderers[i].SetPosition(0, HandleAnchorTrnsforms[i].position);

        

         HandlesLineRenderers[i].GetComponent<LineRenderer>().startWidth = lineFat / LineLengths[i];
            if (HandlesLineRenderers[i].GetComponent<LineRenderer>().startWidth > lineFat) HandlesLineRenderers[i].GetComponent<LineRenderer>().startWidth = lineFat; 
                HandlesLineRenderers[i].GetComponent<LineRenderer>().endWidth = lineThin / LineLengths[i];
            if (HandlesLineRenderers[i].GetComponent<LineRenderer>().endWidth > lineThin) HandlesLineRenderers[i].GetComponent<LineRenderer>().endWidth = lineThin;
            LineLengths[i] = Vector3.Distance(DragHandle.transform.position, HandleAnchorTrnsforms[i].position);

         if (LineLengths[i] <= 0.65f)
         {
            LineLengths[i] = 0.65f;
         }
      }
   }

   private void UpdateAim()
   {
      var pullDirection = ReleasePointTransform.position - (DragHandle.transform.position - ReleasePointTransform.position).normalized;
      AimerTransform.position = pullDirection;
   }

   private Vector3 GetShotDirection()
   {
      return (AimerTransform.position - ReleasePointTransform.transform.position).normalized;
   }
}