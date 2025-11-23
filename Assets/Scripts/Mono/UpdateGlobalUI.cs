using System;
using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Mono
{
    public class UpdateGlobalUI : MonoBehaviour
    {
        [Header("Faith Slider")]
        [SerializeField] private Slider faithSlider;
        [SerializeField] private Image sliderImage;
        [SerializeField] private Color fullColor = Color.yellowNice;
        [SerializeField] private Color emptyColor = Color.grey;
        
        private EntityQuery _entityQuery;
        private float _faithLastFrame;
        private Gradient _faithColorGradient;

        private void Awake()
        {
            SetFaithColorGradient();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ElishaFaith, LocalTransform>();
            _entityQuery = entityManager.CreateEntityQuery(builder);
        }

        private void LateUpdate()
        {
            UpdateFaithSlider();
        }
        
        private void SetFaithColorGradient()
        {
            _faithColorGradient = new Gradient();
            var gck = new GradientColorKey[2];
            gck[0].color = emptyColor;
            gck[0].time = 0.0f;
            gck[1].color = fullColor;
            gck[1].time = 1.0f;
            
            var gak = Array.Empty<GradientAlphaKey>();
            
            _faithColorGradient.SetKeys(gck, gak);
        }
        private void UpdateFaithSlider()
        {
            if(!_entityQuery.TryGetSingleton<ElishaFaith>(out var faith))return;
            
            var faithRatio = faith.CurrentFaith / faith.FaithMax;
            if (Mathf.Approximately(_faithLastFrame, faithRatio)) return;
            
            faithSlider.value = faithRatio;

            sliderImage.color = _faithColorGradient.Evaluate(faithRatio);
            
            _faithLastFrame = faithRatio;
        }
    }
}