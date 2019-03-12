using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrajectory : MonoBehaviour
{
   public SlingShot Slingshot;
   public GameObject PointPrefab;
   public int PointNumber;
   public float InitialPointSize = 0.5f;
   public List<Transform> PointsList;
   [SerializeField]
    private float targetAngle = 50.0f; 

    public Spring2D spring; 

   private Vector3 _updatedPosition = new Vector3(0, 0, 0);

   public void CreatePoints()
   {
      PointsList = new List<Transform>();

      for (var i = 0; i < PointNumber; i++)
      {
         var _point = Instantiate(PointPrefab, transform.position, Quaternion.identity).transform;
         _point.localScale = new Vector3
         (
            InitialPointSize / (i + 1) + (InitialPointSize / 2),
            InitialPointSize / (i + 1) + (InitialPointSize / 2),
            InitialPointSize / (i + 1) + (InitialPointSize / 2)
         );
         _point.transform.parent = transform;
         PointsList.Add(_point);
      }
   }

   public void UpdatePoints()
   {
   
        //Debug.Log(Slingshot.GetAngle());

        float offset = 0.0f;
        //if (Slingshot.GetAngle() < 0) offset = 360; 
      if ((spring.SpringActivated || Input.GetMouseButton(0)) && Slingshot.GetAngle() + offset > transform.parent.eulerAngles.z + targetAngle && Slingshot.GetAngle() + offset <= transform.parent.eulerAngles.z + 180-targetAngle)
        {
         for (var i = 0; i < PointsList.Count; i++)
         {   // based on get velocity and get gight

            
                    _updatedPosition.x = Slingshot.ReleasePointTransform.transform.position.x + i * (Slingshot.GetDistance(Slingshot.GetVelocity()) / PointNumber);
                    _updatedPosition.y = Slingshot.ReleasePointTransform.transform.position.y + Slingshot.GetHeight(Slingshot.GetVelocity(), PointNumber, i);
                    _updatedPosition.z = 0;
         

            PointsList[i].transform.position = _updatedPosition;
            PointsList[i].gameObject.SetActive(true);
         }
      }
      else
      {
         for (var i = 0; i < PointsList.Count; i++)
         {
            PointsList[i].gameObject.SetActive(false);
         }
      }
   }

   private void Start()
   {
      CreatePoints();
   }

   private void Update()
   {
      UpdatePoints();
   }
}