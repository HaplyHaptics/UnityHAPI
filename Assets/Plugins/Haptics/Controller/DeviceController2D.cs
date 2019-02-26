using UnityEngine;
using System.Threading;
using System.Collections;


public class DeviceController2D : MonoBehaviour
{
    [SerializeField]
    public Board board;
    //public Board board;
    [SerializeField]
    private Device device;
    //public Device device; 
    //[SerializeField]
    private NavigationTechnique2D navigation;
    
    private ContactModel2D contacts;
    //public Device device; 
    [SerializeField]
    private GameObject avatar;
    //public Device device; 
    [SerializeField]
    private GameObject end_effector;
    [SerializeField]
    private float deviceFrameAngle;
    private float scale;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private Vector2 initOffset;
    [SerializeField]
    private float fscale;


    private bool firstPos = true; 

    public Vector2 PositionEE { get; private set; }
    public Vector2 VelocityEE { get; private set;  }

    private Vector2 ForceEE;
    private Vector2 ForceContacts;
    private Vector2 ForceNavigation; 
  

    public float TriggerAngle { get; private set; }

    public const int CW = 0;
    public const int CCW = 1;

    private float[] angles;
    private float[] angularVelocities; 
    private float[] position;
    private float[] velocity;

    private float[] forces; 
    private float[] torques;


    [HideInInspector]
    public Rigidbody2D rb_avatar;
    [HideInInspector]
    public Rigidbody2D rb_end_effector;
    [HideInInspector]
    public Rigidbody2D rb_nav_bubble; 




    public void Start()
    {
        Debug.Log("Initializing Board");

        contacts = GetComponentInChildren<ContactModel2D>();
        navigation = GetComponent<NavigationTechnique2D>(); 

        board.Initialize();

        device.Add_actuator(1, CCW, 1);
        device.Add_actuator(2, CCW, 2);

        device.Add_encoder(1, CCW, 155, 13824, 1);
        device.Add_encoder(2, CCW, 25, 13824, 2);

        device.Add_analog_sensor("A0");
        device.Add_analog_sensor("A1");

        device.Device_set_parameters();

        rb_avatar = avatar.GetComponent<Rigidbody2D>();
        rb_end_effector = end_effector.GetComponent<Rigidbody2D>();


        rb_avatar.MovePosition(PositionEE);

        forces = new float[] { 0.0f, 0.0f };
        position = new float[] { 0.0f, 0.0f };
        velocity = new float[] { 0.0f, 0.0f };

        scale = Screen.width / Screen.dpi * 2.54f;
      
    }

    public void Update()
    {
        scale = Screen.width / Screen.dpi * 2.54f *10;
    }

    public void FixedUpdate()
    {
        
        //check if inputQueue has data

        if (board.Data_available())
        {

            ReadDeviceStates();


            if (position != null) //!float.IsNaN(position[0]) && !float.IsNaN(position[1]) && !float.IsInfinity(position[0]) && !float.IsInfinity(position[1]) &&
            {

                // Initialization of position
                if (firstPos)
                {
                    initOffset = new Vector2(position[0] * scale, position[1] * scale);

                    firstPos = false;                
                }


                //DeviceStates to Game Coordinates
                PositionEE = DeviceToGameFrame(new Vector2(position[0] * scale, position[1] * scale)) + initOffset + offset;
                VelocityEE = DeviceToGameFrame(new Vector2(velocity[0] * scale, velocity[1] * scale));
                rb_end_effector.position = PositionEE;


                //Collision Force 
                
                if (contacts.IsColliding)
                {
                    //Debug.Log("here");
                    ForceContacts = contacts.CollisionForce; 

                }
                else
                {
                    ForceContacts = Vector2.zero;

                }

                //Navigation Force
               
                navigation.CalculateNavigationForce();
                ForceNavigation =  navigation.NavigationForce;

                //Update Avatar Position
                navigation.CalculateAvatarPosition(); 
              
                // Net Force to Device
                ForceEE = GameToDeviceeFrame(fscale*(ForceContacts+ForceNavigation));

                forces[0] = ForceEE.x;
                forces[1] = ForceEE.y;



            }

        }

        WriteDeviceForces();

     }

    private void OnDisable()
    {
        device.Set_device_torques(new float[] { 0.0f, 0.0f });
        device.Device_write_torques();
        board.ClosePort();
    }




    

    private Vector2 DeviceToGameFrame(Vector2 vec)
    {
        Vector2 tmp = new Vector2
        {
            x = Mathf.Cos(deviceFrameAngle * Mathf.Deg2Rad) * vec.x + Mathf.Sin(deviceFrameAngle * Mathf.Deg2Rad) * vec.y,
            y = -Mathf.Sin(deviceFrameAngle * Mathf.Deg2Rad) * vec.x + Mathf.Cos(deviceFrameAngle * Mathf.Deg2Rad) * vec.y
        };


        return tmp; 
    }

    private Vector2 GameToDeviceeFrame(Vector2 vec)
    {
        Vector2 tmp = new Vector2
        {
            x = Mathf.Cos(deviceFrameAngle*Mathf.Deg2Rad) * vec.x - Mathf.Sin(deviceFrameAngle * Mathf.Deg2Rad) * vec.y,
            y = Mathf.Sin(deviceFrameAngle * Mathf.Deg2Rad) * vec.x + Mathf.Cos(deviceFrameAngle * Mathf.Deg2Rad) * vec.y
        };

        return tmp;
    }

    private void ReadDeviceStates()
    {
        device.Device_read_data();
        angles = device.Get_device_angles();


        angularVelocities = device.Get_device_angular_velocities();
        position = device.Get_device_position(angles);
        velocity = device.Get_device_velocities(angularVelocities);
    }

    private void WriteDeviceForces()
    {
        //Write Forces
        device.Set_device_torques(forces);
        device.Device_write_torques();
    }
    


}
