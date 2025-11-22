using Mono;
using Unity.Entities;
using UnityEngine;

namespace Component
{
    public struct AnimatorRefComponent : IComponentData
    {
        public UnityObjectRef<Transform> TransGo;
        public UnityObjectRef<AnimatorController>  AnimatorGo;
    }

    public struct AudioRefComponent : IComponentData
    {
        public UnityObjectRef<AnimatorAudioController> AudioControllerGo;
    }
}