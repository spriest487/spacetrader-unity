using UnityEngine;
using System.Collections;

public class ScalableParticle : MonoBehaviour
{
    private class ParticleChild
    {
        private float initialStartSize;
        private float initialStartSpeed;

        private ParticleSystem ps;

        public ParticleChild(ParticleSystem ps)
        {
            this.ps = ps;
            initialStartSize = ps.startSize;
            initialStartSpeed = ps.startSpeed;
        }

        public void ApplyScale(float scale)
        {
            ps.startSize = initialStartSize * scale;
            ps.startSpeed = initialStartSpeed * scale;
        }
    }

    private ParticleChild[] ps;
    
    [SerializeField]
    private float particleScale = 1;

    public float ParticleScale { get { return particleScale; } set { particleScale = value; } }

    void Awake()
    {
        var children = GetComponentsInChildren<ParticleSystem>();

        ps = new ParticleChild[children.Length];
        for (int child = 0; child < children.Length; ++child)
        {
            ps[child] = new ParticleChild(children[child]);
        }
    }

    void Update()
    {
        foreach (var child in ps)
        {
            child.ApplyScale(ParticleScale);
        }
    }
}
