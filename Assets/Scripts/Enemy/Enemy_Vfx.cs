using UnityEngine;

public class Enemy_Vfx : EntityHit_Vfx
{
    [Header("Counter Attack Window")]
    [SerializeField] private GameObject attackAlert;
    
    public void EnableAttackAlert(bool enable) => attackAlert.SetActive(enable);
}
