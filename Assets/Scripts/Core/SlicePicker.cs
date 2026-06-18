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
            float total = 0f;
            for (int i = 0; i < slices.Count; i++) total += slices[i].Weight;

            double roll = rng.NextDouble() * total;
            for (int i = 0; i < slices.Count; i++)
            {
                roll -= slices[i].Weight;
                if (roll <= 0d) return i;
            }
            return slices.Count - 1;
        }
    }
}
