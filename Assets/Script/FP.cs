using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP : MonoBehaviour
{
    
    public void Attack(){
        gameObject.tag="AttackPoint";
        Debug.Log("PlayerAttacking");
        Invoke("FinishAttack",0.5f);
    }

    void FinishAttack(){
        Debug.Log("Change Back Tag");
        gameObject.tag="Untagged";
    }
}
