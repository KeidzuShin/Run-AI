using UnityEngine;

[CreateAssetMenu(fileName = "AIStats", menuName = "ScriptableObject/Stats")]
public class AIStatus : ScriptableObject
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {

    }

    //Don't forget to reset values on GameManager
    public void ResetValues()
    {
        health = 100f;
    }
}
