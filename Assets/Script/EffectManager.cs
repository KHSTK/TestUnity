using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public PoolTool poolTool;
    private GameObject bloodEffect;
    public void CreateBloodEffect(object obj)
    {
        Character taget = (Character)obj;
        bloodEffect = poolTool.GetGameObjectFromPool();
        bloodEffect.transform.position = taget.transform.position;
        StartCoroutine(ReleaseBloodEffect());
    }
    IEnumerator ReleaseBloodEffect()
    {
        yield return new WaitForSeconds(0.4f);
        poolTool.ReleaseGameObjectToPool(bloodEffect);
    }

}
