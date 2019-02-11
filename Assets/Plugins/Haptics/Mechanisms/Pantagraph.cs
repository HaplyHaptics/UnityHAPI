using UnityEngine;

public class Pantagraph : Mechanism
{
	[SerializeField]
	private float l = 0.05f, L = 0.07f, d = 0.02f;

	private float th1, th2;
    private float om1, om2; 
	private float tau1, tau2;
	private float f_x, f_y;
	private float x_E, y_E;
    private float vx_E, vy_E; 


	private float J11, J12, J21, J22; 
	private const float gain = 0.1f;

	public override void ForwardKinematics(float[] angles)
	{
		th1 = Mathf.PI / 180 * angles[0];
		th2 = Mathf.PI / 180 * angles[1];


        // Forward Kinematics
        float c1 = Mathf.Cos(th1);
		float c2 = Mathf.Cos(th2);
		float s1 = Mathf.Sin(th1);
		float s2 = Mathf.Sin(th2);

		float xA = l * c1;
		float yA = l * s1;
		float xB = d + l * c2;
		float yB = l * s2;
		float R = Mathf.Pow(xA, 2) + Mathf.Pow(yA, 2);
		float S = Mathf.Pow(xB, 2) + Mathf.Pow(yB, 2);
		float M = (yA - yB) / (xB - xA);
		float N = 0.5f * (S - R) / (xB - xA);
		float a = Mathf.Pow(M, 2) + 1;
		float b = 2 * (M * N - M * xA - yA);
		float c = Mathf.Pow(N, 2) - 2 * N * xA + R - Mathf.Pow(L, 2);
		float Delta = Mathf.Pow(b, 2) - 4 * a * c;

		y_E = (-b + Mathf.Sqrt(Delta)) / (2 * a);
		x_E = M * y_E + N;

		float phi1 = Mathf.Acos((x_E - l * c1) / L);
		float phi2 = Mathf.Acos((x_E - d - l * c2) / L);
		float s21 = Mathf.Sin(phi2 - phi1);
		float s12 = Mathf.Sin(th1 - phi2);
		float s22 = Mathf.Sin(th2 - phi2);
		J11 = -(s1 * s21 + Mathf.Sin(phi1) * s12) / s21;
		J21 = (c1 * s21 + Mathf.Cos(phi1) * s12) / s21;
		J12 = Mathf.Sin(phi1) * s22 / s21;
		J22 = -Mathf.Cos(phi1) * s22 / s21;


 


	}

    public override void VelocityCalculation(float[] angularVelocities)
    {

        om1 = Mathf.PI / 180 * angularVelocities[0];
        om2 = Mathf.PI / 180 * angularVelocities[1];

        vx_E = J11 * om1 + J12 * om2;
        vy_E = J21 * om1 + J22 * om2;

    }

	public override void TorqueCalculation(float[] force)
	{
		f_x = force[0];
		f_y = force[1];

		tau1 = J11 * f_x + J21 * f_y;
		tau2 = J12 * f_x + J22 * f_y;

		tau1 = tau1 * gain;
		tau2 = tau2 * gain;

	}

	public override void ForceCalculation() {}

	public override void PositionControl(float[] position) {}

	public override void InverseKinematics(){}

	public override void Set_mechanism_parameters(float[] parameters)
	{
		l = parameters[0];
		L = parameters[1];
		d = parameters[2];
	}

	public override void Set_sensor_data(float[] data) {}

	public override float[] Get_coordinate()
	{
		float[] temp = { x_E, y_E};
		return temp;
	}

	public override float[] Get_torque()
	{
		float[] temp = { tau1, tau2};
		return temp;
	}

	public override float[] Get_angle()
	{
		float[] temp = { th1, th2};
		return temp;
	}

    public override float[] Get_velocity()
    {
        float[] temp = { vx_E, vy_E };
        return temp;
    }

    public override float[] Get_angular_velocity()
    {
        float[] temp = { om1, om2 };
        return temp;
    }
}
