using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoarManager : MonoBehaviour
{
    [Header("野猪管理器设置")]
    public GameObject boarPrefab;

    public int initialBoarCount = 5;
    public int maxBoarCount = 15;
    public float spawnInterval = 2f;

    [Tooltip("野猪生成区域的中心点")]
    public Vector3 spawnAreaCenter = Vector3.zero;

    [Tooltip("野猪生成区域的大小")]
    public Vector3 spawnAreaSize = new Vector3(5f, 0f, 5f);

    [Tooltip("野猪之间的最小距离")]
    public float minDistanceBetweenBoars = 1f;
    public float time = 1f;

    // 对象池组件
    private PoolTool poolTool;

    // 当前活跃的野猪列表
    private List<GameObject> activeBoars = new List<GameObject>();

    // 生成计时器
    private float spawnTimer = 1f;

    // 当前野猪数量
    public int currentBoarCount = 0;

    private void Awake()
    {
        // 添加PoolTool组件
        poolTool = GetComponentInChildren<PoolTool>();
        poolTool.objPrefab = boarPrefab;
    }

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        // 初始生成野猪
        StartCoroutine(InitialSpawn());

        // 启动野猪生成协程
        StartCoroutine(SpawnBoarRoutine());
    }

    private IEnumerator InitialSpawn()
    {
        // 等待一帧，确保对象池已初始化
        yield return null;

        // 初始生成指定数量的野猪
        for (int i = 0; i < initialBoarCount; i++)
        {
            SpawnBoar();
        }
    }

    private IEnumerator SpawnBoarRoutine()
    {
        while (true)
        {
            // 如果当前野猪数量小于最大值，则生成新的野猪
            if (currentBoarCount < maxBoarCount)
            {
                spawnTimer += Time.deltaTime;

                if (spawnTimer >= spawnInterval)
                {
                    SpawnBoar();
                    spawnTimer = 0f;
                }
            }

            yield return null;
        }
    }

    private void SpawnBoar()
    {
        // 如果已经达到最大数量，则不再生成
        if (currentBoarCount >= maxBoarCount)
            return;

        // 尝试在指定区域内找到一个不重叠的生成点
        Vector3 spawnPosition = FindNonOverlappingPosition();

        // 如果找不到合适的位置，则不生成
        if (spawnPosition == Vector3.zero)
            return;

        // 从对象池获取野猪
        GameObject boarObject = poolTool.GetGameObjectFromPool();
        if (activeBoars.Count < 5)
        {
            StartCoroutine(InitialSpawn());
        }
        // 设置野猪位置
        boarObject.transform.position = spawnPosition;

        // 获取Boar组件并重置状态
        Boar boar = boarObject.GetComponent<Boar>();
        if (boar != null)
        {
            // 确保野猪处于活动状态
            boar.gameObject.SetActive(true);
        }

        // 添加到活跃野猪列表
        activeBoars.Add(boarObject);
        currentBoarCount++;
    }
    public void ReleaseBoar(object obj)
    {
        Debug.Log("ReleaseBoar");
        Boar boar = obj as Boar;
        currentBoarCount--;
        StartCoroutine(HandleBoarDeath(boar));
    }

    private Vector3 FindNonOverlappingPosition()
    {
        // 尝试最多100次找到一个合适的位置
        for (int i = 0; i < 100; i++)
        {
            // 在生成区域内随机生成一个位置
            float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
            float randomZ = Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2);
            Vector3 randomPosition = new Vector3(randomX, spawnAreaCenter.y, randomZ);

            // 检查该位置是否与其他野猪重叠
            bool isOverlapping = false;
            foreach (GameObject boar in activeBoars)
            {
                if (boar != null && boar.activeInHierarchy)
                {
                    float distance = Vector3.Distance(randomPosition, boar.transform.position);
                    if (distance < minDistanceBetweenBoars)
                    {
                        isOverlapping = true;
                        break;
                    }
                }
            }

            // 如果不重叠，返回该位置
            if (!isOverlapping)
            {
                return randomPosition;
            }
        }

        // 如果找不到合适的位置，返回零向量
        return Vector3.zero;
    }

    private IEnumerator HandleBoarDeath(Boar boar)
    {
        // 延迟回收野猪
        yield return new WaitForSeconds(0.5f);

        // 如果野猪对象仍然存在，处理死亡逻辑
        if (boar != null && boar.gameObject != null)
        {
            // 从活跃列表中移除
            if (activeBoars.Contains(boar.gameObject))
            {
                activeBoars.Remove(boar.gameObject);
            }

            // 回收到对象池
            poolTool.ReleaseGameObjectToPool(boar.gameObject);

            // 减少当前野猪数量
            currentBoarCount--;
        }
    }

    /// <summary>
    /// 销毁所有野猪并终止所有协程
    /// </summary>
    public void DestroyAllBoars()
    {
        // 停止所有协程
        StopAllCoroutines();

        // 遍历所有活跃的野猪
        for (int i = activeBoars.Count - 1; i >= 0; i--)
        {
            if (activeBoars[i] != null)
            {
                // 直接回收到对象池
                Destroy(activeBoars[i]);
            }
        }

        // 清空活跃野猪列表
        activeBoars.Clear();

        // 重置野猪数量
        currentBoarCount = 0;

        // 重置生成计时器
        spawnTimer = 0f;
    }

    // 在Scene视图中绘制生成区域
    private void OnDrawGizmosSelected()
    {
        // 绘制生成区域
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);

        // 绘制生成区域边框
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
