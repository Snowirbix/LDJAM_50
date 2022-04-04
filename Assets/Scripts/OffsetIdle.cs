using UnityEngine;

public class OffsetIdle : MonoBehaviour
{
    public float offsetIdle = 0f;

    private void Awake()
    {
        this.Q<Animator>().SetFloat("Delay", offsetIdle);
    }
}
