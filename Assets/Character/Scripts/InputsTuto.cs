using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputsTuto : MonoBehaviour
{
    public string controlScheme;

    private void Start()
    {
        var input = new Character_Controls();
    }

    private void OnEnable()
    {
        InputUser.onChange += OnInputDeviceChange;
    }

    private void OnDisable()
    {
        InputUser.onChange -= OnInputDeviceChange;
    }

    private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged) {
            controlScheme = user.controlScheme.Value.name;
        }
    }

    private void Update()
    {
        //Input.
    }
}
