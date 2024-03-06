/*
    * This is a script that contains the base calculations for the game. This includes the force of gravity, the atmosphere, and aerodynamics.
    * This script is attached to the ship object.

    Code References:
        ! Cross Sectional Area Calculations:
            * MatthewFK: https://forum.unity.com/threads/how-do-i-get-the-cross-sectional-surface-area-of-my-geometry-mesh-in-meters.461278/
            * dLopreiato: https://gist.github.com/dLopreiato/7fd142d0b9728518552188794b8a750c
            * dLopreiato references this: https://loyc-etc.blogspot.com/2014/05/2d-convex-hull-in-c-45-lines-of-code.html

    * The following is a list of the formulas used in this script:

    * g = G(m1*m2)/r^2 - This is the formula for the force of gravity between two objects, where m is kg and r is meters.
    * F = m * g - This is the formula for the force of gravity, where m is mass and g is the acceleration due to gravity.
    * Relative Density = P*(T0/T) - This is the formula for the relative density of air, where P is the relative air pressure, T0 is the temperature at sea level, and T is the temperature at altitude.
    * P = (1-((B/T0)h))^(g/RB) - This is the formula for air pressure, where B is the lapse rate, T0 is the temperature at sea level, h is the altitude, g is the acceleration due to gravity, and R is the gas constant.
    * T = T0 + (B * h) - This is the formula for the temperature at a given altitude, where T is the temperature at altitude, T0 is the temperature at sea level, B is the lapse rate, and h is the altitude.
    * Lapse Rate = 0.0065f - This is the lapse rate at altitudes less than 11,000 meters.
    * Lapse Rate = 0f - This is the lapse rate at altitudes between 11,000 and 20,000 meters.
    * Lapse Rate = -0.001f - This is the lapse rate at altitudes between 20,000 and 32,000 meters.
    * Lapse Rate = -0.0028f - This is the lapse rate at altitudes between 32,000 and 47,000 meters.
    * Lapse Rate = 0f - This is the lapse rate at altitudes between 47,000 and 51,000 meters.
    * Lapse Rate = 0.0028f - This is the lapse rate at altitudes between 51,000 and 71,000 meters.
    * Lapse Rate = 0.002f - This is the lapse rate at altitudes between 71,000 and 86,000 meters.
    * Lapse Rate = 0f - This is the lapse rate at altitudes above 86,000 meters.
    * Drag = (1/2)*(D)*(V^2)*(Cd)*(A) - This is the formula for drag, where D is the air density, V is the velocity, Cd is the drag coefficient, and A is the cross-sectional area.
    * Lift = (1/2)*(D)*(V^2)*(Cl)*(A) - This is the formula for lift, where D is the air density, V is the velocity, Cl is the lift coefficient, and A is the cross-sectional area.
    * Normal Force = L*cos(a) + D*sin(a) - This is the formula for the normal force, where L is the lift, D is the drag, and a is the angle of attack (simplified to the object's forward vector vs the worl's forward vector).

*/

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// g = G(m1*m2)/r^2 - This is the formula for the force of gravity between two objects, where m is kg and r is meters.
namespace Calculator {
    public class GeneralCalculations : MonoBehaviour
    {
        // Constants
        private static float G = 6.67430f * Mathf.Pow(10, -11); // This is in m^3/(kg*s^2)

        // These will be visible to other classes.
        [SerializeField]
        protected float DragCoefficient = 1f;
        [SerializeField]
        protected float LiftCoefficient = 1f;
        [SerializeField]
        protected float ASL = 0f;
        private static int MSLTempC = 15; // This is in Degrees Celsius
        private static float MSLTempK = MSLTempC + 273.15f; // This is in Kelvin
        [SerializeField]
        private float TempK = MSLTempK;
        [SerializeField]
        private float AirPressure = 1013.25f; // This is in hPa
        [SerializeField]
        private float Density = 1.225f; // This is in kg/m^3
        private static float PlanetMass = 5.972f * Mathf.Pow(10, 24); // This is in kg
        private static float PlanetRadius = 6371000; // This is in meters
        [SerializeField]
        private float Gravity = G*PlanetMass/(Mathf.Pow(PlanetRadius, 2)); // This is in m/s^2
        [SerializeField]
        private float drag = 0f;
        private float lift = 0f;
        private float newtonWeight = 0f; // This is in Newtons

        // These will be private so only this class can edit/view it.
        private float LapseRate = 0.0065f; // This is in degrees Celsius per meter
        private float LastAltitude = 0f;
        private float crossSectionalArea = 1f;
        private Rigidbody rb;

        [SerializeField]
        private float AirSpeed = 0f;
        [SerializeField]
        private float GroundSpeed = 0f;
        [SerializeField]
        private float Speed = 0f;
        [SerializeField]
        private int SpeedDirection = 0;
        [SerializeField]
        private float MachNumber = 0f;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>(); // Get the rigidbody so we don't need to automatically assign it.
            rb.useGravity = false;

            ASL = transform.position.y;
        }

        public float getIAS()
        {
            return AirSpeed;
        }

        public float getSpeed()
        {
            return Speed;
        }

        public float getGS()
        {
            return GroundSpeed;
        }

        public float getAlt()
        {
            return ASL;
        }

        public float getDensity()
        {
            return Density;
        }

        public float getMach()
        {
            return MachNumber;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            ASL = transform.position.y+100000;

            // 100,000 meters is the maximum altitude for the atmosphere.
            if (ASL < 100000) {
                CalculateAtmosphere();
            } else {
                AirPressure = 0f;
                Density = 0f;
            }

            // Calculate the force of gravity.
            CalculateGravity();

            // Calculate aerodynamics
            CalculateAerodynamics();

            Vector3 projection = Vector3.Project(rb.velocity, transform.forward);
            float dot = Vector3.Dot(rb.velocity.normalized, transform.forward);

            switch (dot) {
                case float n when n > 0:
                    SpeedDirection = 1;
                    break;
                case float n when n < 0:
                    SpeedDirection = -1;
                    break;
                case float n when n == 0:
                    SpeedDirection = 0;
                    break;
            }

            Speed = rb.velocity.magnitude;
            AirSpeed = projection.magnitude;
            GroundSpeed = AirSpeed / (Density / 1.225f);
            if (float.IsNaN(GroundSpeed)) {
                GroundSpeed = 0f;
            }

            MachNumber = Speed/(331f*Mathf.Sqrt(TempK/273.15f));
        }

        private void CalculateAtmosphere()
        {
            // Calculate the lapse rate.
            CalculateLapseRate();

            // Calculate temperature at altitude.
            CalculateTemperatureAtAltitude();

            // Calculate air pressure.
            CalculateAirPressure();

            // Calculate the air density.
            CalculateDensity();
        }

        private void CalculateAerodynamics()
        {
            // Very important that this gets calculated FIRST.
            //!CalculateCrossSection(); // Commented out for now, as it is not working as intended.
            crossSectionalArea = 82.314f;

            // Calculate the drag.
            CalculateDrag();

            // Calculate the lift.
            CalculateLift();

            // Apply the forces.
            rb.AddForce(rb.velocity.normalized * -drag, ForceMode.Force);
            rb.AddForce(transform.up * lift, ForceMode.Force);

            // //! Normal force is not working as intended, so it is commented out for now.
            // // Get normal force:
            // float angle = Mathf.Acos(Vector3.Dot(transform.forward, rb.velocity.normalized)/(rb.velocity.normalized.magnitude*transform.forward.magnitude));

            // float normal = lift*Mathf.Cos(angle) + drag*Mathf.Sin(angle);
            // print("Lift: " + lift + " Drag: " + drag + " Angle: " + angle + " Normal: " + normal);

            // // Apply the normal force.
            // rb.AddRelativeForce(Vector3.up * normal, ForceMode.Force);
        }

        private void CalculateGravity()
        {
            // You may be asking why this is a different formula than the one commonly associated with gravity; and to that the response is simple:
            // This is only the force of gravity exerted by the planet on 1 kg, which is the acceleration of another mass towards it due to gravity.
            // To get the gravity between the two objects, we utilize the typical F=m*a formula, and that applies the gravitational force as expected.
            // g = G(m)/r^2
            // Where:
            // g = acceleration due to gravity (m/s^2)
            // G = gravitational constant
            // m = Mass of planetary body (kg)
            // r = distance between the centers of the masses

            // The force of gravity is the force with which the Earth, Moon, or other massively large object attracts another object towards itself.
            // By definition, this is the weight of the object.

            // The force of gravity is the mass of the object multiplied by the acceleration due to gravity.
            // F = m * g
            // Where:
            // F = force of gravity
            // m = mass of the object
            // g = acceleration due to gravity

            // Firstly, lets get the gravitational acceleration at the current altitude:
            Gravity = G*PlanetMass/Mathf.Pow(PlanetRadius + ASL, 2);

            // Now, lets calculate and apply the force of gravity:
            newtonWeight = rb.mass * Gravity;
            rb.AddForce(Vector3.down * newtonWeight, ForceMode.Force);
        }

        private void CalculateCrossSection()
        {   
            try
            {
                crossSectionalArea = CrossSectionArea(gameObject, rb.velocity.normalized);
                Warning.Severe(crossSectionalArea.ToString());
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        private void CalculateDrag()
        {
            // Drag = (1/2)*(D)*(V^2)*(Cd)*(A)
            // Where:
            // D = air density
            // V = velocity
            // Cd = drag coefficient
            // A = cross-sectional area
            
            drag = 1f/2f * Density * Mathf.Pow(rb.velocity.magnitude, 2) * DragCoefficient * crossSectionalArea;
        }

        private void CalculateLift()
        {
            // Lift = (1/2)*(D)*(V^2)*(Cl)*(A)
            // Where:
            // D = air density
            // V = velocity
            // Cl = lift coefficient
            // A = cross-sectional area

            lift = 1f/2f * Density * Mathf.Pow(rb.velocity.magnitude, 2) * LiftCoefficient * crossSectionalArea;
        }

        private void CalculateDensity()
        {
            // Relative Density = P*(T0/T)
            // Where:
            // P =  relative air pressure
            // T0 = temperature at sea level
            // T = temperature at altitude

            Density = AirPressure * (MSLTempK/TempK) / 1013.25f * 1.225f;
        }

        private void CalculateAirPressure()
        {
            // P = (1-((B/T0)h))^(g/RB)
            // Where:
            // P = air pressure
            // B = lapse rate
            // T0 = temperature at sea level
            // h = altitude
            // g = acceleration due to gravity
            // R = gas constant

            float R = 287.057f; // This is in J/(mol*K)

            if (LapseRate != 0)
            {
                AirPressure = Mathf.Pow(1-((LapseRate/MSLTempK)*ASL), 9.81f/(R*LapseRate))*1013.25f;
            }
        }

        private void CalculateTemperatureAtAltitude()
        {
            // The temperature at a given altitude can be calculated using the following formula:
            // T = T0 + (B * h)
            // Where:
            // T = temperature at altitude
            // T0 = temperature at sea level
            // B = lapse rate
            // h = altitude

            float ChangeInAltitude = ASL - LastAltitude;
            LastAltitude = ASL;

            // TempK is a summation of all of the temperature changes.
            TempK -= LapseRate * ChangeInAltitude;
        }

        private void CalculateLapseRate()
        {
            // Depending on altitude, the lapse rate changes.
            // The lapse rate is the rate at which temperature changes with an increase in altitude.

            // If the altitude is less than 11,000 meters, the lapse rate is -0.0065 degrees Celsius per meter.
            // If the altitude is between 11,000 and 20,000 meters, the lapse rate is 0 degrees Celsius per meter.
            // If the altitude is between 20,000 and 32,000 meters, the lapse rate is 0.001 degrees Celsius per meter.
            // If the altitude is between 32,000 and 47,000 meters, the lapse rate is 0.0028 degrees Celsius per meter.
            // If the altitude is between 47,000 and 51,000 meters, the lapse rate is 0 degrees Celsius per meter.
            // If the altitude is between 51,000 and 71,000 meters, the lapse rate is -0.0028 degrees Celsius per meter.
            // If the altitude is between 71,000 and 86,000 meters, the lapse rate is -0.002 degrees Celsius per meter.

            if (ASL < 11000)
            {
                LapseRate = 0.0065f;
            }
            else if (ASL >= 11000 && ASL < 20000)
            {
                LapseRate = 0f;
            }
            else if (ASL >= 20000 && ASL < 32000)
            {
                LapseRate = -0.001f;
            }
            else if (ASL >= 32000 && ASL < 47000)
            {
                LapseRate = -0.0028f;
            }
            else if (ASL >= 47000 && ASL < 51000)
            {
                LapseRate = 0f;
            }
            else if (ASL >= 51000 && ASL < 71000)
            {
                LapseRate = 0.0028f;
            }
            else if (ASL >= 71000 && ASL < 86000)
            {
                LapseRate = 0.002f;
            }
            else // Assume constant temperature above 86000 meters
            {
                LapseRate = 0f;
            }
        }

        public static Dictionary<GameObject, Vector3[]> verticesDict = new Dictionary<GameObject, Vector3[]>();
    
        public static float CrossSectionArea(GameObject g, Vector3 normal)
        {
            if(normal.x == 0f && normal.y == 0f && normal.z == 0f)
            {
                return 0f;
            }
    
            Vector3[] vertices;
        
            PutVerticesInDict(g, out vertices);
        
    
            normal = normal.normalized;
    
            Vector3 perp1 = Perp(normal).normalized;
            Vector3 perp2 = Vector3.Cross(normal, perp1);
    
    
            List<Vector2> inThePlane = new List<Vector2>(vertices.Length);
        
    
            for (int i = 0; i < vertices.Length; i++)
            {
            
                inThePlane.Add(new Vector2(Vector3.Dot(perp1, vertices[i]), Vector3.Dot(perp2, vertices[i])));
            }
            return HullArea(ConvexHull.ComputeConvexHull(inThePlane));
        }
        public static void PutVerticesInDict(GameObject g, out Vector3[] vertices)
        {
            if (!verticesDict.TryGetValue(g, out vertices))
            {
                MeshFilter[] meshes = g.GetComponentsInChildren<MeshFilter>();
    
    
                HashSet<Vector3> svertices = new HashSet<Vector3>();
                foreach (MeshFilter m in meshes)
                {
    
                    foreach (Vector3 v3 in m.mesh.vertices)
                    {
    
                        svertices.Add(m.transform.TransformPoint(v3));
                    }
                }
    
                vertices = new Vector3[svertices.Count];
                svertices.CopyTo(vertices);
                verticesDict.Add(g, vertices);
            }
        }
        public static Vector3 Perp(Vector3 v3)
        {
            return v3.z < v3.x ? new Vector3(v3.y, -v3.x, 0) : new Vector3(0, -v3.z, v3.y);
        }
    
    
        public static float HullArea(IList<Vector2> hull)
        {
        
            float sum = 0f;
            for (int i = 1; i < hull.Count - 1; i++)
            {
                sum += TriangleArea(hull[0], hull[i], hull[i + 1]);
            }
            return sum;
        }
        public static float TriangleArea(Vector2 v0, Vector2 v1, Vector2 v2)
        {
    
            return Mathf.Abs((v1.x - v0.x) * (v2.y - v0.y) - (v2.x - v0.x) * (v1.y - v0.y)) * .5f;
        }
    }

    public static class ConvexHull
    {
        public static IList<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false)
        {
            if (!sortInPlace)
                points = new List<Vector2>(points);
            points.Sort((a, b) =>
                a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : -1));

            // Importantly, DList provides O(1) insertion at beginning and end
            CircularList<Vector2> hull = new CircularList<Vector2>();
            int L = 0, U = 0; // size of lower and upper hulls

            // Builds a hull such that the output polygon starts at the leftmost Vector2.
            for (int i = points.Count - 1; i >= 0; i--)
            {
                Vector2 p = points[i], p1;

                // build lower hull (at end of output list)
                while (L >= 2 && (p1 = hull.Last).Sub(hull[hull.Count - 2]).Cross(p.Sub(p1)) >= 0)
                {
                    hull.PopLast();
                    L--;
                }
                hull.PushLast(p);
                L++;

                // build upper hull (at beginning of output list)
                while (U >= 2 && (p1 = hull.First).Sub(hull[1]).Cross(p.Sub(p1)) <= 0)
                {
                    hull.PopFirst();
                    U--;
                }
                if (U != 0) // when U=0, share the Vector2 added above
                    hull.PushFirst(p);
                U++;
                Debug.Assert(U + L == hull.Count + 1);
            }
            hull.PopLast();
            return hull;
        }

        private static Vector2 Sub(this Vector2 a, Vector2 b)
        {
            return a - b;
        }

        private static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        private class CircularList<T> : List<T>
        {
            public T Last
            {
                get
                {
                    return this[this.Count - 1];
                }
                set
                {
                    this[this.Count - 1] = value;
                }
            }

            public T First
            {
                get
                {
                    return this[0];
                }
                set
                {
                    this[0] = value;
                }
            }

            public void PushLast(T obj)
            {
                this.Add(obj);
            }

            public T PopLast()
            {
                if (this.Count == 0) return this[this.Count];
                
                T retVal = this[this.Count - 1];
                this.RemoveAt(this.Count - 1);
                return retVal;
            }

            public void PushFirst(T obj)
            {
                this.Insert(0, obj);
            }

            public T PopFirst()
            {
                T retVal = this[0];
                this.RemoveAt(0);
                return retVal;
            }
        }
    }
}