public class Actuator
{
	public int Actuators { get; set; }
    public int Direction { get; set; }
	public float Torque { get; set; }
    public int Port { get; set; }

	/**
	 * Creates an Actuator using the given motor port position
	 *
	 * @param	actuator actuator index
	 * @param	port motor port position for actuator
	 */
	public Actuator(int actuator, int direction, int port)
	{
		Actuators = actuator;
		Direction = direction;
		Port = port;
	}
}
