#pragma warning disable 0649

using System;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    [SerializeField]
    private DamageNumber damageNumber;

    [SerializeField]
    private BracketManager brackets;

    public void OnDamageNotification(DamageNotification notification)
    {
        var bracket = brackets.FindBracket(notification.Target.gameObject);
        if (!bracket)
        {
            return;
        }

        var pos = bracket.transform.position;

        DamageNumber.CreateFromPrefab(damageNumber, notification.Amount, pos, this);        
    }
}
