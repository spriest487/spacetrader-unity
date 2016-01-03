using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(Graphic))]
public class DamageNumber : MonoBehaviour
{
    const float DISPLAY_DURATION = 0.5f;
    const float ANIMATE_SPEED = 50.0f;
    const float JITTER_X = 5.0f;
    const float ANIMATE_SCALE = 2.0f;

    [SerializeField]
    private float spawned;

    private Graphic graphic;
    
    public static DamageNumber CreateFromPrefab(DamageNumber prefab,
        int amount,
        Vector3 pos,
        DamageNumbers parent)
    {
        var number = Instantiate(prefab);

        pos.x += Random.Range(-JITTER_X, JITTER_X);

        number.GetComponent<Text>().text = amount.ToString();
        number.spawned = Time.time;
        number.transform.SetParent(parent.transform, false);
        number.transform.position = pos;

        return number;
    }

    void Start()
    {
        graphic = GetComponent<Graphic>();
    }

    public void Update()
    {
        float life = Mathf.Clamp01((Time.time - spawned) / DISPLAY_DURATION);

        var color = graphic.color;
        color.a = 1 - life;
        graphic.color = color;

        var pos = transform.position;
        pos.y = pos.y + (Time.deltaTime * ANIMATE_SPEED);
        transform.position = pos;

        var scale = transform.localScale;
        var animateScale = (Time.deltaTime * ANIMATE_SCALE);
        scale.x += animateScale;
        scale.y += animateScale;
        transform.localScale = scale;

        if (life >= 1)
        {
            Destroy(gameObject);
        }
    }
}
