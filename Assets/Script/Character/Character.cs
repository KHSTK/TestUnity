using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [Header("基础属性")]
    public float maxHp = 100f;
    public float currentHP = 100f;

    [Header("受伤无敌")]
    public float invulnerableDuration = 0.3f;
    public float invulnerableCounter = 0f;
    public bool invulnerable = false;

    [Header("血条设置")]
    public GameObject hpBarPrefab;
    public Vector3 hpBarOffset = new Vector3(0, 2f, 0);

    [Header("广播事件")]
    public ObjectEventSO CreateBloodEvent;
    public ObjectEventSO DeadEvent;

    protected StateHPBar hpBar;
    public bool isAlive = true;
    void Start()
    {
        Init();
    }
    public void Init()
    {
        currentHP = maxHp;
        isAlive = true;
        CreateHealthBar();
        if (hpBar != null)
        {
            hpBar.OnHPChange(currentHP / maxHp);
        }
    }
    protected virtual void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }

    // 创建世界空间血条
    protected virtual void CreateHealthBar()
    {
        if (hpBarPrefab == null) return;

        // 创建血条作为子对象
        GameObject hpBarObj = Instantiate(hpBarPrefab, transform);
        hpBarObj.transform.localPosition = hpBarOffset;

        // 配置世界空间Canvas
        ConfigureWorldSpaceCanvas(hpBarObj);

        hpBar = hpBarObj.GetComponentInChildren<StateHPBar>();

        if (hpBar != null)
            hpBar.OnHPChange(currentHP / maxHp);
    }

    // 配置世界空间Canvas
    private void ConfigureWorldSpaceCanvas(GameObject canvasObject)
    {
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        if (canvas == null) return;

        canvas.renderMode = RenderMode.WorldSpace;

        // 调整Canvas大小和缩放
        RectTransform rect = canvasObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(2f, 0.3f);
        rect.localScale = Vector3.one * 0.01f;
    }

    // 更新血条显示
    protected virtual void UpdateHealthBar()
    {
        if (hpBar != null)
        {
            float hpPercent = currentHP / maxHp;
            hpBar.OnHPChange(hpPercent);
        }
    }

    public virtual void TakeDamage(Attack attacker)
    {
        if (invulnerable || !isAlive) return;

        CreateBloodEvent?.RaiseEvent(this, this);

        if (currentHP <= attacker.damage)
        {
            currentHP = 0;
            UpdateHealthBar();
            Dead();
            return;
        }

        currentHP -= attacker.damage;
        UpdateHealthBar();
        TriggerInvulnerable();
    }

    public void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    protected virtual void Dead()
    {
        isAlive = false;
        DeadEvent?.RaiseEvent(this, this);
        // 隐藏血条
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);
    }

}