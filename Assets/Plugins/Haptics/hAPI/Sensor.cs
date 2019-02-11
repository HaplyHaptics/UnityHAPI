public class Sensor
{
	public int Encoder { get; set; }
    public int Direction  {get; set; }
    public float EncoderOffset { get; set; }
    public float EncoderResolution { get; set; }
    public float Value { get; set; }
    public float Velocity { get; set; }
    public int Port { get; set; }



    /**
 * Constructs a Sensor set using motor port position one
 */
    public Sensor()
    {
        Encoder = 0;
        Direction = 0;
        EncoderOffset = 0.0f;
        EncoderResolution = 0.0f;
        Port = 0;
    }


    /**
	 * Constructs a Sensor with the given motor port position, to be initialized with the given angular offset,
	 * at the specified step resoluiton (used for construction of encoder sensor)
	 *
	 * @param	encoder encoder index
	 * @param	offset initial offset in degrees that the encoder sensor should be initialized at
	 * @param	resolution step resolution of the encoder sensor
	 * @param	port specific motor port the encoder sensor is connect at (usually same as actuator)
	 */
    public Sensor(int encoder, int direction, float encoderOffset, float encoderResolution, int port)
	{
		Encoder = encoder;
		Direction = direction;
		EncoderOffset = encoderOffset;
		EncoderResolution = encoderResolution;
		Port = port;
	}
}
