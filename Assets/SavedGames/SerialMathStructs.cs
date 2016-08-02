using System;
using UnityEngine;

namespace SavedGames
{
    [Serializable]
    struct SerializedVector3
    {
        public float X, Y, Z;

        public Vector3 AsVector()
        {
            return new Vector3(X, Y, Z);
        }

        public SerializedVector3(Vector3 vec3)
        {
            X = vec3.x;
            Y = vec3.y;
            Z = vec3.z;
        }
    }

    [Serializable]
    struct SerializedQuaternion
    {
        public float X, Y, Z, W;

        public Quaternion AsQuaternion()
        {
            return new Quaternion(X, Y, Z, W);
        }

        public SerializedQuaternion(Quaternion quat)
        {
            X = quat.x;
            Y = quat.y;
            Z = quat.z;
            W = quat.w;
        }
    }
}
