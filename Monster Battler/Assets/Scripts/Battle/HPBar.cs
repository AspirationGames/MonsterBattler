using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public bool HPisUpdating {get; private set;}
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3 (hpNormalized, 1f); 
    }

    public IEnumerator SetHPSmooth(float newHP)
    {
        HPisUpdating = true;

        float currentHP = health.transform.localScale.x;
        float chaneAmt = currentHP - newHP;

        while(currentHP - newHP > Mathf.Epsilon)
        {
            currentHP -= chaneAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHP, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHP, 1f);

        HPisUpdating = false;
        
    }
}
