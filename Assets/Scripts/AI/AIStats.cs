using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIStats : MonoBehaviour
{
    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 0.5f;
    public float attackRange = 10f;
    public float searchRange = 15f;

    [Header("Projectile")]
    public float shootingForce = 40f;
    public float shootingUpwardForce = 1f;
    public float spread = 5f;

    [Header("AI Base Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float lowHealth = 30f;
    public float regenRate = 0f;

    public Slider slider;

    void Start()
    {
        //Unique Randomizer
        Random.InitState(GetInstanceID());
        //Set random stats for each AI
        SetRandomStats();
    }

    void Update()
    {        
        if (currentHealth <= 0)
        {//Death Trigger
            gameObject.GetComponent<Animator>().SetBool("DeathTrigger", true);
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            currentHealth = 0;
            Invoke(nameof(destroy), 5f);
        }

        slider.value = currentHealth / maxHealth;
    }

    public void SetRandomStats()
    {//Match Values with Gamemanager.cs
        //Attack
        attackDamage = Mathf.Clamp(attackDamage + 0.02f * Time.deltaTime, 10f, 50f);
        attackCooldown = Mathf.Clamp(attackCooldown - 0.0001f * Time.deltaTime, 0.2f, 0.5f); 
        attackRange = Mathf.Clamp(attackRange + 0.01f * Time.deltaTime, 10f, 30f); 
        searchRange = Mathf.Clamp(searchRange + 0.01f * Time.deltaTime, 15f, 35f);

        //Projectile
        shootingForce = Mathf.Clamp(shootingForce + 0.01f * Time.deltaTime, 40f, 60f);
        shootingUpwardForce = 1f;
        spread = Mathf.Clamp(spread - 0.002f * Time.deltaTime, 0f, 5f);

        //AI Base Stats
        maxHealth = Mathf.Clamp(maxHealth + 0.05f * Time.deltaTime, 100f, 200f);
        currentHealth = maxHealth;
        lowHealth = Mathf.Clamp(lowHealth + 0.06f * Time.deltaTime, 30f, 150f);
        regenRate = Mathf.Clamp(regenRate + 0.002f * Time.deltaTime, 0f, 5f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void destroy()
    {
        Destroy(gameObject);
    }
}
