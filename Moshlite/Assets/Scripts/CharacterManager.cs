using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager S;

    [SerializeField] private List<BulbHeadController> chars;

    [SerializeField] private GravityZone upwardGravZone;
    [SerializeField] private GravityZone downwardGravZone;

    private UnityEvent bangHeadEvent = new UnityEvent();
    private UnityEvent crowdKillEvent = new UnityEvent();
    private UnityEvent moshTowardsCenterEvent = new UnityEvent();
    private UnityEvent jumpEvent = new UnityEvent();
    private UnityEvent pulseScaleEvent = new UnityEvent();

    public Transform upperPitCenter;
    public Transform lowerPitCenter;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("More than one Character Manager!!!");
        }
    }

    public void Subscribe(BulbHeadController c)
    {
        bangHeadEvent.AddListener(c.BangHead);
        jumpEvent.AddListener(c.Jump);
        crowdKillEvent.AddListener(c.CrowdKill);
        moshTowardsCenterEvent.AddListener(c.MoshTowardsCenter);
        pulseScaleEvent.AddListener(c.PulseScale);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bangHeadEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            jumpEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            crowdKillEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            moshTowardsCenterEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            pulseScaleEvent.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            ReverseGravityZones();
        }
    }

    public void ToggleAllCharacterGravity()
    {
        foreach (BulbHeadController c in chars)
        {
            c.gravityScalar *= -1.0f;
        }
    }

    // TODO: determine ratio by velocity of midi note
    public void ToggleRandomCharactersGravity(float ratio) // 1.0 for all, 0.0 for none
    {
        ratio = Mathf.Clamp(ratio, 0.0f, 1.0f);
        foreach (BulbHeadController c in chars)
        {
            if (Random.Range(0.0f, 1.0f) < ratio)
            {
                c.gravityScalar *= -1.0f;
            }
        }
    }

    public void ReverseGravityZones()
    {
        upwardGravZone.ToggleGravityZone();
        downwardGravZone.ToggleGravityZone();
    }
}
