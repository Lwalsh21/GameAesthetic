using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;

    public Material attackMaterial;
    public Material idleMaterial;
    public Material followMaterial;
    public int unitdamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = null;
        }
    }
    // for debug purposes, to visualize the current state of the unit
    public void SetAttackMaterial()
    {
        GetComponent<Renderer>().material = attackMaterial;
    }
    public void SetIdleMaterial()
    {
        GetComponent<Renderer>().material = idleMaterial;
    }
    public void SetFollowMaterial()
    {
        GetComponent<Renderer>().material = followMaterial;
    }
}
