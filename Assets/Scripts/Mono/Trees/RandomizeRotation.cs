using UnityEngine;
using Random = UnityEngine.Random;

namespace Mono.Trees
{
    public class RandomizeRotation : MonoBehaviour
    {
        public Material barkMaterial;
        public Material leafMaterial;
        
        
        [ContextMenu("Randomize")]
        public void RotateTreesRandomly()
        {
            var trees = GameObject.FindGameObjectsWithTag("Trees");
            foreach (var tree in trees)
            {
                tree.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
                tree.transform.rotation = Quaternion.Euler(Random.Range(-2, 2), Random.Range(0, 360), Random.Range(-2, 2));
            }
        }
        [ContextMenu("RestoreSharedMaterials")]
         public void RestoreSharedMaterials()
         {
             var trees = GameObject.FindGameObjectsWithTag("Trees");
     
             foreach (var tree in trees)
             {
                 var rendererComponent = tree.GetComponent<Renderer>();
     
                 // Reset to prefab/shared material references
                 rendererComponent.sharedMaterials = new[]
                 {
                     barkMaterial, leafMaterial
                 };
                 print("fixed tree");
             }
        }
         
        public Material material;

        [ContextMenu("Log Shader Properties")]
        public void LogShaderProperties()
        {
            if (material == null)
            {
                Debug.LogWarning("Material is null.");
                return;
            }

            var shader = material.shader;
            var count = shader.GetPropertyCount();

            Debug.Log($"Shader: {shader.name}");
            Debug.Log($"Property Count: {count}");

            for (var i = 0; i < count; i++)
            {
                var propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i);
                Debug.Log($"[{i}] {propName} ({propType})");
            }
        }
    }
}
