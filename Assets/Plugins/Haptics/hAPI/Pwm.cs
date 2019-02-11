using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pwm
{
    public int Pin { get; set; }
    public int Value { get; set; }

    /**
 * Constructs an empty PWM output for use
 */
    public Pwm()
    {
        Pin = 0;
        Value = 0; 
    }


    /**
     * Constructs a PWM output at the specified pin and at the desired percentage 
     * 
     *	@param	pin pin to output pwm signal
     * @param 	pulseWidth percent of pwm output, value between 0 to 100
     */
    public Pwm(int pin, float pulseWidth)
    {
        Pin = pin;

        if (pulseWidth > 100.0)
        {
            Value = 255;
        }
        else
        {
            Value = (int)(pulseWidth * 255 / 100);
        }
    }


    /**
	 * Set value variable of pwm
	 */
    public void Set_pulse(float percent)
    {

        if (percent > 100.0)
        {
            Value = 255;
        }
        else if (percent < 0)
        {
            Value = 0;
        }
        else
        {
            Value = (int)(percent * 255 / 100);
        }
    }

    /**
	 * @return percent value of pwm signal	 
	 */
    public float get_pulse()
    {

        float percent = Value * 100 / 255;

        return percent;
    }



}
