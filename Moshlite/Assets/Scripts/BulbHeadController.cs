using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private Vector3 chestForce = new Vector3(0.0f, 1080.0f, 0.0f);
    [SerializeField] private Vector3 headForce = new Vector3(0.0f, 100.0f, 0.0f);
    [SerializeField] private Vector3 feetForce = new Vector3(0.0f, -1600.0f, 0.0f);

    private Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f); // Represents "real gravity", set by gravity zones
    [HideInInspector] public Vector3 gravityScalar = new Vector3(1.0f, 1.0f, 1.0f); // Can be changed for effects
    [HideInInspector] public bool useGravity = true;

    // Scale xyz must be equal
    private bool isBig = false;
    private Coroutine scaleCoroutine = null;
    private float defaultScale = 1.2f;
    private float maxScaleScalar = 2.0f;

    void Start()
    {
        defaultScale = transform.localScale.x;
        if (transform.localScale.y != defaultScale || transform.localScale.z != defaultScale)
        {
            Debug.LogWarning("Scale for bulbheads should be equal on axes");
        }
        
        rbList = new List<Rigidbody>{ hips, chest, head, rightUpperArm, rightForeArm, rightHand, rightIndex, rightFingers,
                                      leftUpperArm, leftForeArm, leftHand, leftIndex, leftFingers, rightUpperLeg, rightLowerLeg, 
                                      rightAnkle, rightFoot, leftUpperLeg, leftLowerLeg, leftAnkle, leftFoot };
        foreach (Rigidbody rb in rbList)
        {
            rb.drag *= dragScalar;
            rb.mass *= massScalar;
            rb.useGravity = false;
        }

        CharacterManager.S.Subscribe(this);
    }

    void FixedUpdate()
    {
        // Apply persistent forces

        // If there's no gravity, don't apply persistent forces to make body rigid
        // Otherwise make sure feet are always pointed in the direction of gravity
        if (useGravity)
        {
            if (gravity.y > 0.0f)
            {
                // Forces need to be upped a lot when gravity is reversed and I'm not sure why
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
        hips.AddRelativeForce(new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(1500.0f, 2750.0f), Random.Range(-100.0f, 100.0f)), ForceMode.Impulse);
    }

    public void BangHead()
    {
        head.AddRelativeForce(new Vector3(0.0f, -40.0f, 50.0f), ForceMode.Impulse);
        chest.AddRelativeForce(new Vector3(0.0f, 0.0f, 35.0f), ForceMode.Impulse);
        hips.AddRelativeForce(new Vector3(0.0f, 0.0f, -35.0f), ForceMode.Impulse);
    }

    public void CrowdKill()
    {
        float x = Random.Range(100.0f, 250.0f);
        float z = Random.Range(100.0f, 250.0f);
        float up = Random.Range(40.0f, 80.0f);

        if (Random.Range(0, 2) == 0)
        {
            // Right arm forward, left back (maybe??)
            rightForeArm.AddRelativeForce(new Vector3(-x, up, -z), ForceMode.Impulse);
            leftForeArm.AddRelativeForce(new Vector3(x, up, z), ForceMode.Impulse);

            rightUpperLeg.AddRelativeForce(new Vector3(0.0f, 25.0f, 25.0f), ForceMode.Impulse);
            rightLowerLeg.AddRelativeForce(new Vector3(0.0f, 60.0f, 0.0f), ForceMode.Impulse);
            // Have to add a ton of force to cancle out persistant foot force
            rightFoot.AddRelativeForce(new Vector3(0.0f, 300.0f, 400.0f), ForceMode.Impulse);

            chest.AddTorque(new Vector3(0.0f, 200.0f, 0.0f), ForceMode.Impulse); // Twist chest 
        }
        else
        {
            rightForeArm.AddRelativeForce(new Vector3(x, up, z), ForceMode.Impulse);
            leftForeArm.AddRelativeForce(new Vector3(-x, up, -z), ForceMode.Impulse);

            leftUpperLeg.AddRelativeForce(new Vector3(0.0f, 25.0f, 25.0f), ForceMode.Impulse);
            leftLowerLeg.AddRelativeForce(new Vector3(0.0f, 60.0f, 0.0f), ForceMode.Impulse);
            // Have to add a ton of force to cancle out persistant foot force
            leftFoot.AddRelativeForce(new Vector3(0.0f, 300.0f, 400.0f), ForceMode.Impulse);

            chest.AddTorque(new Vector3(0.0f, -200.0f, 0.0f), ForceMode.Impulse); // Twist chest
        }
        BangHead();
        hips.AddRelativeForce(new Vector3(0.0f, 0.0f, 100.0f), ForceMode.Impulse);
    }

    public void MoshTowardsCenter()
    {
        // Determine which pit to mosh towards
        Vector3 dirToLower = CharacterManager.S.lowerPitCenter.position - hips.transform.position;
        Vector3 dirToUpper = CharacterManager.S.upperPitCenter.position - hips.transform.position;

        Vector3 dir;
        if (dirToLower.sqrMagnitude < dirToUpper.sqrMagnitude)
        {
            dir = dirToLower;
        }
        else
        {
            dir = dirToUpper;
        }

        hips.AddForce(new Vector3(dir.x * Random.Range(100.0f, 150.0f), 600.0f, dir.z * Random.Range(100.0f, 150.0f)), ForceMode.Impulse);
        CrowdKill();
    }

    public void ToggleScale()
    {
        if (scaleCoroutine != null) return;

        scaleCoroutine = StartCoroutine(ScaleUpOrDown(!isBig));
    }

    private IEnumerator ScaleUpOrDown(bool scaleUp)
    {
        if (scaleUp)
        {
            float curScale = defaultScale;
            while (curScale < maxScaleScalar)
            {
                curScale = Mathf.Lerp(defaultScale, maxScaleScalar, 10.0f * Time.deltaTime);
                transform.localScale = new Vector3(curScale, curScale, curScale);
                yield return null;
            }
            transform.localScale = new Vector3(maxScaleScalar, maxScaleScalar, maxScaleScalar);
        }
        else
        {
            float curScale = maxScaleScalar;
            while (curScale > defaultScale)
            {
                curScale = Mathf.Lerp(maxScaleScalar, defaultScale, Time.deltaTime);
                transform.localScale = new Vector3(curScale, curScale, curScale);
                yield return null;
            }
            transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        }

        isBig = scaleUp;
        scaleCoroutine = null;
    }

    public void ScaleMass(float scalar)
    {
        foreach (Rigidbody rb in rbList)
        {
            rb.mass *= scalar;
        }
    }

    public void TurnGravityOff()
    {
        useGravity = false;
    }

    // To be used by gravity zones, NOT for effects
    public void SetRealGravity(Vector3 newGrav)
    {
        gravity = newGrav;
    }
}
