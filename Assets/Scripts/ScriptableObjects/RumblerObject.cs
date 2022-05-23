using UnityEngine;

[CreateAssetMenu(fileName = "Rumble", menuName = "ScriptableObjects/RumblerObject", order = 1)]
public class RumblerObject : ScriptableObject
{
    public AnimationCurve lowCurve;
    public AnimationCurve highCurve;
}
