using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
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
    [SerializeField] private Vector3 chestForce = new Vector3(0.0f, 1200.0f, 0.0f);
    [SerializeField] private Vector3 headForce = new Vector3(0.0f, 60.0f, 0.0f);
    [SerializeField] private Vector3 feetForce = new Vector3(0.0f, -1600.0f, 0.0f);

    void Start()
    {
        rbList = new List<Rigidbody>{ hips, chest, head, rightUpperArm, rightForeArm, rightHand, rightIndex, rightFingers,
                                      leftUpperArm, leftForeArm, leftHand, leftIndex, leftFingers, rightUpperLeg, rightLowerLeg, 
                                      rightAnkle, rightFoot, leftUpperLeg, leftLowerLeg, leftAnkle, leftFoot };
        foreach (Rigidbody rb in rbList)
        {
            rb.drag *= dragScalar;
            rb.mass *= massScalar;
        }
    }

    void FixedUpdate()
    {
        // Apply persistent forces
        chest.AddForce(chestForce);
        head.AddForce(headForce);

        leftAnkle.AddForce(feetForce);
        rightAnkle.AddForce(feetForce);
    }

    public void Jump()
    {
        hips.AddForce(new Vector3(0.0f, 800.0f, 0.0f), ForceMode.Impulse);
    }

    public void BangHead()
    {
        head.AddForce(new Vector3(0.0f, -50.0f, 50.0f), ForceMode.Impulse);
        chest.AddForce(new Vector3(0.0f, 0.0f, -25.0f));
    }

    public void ScaleMass(float scalar)
    {
        foreach (Rigidbody rb in rbList)
        {
            rb.mass *= massScalar;
        }
    }
}
