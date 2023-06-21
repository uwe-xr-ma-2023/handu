/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2023.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;

namespace Leap.Unity.Examples
{
    public class ProjectionPostProcessProvider : PostProcessProvider
    {

        [Header("Projection")]
        public Transform headTransform;

        [Tooltip("The scale of the projection of any hand distance from the approximated "
               + "shoulder beyond the handMergeDistance.")]
        [Range(0f, 15f)]
        public float projectionScale = 10f;

        [Tooltip("The distance from the approximated shoulder beyond which any additional "
               + "distance is exponentiated by the projectionExponent.")]
        [Range(0f, 1f)]
        public float handMergeDistance = 0.35f;

        [Tooltip("Keep the projection at a fixed distance")]
        public bool fixedDistance = false;

        [Header("Rotation")]

        [Range(-360f, 360f)]
        public float handRotationXAxis = 0f;

        [Range(-360f, 360f)]
        public float handRotationYAxis = 0f;

        [Range(-360f, 360f)]
        public float handRotationZAxis = 0f;


        public override void ProcessFrame(ref Frame inputFrame)
        {
            // Calculate the position of the head and the basis to calculate shoulder position.
            if (headTransform == null) { headTransform = Camera.main.transform; }
            Vector3 headPos = headTransform.position;
            var shoulderBasis = Quaternion.LookRotation(
              Vector3.ProjectOnPlane(headTransform.forward, Vector3.up),
              Vector3.up);

            foreach (var hand in inputFrame.Hands)
            {
                // Approximate shoulder position with magic values.
                Vector3 shoulderPos = headPos
                                      + (shoulderBasis * (new Vector3(0f, -0.13f, -0.1f)
                                      + Vector3.left * 0.15f * (hand.IsLeft ? 1f : -1f)));

                // Calculate the projection of the hand if it extends beyond the
                // handMergeDistance.
                Vector3 shoulderToHand = hand.PalmPosition - shoulderPos;
                Quaternion handRotation = hand.Rotation * Quaternion.Euler(handRotationXAxis, handRotationYAxis, handRotationZAxis);
                float handShoulderDist = shoulderToHand.magnitude;
                float projectionDistance = fixedDistance ? 1f : Mathf.Max(0f, handShoulderDist - handMergeDistance);
                float projectionAmount = 1f + (projectionDistance * projectionScale);
                hand.SetTransform(shoulderPos + shoulderToHand * projectionAmount,
                                  handRotation);
            }
        }

    }
}