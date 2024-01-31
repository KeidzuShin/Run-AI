using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    [System.Serializable]
    public class RandomFailure : ActionNode {

        [Range(0,1)]
        public float chanceOfFailure = 0.5f;

        protected override void OnStart() {
            // Generate a unique seed based on the position of the GameObject
            int seed = GetSeedFromPosition(context.agent.transform.position);

            // Create a new instance of the random number generator with the unique seed
            System.Random random = new System.Random(seed);
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            float value = Random.value;
            if (value > chanceOfFailure) {
                return State.Failure;
            }
            return State.Success;
        }
        private int GetSeedFromPosition(Vector3 position)
        {
            // Convert the position values to integers
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            int z = Mathf.RoundToInt(position.z);

            // Generate a seed based on the position values
            int seed = x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();

            return seed;
        }
    }
}