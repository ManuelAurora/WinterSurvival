using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneController : MonoBehaviour
{
   public SkinnedMeshRenderer meshRenderer;
   public Material coatMaterial;
   public Material cuffsMaterial;
   public GameObject bag;
   public GameObject hood;

   public void UpgradeCoat()
   {
      // Get a copy of the materials array
      Material[] materials = meshRenderer.materials;

      // Modify the materials you want to change
      materials[6] = cuffsMaterial;
      materials[7] = coatMaterial;

      // Assign the modified array back to the renderer
      meshRenderer.materials = materials;

      bag.SetActive(true);
      hood.SetActive(true);
   }
}
