using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Mono
{
    public class UpdatePlayerUI : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Image elishaFace;
        
        private RectTransform _playerUICanvasRect;
        private EntityQuery _elishaQuery;

        private void Awake()
        {
            _playerUICanvasRect = elishaFace.canvas.GetComponent<RectTransform>();
        }

        private void Start()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _elishaQuery = entityManager.CreateEntityQuery(
                new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform, ElishaFaith>());
        }

        private void Update()
        {
            MoveElishaFaceOverElisha();
        }

        private void MoveElishaFaceOverElisha()
        {
            var elishaPos = _elishaQuery.GetSingleton<LocalTransform>().Position;
            elishaPos.y += 4f;


            var elishaViewPoint = playerCamera.WorldToViewportPoint(elishaPos);


            if (elishaViewPoint.z < 0)
            {
                elishaFace.gameObject.SetActive(false);
                return;
            }
            elishaFace.gameObject.SetActive(true);

            var canvasSize = _playerUICanvasRect.sizeDelta;

            var localPos = new Vector2(
                (elishaViewPoint.x - 0.5f) * canvasSize.x,
                (elishaViewPoint.y - 0.5f) * canvasSize.y
            );

            elishaFace.rectTransform.anchoredPosition = localPos;
        }
    }
}