using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    Rigidbody rb;

    public float speed = 300;
    [SerializeField] float bulletDamage = 10;
    [SerializeField] float maxLifeTime = 10;
    float currentLifeTime;

    private void OnEnable()
    {
        currentLifeTime = maxLifeTime;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed * Time.fixedDeltaTime;

        currentLifeTime -= Time.fixedDeltaTime;
        if (currentLifeTime <= 0)
        {
            ObjectPooler.instance.ReturnToPool("Bullet", gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthBehaviour>().Damage(bulletDamage);
            ObjectPooler.instance.ReturnToPool("Bullet", gameObject);
        }

        if (other.CompareTag("Cell"))
        {
            ObjectPooler.instance.ReturnToPool("Bullet", gameObject);
        }
    }
}
