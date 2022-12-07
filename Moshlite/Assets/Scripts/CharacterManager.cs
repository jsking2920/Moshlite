using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<BulbHeadController> chars;

    [SerializeField] private GravityZone upwardGravZone;
    [SerializeField] private GravityZone downwardGravZone;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (BulbHeadController c in chars)
            {
                c.BangHead();
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            foreach (BulbHeadController c in chars)
            {
                c.Jump();
            }
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

    public void TurnGravityOff()
    {
        foreach (BulbHeadController c in chars)
        {
            c.useGravity = false;
        }
    }

    // Add randomness to forces
    // figure out mapping of effects
    // set up some coroutine animations for specific effects
    // Set up different classes of character (jumpers/head bangers/etc)
}
