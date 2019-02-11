using UnityEngine;
using System.IO.Ports;

public class Board : MonoBehaviour
{
	[SerializeField]
	protected string windowsComPort;
	[SerializeField]
    protected string macComPort;
	[SerializeField]
    protected int baudRate;
    [SerializeField]
    protected int serialTimeout;

    public SerialPort Port {  get; protected set; }

    protected bool hasBeenInitialized;

    public int receivedPacketSize;

    public virtual void Initialize()
	{
		if (hasBeenInitialized)
		{
			Debug.Log("Board Already Initialized");
			return;
		}

		try
		{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			Port = new SerialPort( windowsComPort, baudRate );
#else
		port = new SerialPort(macComPort, baudRate);
#endif
			Port.ReadTimeout = serialTimeout;
            Port.WriteTimeout = serialTimeout;
            Port.DtrEnable = true;
            Port.RtsEnable = true;


            Port.Open();
			Debug.Log( "Initialized Board" );
			Debug.Log(Port.IsOpen );

			hasBeenInitialized = true;
		}
		catch ( System.Exception exception )
		{
			Debug.LogException( exception );
		}
	}

	public virtual void ClosePort()
	{
        Port.Close();
    
        hasBeenInitialized = false; 
		Debug.Log("Port closed");
	}

	/**
	 * Formats and transmits data over the serial port
	 * 
	 * @param	 communicationType type of communication taking place
	 * @param	 deviceID ID of device transmitting the information
	 * @param	 bData byte inforamation to be transmitted
	 * @param	 fData float information to be transmitted
	 */
	public virtual void Transmit(byte communicationType, byte deviceID, byte[] bData, float[] fData)
	{
		byte[] outData = new byte[2 + bData.Length + 4 * fData.Length];
		byte[] segments = new byte[4];

		outData[0] = communicationType;
		outData[1] = deviceID;

		System.Array.Copy(bData, 0, outData, 2, bData.Length);

		int j = 2 + bData.Length;
		for (int i = 0; i < fData.Length; i++)
		{
			segments = FloatToBytes(fData[i]);
			System.Array.Copy(segments, 0, outData, j, 4);
			j = j + 4;
		}

        Port.Write(outData, 0, outData.Length);
	}

	/**
	 * Receives data from the serial port and formats data to return a float data array
	 * 
	 * @param	 type type of communication taking place
	 * @param	 deviceID ID of the device receiving the information
	 * @param	 expected number for floating point numbers that are expected
	 * @return	formatted float data array from the received data
	 */
	public virtual float[] Receive(byte communicationType, byte deviceID, int expected)
	{
        //Set_buffer(1 + 4 * expected);

        byte[] segments = new byte[4];

		byte[] inData = new byte[1 + 4 * expected];
		float[] data = new float[expected];

        Port.Read(inData,0, inData.Length);

		if (inData[0] != deviceID)
		{
			//Debug.LogError("Error, another device expects this data!");
		}

		int j = 1;

		for (int i = 0; i < expected; i++)
		{
			System.Array.Copy(inData, j, segments, 0, 4);
			data[i] = BytesToFloat(segments);
			j = j + 4;
		}

		return data;
	}



	/**
	 * @return   a boolean indicating if data is available from the serial port
	 */
	public virtual bool Data_available()
	{
		bool available = false;

		if (Port.BytesToRead > 0)
		{
			available = true;
		}

		return available;
	}


    /**
 * Sends a reset command to perform a software reset of the Haply board
 *
 */
    private void Reset_board()
    {
        byte communicationType = 0;
        byte deviceID = 0;
        byte[] bData = new byte[0];
        float[] fData = new float[0];

        Transmit(communicationType, deviceID, bData, fData);
    }



    /**
	 * Set serial buffer length for receiving incoming data
	 *
	 * @param   length number of bytes expected in read buffer
	 */
    private void Set_buffer(int length)
	{
        Port.ReadBufferSize = length; 
	}

	/**
	 * Translates a float point number to its raw binary format and stores it across four bytes
	 *
	 * @param	val floating point number
	 * @return   array of 4 bytes containing raw binary of floating point number
	 */
	protected byte[] FloatToBytes(float val)
	{
		return System.BitConverter.GetBytes(val);
	}

	/**
	 * Translates a binary of a float point to actual float point
	 *
	 * @param	segment array containing raw binary of floating point
	 * @return   translated floating point number
	 */
	protected float BytesToFloat(byte[] segment)
	{
		return System.BitConverter.ToSingle(segment, 0);
	}

	public static byte[] SubArray(byte[] data, int index, int length)
	{
		byte[] result = new byte[length];
		System.Array.Copy(data, index, result, 0, length);
		return result;
	}


}
