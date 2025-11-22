using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateHPBar : MonoBehaviour
{
    public Image HPImage;
    public Image HPDeadlyImage;
    public float deadlyFollowSpeed = 0.5f; // HPDeadlyImage跟随HPImage的速度

    private float targetHPAmount; // 目标HP值

    void Start()
    {
        // 初始化时，让两个血条值相同
        targetHPAmount = HPImage.fillAmount;
        HPDeadlyImage.fillAmount = targetHPAmount;
    }

    void Update()
    {
        // 如果HPDeadlyImage的值大于目标值，则缓慢减少
        if (HPDeadlyImage.fillAmount > targetHPAmount)
        {
            HPDeadlyImage.fillAmount = Mathf.MoveTowards(
                HPDeadlyImage.fillAmount,
                targetHPAmount,
                deadlyFollowSpeed * Time.deltaTime
            );
        }
        // 如果HPDeadlyImage的值小于目标值（比如加血），则立即更新
        else if (HPDeadlyImage.fillAmount < targetHPAmount)
        {
            HPDeadlyImage.fillAmount = targetHPAmount;
        }
    }

    public void OnHPChange(float percentage)
    {
        // 立即更新主血条
        HPImage.fillAmount = percentage;
        // 更新目标值，让HPDeadlyImage在Update中缓慢跟上
        targetHPAmount = percentage;
    }
}
