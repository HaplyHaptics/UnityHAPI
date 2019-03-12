using UnityEngine;

public class DragHandle : MonoBehaviour
{
    private Vector3 _offset;

    private Vector2 _defaulPos;

    private Vector2 _currentPosition;

    private bool CurrentDrawState { get; set; } = false;
    private bool LastDrawState { get; set; } = false;

    public Vector2 LaunchVelocity { get; private set; } = Vector2.zero; 

    public event OnDragHandleReleaseDelegate OnDragHandleReleaseEvent;

    [SerializeField]
    private float reloadTime = 1.0f; 
    [SerializeField]
    private float releaseTime = 0.3f; 

    float timeOfLastFire = 0.0f;
    float timeOfRelease = 0.0f; 
  

    //[HideInInspector]
   public enum SlingShotState { RELOAD, LOADED, DRAWN, FIRE};

   public SlingShotState Sling { get; private set; } 

   private void Start()
   {
        //_defaulPos = new Vector2(-6.13f, -1.87f);
        _defaulPos = this.transform.position; 
        transform.position = _defaulPos;
        
    }

   private void OnMouseDown()
   {
      _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint
      (
         new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)
      );
      Cursor.visible = false;
   }

   private void OnMouseDrag()
   {
      var currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
      _currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + _offset;

      transform.position = _currentPosition;

        
    }


   private void OnMouseUp()
   {
      Cursor.visible = true;

      if (OnDragHandleReleaseEvent != null)
      {
         OnDragHandleReleaseEvent.Invoke();
      }

        //transform.position = new Vector2( _defaulPos.magnitude * Mathf.Sin(Mathf.Deg2Rad*transform.parent.eulerAngles.z), - _defaulPos.magnitude * Mathf.Cos(Mathf.Deg2Rad * transform.parent.eulerAngles.z)); 





    }

    private void Update()
    {

       
        FiringLogic();
    }

    private void FiringLogic()
    {
        //Debug.Log(Sling.ToString());

        CurrentDrawState = GetComponent<Spring2D>().SpringActivated;

        switch (Sling)
        {
            
            case SlingShotState.RELOAD:

       
                if ((Time.time - timeOfLastFire) > reloadTime && !CurrentDrawState)
                {
                    Sling = SlingShotState.LOADED;
                }


                break;


            case SlingShotState.LOADED:

                if (CurrentDrawState)
                {
                    Sling = SlingShotState.DRAWN;
                    LastDrawState = true; 
                    
                }
            
          

                break; 

            case SlingShotState.DRAWN:


                if (!CurrentDrawState && LastDrawState )
                {
                    timeOfRelease = Time.time;
                    //Debug.Log("here"); 
                    LastDrawState = CurrentDrawState;
                }


                if ((Time.time - timeOfRelease) > releaseTime && GetComponent<Spring2D>().SpringDot > 0 && !CurrentDrawState)  
                {
                    Sling = SlingShotState.LOADED;
                    
                }

                else if ((Time.time - timeOfRelease) <= releaseTime && !CurrentDrawState && GetComponent<Spring2D>().SpringDot < 0)
                {
                    Sling = SlingShotState.FIRE; 
                }

           

                break;

            case SlingShotState.FIRE:

                LaunchVelocity = GetComponent<Spring2D>().deviceController.VelocityEE;

                if (OnDragHandleReleaseEvent != null)
                {
                    OnDragHandleReleaseEvent.Invoke();
                }

                timeOfLastFire = Time.time; 
                Sling = SlingShotState.RELOAD; 



                break; 
            
        }


        //transform.position = new Vector2( _defaulPos.magnitude * Mathf.Sin(Mathf.Deg2Rad*transform.parent.eulerAngles.z), - _defaulPos.magnitude * Mathf.Cos(Mathf.Deg2Rad * transform.parent.eulerAngles.z)); 





    }




}