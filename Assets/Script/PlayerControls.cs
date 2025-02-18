using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerControls : MonoBehaviour
{
    public float movespeed = 5f;
    public float rotationspeed = 700f;
    public float cameraDistance = 5f;
    public float cameraheight = 2f;
    public float lookSpeedX = 2f;
    public float lookSpeedY = 2f;

    public CharacterController characterController;
    public Transform CameraTransform;
    public Text uiText;

    private bool isNearDraggable = false;
    private bool isHoldingObject = false;
    private GameObject currentDraggableObject = null;

    private float rotationX = 0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        CameraTransform = Camera.main.transform;
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        HandleCamera();


        if (isNearDraggable && Input.GetKey(KeyCode.E))
        {
           
              
                    if (!isHoldingObject)
                    {

                        PickUpObject();
                    }
                    else
                    {

                        DropObject();
                    }
                }
            }
        

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + CameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationspeed, 0.1f);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            characterController.Move(moveDirection * movespeed * Time.deltaTime);
        }
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");


        transform.Rotate(Vector3.up * mouseX * rotationspeed * Time.deltaTime);


        rotationX -= mouseY * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);
        CameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void HandleCamera()
    {
        Vector3 cameraPosition = transform.position - transform.forward * cameraDistance + Vector3.up * cameraheight;
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, cameraPosition, Time.deltaTime * 5f);
        CameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Draggable"))
        {
            currentDraggableObject = other.gameObject;
            uiText.text = "Pick Up";
            isNearDraggable = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Draggable"))
        {
            
            if (!isHoldingObject)
            {
                uiText.text = "";
            }
            currentDraggableObject = null;
            isNearDraggable = false;
        }
    }


    private void PickUpObject()
    {
        if (currentDraggableObject != null)
        {

            Rigidbody rb = currentDraggableObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;  
            }
            currentDraggableObject.transform.SetParent(transform);
            currentDraggableObject.transform.localPosition = new Vector3(0, 1, 2);
            uiText.text = "Drop";
            isHoldingObject = true;
        }
    }


    private void DropObject()
    {
        if (currentDraggableObject != null)
        {
            currentDraggableObject.transform.SetParent(null);
            Rigidbody rb = currentDraggableObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            currentDraggableObject = null;
            uiText.text = "";
            isHoldingObject = false;
            Debug.Log("Dropped object!");
        }
        else
        {
            Debug.LogWarning("No object to drop!");
        }
    }
}