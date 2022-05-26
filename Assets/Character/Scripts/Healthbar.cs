using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Range(0f, 1f)]
    public float deltaTime = 0.2f;

    public RectTransform backgroundBar;
    public RectTransform damageBar;
    public RectTransform healthBar;
    protected float maxWidth;

    private WaitForSeconds delay;

    private void Awake()
    {
        maxWidth = healthBar.rect.width;
        delay = new WaitForSeconds(deltaTime);
    }

    public void Damage (int damageAmount, int health, int maxHealth)
    {
        float healthRatio = (float)health / (float)maxHealth;
        float newWidth = healthRatio * maxWidth;

        ChangeWidthBar(healthBar, newWidth);
        StartCoroutine(DelayDamageBar(newWidth));
    }

    private IEnumerator DelayDamageBar(float width)
    {
        yield return delay;
        ChangeWidthBar(damageBar, width);
    }

    private void ChangeWidthBar(RectTransform bar, float width)
    {
        bar.sizeDelta = new Vector2(width, bar.sizeDelta.y);
    }
}