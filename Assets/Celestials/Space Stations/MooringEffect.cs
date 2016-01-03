using UnityEngine;
using System.Collections.Generic;

public class MooringEffect : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    public string materialTintProperty = "_TintColor";

    public int rate = 5;
    public float lifetime = 1f;
    public float speed = 1f;

    public Gradient gradient;

    private LinkedList<float> particles;
    private float lastParticle;

    void Start()
    {
        particles = new LinkedList<float>();

        float interval = 1f / rate;
        
        lastParticle = Time.time;
        float last = Time.time - lifetime;

        while (lastParticle > last)
        {
            particles.AddFirst(lastParticle);

            lastParticle -= interval;
        }
    }
    
    void Update()
    {
        if (particles == null)
        {
            particles = new LinkedList<float>();
        }

        var interval = 1f / rate;
        var now = Time.time;

        //add new particle each interval
        if (now > lastParticle + interval)
        {
            particles.AddFirst(now);
            lastParticle = now;
        }

        //remove old particles
        while (particles.Last != null && particles.Last.Value + lifetime < now)
        {
            particles.RemoveLast();
        }

        //render particles
        foreach (var particle in particles)
        {
            float age = (now - particle) / lifetime;

            var color = gradient.Evaluate(age);
            var offset = transform.forward * speed * age * transform.localScale.z;

            var pos = transform.position + offset;
            var rot = transform.rotation;
            var scale = transform.localScale;

            var particleMat = Matrix4x4.TRS(pos, rot, scale);

            var matProperties = new MaterialPropertyBlock();
            if (materialTintProperty != null)
            {
                matProperties.SetColor("_TintColor", color);
            }

            if (mesh)
            {
                Graphics.DrawMesh(mesh, particleMat, material, 0, null, 0, matProperties);
            }
        }
    }

    void OnDrawGizmos()
    {
        Vector3 meshSize;
        if (mesh)
        {
            meshSize = mesh.bounds.size;
        }
        else
        {
            meshSize = new Vector3(1, 1, 1);
        }
        meshSize.z = lifetime * speed / 4;

        for (int i = 0; i < 3; ++i)
        {
            meshSize[i] *= transform.localScale[i];
        }

        var oldMat = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + (transform.forward * meshSize.z / 2),
            transform.rotation,
            new Vector3(1, 1, 1));

        Gizmos.DrawWireCube(Vector3.zero, meshSize);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, meshSize.z));

        Gizmos.matrix = oldMat;
    }
}
