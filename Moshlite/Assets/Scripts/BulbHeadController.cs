using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbHeadController : MonoBehaviour
{
    [Header("Body Part Rigidbodies")]
    [SerializeField] private Rigidbody hips;
    [SerializeField] private Rigidbody chest;
    [SerializeField] private Rigidbody head;

    [SerializeField] private Rigidbody rightUpperArm;
    [SerializeField] private Rigidbody rightForeArm;
    [SerializeField] private Rigidbody rightHand;
    [SerializeField] private Rigidbody rightIndex;
    [SerializeField] private Rigidbody rightFingers;
    [SerializeField] private Rigidbody leftUpperArm;
    [SerializeField] private Rigidbody leftForeArm;
    [SerializeField] private Rigidbody leftHand;
    [SerializeField] private Rigidbody leftIndex;
    [SerializeField] private Rigidbody leftFingers;

    [SerializeField] private Rigidbody rightUpperLeg;
    [SerializeField] private Rigidbody rightLowerLeg;
    [SerializeField] private Rigidbody rightAnkle;
    [SerializeField] private Rigidbody rightFoot;
    [SerializeField] private Rigidbody leftUpperLeg;
    [SerializeField] private Rigidbody leftLowerLeg;
    [SerializeField] private Rigidbody leftAnkle;
    [SerializeField] private Rigidbody leftFoot;

    private List<Rigidbody> rbList;

    [Header("Physics Scalars")]
    // Scalars applied to all body parts at start
    [SerializeField] private float dragScalar = 1.0f; 
    [SerializeField] private float massScalar = 2.0f;

    [Header("Persistent Forces")]
    [SerializeField] private Vector3 chestForce = new Vector3(0.0f, 1100.0f, 0.0f);
    [SerializeField] private Vector3 headForce = new Vector3(0.0f, 60.0f, 0.0f);
    [SerializeField] private Vector3 feetForce = new Vector3(0.0f, -1600.0f, 0.0f);

    private Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f); // Represents "real gravity", set by gravity zones
    [HideInInspector] public Vector3 gravityScalar = new Vector3(1.0f, 1.0f, 1.0f); // Can be changed for effects
    [HideInInspector] public bool useGravity = true;

    void Start()
    {
        rbList = new List<Rigidbody>{ hips, chest, head, rightUpperArm, rightForeArm, rightHand, rightIndex, rightFingers,
                                      leftUpperArm, leftForeArm, leftHand, leftIndex, leftFingers, rightUpperLeg, rightLowerLeg, 
                                      rightAnkle, rightFoot, leftUpperLeg, leftLowerLeg, leftAnkle, leftFoot };
        foreach (Rigidbody rb in rbList)
        {
            rb.drag *= dragScalar;
            rb.mass *= massScalar;
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        // Apply persistent forces

        // If there's no gravity, dont apply persistent forces to make body rigid
        // Otherwise make sure feet are always pointed in the direction of gravity
        if (useGravity)
        {
            if (gravity.y > 0.0f)
            {
                // Forces need to be upped a lot when gravity is reversed and I'm not sre why
                chest.AddForce(-2.0f * chestForce);
                head.AddForce(-4.0f * headForce);

                leftAnkle.AddForce(-1.0f * feetForce);
                rightAnkle.AddForce(-1.0f * feetForce);
            }
            else
            {
                chest.AddForce(chestForce);
                head.AddForce(headForce);

                leftAnkle.AddForce(feetForce);
                rightAnkle.AddForce(feetForce);
            }
            //leftFoot.AddForce(feetForce); // generates some wacky footwork but is less stable
            //rightFoot.AddForce(feetForce);

            foreach (Rigidbody rb in rbList)
            {
                rb.AddForce(new Vector3(gravity.x * gravityScalar.x, gravity.y * gravityScalar.y, gravity.z * gravityScalar.z), ForceMode.Acceleration);
            }
        }
    }

    public void Jump()
    {
        hips.AddRelativeForce(new Vector3(0.0f, 800.0f, 0.0f), ForceMode.Impulse);
    }

    public void BangHead()
    {
        head.AddRelativeForce(new Vector3(0.0f, -50.0f, 50.0f), ForceMode.Impulse);
        chest.AddRelativeForce(new Vector3(0.0f, 0.0f, -25.0f));
    }

    public void ScaleMass(float scalar)
    {
        foreach (Rigidbody rb in rbList)
        {
            rb.mass *= scalar;
        }
    }

    // To be used by gravity zones, NOT for effects
    public void SetRealGravity(Vector3 newGrav)
    {
        gravity = newGrav;
    }
}
