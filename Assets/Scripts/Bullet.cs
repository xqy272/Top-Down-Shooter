using Enemy;
using Object_Pool;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float impactForce;
    
    private Rigidbody rb;
    private SphereCollider sc;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;
    
    private bool _bulletDisabled;
    private float _flyDistance;
    private Vector3 _startPosition;


    private void Awake()
    {
        sc = GetComponentInChildren<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        FadeTrailIfNeeded();
        DisableBullet();
        ReturnToPoolIfNeeded();
    }

    private void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime;
    }

    private void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
            ObjectPool.Instance.ReturnObject(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        CreateBulletImpactFX(collision);
        ObjectPool.Instance.ReturnObject(gameObject);
        
        Enemy.Enemy enemy = collision.gameObject.GetComponentInParent<Enemy.Enemy>();
        EnemyShield shield = collision.gameObject.GetComponent<EnemyShield>();

        if (shield)
        {
            shield.ReduceDurability();
        }
        
        if (enemy)
        {
            enemy.GetHit();
            enemy.HitImpact(rb.linearVelocity.normalized * impactForce, collision.contacts[0].point, collision.collider.attachedRigidbody);
        }
    }

    private void DisableBullet()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance && !_bulletDisabled)
        {
            sc.enabled = false;
            meshRenderer.enabled = false;
            _bulletDisabled = true;
        }
    }

    public void BulletSetup(float flyDistance, float impactF)
    {
        impactForce = impactF;
        
        sc.enabled = true;
        meshRenderer.enabled = true;
        _bulletDisabled = false;
        
        trailRenderer.time = 0.25f;
        _startPosition = transform.position;
        _flyDistance = flyDistance + 0.5f;
    }

    private void CreateBulletImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            
            GameObject newBulletImpactFX = ObjectPool.Instance.GetObject(bulletImpactFX);
            newBulletImpactFX.transform.position = contact.point;
            
            ObjectPool.Instance.ReturnObject(newBulletImpactFX, 0.5f);
        }
    }
}
