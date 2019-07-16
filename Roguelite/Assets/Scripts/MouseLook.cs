using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform player, look;
    public bool invert = false;
    //[SerializeField]
    //private bool canUnlock = true;
    public float sensitivity = 5f;
    [SerializeField]
    private float rollAngle = 10f;
    [SerializeField]
    private float rollSpeed = 3f;
    [SerializeField]
    private Vector2 defaultLookLimits = new Vector2(-70f, 80f);
    public Vector2 lookAngles;
    public Vector2 currentMouseLook;
    private float currentRollAngle;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void FixedUpdate()
    {
        //CursorLock();

        if(Cursor.lockState == CursorLockMode.Locked)
        {
            Look();
        }
    }

    //void CursorLock()
    //{
    //    if(Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        if(Cursor.lockState == CursorLockMode.Locked)
    //        {
    //            Cursor.lockState = CursorLockMode.None;
    //        }

    //        else
    //        {
    //            Cursor.lockState = CursorLockMode.Locked;
    //            Cursor.visible = false;
    //        }
    //    }
    //}

    private void Look()
    {
        currentMouseLook = new Vector2(Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
        lookAngles.x += currentMouseLook.x * sensitivity * (invert ? 1f : -1f);
        lookAngles.y += currentMouseLook.y * sensitivity;
        lookAngles.x = Mathf.Clamp(lookAngles.x, defaultLookLimits.x, defaultLookLimits.y);
        currentRollAngle = Mathf.Lerp(currentRollAngle, Input.GetAxisRaw("Mouse X") * rollAngle, Time.deltaTime * rollSpeed);
        look.localRotation = Quaternion.Euler(lookAngles.x, 0f, currentRollAngle);
        player.localRotation = Quaternion.Euler(0f, lookAngles.y, 0f);
    }
}
