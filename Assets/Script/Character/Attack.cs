using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("基本属性")]
    public int damage;
    private void OnTriggerStay(Collider collision)
    {
        Debug.Log("攻击了：" + collision.name);
        //访问被攻击的人
        collision.GetComponent<Character>()?.TakeDamage(this);
    }
}
