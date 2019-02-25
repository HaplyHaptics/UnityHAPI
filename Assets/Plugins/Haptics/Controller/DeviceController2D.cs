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
    [SerializeField]
    private NavigationTechnique2D navigation;
    //public Device device; 
    [SerializeField]
    private GameObject avatar;
    //public Device device; 
    [SerializeField]
    private GameObject end_effector;
    [SerializeField]
    private float deviceFrameAngle;
    [SerializeField]
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

    public Vector2 ForceEE;
    public Vector2 ForceContacts;
    public Vector2 ForceNavigation; 
    public Vector2 ForceVC; 

    public float TriggerAngle { get; private set; }

    public const int CW = 0;
    public const int CCW = 1;

    public float distance_threshold =10.0f; 



    private float[] angles;
    private float[] angularVelocities; 
    private float[] position;
    private float[] velocity;

    public float k_stiffness;
    public float b_damping;

    public float k_vc;
    public float b_vc;

    private float[] forces; 
    private float[] torques;

    private Rigidbody2D rb_avatar;
    private Rigidbody2D rb_end_effector;




    public void Start()
    {
        Debug.Log("Initializing Board");



        board.Initialize();

        device.Add_actuator(1, CW, 1);
        device.Add_actuator(2, CW, 2);

        device.Add_encoder(1, CW, 180, 13824, 1);
        device.Add_encoder(2, CW, 0, 13824, 2);

        device.Add_analog_sensor("A0");
        device.Add_analog_sensor("A1");

        device.Device_set_parameters();

        rb_avatar = avatar.GetComponent<Rigidbody2D>();
        rb_end_effector = end_effector.GetComponent<Rigidbody2D>();


        rb_avatar.MovePosition(PositionEE);

        forces = new float[] { 0.0f, 0.0f };
        position = new float[] { 0.0f, 0.0f };
        velocity = new float[] { 0.0f, 0.0f };


    }

    public void FixedUpdate()
    {
        


     

        //check if inputQueue has data

        if (board.Data_available())
        {
            device.Device_read_data();
            angles = device.Get_device_angles();
            angularVelocities = device.Get_device_angular_velocities();
            position = device.Get_device_position(angles);
            velocity = device.Get_device_velocities(angularVelocities);

            if (position != null) //!float.IsNaN(position[0]) && !float.IsNaN(position[1]) && !float.IsInfinity(position[0]) && !float.IsInfinity(position[1]) &&
            {
                //readyToRead = false;


    


                if (firstPos)
                {
                    initOffset = new Vector2(position[0] * scale, position[1] * scale);

                    firstPos = false;
                                       
                }


                // have the device position 
                //need to ma
                PositionEE = DeviceToGameFrame(new Vector2(position[0] * scale, position[1] * scale)) + initOffset + offset;
                VelocityEE = DeviceToGameFrame(new Vector2(velocity[0] * scale, velocity[1] * scale));


                rb_end_effector.position = PositionEE;
        

                ForceVC = k_vc * (PositionEE - rb_avatar.position) + b_vc * (VelocityEE - rb_avatar.velocity);


                // if contact
                if (end_effector.GetComponent<EndEffector2D>().IsColliding())
                {
                    //Debug.Log("here");

                    ForceContacts = -k_stiffness * (PositionEE - rb_avatar.position) - b_damping * (VelocityEE - rb_avatar.velocity);
              
        
                }
                else
                {
                    ForceContacts = Vector2.zero;

                }

                navigation.CalculateNavigationForce();

                ForceNavigation = navigation.NavigationForce; 

                ForceEE = GameToDeviceeFrame(ForceContacts+ForceNavigation);

                forces[0] = ForceEE.x;
                forces[1] = ForceEE.y;



                rb_avatar.AddForce(fscale * ForceVC);

          

                if ((rb_avatar.position - rb_end_effector.position).magnitude > distance_threshold)
                {

                    //navigation method 

                    //rb_avatar.MovePosition(rb_end_effector.position);
                    
                    rb_avatar.position = navigation.AvatarPosition;

                }

            }

        }

        device.Set_device_torques(forces);
        device.Device_write_torques();

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

    


}
