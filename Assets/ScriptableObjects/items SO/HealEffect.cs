using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/Heal")]
public class HealEffect : ItemEffect
{
    public int healAmount = 20;

    public override bool Apply(GameObject user)
    {
        var hp = user.GetComponent<HealthScript>();
        if (hp == null)
        {
            Debug.LogError("HealEffect: PlayerHealth not found on user.");
            return false;
        }

        hp.addHealth(healAmount);
        return true;
    }
}
