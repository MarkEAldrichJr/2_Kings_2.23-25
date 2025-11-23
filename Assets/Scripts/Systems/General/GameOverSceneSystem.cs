using Systems.Elisha;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Systems.General
{
    [UpdateAfter(typeof(GameOverKillEveryoneSystem))]
    public partial struct GameOverSceneSystem : ISystem
    {
        private EntityQuery _gameOverQuery;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var builder = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameOverTag>();
            _gameOverQuery = state.GetEntityQuery(builder);
            
            state.RequireForUpdate(_gameOverQuery);
        }

        public void OnUpdate(ref SystemState state)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            state.EntityManager.DestroyEntity(_gameOverQuery);
        }
    }
}