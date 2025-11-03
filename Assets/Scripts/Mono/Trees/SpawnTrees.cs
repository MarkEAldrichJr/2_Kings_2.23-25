using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mono.Trees
{
    public class SpawnTrees : MonoBehaviour
    {
        [SerializeField] private GameObject treePrefab;
        [SerializeField] private uint numberOfTrees;
        [SerializeField] private float distFromCenter;
        [SerializeField] private float distFromEachOther;
        
        [ContextMenu("Spawn Trees")]
        public void SpawnRandomTrees()
        {
            var spawnPoint = float3.zero;
            RaycastHit hit;
            
            for (var i = 0; i < numberOfTrees; i++)
            {
                spawnPoint.x = Random.Range(-distFromCenter, distFromCenter);
                spawnPoint.y = 1000f;
                spawnPoint.z = Random.Range(-distFromCenter, distFromCenter);

                if (Physics.Raycast(spawnPoint, Vector3.down, out hit, 1100f,
                        LayerMask.GetMask("Terrain")))
                {
                    Instantiate(treePrefab, hit.point, Quaternion.identity);
                }
            }
        }

        [ContextMenu("Wipe Out Trees")]
        public void WipeOutTrees()
        {
            var trees = GameObject.FindGameObjectsWithTag("Trees");
            
            for (var i = 0; i < trees.Length; i++)
            {
                DestroyImmediate(trees[i]);
            }
        }
    }
}
