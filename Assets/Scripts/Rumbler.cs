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
        var time = Time.time;

        var intensity = rumblerObjects
            .Aggregate(Vector2.zero, (a, r) =>
            {
                var dt = time - r.Value;
                if (dt < r.Key.lowCurve.keys.Last().time)
                {
                    a.x = Mathf.Max(a.x, r.Key.lowCurve.Evaluate(dt));
                }
                if (dt < r.Key.highCurve.keys.Last().time)
                {
                    a.y = Mathf.Max(a.y, r.Key.highCurve.Evaluate(dt));
                }
                return a;
            });

        Gamepad.current.SetMotorSpeeds(intensity.x, intensity.y);
    }
}
