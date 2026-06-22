using System.Collections.Generic;
using WheelDemo.Data;

namespace WheelDemo.Core
{
    // Weighted random. The landing slice is chosen here, before any animation;
    // the wheel just travels to it. Seedable for deterministic tests.
    public class SlicePicker
    {
        private readonly System.Random rng;

        public SlicePicker(int? seed = null)
        {
            rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        }

        public int PickSliceIndex(WheelConfigSO config)
        {
            var slices = config.Slices;
            var weights = new float[slices.Count];
            for (int i = 0; i < slices.Count; i++) weights[i] = slices[i].Weight;
            return PickIndexByWeights(weights);
        }

        // Pure, Unity-free core so it can be unit-tested directly with a seeded
        // RNG and plain weights, without authoring a ScriptableObject.
        public int PickIndexByWeights(IReadOnlyList<float> weights)
        {
            float total = 0f;
            for (int i = 0; i < weights.Count; i++) total += weights[i];

            double roll = rng.NextDouble() * total;
            for (int i = 0; i < weights.Count; i++)
            {
                roll -= weights[i];
                if (roll <= 0d) return i;
            }
            return weights.Count - 1;
        }
    }
}
