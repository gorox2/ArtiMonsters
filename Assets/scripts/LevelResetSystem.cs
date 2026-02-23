using System.Linq;
using UnityEngine;

public static class LevelResetSystem 
{
    public static void ResetEnemiesAndBoss()
    {
        var resettables = Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IDeathResettable>();
        foreach (var r in resettables)
            r.ResetOnPlayerDeath();

        
    }

}
