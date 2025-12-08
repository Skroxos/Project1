using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
   [SerializeField] private GameObject inventoryUI;

   private void Start()
   {
         inventoryUI.SetActive(false);
   }

   private void Update()
   {
       if (Input.GetKeyDown(KeyCode.I))
       {
           inventoryUI.SetActive(!inventoryUI.activeSelf);
       }

       if (Input.GetKeyDown(KeyCode.Space))
       {
           ShootRaycast();
       }
   }

   private void ShootRaycast()
   {
      
         if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 100f))
         {
              IDamagable damagable = hitInfo.collider.GetComponent<IDamagable>();
              if (damagable != null)
              {
                damagable.TakeDamage(10);
              }
         }
   }
}
