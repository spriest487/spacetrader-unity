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
            initialStartSize = ps.main.startSize.constant;
            initialStartSpeed = ps.main.startSpeed.constant;
        }

        public void ApplyScale(float scale)
        {
            var main = ps.main;
            var startSize = main.startSize;
            startSize.constant = initialStartSize * scale;

            var startSpeed = main.startSpeed;
            startSpeed.constant = initialStartSpeed * scale;

            main.startSize = startSize;
            main.startSpeed = startSpeed;
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
