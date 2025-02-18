using UnityEngine;
using UnityEngine.UI;

public class GrabandDrop : MonoBehaviour
{
  

    public Camera playerCamera;
    public float interactDistance = 3f;
    public GameObject draggableObject;
    public Text uiText;

    private bool isCarrying = false;
    private GameObject pickedObject = null;
    private Vector3 initialObjectPosition;
    private Rigidbody objectRigidbody;

    // Update is called once per frame
    void Update()
    {
        HandleObjectInteraction();
    }

    
    void HandleObjectInteraction()
    {
        if (isCarrying)
        {
            
            if (pickedObject != null)
            {
                
                pickedObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward * interactDistance;

               
                pickedObject.transform.position = new Vector3(pickedObject.transform.position.x, initialObjectPosition.y, pickedObject.transform.position.z);
            }
        }

       
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
           
            if (hit.collider.CompareTag("Draggable"))
            {
               
                if (!isCarrying)
                {
                    uiText.text = " Pick Up";
                }
                else
                {
                    uiText.text = " Drop";
                }

               
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (isCarrying)
                    {
                        DropObject(hit.collider.gameObject);
                    }
                    else
                    {
                        PickUpObject(hit.collider.gameObject);
                    }
                }
            }
        }
        else
        {
           
            uiText.text = "";
        }
    }

    
    void PickUpObject(GameObject obj)
    {
        pickedObject = obj;
        objectRigidbody = pickedObject.GetComponent<Rigidbody>();
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = true; 
        }
        isCarrying = true;
        initialObjectPosition = pickedObject.transform.position;
    }

    
    void DropObject(GameObject obj)
    {
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = false; 
        }
        pickedObject = null;
        isCarrying = false;
    }
}
