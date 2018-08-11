using Unity.Entities;
using UnityEngine;

namespace Data {
    [System.Serializable]
    public struct BuildingLevelData : IComponentData {
        public int Level;
    }

    [System.Serializable]
    public struct BuildingNameData : IComponentData {
        public string Name;
    }

    [System.Serializable]
    public struct EffectivenessData : IComponentData {
        public float ManualEffectiveness;
        public float AvailableEffectiveness;
    }

    [System.Serializable]
    public partial struct GainPlan : IComponentData {
        [SerializeField] float[] an;
    }

    [System.Serializable]
    public struct ResourceGenerationData : IComponentData {
        public GainPlan Levels;
        public ResourceType ResourceType;
    }

    [System.Serializable]
    public struct ResourceDepositData : IComponentData {
        public ResourceType Type;
        public float Amount;
    }

}