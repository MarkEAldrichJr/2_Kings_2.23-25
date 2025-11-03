using UnityEngine;
using Random = UnityEngine.Random;

namespace Mono.Trees
{
    public class RandomizeRotation : MonoBehaviour
    {
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
    }
}
