using UnityEngine;
using System.Threading;
using System.Collections;


public class DeviceController3D : MonoBehaviour
{
    [SerializeField]
    private Board board;
    //public Board board;
    [SerializeField]
    private Device device;
    //public Device device; 
    [SerializeField]
    private GameObject avatar;
    //public Device device; 
    [SerializeField]
    private GameObject end_effector;

    public Vector2 PositionEE { get; private set; }

	public float TriggerAngle { get; private set; }

	public Vector2 PrevPositionEE { get; private set; }
	public Vector2 ForceEE;

	public float TriggerTorque;
	public float VibrationTorque; 
	

	public const int CW = 0;
	public const int CCW = 1;

    private Thread thread;

    private bool isLooping = false; 

    private Queue outputQueue;
    private Queue inputQueue;

    private float[] angles;
    private float[] position;

    public float k_stiffness;
    public float b_damping;

    public Vector3 forces = Vector3.zero;
    private float[] torques;

    private float[] fEE = { 0.0f, 0.0f };

    private Object thisLock = new Object();

    private  Rigidbody rb_avatar;

    private Transform tf_end_effector; 

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

        rb_avatar = avatar.GetComponent<Rigidbody>();
        tf_end_effector = end_effector.GetComponent<Transform>();

   

        thread = new Thread(ThreadLoop); ;
        isLooping = true; 
        thread.Start();

    }



    public void FixedUpdate()
	{
        //outFlag = true;

        //if (board.port.IsOpen)
        //{


        //    try
        //    {
        //        device.Device_read_data();
        //        angles = device.Get_device_angles();
        //        position = device.Get_device_position(angles);


        //        PrevPositionEE = PositionEE;
        //        PositionEE = new Vector2(position[0], position[1]);
        //        //Debug.Log(position[0] + ", " + position[1]);
        //        //Debug.Log(Time.time);
        //        TriggerAngle = position[2];

        //        //Debug.Log(position[2]);
        //        if (timesPositionSet < 2)
        //        {
        //            ++timesPositionSet;
        //        }


        //    }
        //    catch (System.TimeoutException TimeoutException)
        //    {


        //    }

        //    device.Set_device_torques(new float[] { ForceEE.x, ForceEE.y, TriggerTorque, VibrationTorque });
        //    device.Device_write_torques();


        //}

   
       
       

        if (position != null)
        {


            tf_end_effector.position = new Vector3(position[0] * 100.0f, 1.0f, position[1] * 100.0f);
            //rb_avatar.position = new Vector3(position[0] * 100.0f, 0.0f, position[1] * 100.0f);
        

            // if contact
            if (end_effector.GetComponent<EndEffector3D>().IsColliding())
            {
                //Debug.Log("here");

                forces = -k_stiffness * (tf_end_effector.position - rb_avatar.position) - b_damping * (-rb_avatar.velocity);
                fEE[0] = forces.x; 
                fEE[1] = forces.z;
            }
            else
            {
                forces = new Vector3(0.0f, 0.0f, 0.0f);
                fEE[0] = forces.x;
                fEE[1] = forces.z; 
            }

            
            rb_avatar.MovePosition(tf_end_effector.position);

        }


        //rb_avatar.position.Set(position[0], 0.0f, position[1]);

  

        
            

    }

    private void OnDisable()
	{
		device.Set_device_torques(new float[] { 0.0f, 0.0f});
		device.Device_write_torques();
        isLooping = false;
        board.ClosePort();
	}



    public void StartThread()
    {


        // Creates and starts the thread
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    public void ThreadLoop()
    {
        while (isLooping)
        {
            //lock (thisLock)
            //{


                if (board.Port.IsOpen)
                {


                    try
                    {
                        device.Device_read_data();
                        angles = device.Get_device_angles();
                        position = device.Get_device_position(angles);


                        PrevPositionEE = PositionEE;
                        PositionEE = new Vector2(position[0], position[1]);





                    }
                    catch (System.TimeoutException TimeoutException)
                    {
                        //Debug.Log(TimeoutException);
                    }

                    
                    device.Set_device_torques(fEE);
                    device.Device_write_torques();


                }

            //}
        }

    }


}
