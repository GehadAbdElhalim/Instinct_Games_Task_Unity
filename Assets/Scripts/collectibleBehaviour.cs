using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class collectibleBehaviour : MonoBehaviour
{
    public static UnityEvent OnCollectibleDestroyed = new UnityEvent();

    [SerializeField] float followSpeed = 10;
    [SerializeField] float rotationSpeed = 10;
    float outerSphereRadius;
    float innerSphereRadius;
    bool followPlayer = false;

    private GameObject player;

    private void Start()
    {
        player = GameManager.instance.player;

        List<float> sphereRadii = new List<float>();

        foreach (SphereCollider col in GetComponents<SphereCollider>())
        {
            sphereRadii.Add(col.radius);
        }

        sphereRadii.Sort();

        innerSphereRadius = sphereRadii[0];
        outerSphereRadius = sphereRadii[sphereRadii.Count - 1];
    }

    private void FixedUpdate()
    {
        if (followPlayer)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.Translate(dir * followSpeed * Time.fixedDeltaTime, Space.World);
        }
        else
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance > innerSphereRadius)
            {
                followPlayer = true;
            }
            else
            {
                followPlayer = false;
                GameManager.instance.score += 1;
                OnCollectibleDestroyed.Invoke();
                ObjectPooler.instance.ReturnToPool("Collectible", this.gameObject);
            }
        }

        if(other.tag == "Turret")
        {
            followPlayer = false;
            OnCollectibleDestroyed.Invoke();
            ObjectPooler.instance.ReturnToPool("Collectible", this.gameObject);
        }
    }
}
