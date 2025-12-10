using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
   [SerializeField] private GameObject inventoryUI;
   [SerializeField] private Camera playerCamera;

   private void Start()
   {
       //  inventoryUI.SetActive(false);
   }

   private void Update()
   {
       // if (Input.GetKeyDown(KeyCode.I))
       // {
       //     inventoryUI.SetActive(inventoryUI.activeSelf);
       // }

       if (Input.GetKeyDown(KeyCode.Space))
       {
           ShootRaycast();
       }
       if (Input.GetKeyDown(KeyCode.E))
       {
           TryToInteract();
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

   private void TryToInteract()
   {
       Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f))
         {
              IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
              if (interactable != null)
              {
                interactable.Interact(this.gameObject);
              }
         }
   }
}
