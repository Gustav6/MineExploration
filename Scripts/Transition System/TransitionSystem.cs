using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineExploration
{
    public class TransitionSystem
    {
        public static readonly Dictionary<Transform, List<BaseTransition>> transitionPairs = [];
        private static BaseTransition tempTransition;

        public static void UpdateTransitions(GameTime gameTime)
        {
            foreach (Transform id in transitionPairs.Keys)
            {
                foreach (BaseTransition transition in transitionPairs[id])
                {
                    transition.Update(gameTime);
                }
            }

            for (int i = transitionPairs.Keys.Count - 1; i >= 0; i--)
            {
                for (int j = transitionPairs.ElementAt(i).Value.Count - 1; j >= 0; j--)
                {
                    tempTransition = transitionPairs.ElementAt(i).Value[j];

                    if (tempTransition.IsRemoved)
                    {
                        tempTransition.Execute?.Invoke();
                        transitionPairs.ElementAt(i).Value.Remove(tempTransition);

                        if (transitionPairs.ElementAt(i).Value.Count == 0)
                        {
                            transitionPairs.Remove(transitionPairs.ElementAt(i).Key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add transition that effects the attached object.
        /// The attached object will "own" this transition.
        /// </summary>
        /// <param name="attachedTo"></param>
        /// <param name="transition"></param>
        /// <param name="removeTransitionsWithSameID"></param>
        /// <param name="loop"></param>
        public static void AddTransition(Transform attachedTo, BaseTransition transition, bool removeTransitionsWithSameID = true, bool loop = false)
        {
            transition.Loop = loop;

            if (transitionPairs.TryGetValue(attachedTo, out List<BaseTransition> value))
            {
                if (removeTransitionsWithSameID)
                {
                    RemoveTransitionsWithSameID(attachedTo, transition);
                }

                value.Add(transition);
            }
            else
            {
                transitionPairs.Add(attachedTo, [transition]);
            }

            transition.Start();
        }

        public static void RemoveTransitionsWithSameID(Transform id, BaseTransition transition)
        {
            for (int i = transitionPairs[id].Count - 1; i >= 0; i--)
            {
                if (transition.GetType() == transitionPairs[id].ElementAt(i).GetType())
                {
                    transitionPairs[id].RemoveAt(i);
                }
            }
        }

        public static float Crossfade(float t, float a, float b) => ((1 - t) * a) + (t * b);

        public static float Flip(float t) => 1 - t;

        public static float BounceClampTop(float t) => MathF.Abs(t);
        public static float BounceClampBottom(float t) => 1 - MathF.Abs(1 - t);

        public static float Scale(float t) => t * t;
        public static float ReverseScale(float t) => t * (1 - t);

        public static float SmoothStart2(float t) => t * t;
        public static float SmoothStart3(float t) => t * t * t;
        public static float SmoothStart4(float t) => t * t * t * t;

        public static float SmoothStop2(float t) => 1 - ((1 - t) * (1 - t));
        public static float SmoothStop3(float t) => 1 - ((1 - t) * (1 - t) * (1 - t));
        public static float SmoothStop4(float t) => 1 - ((1 - t) * (1 - t) * (1 - t) * (1 - t));

        public static float SinCurve(float t, float interval, float amplitude, float offset = 0) => (amplitude * MathF.Sin(t * MathF.PI * interval)) + offset;
        public static float CosCurve(float t, float interval, float amplitude, float offset = 0) => (amplitude * MathF.Cos(t * MathF.PI * interval)) + offset;

        public static float NormalizedBezier3(float t, float windUp, float overShoot)
        {
            float s = 1 - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            return (3 * windUp * s2 * t) + (3 * overShoot * s * t2) + t3;
        }
        public static float NormalizedBezier4(float t, float b, float c, float d)
        {
            float s = 1 - t;
            float t2 = t * t;
            float s2 = s * s;
            float t3 = t2 * t;
            float s3 = s2 * s;
            float t4 = t3 * t;
            return (4 * b * s3 * t) + (8 * c * s2 * t2) + (4 * d * s * t3) + t4;
        }
    }
    public enum TransitionType
    {
        SmoothStart2,
        SmoothStart3,
        SmoothStart4,

        SmoothStop2,
        SmoothStop3,
        SmoothStop4,

        SinCurve,
        CosCurve,
    }
}
