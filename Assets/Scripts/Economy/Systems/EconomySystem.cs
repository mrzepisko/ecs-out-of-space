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
        public BuildingLevelComponent level;
    }

    protected override void OnUpdate() {
        float dt = Time.deltaTime;
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
            AddResourceGeneration(entity.resource.Value.ResourceType, entity.resource.Value.Levels.Evaluate(entity.level.Value, entity.effectiveness.Value) * dt);
        }

        //deposit resources
        {
            var entities = GetEntities<ResourceDepositEntity>();
            for (int i = 0; i < entities.Length; i++) {
                var entity = entities[i];
                foreach (ResourceType key in System.Enum.GetValues(typeof(ResourceType))) {
                    float value;
                    if (!resourceGeneration.TryGetValue(key, out value) || value <= 0f) {
                        continue;
                    }
                    float deposited = DepositResource(key, value, ref entity);
                    resourceGeneration[key] -= deposited;
                }
            }
        }
    }

    void AddResourceGeneration(ResourceType resource, float value) {
        float newValue;
        if (resourceGeneration.TryGetValue(resource, out newValue)) {
            resourceGeneration[resource] += value;
        } else {
            resourceGeneration.Add(resource, value);
        }

    }

    float DepositResource(ResourceType resource, float value, ref ResourceDepositEntity entity) {
        float val;
        if (resource.Equals(entity.deposit.Value.Type) && resourceGeneration.TryGetValue(resource, out val)) {
            float availableSpace = entity.deposit.Value.Capacity.Evaluate(entity.level.Value) - entity.deposit.Value.Amount;
            float depositValue = Mathf.Min(availableSpace, value);
            entity.deposit.Value.Amount += depositValue;
            return depositValue;
        } else {
            return 0f;
        }
    }

}
