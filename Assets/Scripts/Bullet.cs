using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX;


    private void OnCollisionEnter(Collision collision)
    {
        CreateBulletImpactFX(collision);
        Destroy(gameObject);
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
