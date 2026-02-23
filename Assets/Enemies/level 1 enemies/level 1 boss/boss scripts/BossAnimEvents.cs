using UnityEngine;
using System;

public class BossAnimEvents : MonoBehaviour
{
    public event Action AttackStart;
    public event Action AttackHit;
    public event Action AttackEnd;

    public event Action CastStart;
    public event Action CastFire;
    public event Action CastEnd;

    public event Action HurtEnd; 

   
    public void AE_AttackStart() => AttackStart?.Invoke();
    public void AE_AttackHit() => AttackHit?.Invoke();
    public void AE_AttackEnd() => AttackEnd?.Invoke();

    public void AE_CastStart() => CastStart?.Invoke();
    public void AE_CastFire() => CastFire?.Invoke();
    public void AE_CastEnd() => CastEnd?.Invoke();

    public void AE_HurtEnd() => HurtEnd?.Invoke();
}
