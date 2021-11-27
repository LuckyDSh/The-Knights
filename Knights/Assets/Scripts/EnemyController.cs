/*
*	TickLuck
*	All rights reserved
*/
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Fields
    [SerializeField] private float forwardSpeed;
    [SerializeField] private int number_of_picks;
    [SerializeField] private GameObject PickUp_pref;
    [SerializeField] private Transform stackPosition;
    [SerializeField] private float KickForce;

    private Transform parentPickUp;
    private Rigidbody rb;
    public static Rigidbody rb_buffer;

    public static bool is_moving;

    public static int stackLength;
    private float kickForce_buffer;
    #endregion

    #region Unity Methods
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Set true when player starts the game
        is_moving = false;
        Camera.main.fieldOfView = 60f;
    }

    void Update()
    {
        if (is_moving)
            MoveForward();

        if (PlayerStackColorController.atEnd)
        {
            rb_buffer = rb;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pickup")
        {
            Transform otherTransform = other.transform;
            stackLength++;

            if (other.transform.parent != null)
            {
                otherTransform = other.transform.parent;
            }

            #region SETTINGS

            Rigidbody otherRB = otherTransform.GetComponent<Rigidbody>();
            otherRB.isKinematic = true;

            other.GetComponent<Collider>().enabled = false;
            // other.gameObject.GetComponent<Pickup_Controller>().is_Picked = true;
            #endregion

            if (parentPickUp == null)
            {
                #region SET NEW PICKUP PARENT
                parentPickUp = otherTransform;
                parentPickUp.position = stackPosition.position;
                parentPickUp.transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
                parentPickUp.parent = stackPosition;
                #endregion
            }
            else
            {
                #region PLACE PICKUP ON FRONT OF PARENT
                parentPickUp.position -= Vector3.forward * (otherTransform.localScale.z);
                otherTransform.position = stackPosition.position;
                otherTransform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
                otherTransform.parent = parentPickUp;

                // Disable the Pick
                otherTransform.GetChild(0).gameObject.SetActive(false);
                #endregion
            }
        }

        if (other.tag == "FinishLineStart")
        {
            PlayerStackColorController.atEnd = true;
        }

        if (other.tag == "Pick")
        {
            if (PlayerStackColorController.atEnd)
            {
                Camera.main.GetComponent<CameraMovementController>().Target = transform;
                //Camera.main.fieldOfView += 20f;
                PlayerStackColorController.is_Playing = false;

                // Drop the stick
                stackPosition.transform.SetParent(null);

                // Disable the Pick of Player
                parentPickUp.GetChild(0).GetComponent<BoxCollider>().enabled = false;

                // Disable Motion
                is_moving = false;
                rb.velocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.None;

                // Set the force depending on the Difference in length 
                kickForce_buffer = (PlayerStackColorController.stackLength / stackLength) * KickForce;

                // Z is possitive to kick opposite way to motion
                rb.AddForce(new Vector3(0, kickForce_buffer, kickForce_buffer), ForceMode.Impulse);
            }
        }
    }
    #endregion

    #region InicializePick
    private void InicializePick(GameObject other)
    {
        Transform otherTransform = other.transform;

        if (other.transform.parent != null)
        {
            otherTransform = other.transform.parent;
        }

        #region SETTINGS

        Rigidbody otherRB = otherTransform.GetComponent<Rigidbody>();
        otherRB.isKinematic = true;

        other.GetComponent<Collider>().enabled = false;
        // other.gameObject.GetComponent<Pickup_Controller>().is_Picked = true;
        #endregion

        if (parentPickUp == null)
        {
            #region SET NEW PICKUP PARENT
            parentPickUp = otherTransform;
            parentPickUp.position = stackPosition.position;
            parentPickUp.transform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            parentPickUp.parent = stackPosition;
            #endregion
        }
        else
        {
            #region PLACE PICKUP ON FRONT OF PARENT
            parentPickUp.position -= Vector3.forward * (otherTransform.localScale.z);
            otherTransform.position = stackPosition.position;
            otherTransform.rotation = Quaternion.Euler(new Vector3(0, 180f, 0));
            otherTransform.parent = parentPickUp;

            // Disable the Pick
            otherTransform.GetChild(0).gameObject.SetActive(false);
            #endregion
        }
    }
    #endregion

    private void MoveForward()
    {
        rb.velocity = -Vector3.forward * forwardSpeed;
    }
}
