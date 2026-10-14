using System;
using System.Collections;
using Mono.Cecil;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Player Player => ServiceLocator.GetFirst<Player>();

    public Image InstantHealth;
    public Image DelayedHealth;

    public Image BackgroundHealth;

    public float WidthPerUnit;

    public float LerpSpeed = 2.0f;

    public float LerpDelay = 1.0f;
    private float TargetScale = 1.0f;

    private Coroutine HealthDelayedRoutine = null;

    void Awake()
    {
        ServiceLocator.Register(this);
    }

    void Start()
    {
        Player.OnHealthChanged += OnHealthChanged;
        Player.Vigor.OnLevelChanged += OnMaxHealthChanged;
        BackgroundHealth.rectTransform.sizeDelta = DelayedHealth.rectTransform.sizeDelta = 
            InstantHealth.rectTransform.sizeDelta = new Vector2(WidthPerUnit, InstantHealth.rectTransform.sizeDelta.y);

        Vector3 scale = Vector3.one;

        scale.x = Player.Vigor.GetValue();

        BackgroundHealth.rectTransform.localScale = scale;        

        scale.x = Player.GetHealth;

        InstantHealth.rectTransform.localScale = scale;

        DelayedHealth.rectTransform.localScale = scale;
    }

    void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }

    private void OnHealthChanged(float current)
    {
        float t = current;

        Vector3 instantScale = InstantHealth.rectTransform.localScale;
        
        //Healing
        if(t > instantScale.x)
        {
            instantScale.x = t;

            InstantHealth.rectTransform.localScale = instantScale;

            DelayedHealth.rectTransform.localScale = instantScale;
        }
        //Damaged
        else
        {
            // Instant bar
            instantScale.x = t;
            InstantHealth.rectTransform.localScale = instantScale;

            // Target for delayed bar
            TargetScale = t;

            if (HealthDelayedRoutine == null)
            {
                HealthDelayedRoutine = StartCoroutine(FillDelayTo());
            }
        }
    }

    private void OnMaxHealthChanged(float max)
    {
        Vector3 scale = BackgroundHealth.rectTransform.localScale;
        scale.x = max;
        BackgroundHealth.rectTransform.localScale = scale;
    }

    private IEnumerator FillDelayTo()
    {
        yield return new WaitForSeconds(LerpDelay);

        while (!Mathf.Approximately(DelayedHealth.rectTransform.localScale.x, TargetScale))
        {
            Vector3 scale = DelayedHealth.rectTransform.localScale;

            scale.x = Mathf.MoveTowards(
                scale.x,
                TargetScale,
                LerpSpeed * Time.deltaTime
            );

            DelayedHealth.rectTransform.localScale = scale;

            yield return null;
        }

        HealthDelayedRoutine = null;
    }

}