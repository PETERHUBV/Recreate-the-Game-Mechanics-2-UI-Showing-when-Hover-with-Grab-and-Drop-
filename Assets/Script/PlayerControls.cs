
using JetBrains.Annotations;
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

    public void Update()
    {
        MovePlayer();
        RotatePlayer();
        HandleCamera();
        DropandPick();




    void DropandPick()
        {
            if (isNearDraggable && Input.GetKeyDown(KeyCode.E))
            {


                if (!isHoldingObject)
                {

                    PickUpObject();
                }
            }
            if (isHoldingObject && Input.GetKeyDown(KeyCode.R))
            {


                DropObject();
            }
        }
    }
       



    private void MovePlayer()
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

    private void HandleCamera()
    {
        Vector3 cameraPosition = transform.position - transform.forward * cameraDistance + Vector3.up * cameraheight;
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, cameraPosition, Time.deltaTime * 5f);
        CameraTransform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
    }


    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Draggable") && !isHoldingObject)
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

    private bool IsObjectWithinRange(Collider other)
    {
        float grabRange = 3f; 
        float distance = Vector3.Distance(transform.position, other.transform.position);
        return distance <= grabRange;
    }



    private void PickUpObject()
    {
        if (currentDraggableObject != null && !isHoldingObject)
        {
           
            Rigidbody rb = currentDraggableObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
            currentDraggableObject.transform.SetParent(transform);
            currentDraggableObject.transform.localPosition = new Vector3(0, 1f, 2f);
            uiText.text = "Drop";
            isHoldingObject = true;

           
        }
    }


    private void DropObject()
    {
        
        if (currentDraggableObject != null && isHoldingObject)
        {
            
            currentDraggableObject.transform.SetParent(null);
            Rigidbody rb = currentDraggableObject.GetComponent<Rigidbody>();
            if (rb != null && !isHoldingObject)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
            }
            currentDraggableObject = null;
            uiText.text = "";
            isHoldingObject = false;
            Debug.Log("Dropped object!");
        }
        else
        {
            Debug.Log("No object to drop or not holding anything.");
        }
    }
}