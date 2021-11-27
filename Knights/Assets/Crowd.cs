/*
*	TickLuck
*	All rights reserved
*/
using UnityEngine;
using UnityEngine.AI;

public class Crowd : MonoBehaviour
{
    #region Fields
    [SerializeField] private joinPoint[] joinPoints;
    [SerializeField] private Material newMaterial;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private MeshRenderer thisMaterial;

    private Transform crowd_target;
    private Transform current_target;

    public bool is_busy;
    #endregion

    #region Unity Methods
    void Start()
    {
        // Make element face the direction of motion
        rb = GetComponent<Rigidbody>();
        thisMaterial = GetComponent<MeshRenderer>();
        //agent = GetComponent<NavMeshAgent>();
        is_busy = false;
        crowd_target = GameObject.FindGameObjectWithTag("CROWD_TARGET").transform;
    }

    private void Update()
    {
        if (is_busy)
        {
            //CrowdMotionControl();
        }
    }

    public void CrowdMotionControl()
    {
        if (Vector3.Distance(transform.position, current_target.position) >= 2f)
            agent.SetDestination(current_target.position);
        else
            agent.SetDestination(current_target.position);
    }

    public void ChangeMaterial()
    {
        if (newMaterial != null)
            thisMaterial.material = newMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crowd")
        {
            foreach (var join in joinPoints)
            {
                if (!join.is_busy)
                {
                    // parent new element and set join point as busy
                    // disable tag for avoiding double collisions
                    // color element on collision

                    other.GetComponent<Crowd>().joinPoints[0].transform.SetParent(join.transform);
                    other.GetComponent<Crowd>().joinPoints[0].is_busy = true;
                    other.GetComponent<Crowd>().ChangeMaterial();
                    other.GetComponent<Crowd>().current_target = join.transform;

                    other.transform.position = new Vector3(join.transform.position.x,
                        other.transform.position.y, join.transform.position.z);
                    other.transform.SetParent(join.transform);

                    other.gameObject.tag = "Untagged";
                    join.is_busy = true;
                    return;
                }
            }
        }

        if (other.tag == "Obstacle")
        {
            if (gameObject.tag != "Player")
            {
                is_busy = false;

                foreach (var join in joinPoints)
                {
                    // free parent joinPoint
                    // disable element
                    join.gameObject.SetActive(false);
                }
                gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
