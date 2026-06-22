using NUnit.Framework;
using WheelDemo.Core;

namespace WheelDemo.Tests
{
    // SlicePicker's weighting maths is exercised through the pure
    // PickIndexByWeights core, so no ScriptableObject authoring is needed.
    public class SlicePickerTests
    {
        [Test]
        public void SingleWeight_AlwaysReturnsIndexZero()
        {
            var picker = new SlicePicker(seed: 1);
            for (int i = 0; i < 100; i++)
                Assert.AreEqual(0, picker.PickIndexByWeights(new[] { 1f }));
        }

        [Test]
        public void AllWeightOnFirstSlice_AlwaysReturnsThatSlice()
        {
            var picker = new SlicePicker(seed: 7);
            for (int i = 0; i < 500; i++)
                Assert.AreEqual(0, picker.PickIndexByWeights(new[] { 10f, 0f, 0f }));
        }

        [Test]
        public void AllWeightOnLastSlice_AlwaysReturnsThatSlice()
        {
            var picker = new SlicePicker(seed: 7);
            for (int i = 0; i < 500; i++)
                Assert.AreEqual(2, picker.PickIndexByWeights(new[] { 0f, 0f, 10f }));
        }

        [Test]
        public void ReturnedIndex_IsAlwaysInRange()
        {
            var picker = new SlicePicker(seed: 99);
            var weights = new[] { 1f, 2f, 3f, 4f };
            for (int i = 0; i < 1000; i++)
            {
                int idx = picker.PickIndexByWeights(weights);
                Assert.GreaterOrEqual(idx, 0);
                Assert.Less(idx, weights.Length);
            }
        }

        [Test]
        public void SameSeed_ProducesIdenticalSequences()
        {
            var a = new SlicePicker(seed: 42);
            var b = new SlicePicker(seed: 42);
            var weights = new[] { 1f, 1f, 1f, 1f, 1f };
            for (int i = 0; i < 200; i++)
                Assert.AreEqual(a.PickIndexByWeights(weights), b.PickIndexByWeights(weights));
        }

        [Test]
        public void DifferentSeeds_DivergeAtLeastOnce()
        {
            var a = new SlicePicker(seed: 1);
            var b = new SlicePicker(seed: 2);
            var weights = new[] { 1f, 1f, 1f, 1f, 1f };
            bool diverged = false;
            for (int i = 0; i < 200 && !diverged; i++)
                diverged = a.PickIndexByWeights(weights) != b.PickIndexByWeights(weights);
            Assert.IsTrue(diverged, "Two different seeds should not produce identical 200-pick sequences.");
        }

        [Test]
        public void Distribution_RoughlyMatchesWeights()
        {
            // weights 1:3 -> index 1 should land ~75% of the time.
            var picker = new SlicePicker(seed: 12345);
            var weights = new[] { 1f, 3f };
            const int n = 20000;
            int ones = 0;
            for (int i = 0; i < n; i++)
                if (picker.PickIndexByWeights(weights) == 1) ones++;

            double share = (double)ones / n;
            Assert.That(share, Is.EqualTo(0.75).Within(0.03),
                $"Expected ~0.75 for index 1, got {share:F3}");
        }

        [Test]
        public void ZeroWeightSlices_AreEffectivelyNeverPicked()
        {
            // Middle slice has all the weight; the zero-weight neighbours should
            // not be selected (the leading roll==0 edge aside, which a seeded
            // RNG does not hit here).
            var picker = new SlicePicker(seed: 2024);
            var weights = new[] { 0f, 5f, 0f };
            for (int i = 0; i < 1000; i++)
                Assert.AreEqual(1, picker.PickIndexByWeights(weights));
        }
    }
}
