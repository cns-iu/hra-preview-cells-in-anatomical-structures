using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControls : MonoBehaviour
{
    [SerializeField] Transform m_Target;

    [SerializeField] private bool m_ISMovementAllowed = false;
    [SerializeField] private bool m_ISRotationAllowed = false;
    [SerializeField] private bool m_ISZoomAllowed = false;

    [SerializeField] Vector3 m_DefaultTargetPosition;
    [SerializeField] Quaternion m_DefaultTargetRotation;

    [SerializeField] float rotationSpeed = 1;

    [SerializeField] float m_PanSpeed;

    [SerializeField] float smoothFactor = 1;

    [SerializeField] Vector3 _cameraOffset;

    [SerializeField] Vector3 m_DefaultCameraOffset;
    [SerializeField] Quaternion camTurnAngleX;
    [SerializeField] Quaternion camTurnAngleY;

    [SerializeField] public bool m_CanBeUsed = true;

    private Vector3 initialPosition;
    private Vector3 targetInitialPosition;



    //void OnEnable()
    //{
    //    SliderPointerHandler.SliderEnterEvent += SetCameraUsage;
    //    CameraViewManager.CameraResetEvent += ResetCamera;
    //}

    //void OnDestroy()
    //{
    //    SliderPointerHandler.SliderEnterEvent -= SetCameraUsage;
    //    CameraViewManager.CameraResetEvent -= ResetCamera;
    //}



    void Start()
    {
        //set all the variables for reseting the camera
        SetDefaultCameraOffset();
        SetDefaultTarget();

        SetCameraInitialPosition();
    }

    /// <summary>
    /// IDK what the importance of this is but its here
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    /// <summary>
    /// Set the targets inital position and rotation
    /// </summary>
    void SetDefaultTarget()
    {
        m_DefaultTargetPosition = m_Target.transform.position;
        m_DefaultTargetRotation = m_Target.transform.rotation;
    }

    /// <summary>
    /// Set the camera's intial offset
    /// </summary>
    void SetDefaultCameraOffset()
    {
        m_DefaultCameraOffset = this.transform.position - m_Target.transform.position;
        _cameraOffset = m_DefaultCameraOffset;
    }

    /// <summary>
    /// Set the camera's inital position using the offset
    /// </summary>
    void SetCameraInitialPosition()
    {
        //
        Vector3 newPos = m_Target.position + _cameraOffset;
        this.transform.position = Vector3.Slerp(this.transform.position, newPos, smoothFactor);

        //make the camera look at the target
        this.transform.LookAt(m_Target.transform);

        //set the actual position
        initialPosition = this.transform.position;
        targetInitialPosition = m_Target.position;
    }

    /// <summary>
    /// Reset the camera target to its initial position
    /// </summary>
    void ResetCameraTarget()
    {
        _cameraOffset = m_DefaultCameraOffset;
        m_Target.transform.position = m_DefaultTargetPosition;
        m_Target.transform.rotation = m_DefaultTargetRotation;
    }

    /// <summary>
    /// Reset the camera back to its inital position
    /// </summary>
    public void ResetPosition()
    {
        //Get the target back in the original spot
        ResetCameraTarget();

        //reset the camera to its inital position
        this.transform.position = initialPosition;
        this.transform.LookAt(m_Target.transform); //has to be after resetcameratarget
    }



    /// <summary>
    /// Has to be late update to allow others to change first
    /// change the camera position and rotation
    /// </summary>
    private void LateUpdate()
    {
        //eject if false
        if (!m_CanBeUsed) return;

        //rotate the camera if the bool is true and the mouse clicked
        if (m_ISRotationAllowed)
        {
            if (Input.GetMouseButton(0))
            {
                Quaternion camTurnAngleX = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
                Quaternion camTurnAngleY = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotationSpeed, Vector3.back);

                if (transform.rotation.eulerAngles.y >= 0f && transform.rotation.eulerAngles.y <= 180f)
                {
                    camTurnAngleY = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * rotationSpeed, Vector3.back);
                }

                _cameraOffset = camTurnAngleX * camTurnAngleY * _cameraOffset;
                Vector3 newPos = m_Target.position + _cameraOffset;
                this.transform.position = Vector3.Slerp(this.transform.position, newPos, smoothFactor);

                //make the camera look at the target
                this.transform.LookAt(m_Target.transform);
            }
        }
        
        //move the camera if the bool is true and the mouse is clicked
        if (m_ISMovementAllowed)
        {
            if (Input.GetMouseButton(1))
            {
                float camPosDeltaX = Input.GetAxis("Mouse X");
                float camPosDeltaY = Input.GetAxis("Mouse Y");

                transform.Translate(Vector3.right * -camPosDeltaX * m_PanSpeed, Space.Self);
                transform.Translate(Vector3.up * -camPosDeltaY * m_PanSpeed, Space.Self);
                m_Target.transform.rotation = transform.rotation;
                m_Target.transform.Translate(Vector3.right * -camPosDeltaX * m_PanSpeed, Space.Self);
                m_Target.transform.Translate(Vector3.up * -camPosDeltaY * m_PanSpeed, Space.Self);
            }
        }

        //zoom the camera if the bool is true and the scroll wheel is moved
        if (m_ISZoomAllowed)
        {
            if (Input.mouseScrollDelta != new Vector2(0f, 0f) && !Input.GetKey(KeyCode.LeftShift))
            {
                GetComponent<Camera>().fieldOfView -= Input.mouseScrollDelta.y;
            }
        }
    }
}
