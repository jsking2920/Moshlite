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
    [HideInInspector] public bool useGravity = true;

    // Scale xyz must be equal
    private Coroutine scaleCoroutine = null;
    private float defaultScale = 1.2f;
    private float maxScale = 1.7f;

    void Start()
    {
        defaultScale = hips.transform.localScale.x;
        if (hips.transform.localScale.y != defaultScale || hips.transform.localScale.z != defaultScale)
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
                rb.AddForce(gravity, ForceMode.Acceleration);
            }
        }
        else
        {
            // Characters are floating inexplicably with gravity off even though no forcesa are being applied to them, so do all this to cancel it out
            // And generally make them float with their feet towards the ground
            
            // On this note: There seems to be a mystery force on all the character pulling them upwards at all times (see comment above too)
  
            // TODO: make this all less gross; shouldn't need to case or apply any forces if gravity is off
            if (gravity.y > 0.0f)
            {
                hips.AddForce(-0.76f * chestForce); // Pull down to correct for mystery force

                // Pull on feet and to reorient a bit
                leftAnkle.AddForce(-0.1f * feetForce); 
                rightAnkle.AddForce(-0.1f * feetForce);

                head.AddForce(-2.0f * headForce);
            }
            else
            {
                hips.AddForce(-0.51f * chestForce); // Pull down to correct for mystery force      
            }

            // perlin noise 
            // float p = Unity.Mathematics.noise.pnoise(new Unity.Mathematics.float2(Time.realtimeSinceStartup, 0.0f), new Unity.Mathematics.float2(5.0f, 1.0f));
        }
    }

    public void Jump()
    {
        hips.AddRelativeForce(new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(1500.0f, 2500.0f), Random.Range(-100.0f, 100.0f)), ForceMode.Impulse);
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

    public void PulseScale()
    {
        if (scaleCoroutine != null) return;
 
        scaleCoroutine = StartCoroutine(ScaleEffect());
    }

    // Quick pulse effect; Ragdolls aren't stable when scaled up (colliders overlap) so toggle effect isn't an option
    private IEnumerator ScaleEffect()
    {
        float curScale = defaultScale;
        while (curScale < maxScale)
        {
            curScale += Time.deltaTime * 5.0f;
            hips.transform.localScale = new Vector3(curScale, curScale, curScale);
            yield return null;
        }
        while (curScale > defaultScale)
        {
            curScale -= Time.deltaTime * 5.0f;
            hips.transform.localScale = new Vector3(curScale, curScale, curScale);
            yield return null;
        }
        hips.transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        scaleCoroutine = null;
    }

    public void ScaleMass(float scalar)
    {
        foreach (Rigidbody rb in rbList)
        {
            rb.mass *= scalar;
        }
    }

    public void ToggleGravity()
    {
        useGravity = !useGravity;

        // Toss up in the air a bit to make the effect obvious
        chest.AddForce(Vector3.up * 40.0f, ForceMode.Impulse);
    }

    // To be used by gravity zones, NOT for effects
    public void SetRealGravity(Vector3 newGrav)
    {
        gravity = newGrav;
    }
}