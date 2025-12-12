using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    
    public float shootInterval = 5f;

    private void Start()
    {
        StartCoroutine(ShootRaycastRoutine());
    }

    private IEnumerator ShootRaycastRoutine()
    {
        while (true)
        {
            ShootRaycast();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootRaycast()
    {
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out RaycastHit hitInfo, 100f))
        {
            IDamagable damagable = hitInfo.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(10);
            }
        }
    }
}
