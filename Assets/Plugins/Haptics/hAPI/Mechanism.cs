using UnityEngine;

public abstract class Mechanism : MonoBehaviour
{
	/**
	 * Performs the forward kinematics physics calculation of a specific physical mechanism
	 *
	 * @param	angles angular inpujts of physical mechanisms (array element length based
	 *			on the degree of freedom of the mechanism in question)
	 */
	public abstract void ForwardKinematics(float[] angles);


    /**
 * Performs velocity calculations at the end-effector of the device 
 *
 * @param	angularVelocities the angular velocities in (deg/s) of the active encoders 
 */
    public abstract void VelocityCalculation(float[] angularVelocities);

    /**
	 * Performs torque calculations that actuators need to output
	 *
	 * @param	force force values calculated from physics simulation that needs to be conteracted
	 */
    public abstract void TorqueCalculation(float[] forces);

	/**
	 * Performs force calculations
	 */
	public abstract void ForceCalculation();

	/**
	 * Performs calculations for position control
	 */
	public abstract void PositionControl(float [] position);

	/**
	 * Performs inverse kinematics calculations
	 */
	public abstract void InverseKinematics();

	/**
	 * Initializes or changes mechanisms parameters
	 *
	 * @param	parameters mechanism parameters 
	 */
	public abstract void Set_mechanism_parameters(float[] parameters);

	/**
	 * Sets and updates sensor data that may be used by the mechanism
	 *
	 * @param	data sensor data from sensors attached to Haply board
	 */
	public abstract void Set_sensor_data(float[] data);

	/**
	 * @return	end-effector coordinate position
	 */
	public abstract float[] Get_coordinate();

	/**
	 * @return	torque values from physics calculations
	 */
	public abstract float[] Get_torque();

	/**
	 * @return	angle values from physics calculations
	 */
	public abstract float[] Get_angle();

    /**
 * @return	velocity values from physics calculations
 */
    public abstract float[] Get_angular_velocity();

    /**
	 * @return	velocity values from physics calculations
	 */
    public abstract float[] Get_velocity();
}
