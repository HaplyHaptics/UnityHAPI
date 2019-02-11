using UnityEngine;

public class Device : MonoBehaviour
{
	[SerializeField]
	private byte deviceID;
	[SerializeField]
	private Mechanism mechanism;

	private byte communicationType;

	private int actuatorsActive;
	private int encodersActive;
	private int sensorsActive;
    private int pwmsActive;

    private Actuator[] motors = new Actuator[0];
	private Sensor[] encoders = new Sensor[0];
	private Sensor[] sensors = new Sensor[0];
    private Pwm[] pwms = new Pwm[0]; 

	private byte[] actuatorPositions = { 0, 0, 0, 0 };
	private byte[] encoderPositions = { 0, 0, 0, 0 };

	[SerializeField]
	private Board boardLink;

	// device setup functions
	/**
	 * add new actuator to platform
	 *
	 * @param	actuator index of actuator (and index of 1-4)
	 * @param	roatation positive direction of actuator rotation
	 * @param	port specified motor port to be used (motor ports 1-4 on the Haply board) 
	 */
	public void Add_actuator(int actuator, int rotation, int port)
	{
		bool error = false;

		if (port < 1 || port > 4)
		{
			Debug.LogError("error: encoder port index out of bounds");
			error = true;
		}

		if (actuator < 1 || actuator > 4)
		{
			Debug.LogError("error: encoder index out of bound!");
			error = true;
		}

		int j = 0;
		for (int i = 0; i < actuatorsActive; i++)
		{
			if (motors[i].Actuators < actuator)
			{
				j++;
			}

			if (motors[i].Actuators == actuator)
			{
				Debug.LogError("error: actuator " + actuator + " has already been set");
				error = true;
			}
		}

		if (!error)
		{
			Actuator[] temp = new Actuator[actuatorsActive + 1];

			System.Array.Copy(motors, 0, temp, 0, motors.Length);

			if (j < actuatorsActive)
			{
				System.Array.Copy(motors, j, temp, j + 1, motors.Length - j);
			}

			temp[j] = new Actuator(actuator, rotation, port);
			Actuator_assignment(actuator, port);

			motors = temp;
			actuatorsActive++;
		}
	}

	/**
	 * Add a new encoder to the platform
	 *
	 * @param	actuator index of actuator (an index of 1-4)
	 * @param	positive direction of rotation detection
	 * @param	offset encoder offset in degrees
	 * @param	resolution encoder resolution
	 * @param	port specified motor port to be used (motor ports 1-4 on the Haply board) 
	 */
	public void Add_encoder(int encoder, int rotation, float offset, float resolution, int port)
	{
		bool error = false;

		if (port < 1 || port > 4)
		{
			Debug.LogError("error: encoder port index out of bounds");
			error = true;
		}

		if (encoder < 1 || encoder > 4)
		{
			Debug.LogError("error: encoder index out of bound!");
			error = true;
		}

		// determine index for copying
		int j = 0;
		for (int i = 0; i < encodersActive; i++)
		{
			if (encoders[i].Encoder < encoder)
			{
				j++;
			}

			if (encoders[i].Encoder == encoder)
			{
				Debug.LogError("error: encoder " + encoder + " has already been set");
				error = true;
			}
		}

		if (!error)
		{
			Sensor[] temp = new Sensor[encodersActive + 1];

			System.Array.Copy(encoders, 0, temp, 0, encoders.Length);

			if (j < encodersActive)
			{
				System.Array.Copy(encoders, j, temp, j + 1, encoders.Length - j);
			}

			temp[j] = new Sensor(encoder, rotation, offset, resolution, port);
			Encoder_assignment(encoder, port);

			encoders = temp;
			encodersActive++;
		}
	}


	/**
	 * Add an analog sensor to platform
	 *
	 * @param	pin the analog pin on haply board to be used for sensor input (Ex: A0)
	 */
	public void Add_analog_sensor(string pin)
	{
		// set sensor to be size zero
		bool error = false;

		char[] temp = pin.ToCharArray();

		char port = temp[0];
		string number = pin.Substring(1);

		int value = System.Int32.Parse(number);
		value = value + 54;

		for (int i = 0; i < sensorsActive; i++)
		{
			if (value == sensors[i].Port)
			{
				Debug.LogError("error: Analog pin: A" + (value - 54) + " has already been set");
				error = true;
			}
		}

		if (port != 'A' || value < 54 || value > 65)
		{
			Debug.LogError("error: outside analog pin range");
			error = true;
		}

		if (!error)
		{

			Sensor[] stemp = new Sensor[sensors.Length + 1];
			System.Array.Copy(sensors, stemp, sensors.Length);
			stemp[sensorsActive] = new Sensor();
			stemp[sensorsActive].Port = value;
			sensors = stemp;
			sensorsActive++;
		}
	}

	/**
	 * Gathers all encoder, sensor setup inforamation of all encoders, and sensors that are initialized and 
	 * sequentialy formats the data based on specified sensor index positions to send over serial port
	 * interface for hardware device initialization
	 */
	public void Device_set_parameters()
	{
		communicationType = 1;

		int control;

		float[] encoderParameters;

		byte[] encoderParams;
		byte[] motorParams;
		byte[] sensorParams;
        byte[] pwmParams;

        if (encodersActive > 0)
		{
			encoderParams = new byte[encodersActive + 1];
			control = 0;

			for (int i = 0; i < encoders.Length; i++)
			{
				if (encoders[i].Encoder != (i + 1))
				{
					Debug.LogError("warning, improper encoder indexing");
					encoders[i].Encoder = i + 1;
					encoderPositions[encoders[i].Port - 1] = (byte)encoders[i].Encoder;
				}
			}

			for (int i = 0; i < encoderPositions.Length; i++)
			{
				control = control >> 1;

				if (encoderPositions[i] > 0)
				{
					control = control | 0x0008;
				}
			}

			encoderParams[0] = (byte)control;

			encoderParameters = new float[2 * encodersActive];

			int j = 0;
			for (int i = 0; i < encoderPositions.Length; i++)
			{
				if (encoderPositions[i] > 0)
				{
					encoderParameters[2 * j] = encoders[encoderPositions[i] - 1].EncoderOffset;
					encoderParameters[2 * j + 1] = encoders[encoderPositions[i] - 1].EncoderResolution;
					j++;
					encoderParams[j] = (byte)encoders[encoderPositions[i] - 1].Direction;
				}
			}
		}
		else
		{
			encoderParams = new byte[1];
			encoderParams[0] = 0;
			encoderParameters = new float[0];
		}

		if (actuatorsActive > 0)
		{
			motorParams = new byte[actuatorsActive + 1];
			control = 0;

			for (int i = 0; i < motors.Length; i++)
			{
				if (motors[i].Actuators != (i + 1))
				{
					Debug.LogError("warning, improper actuator indexing");
					motors[i].Actuators = i + 1;
					actuatorPositions[motors[i].Port - 1] = (byte)motors[i].Actuators;
				}
			}

			for (int i = 0; i < actuatorPositions.Length; i++)
			{
				control = control >> 1;

				if (actuatorPositions[i] > 0)
				{
					control = control | 0x0008;
				}
			}

			motorParams[0] = (byte)control;

			int j = 1;
			for (int i = 0; i < actuatorPositions.Length; i++)
			{
				if (actuatorPositions[i] > 0)
				{
					motorParams[j] = (byte)motors[actuatorPositions[i] - 1].Direction;
					j++;
				}
			}
		}
		else
		{
			motorParams = new byte[1];
			motorParams[0] = 0;
		}

		if (sensorsActive > 0)
		{
			sensorParams = new byte[sensorsActive + 1];
			sensorParams[0] = (byte)sensorsActive;

			for (int i = 0; i < sensorsActive; i++)
			{
				sensorParams[i + 1] = (byte)sensors[i].Port;
			}

			System.Array.Sort(sensorParams);

			for (int i = 0; i < sensorsActive; i++)
			{
				sensors[i].Port = sensorParams[i + 1];
			}
		}
		else
		{
			sensorParams = new byte[1];
			sensorParams[0] = 0;
		}



        if (pwmsActive > 0)
        {
            byte[] temp = new byte[pwmsActive];

            pwmParams = new byte[pwmsActive + 1];
            pwmParams[0] = (byte)pwmsActive;


            for (int i = 0; i < pwmsActive; i++)
            {
                temp[i] = (byte)pwms[i].Pin;
            }

            System.Array.Sort(temp);

            for (int i = 0; i < pwmsActive; i++)
            {
                pwms[i].Pin= temp[i];
                pwmParams[i + 1] = (byte)pwms[i].Pin;
            }

        }
        else
        {
            pwmParams = new byte[1];
            pwmParams[0] = 0;
        }





        byte[] encMtrSenPwm = new byte[motorParams.Length + encoderParams.Length + sensorParams.Length + +pwmParams.Length];
		System.Array.Copy(motorParams, 0, encMtrSenPwm, 0, motorParams.Length);
		System.Array.Copy(encoderParams, 0, encMtrSenPwm, motorParams.Length, encoderParams.Length);
		System.Array.Copy(sensorParams, 0, encMtrSenPwm, motorParams.Length + encoderParams.Length, sensorParams.Length);
        System.Array.Copy(pwmParams, 0, encMtrSenPwm, motorParams.Length + encoderParams.Length + sensorParams.Length, pwmParams.Length);


        boardLink.receivedPacketSize = 4 * (2 * encodersActive + sensorsActive) + 1;

        boardLink.Transmit(communicationType, deviceID, encMtrSenPwm, encoderParameters);


	}

	/**
	 * assigns actuator positions based on actuator port
	 */
	private void Actuator_assignment(int actuator, int port)
	{
		if (actuatorPositions[port - 1] > 0)
		{
			Debug.LogError("warning, double check actuator port usage");
		}

		actuatorPositions[port - 1] = (byte) actuator;
	}

	/**
	 * assigns encoder positions based on actuator port
	 */
	private void Encoder_assignment(int encoder, int port)
	{
		if (encoderPositions[port - 1] > 0)
		{
			Debug.LogError("warning, double check encoder port usage");
		}

		encoderPositions[port - 1] = (byte) encoder;
	}

    // device communication functions
    /**
	 * Receives angle position and sensor inforamation from the serial port interface and updates each indexed encoder 
	 * sensor to their respective received angle and any analog sensor that may be setup
	 */
    public void Device_read_data()
    {
        communicationType = 2;

        int dataCount = 0;

        float[] device_data = boardLink.Receive(communicationType, deviceID, sensorsActive + 2 * encodersActive);

        for (int i = 0; i < sensorsActive; i++)
        {
            sensors[i].Value = device_data[dataCount];
            dataCount++;
        }

        for (int i = 0; i < encoderPositions.Length; i++)
        {
            if (encoderPositions[i] > 0)
            {
                encoders[encoderPositions[i] - 1].Value = device_data[dataCount];
                dataCount++;
                encoders[encoderPositions[i] - 1].Velocity = device_data[dataCount];
                dataCount++;
            }
        }

    
    }

	/**
	 * Requests data from the hardware based on the initialized setup. function also sends a torque output 
	 * command of zero torque for each actuator in use
	 */
	public void Device_read_request()
	{
		communicationType = 2;
        byte[] pulses = new byte[pwmsActive];
        float[] encoderRequest = new float[actuatorsActive];

        for (int i = 0; i < pwms.Length; i++)
        {
            pulses[i] = (byte)pwms[i].Value;
        }


        int j = 0;
		for (int i = 0; i < actuatorPositions.Length; i++)
		{
			if (actuatorPositions[i] > 0)
			{
				encoderRequest[j] = 0;
				j++;
			}
		}

		boardLink.Transmit(communicationType, deviceID, pulses, encoderRequest);
	}

	/**
	 * Transmits specific torques that has been calculated and stored for each actuator over the serial
	 * port interface
	 */
	public void Device_write_torques()
	{
		communicationType = 2;
        byte[] pulses = new byte[pwmsActive];
        float[] deviceTorques = new float[actuatorsActive];

        for (int i = 0; i < pwms.Length; i++)
        {
            pulses[i] = (byte)pwms[i].Value;
        }


        int j = 0;
		for (int i = 0; i < actuatorPositions.Length; i++)
		{
			if (actuatorPositions[i] > 0)
			{
				deviceTorques[j] = motors[actuatorPositions[i] - 1].Torque;
				j++;
			}
		}

		boardLink.Transmit(communicationType, deviceID, pulses, deviceTorques);
	}

    /**
     * Set pulse of specified PWM pin
     */
        public void Set_pwm_pulse(int pin, float pulse)
        {

            for (int i = 0; i < pwms.Length; i++)
            {
                if (pwms[i].Pin == pin)
                {
                    pwms[i].Set_pulse(pulse);
                }
            }
        }


    /**
     * Gets percent PWM pulse value of specified pin
     */
    public float Get_pwm_pulse(int pin)
    {

        float pulse = 0;

        for (int i = 0; i < pwms.Length; i++)
        {
            if (pwms[i].Pin == pin)
            {
                pulse = pwms[i].get_pulse();
            }
        }

        return pulse;
    }




    /**
 * Gathers current state of angles information from encoder objects
 *
 * @returns	most recent angles information from encoder objects
 */
    public float[] Get_device_angles()
    {
        float[] angles = new float[encodersActive];

        for (int i = 0; i < encodersActive; i++)
        {
            angles[i] = encoders[i].Value;
        }

        return angles;
    }

    /**
 * Gathers current state of the angular velocity information from encoder objects
 *
 * @returns	most recent angles information from encoder objects
 */
    public float[] Get_device_angular_velocities()
    {
        float[] angualrVelocities = new float[encodersActive];

        for (int i = 0; i < encodersActive; i++)
        {
            angualrVelocities[i] = encoders[i].Velocity;
        }

        return angualrVelocities;
    }





    /**
	 * Gathers current data from sensor objects
	 *
	 * @returns	most recent analog sensor information from sensor objects
	 */
    public float[] Get_sensor_data()
	{
		float[] data = new float[sensorsActive];

		for (int i = 0; i < sensorsActive; i++)
		{
			data[i] = sensors[i].Value;
		}

		return data;
	}

	/**
	 * Performs physics calculations based on the given angle values
	 *
	 * @param	  angles angles to be used for physics position calculation
	 * @returns	end-effector coordinate position
	 */
	public float[] Get_device_position(float[] angles)
	{
		mechanism.ForwardKinematics(angles);
		float[] endEffectorPosition = mechanism.Get_coordinate();

		return endEffectorPosition;
	}

    /**
* Gathers current state of angles information from encoder objects
*
* @returns    most recent angles information from encoder objects
*/
    public float[] Get_device_velocities(float[] angularVelocities)
    {
        mechanism.VelocityCalculation(angularVelocities);
        float[] endEffectorVelocity = mechanism.Get_velocity();

        return endEffectorVelocity;
    }


    /**
	 * Calculates the needed output torques based on forces input and updates each initialized 
	 * actuator respectively
	 *
	 * @param	 forces forces that need to be generated
	 * @returns   torques that need to be outputted to the physical device
	 */
    public float[] Set_device_torques(float[] forces)
	{
		mechanism.TorqueCalculation(forces);
		float[] torques = mechanism.Get_torque();

		for (int i = 0; i < actuatorsActive; i++)
		{
			motors[i].Torque = torques[i];
		}

		return torques;
	}
}
