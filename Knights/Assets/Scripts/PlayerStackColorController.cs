/*
*	TickLuck
*	All rights reserved
*/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStackColorController : MonoBehaviour
{
    #region Variables
    [SerializeField] private Color _color;
    [SerializeField] private Renderer[] _renderers;

    [SerializeField] public static bool is_Playing;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float moveSideWaysLerp;

    [SerializeField] private GameObject GO_UI;

    // Change counter when the new Human, weapon is picked
    // Dicrease counter when obstacle is detected
    [SerializeField] private Text COUNTER_txt;
    public static int COUNTER;

    private Rigidbody rb;

    private Transform parentPickUp;
    public static Transform ParentPickUp_buffer;
    [SerializeField] private Transform stackPosition;
    private Vector3 default_weapon_rotation;
    [SerializeField] private float rotate_angle_for_weapon;

    [SerializeField] private float KickForce;
    public static bool atEnd;

    #region Force With Bar
    [SerializeField] private GameObject forceAmountBar;
    [SerializeField] public static float forwardForce; // static
    [SerializeField] private float forceAdder;
    [SerializeField] private float forceReducer;
    #endregion

    [Header("Weapon")]
    [Space]
    [SerializeField] GameObject Weapon;
    private Vector3 weapon_scale_current;
    private Vector3 weapon_scale_default;

    public static Color PlayerColor_buffer;
    public static Action<float> Kick;

    public static int stackLength;
    private float kickForce_buffer;
    #endregion

    #region UnityMethods
    void Start()
    {
        Camera.main.fieldOfView = 60f;
        Time.timeScale = 1f;

        COUNTER = 0;
        COUNTER_txt.text = COUNTER.ToString();

        // Change the scale while score increase 
        // And change the actual local scale using weapon_scale_current
        // Use weapon_scale_default as constraint
        weapon_scale_default = Weapon.transform.localScale;
        weapon_scale_current = weapon_scale_default;
        rotate_angle_for_weapon = 0;
        default_weapon_rotation = new Vector3(0, 0, 0);

        GO_UI.SetActive(false);
        kickForce_buffer = 0;
        stackLength = 0;
        atEnd = false;
        forceAmountBar.SetActive(false);
        //int rand = UnityEngine.Random.Range(0, ColorController.s_pickUpColors_Buffer.Count);
        rb = GetComponent<Rigidbody>();
        //SetColor(ColorController.s_pickUpColors_Buffer[rand].Color);
    }

    void Update()
    {
        PlayerColor_buffer = _color;
        ParentPickUp_buffer = parentPickUp;

        if (is_Playing)
            MoveForward();

        if (atEnd)
        {
            forwardForce -= forceReducer * Time.deltaTime;
            if (forwardForce < 0)
                forwardForce = 0;

            //rotate_angle_for_weapon -= 0.001f * forwardForce;

            //if (rotate_angle_for_weapon < 0f)
            //{
            //    rotate_angle_for_weapon = 0f;
            //    parentPickUp.transform.rotation = Quaternion.Euler(default_weapon_rotation);
            //    return;
            //}

            //parentPickUp.transform.
            //    RotateAround(parentPickUp.GetComponent<Pickup_Controller>().bottom_pointer.position,
            //    Vector3.right, rotate_angle_for_weapon);

            //float angle = 0.045f * forwardForce;

            //if (angle <= 0f)
            //{
            //    angle = 0f;
            //    return;
            //}
        }

        // Work over rotation 
        // Block for rotation in both sides 

        #region PC Control
        if (Input.GetMouseButtonDown(0))
            if (atEnd)
            {
                forwardForce += forceAdder;
                rotate_angle_for_weapon += 0.09f * forwardForce;

                if (rotate_angle_for_weapon >= 90f)
                {
                    rotate_angle_for_weapon = 0f;
                    return;
                }

                parentPickUp.transform.
                    RotateAround(parentPickUp.GetComponent<Pickup_Controller>().bottom_pointer.position,
                    -Vector3.right, rotate_angle_for_weapon);
            }

        if (Input.GetMouseButton(0))
        {
            if (atEnd)
                return;

            if (is_Playing == false)
            {
                is_Playing = true;
                EnemyController.is_moving = true; // Set false when finish
                Debug.Log("EnemyController.is_moving = true");
            }
            EnemyController.is_moving = true; // Set false when finish
            MoveSideWays_usingMouse();
        }
        #endregion

        #region Phone Control
        if (Input.touchCount > 0)
            if (atEnd)
            {
                forwardForce += forceAdder;
                rotate_angle_for_weapon += 0.09f * forwardForce;

                if (rotate_angle_for_weapon >= 90f)
                {
                    rotate_angle_for_weapon = 0f;
                    return;
                }

                parentPickUp.transform.
                    RotateAround(parentPickUp.GetComponent<Pickup_Controller>().bottom_pointer.position,
                    -Vector3.right, rotate_angle_for_weapon);
            }

        if (Input.touchCount > 0)
        {
            if (atEnd)
                return;

            if (is_Playing == false)
            {
                is_Playing = true;
                EnemyController.is_moving = true; // Set false when finish
                Debug.Log("EnemyController.is_moving = true");
            }
            EnemyController.is_moving = true; // Set false when finish
            MoveSideWays_usingTouch();
        }
        #endregion
    }
    #endregion

    #region MoveSideWays_usingTouch
    private void MoveSideWays_usingTouch()
    {
        transform.position = Vector3.Lerp(transform.position,
                  new Vector3(transform.position.x + Input.GetTouch(0).deltaPosition.x,
                  transform.position.y, transform.position.z), moveSideWaysLerp * Time.deltaTime);

        //Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        //RaycastHit hitInfo;
        //if (Physics.Raycast(ray, out hitInfo, 100))
        //{
        //    transform.position = Vector3.Lerp(transform.position,
        //        new Vector3(transform.position.x + hitInfo.point.x, transform.position.y, transform.position.z), moveSideWaysLerp * Time.deltaTime);
        //}
    }
    #endregion

    #region MoveSideWays_usingMouse
    private void MoveSideWays_usingMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100))
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(hitInfo.point.x, transform.position.y, transform.position.z), moveSideWaysLerp * Time.deltaTime);
        }
    }
    #endregion

    private void MoveForward()
    {
        rb.velocity = Vector3.forward * forwardSpeed;
    }

    #region SetColor
    private void SetColor(Color color)
    {
        _color = color;
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.SetColor("_Color", _color);
        }
    }
    #endregion

    public void OnTriggerEnter(Collider other)
    {
        #region FINISH ENTERED
        if (other.tag == "FinishLineStart")
        {
            //EnemyController.is_moving = false; // Set false when finish

            forceAmountBar.SetActive(true);

            // Place player in the middle of the platform
            transform.position = new Vector3(0, transform.position.y, transform.position.z);

            atEnd = true;
        }

        #region FinishLineEnd

        // Using forceAmountBar -> More Power == Bigger Angle to lean == More Force to kick 
        // Time is slowed (0.7f)
        if (other.tag == "FinishLineEnd")
        {
            #region STACK COLORS FASHION
            //rb.velocity = Vector3.zero;
            //is_Playing = false;
            //LaunchStack();
            #endregion

            forceAmountBar.SetActive(false);

        }
        #endregion

        #endregion

        if(other.tag == "Obstacle")
        {
            DicreaseCOUNTER(10);
            ModifyWeaponScale();
        }

        //if (atEnd)
        //    return;

        #region PICKUP
        if (other.tag == "Pickup")
        {
            Transform otherTransform = other.transform;
            stackLength++;

            // increment the COUNTER 
            // inrease the size of weapon
            // increase viewfield of Camera 

            if (other.transform.parent != null)
            {
                otherTransform = other.transform.parent;
            }

            //if (_color == other.GetComponent<Pickup_Controller>().PickUp_color)
            GAME_CONTROLLER.instance.UpdateScore(otherTransform.GetComponent<Pickup_Controller>().value_buffer);

            IncreaseCOUNTER(otherTransform.GetComponent<Pickup_Controller>().value_buffer);

            ModifyWeaponScale();

            #region COLORING
            //else
            //{
            //    #region WRONG PICKUP CHOSEN
            //    GAME_CONTROLLER.instance.UpdateScore(otherTransform.GetComponent<Pickup_Controller>().value_buffer * -1);
            //    Destroy(other.gameObject);
            //    if (parentPickUp != null)
            //    {
            //        if (parentPickUp.childCount > 1)
            //        {
            //            parentPickUp.position -= Vector3.forward * parentPickUp.GetChild(parentPickUp.childCount - 1).localScale.y;
            //            parentPickUp.transform.rotation = Quaternion.Euler(Vector3.zero);
            //            Destroy(parentPickUp.GetChild(parentPickUp.childCount - 1).gameObject);
            //        }
            //        else
            //            Destroy(parentPickUp.gameObject);
            //    }

            //    return;
            //    #endregion
            //}
            #endregion

            #region SETTINGS
            Rigidbody otherRB = otherTransform.GetComponent<Rigidbody>();
            otherRB.isKinematic = true;
            //other.enabled = false;
            other.gameObject.GetComponent<Pickup_Controller>().is_Picked = true;
            #endregion

            if (parentPickUp == null)
            {
                #region SET NEW PICKUP PARENT
                parentPickUp = otherTransform;

                parentPickUp.position = stackPosition.position +
                  Vector3.up * (parentPickUp.gameObject.GetComponent<BoxCollider>().bounds.size.y / 2);

                // Move Weapon Upwards
                parentPickUp.transform.rotation = Quaternion.Euler(-90f, 0, 0);
                parentPickUp.parent = stackPosition;
                default_weapon_rotation = parentPickUp.transform.rotation.eulerAngles;
                #endregion
            }

            else
            {
                otherTransform.gameObject.SetActive(false);

                parentPickUp.position = stackPosition.position +
                 Vector3.up * (parentPickUp.gameObject.GetComponent<BoxCollider>().bounds.size.y / 2);

                #region PLACE PICKUP ON FRONT OF PARENT
                //parentPickUp.position += Vector3.forward * (otherTransform.localScale.z);
                //otherTransform.position = stackPosition.position;
                //otherTransform.rotation = Quaternion.Euler(Vector3.zero);
                //otherTransform.parent = parentPickUp;

                //// Disable the Pick
                //otherTransform.GetChild(0).gameObject.SetActive(false);
                #endregion
            }
        }
        #endregion

        #region PICK
        if (other.tag == "Pick")
        {
            if (atEnd)
            {
                Camera.main.GetComponent<CameraMovementController>().Target = transform;
                Camera.main.fieldOfView += 20f;
                EnemyController.is_moving = false;

                // Drop the stick
                stackPosition.transform.SetParent(null);

                // Disable the Pick of Player
                parentPickUp.GetChild(0).GetComponent<BoxCollider>().enabled = false;

                // Disable Motion
                is_Playing = false;
                rb.velocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.None;

                // Set the force depending on the Difference in length 
                kickForce_buffer = (EnemyController.stackLength / stackLength) * KickForce;

                // Z is negative to kick opposite way to motion
                rb.AddForce(new Vector3(0, kickForce_buffer, -kickForce_buffer), ForceMode.Impulse);

                // Game Over
                StartCoroutine(GameOver());
            }
        }
        #endregion

        if (other.tag == "Obstacle")
        {
            other.gameObject.SetActive(false);
            DicreaseCOUNTER(10);
            ModifyWeaponScale();
        }
    }

    #region COUNTER
    private void IncreaseCOUNTER(int value)
    {
        COUNTER += value;
        Camera.main.fieldOfView += value * 0.1f;
        COUNTER_txt.text = COUNTER.ToString();
    }

    private void DicreaseCOUNTER(int value)
    {
        COUNTER -= value;

        if (COUNTER <= 1)
        {
            COUNTER = 1;
            COUNTER_txt.text = COUNTER.ToString();
            return;
        }

        Camera.main.fieldOfView -= value * 0.1f;
        COUNTER_txt.text = COUNTER.ToString();
    }
    #endregion

    #region ModifyWeaponScale
    private void ModifyWeaponScale()
    {
        weapon_scale_current = weapon_scale_default * COUNTER * 0.01f;

        if (weapon_scale_current.magnitude <= weapon_scale_default.magnitude)
        {
            weapon_scale_current = weapon_scale_default;
            return;
        }

        parentPickUp.transform.localScale = weapon_scale_current;
    }
    #endregion

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        GO_UI.SetActive(true);
    }

    #region LaunchStack
    private void LaunchStack()
    {
        Camera.main.GetComponent<CameraMovementController>().Target = parentPickUp;
        Kick(forwardForce);
    }
    #endregion

    #region KICK ENEMY
    private void KickEnemy()
    {

    }
    #endregion

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ColorWall")
        {
            SetColor(other.GetComponent<ColorWallController>().NewColor);
        }
    }
}
