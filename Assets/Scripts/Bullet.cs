 
using Player;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private SphereCollider sc;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;
    private bool _bulletDisabled;
    private float _flyDistance;

    private Vector3 _startPosition;

    private void Update()
    {
        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance - 1.5f)
            trailRenderer.time -= 2 * Time.deltaTime;

        if (Vector3.Distance(_startPosition, transform.position) > _flyDistance && !_bulletDisabled)
        {
            sc.enabled = false;
            meshRenderer.enabled = false;
            _bulletDisabled = true;
        }

        if (trailRenderer.time <= 0)
            ObjectPool.Instance.ReturnBullet(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreateBulletImpactFX(collision);
        ObjectPool.Instance.ReturnBullet(gameObject);
    }

    public void BulletSetup(float flyDistance)
    {
        sc.enabled = true;
        meshRenderer.enabled = true;
        _bulletDisabled = false;
        
        trailRenderer.time = 0.25f;
        _startPosition = transform.position;
        _flyDistance = flyDistance + 1;
    }

    private void CreateBulletImpactFX(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint contact = collision.contacts[0];
            
            GameObject newBulletImpactFX = Instantiate(bulletImpactFX, contact.point, Quaternion.LookRotation(contact.normal));
            
            Destroy(newBulletImpactFX, 0.5f);
        }
    }
}
