using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI KillText;
    public TextMeshProUGUI HKillText;
    public GameObject OverPanel;
    private int killCount;
    private int hKillCount;
    void Awake()
    {
        AudioManager.Instance.PlayMusic("BGM");
    }
    public void OnPlyaerDead()
    {
        OverPanel.SetActive(true);
        HKillText.text = "最高分：" + hKillCount.ToString();
    }
    public void OnKill()
    {
        killCount++;
        hKillCount = (int)MathF.Max(hKillCount, killCount);
        KillText.text = killCount.ToString();
    }
    public void NewGame()
    {
        //重置分数
        killCount = 0;
        KillText.text = killCount.ToString();
        OverPanel.SetActive(false);
    }
}
