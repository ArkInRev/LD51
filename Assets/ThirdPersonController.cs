using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField]
    private float moveSpeed = 350;
    [SerializeField]
    private float sensitivity = 1f;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private float rotSpeed = 200f;
    [SerializeField]
    private Transform cameraFollowPoint;

    public float upClampRot, downClampRot;

    private float horizontal;
    private float forward;
    private float look;
    private float vertLook;


    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {

    }
    void Update()
    {
        look = (Input.GetAxisRaw("Mouse X") * rotSpeed * sensitivity);
        vertLook = (Input.GetAxisRaw("Mouse Y") * rotSpeed * sensitivity);

        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleCursorLock();
        if (Input.GetKeyDown(KeyCode.Alpha1)) sensitivity = 0.05f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) sensitivity = 0.1f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) sensitivity = 0.15f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) sensitivity = 0.25f;
        if (Input.GetKeyDown(KeyCode.Alpha5)) sensitivity = 0.5f;
        if (Input.GetKeyDown(KeyCode.Alpha6)) sensitivity = 0.75f;
        if (Input.GetKeyDown(KeyCode.Alpha7)) sensitivity = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha8)) sensitivity = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha9)) sensitivity = 4f;
        if (Input.GetKeyDown(KeyCode.Alpha0)) sensitivity = 8f;

    }


    private void FixedUpdate()
    {
        player.Rotate(0, look * Time.fixedDeltaTime, 0, Space.Self);
        cameraFollowPoint.Rotate(-1 * vertLook * Time.fixedDeltaTime, 0, 0, Space.Self);

        Vector3 curVertRot = cameraFollowPoint.localRotation.eulerAngles;
        curVertRot.x = Mathf.Clamp(curVertRot.x, downClampRot, upClampRot);
        if (curVertRot.x < 0) curVertRot.x = 360 + curVertRot.x;
        //cameraFollowPoint.localRotation = Quaternion.Euler(curVertRot);


        forward = Input.GetAxis("Vertical");
        if (forward != 0)
        {
            characterController.SimpleMove(moveSpeed * transform.forward * Time.fixedDeltaTime * forward);
        }
        horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0)
        {
            characterController.SimpleMove(moveSpeed * transform.right * Time.fixedDeltaTime * horizontal);
        }

    }

    private void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}