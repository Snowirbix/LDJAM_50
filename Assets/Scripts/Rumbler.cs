using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rumbler : MonoBehaviour
{
    public Dictionary<RumblerObject, float> rumblerObjects = new();

    public void Play(RumblerObject rumblerObject)
    {
        if (rumblerObjects.ContainsKey(rumblerObject))
        {
            rumblerObjects[rumblerObject] = Time.time;
        }
        else
        {
            rumblerObjects.Add(rumblerObject, Time.time);

        }
    }

    public void Update()
    {
        var intensity = rumblerObjects.Aggregate(Vector2.zero, GetHighestIntensity);
        Gamepad.current.SetMotorSpeeds(intensity.x, intensity.y);
    }


    private Vector2 GetHighestIntensity(Vector2 intensity, KeyValuePair<RumblerObject,float> rumbler)
    {
        var deltaTime = Time.time - rumbler.Value;

        intensity.x = GetMaxValue(intensity.x, rumbler.Key.lowCurve, deltaTime);
        intensity.y = GetMaxValue(intensity.y, rumbler.Key.highCurve, deltaTime);

        return intensity;
    }

    private float GetMaxValue(float value, AnimationCurve animationCurve, float time)
    {
        if(time < animationCurve.keys.Last().time)
        {
            value = Mathf.Max(value, animationCurve.Evaluate(time));
        }
        return value;
    }
}
