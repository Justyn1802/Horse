using System;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
    {
        public Transform start;
        public Transform end;

        [Min(1.0f)] public int segments = 9;
        public float slack = 0.2f;
        
        LineRenderer line;
        
        [System.NonSerialized]
        public List<Vector3> linePoints = new List<Vector3>();

        void Start()
        {
            Generate();
        }

        private void LateUpdate()
        {
            Generate();
        }

        #region Public Methods

        public bool BothEndsExist()
        {
            return start && end;
        }

        public void Generate()
        {
            if (!BothEndsExist())
                return;

            UpdateCatenary();
            SetToLineRenderer();
        }

        public void UpdateCatenary()
        {
            if (!BothEndsExist())
                return;

            float length = (end.position - start.position).magnitude;
            float targetLength = length + slack;

            CreateCatenary(linePoints, start.position, end.position, segments, targetLength);
        }

        public void SetToLineRenderer()
        {
            line = GetComponent<LineRenderer>();

            if (!line)
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.useWorldSpace = false;
            }

            line.positionCount = linePoints.Count;

            for (int i = 0; i < linePoints.Count; i++)
                line.SetPosition(i, transform.InverseTransformPoint(linePoints[i]));
        }

        #endregion

        #region Static Methods

        static float Cosh(float f) => (float)Math.Cosh(f);
        static float Sinh(float f) => (float)Math.Sinh(f);
        static float Coth(float f) => Cosh(f) / Sinh(f);

        /// <summary>
        /// Fills up a list of points with catenary curve from p1 to p2.
        /// If the distance between ends is greater than targetLength, returns just a p1-p2 line.
        /// </summary>
        /// <param name="points">List of points to fill up. Will be overwritten. Must not be null.</param>
        public static void CreateCatenary(List<Vector3> points, in Vector3 p1, in Vector3 p2, int segments, float targetLength)
        {
            if (segments < 1)
                segments = 1;

            Vector3 diff = p2 - p1;

            // Fully taut
            if (segments == 1 || diff.magnitude > targetLength)
            {
                points.Clear();
                points.Add(p1);
                points.Add(p2);
                return;
            }

            float yDiff = diff.y;
            Vector3 planarDiff = diff;
            planarDiff.y = 0;
            float xDiff = planarDiff.magnitude;

            float l = targetLength;

            // Simple step method for finding 'a' from: https://www.alanzucconi.com/2020/12/13/catenary-2/

            const float step = 0.001f;
            float s = 0;
            do
                s += step;
            while (Sinh(s) / s < Mathf.Sqrt(l * l - yDiff * yDiff) / xDiff);

            float a = xDiff / s / 2.0f;
            float p = (xDiff - a * Mathf.Log((l + yDiff) / (l - yDiff))) / 2.0f;
            float q = (yDiff - l * Coth(s)) / 2.0f;

            points.Clear();

            for (int i = 0; i < segments + 1; i++)
            {
                float x = (float)i / segments;

                // from: https://en.wikipedia.org/wiki/Catenary#Determining_parameters
                float y = a * Cosh((x * xDiff - p) / a) + q;

                Vector3 point = planarDiff * x;
                point.y = y;
                points.Add(p1 + point);
            }
        }

        #endregion
        
    }