using Component;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace Mono
{
    public class UpdateGlobalUI : MonoBehaviour
    {
        [SerializeField] private Slider faithSlider;
        private EntityQuery _entityQuery;
        private float _faithLastFrame;
        
        private void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ElishaFaith>();
            _entityQuery = entityManager.CreateEntityQuery(builder);
        }

        private void LateUpdate()
        {
            var faith = _entityQuery.GetSingleton<ElishaFaith>();
            var faithRatio = faith.CurrentFaith / faith.FaithMax;
            if (Mathf.Approximately(_faithLastFrame, faithRatio)) return;
            
            faithSlider.value = faithRatio;
            _faithLastFrame = faithRatio;
        }
    }
}