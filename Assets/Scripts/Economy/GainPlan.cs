using Unity.Entities;

namespace Data {
    public partial struct GainPlan : IComponentData {
        public float Evaluate(float x) {
            if (an == null || an.Length == 0) {
                return 0f;
            }
            float value = an[0];
            for (int n = 1; n < an.Length; n++) {
                value += pow(x, n) * an[n];
            }
            return value > 0f ? value : 0f;
        }

        public float Evaluate(BuildingLevelData entity) {
            return Evaluate((float)entity.Level);
        }

        public float Evaluate(BuildingLevelData entity, EffectivenessData effectiveness) {
            return Evaluate((float)entity.Level) * effectiveness.AvailableEffectiveness;
        }

        float pow(float a, int n) {
            if (n <= 0) return 1;
            return a * pow(a, n - 1);
        }
    }
}