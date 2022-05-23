using UnityEngine;
using UnityEngine.Animations;

public static class ExtensionMethods
{
    /**
     * <summary>Create vec2(x, y)</summary>
     */
    public static Vector2 xy(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }

    /**
     * <summary>Create vec2(x, z)</summary>
     */
    public static Vector2 xz(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }

    /**
     * <summary>Create vec3(x, 0, y)</summary>
     */
    public static Vector3 x0y(this Vector2 vec)
    {
        return new Vector3(vec.x, 0, vec.y);
    }

    /**
     * <summary>Query the component</summary>
     */
    public static T Q<T>(this GameObject obj) where T : Component
    {
        return obj.GetComponent<T>();
    }

    /**
     * <summary>Query the component</summary>
     */
    public static T Q<T>(this Behaviour obj) where T : Component
    {
        return obj.GetComponent<T>();
    }

    /**
     * <summary>Query the component</summary>
     */
    public static T Q<T>(this Transform obj) where T : Component
    {
        return obj.GetComponent<T>();
    }

    public static Vector3 position(this GameObject obj)
    {
        return obj.transform.position;
    }

    public static Quaternion rotation(this GameObject obj)
    {
        return obj.transform.rotation;
    }

    public static Rigidbody SetVelocity(this Rigidbody rb, Vector3 velocity)
    {
        rb.velocity = velocity;
        return rb;
    }

    public static Rigidbody SetIsKinematic(this Rigidbody rb, bool isKinematic)
    {
        rb.isKinematic = isKinematic;
        return rb;
    }

    public static Quaternion DirectionToRotation(this Vector2 vec, Axis axis = Axis.Y)
    {
        Vector3 axe;
        switch (axis)
        {
            case Axis.X:
                axe = Vector3.right;
                break;
            case Axis.Y:
            default:
                axe = Vector3.up;
                break;
            case Axis.Z:
                axe = Vector3.forward;
                break;
            case Axis.None:
                axe = Vector3.zero;
                break;
        }

        double angle = Mathf.Atan2(vec.y, vec.x);
        angle = angle * 180 / Mathf.PI;
        return Quaternion.AngleAxis((float)angle, (Vector3)axe);
    }

    public static bool IsEmpty(this Collider[] colliders)
    {
        return colliders.Length == 0;
    }
}