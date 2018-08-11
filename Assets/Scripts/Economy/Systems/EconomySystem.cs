using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EconomySystem : ComponentSystem {
    [SerializeField] float energyAvailable, energyRequired;
    public float EnergyAvailable { get { return energyAvailable; } }
    public float EnergyRequired { get { return energyRequired; } }

    public Dictionary<ResourceType, float> resourceGeneration = new Dictionary<ResourceType, float>();

    public struct PowerPlantEntity {
        public EnergyGeneratorComponent component;
        public EffectivenessComponent effectiveness;
        public BuildingLevelComponent level;
    }

    public struct EnergyConsumptionEntity {
        public EnergyConsumptionComponent energy;
        public EffectivenessComponent effectiveness;
        public BuildingLevelComponent level;
    }

    public struct BuildingEntity {
        public EffectivenessComponent effectiveness;
        public ResourceGenerationComponent resource;
        public BuildingLevelComponent level;
    }

    public struct ResourceDepositEntity {
        public ResourceDepositComponent deposit;
    }

    protected override void OnUpdate() {
        energyAvailable = 0f;
        energyRequired = 0f;

        //energy generation
        foreach (PowerPlantEntity entity in GetEntities<PowerPlantEntity>()) {
            entity.effectiveness.Value.AvailableEffectiveness = 1f;
            energyAvailable += entity.component.Value.Evaluate(entity.level.Value);
        }
        //energy requirements
        {
            var entities = GetEntities<EnergyConsumptionEntity>();
            foreach (EnergyConsumptionEntity entity in entities) {
                entity.effectiveness.Value.AvailableEffectiveness = 1f;
                energyRequired += entity.energy.Value.Evaluate(entity.level.Value, entity.effectiveness.Value);
            }
            //diminish to available energy
            float globalEffectiveness = Mathf.Clamp01(energyAvailable / energyRequired);
            energyRequired = 0f;

            foreach (EnergyConsumptionEntity entity in entities) {
                entity.effectiveness.Value.AvailableEffectiveness = globalEffectiveness;
                energyRequired += entity.energy.Value.Evaluate(entity.level.Value, entity.effectiveness.Value);
            }
            entities.Dispose();
        }


        //resource generation
        resourceGeneration.Clear();
        foreach (BuildingEntity entity in GetEntities<BuildingEntity>()) {
            AddResource(entity.resource.Value.ResourceType, entity.resource.Value.Levels.Evaluate(entity.level.Value, entity.effectiveness.Value));
        }


        //deposit
        float dt = Time.deltaTime;
        foreach (ResourceDepositEntity entity in GetEntities<ResourceDepositEntity>()) {
            for (int i = 0; i < entity.deposit.Value.Length; i++) {
                float value;
                if (resourceGeneration.TryGetValue(entity.deposit.Value[i].Type, out value)) {
                    entity.deposit.Value[i].Amount += value * dt;
                }
            }
        }
    }

    void AddResource(ResourceType resource, float value) {
        float newValue;
        if (resourceGeneration.TryGetValue(resource, out newValue)) {
            newValue += value;
        } else {
            newValue = value;
        }
        resourceGeneration.Add(resource, newValue);
    }

}
