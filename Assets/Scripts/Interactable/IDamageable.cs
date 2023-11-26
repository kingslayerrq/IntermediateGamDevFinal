using UnityEngine;

public interface IDamageable 
{

    void takeDamage(int damage, Vector2? from = null);
}
