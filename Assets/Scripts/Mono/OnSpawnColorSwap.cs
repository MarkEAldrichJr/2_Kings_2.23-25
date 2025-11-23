using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mono
{
    public class OnSpawnColorSwap : MonoBehaviour
    {
        [Header("Armor")]
        [SerializeField] private Renderer[] armorMeshRenderers;
        [SerializeField] private Color armorLow;
        [SerializeField] private Color armorHigh;
        
        [Header("Hair")]
        [SerializeField] private Renderer[] hairMeshRenderers;
        [SerializeField] private Color hairLow;
        [SerializeField] private Color hairHigh;
        
        [Header("Skin")]
        [SerializeField] private Renderer[] skinMeshRenderers;
        [SerializeField] private Color skinLow;
        [SerializeField] private Color skinHigh;

        private void OnEnable()
        {
            var armorGradient = CreateNewGradient(armorLow, armorHigh);
            var hairColor = CreateNewGradient(hairLow, hairHigh).Evaluate(Random.value);
            var skinColor = CreateNewGradient(skinLow, skinHigh).Evaluate(Random.value);
            
            foreach (var meshRenderer in armorMeshRenderers)
            {
                meshRenderer.material.color = armorGradient.Evaluate(Random.value);
            }
            foreach (var meshRenderer in hairMeshRenderers)
            {
                meshRenderer.material.color = hairColor;
            }
            foreach (var meshRenderer in skinMeshRenderers)
            {
                meshRenderer.material.color = skinColor;
            }
        }

        private static Gradient CreateNewGradient(Color lowColor, Color highColor)
        {
            var gradient = new Gradient();
            
            var gck = new GradientColorKey[2];
            gck[0].color = lowColor;
            gck[0].time = 0.0f;
            gck[1].color = highColor;
            gck[1].time = 1.0f;
            
            var gak = Array.Empty<GradientAlphaKey>();
            
            gradient.SetKeys(gck, gak);
            return gradient;
        }
    }
}