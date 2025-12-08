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
   }
}
