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
    private UnityEvent toggleGravityEvent = new UnityEvent();
    private UnityEvent pulseScaleEvent = new UnityEvent();
    private UnityEvent randomSmallDanceEvent = new UnityEvent();
    private UnityEvent flickerEvent = new UnityEvent();
    private UnityEvent toggleLightEvent = new UnityEvent();

    public Transform upperPitCenter;
    public Transform lowerPitCenter;

    /* Other ideas for effects 
        - Influence intensity of animations with midi velocity
    */

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
        toggleGravityEvent.AddListener(c.ToggleGravity);
        randomSmallDanceEvent.AddListener(c.RandomSmallDance);
        toggleLightEvent.AddListener(c.ToggleLight);
        flickerEvent.AddListener(c.Flicker);
    }

    public void RandomMosh()
    {
        foreach (BulbHeadController c in chars)
        {
            float r = Random.Range(0.0f, 1.0f);
            if (r < 0.35f)
            {
                c.BangHead();
            }
            else if (r < 0.5f)
            {
                c.CrowdKill();
            }
            else if(r < 0.7f)
            {
                c.MoshTowardsCenter();
            }
            else if (r < 0.8f)
            {
                c.Jump();
            }
            else
            {
                // Do nothing
            }
        }
    }

    public void BangHeads()
    {
        bangHeadEvent.Invoke();
    }

    public void Jump()
    {
        jumpEvent.Invoke();
    }

    public void CrowdKill()
    {
        crowdKillEvent.Invoke();
    }

    public void MoshTowardsCenter()
    {
        moshTowardsCenterEvent.Invoke();
    }

    public void RandomSmallDance()
    {
        randomSmallDanceEvent.Invoke();
    }

    public void PulseScale()
    {
        pulseScaleEvent.Invoke();
    }

    public void ToggleGravity()
    {
        toggleGravityEvent.Invoke();
    }

    // Pushes all characters towards center
    public void ReverseGravityZones()
    {
        upwardGravZone.ToggleGravityZone();
        downwardGravZone.ToggleGravityZone();
    }

    public void Flicker()
    {
        flickerEvent.Invoke();
    }

    public void ToggleLights()
    {
        toggleLightEvent.Invoke();
    }
}
