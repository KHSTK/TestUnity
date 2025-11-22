using System;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListener<T> : MonoBehaviour
{
    public BaseEventSO<T> eventSO;
    public UnityEvent<T> responseEvent;

    private string _listenerInfo;

    private void Awake()
    {
        // 生成监听者标识信息
        _listenerInfo = $"{gameObject.name} ({GetType().Name})";
    }

    private void OnEnable()
    {
        if (eventSO != null)
        {
            eventSO.OnEventRaised += ResponseEvent;
#if UNITY_EDITOR
            eventSO.AddListener(_listenerInfo);
#endif
        }
    }

    private void OnDisable()
    {
        if (eventSO != null)
        {
            eventSO.OnEventRaised -= ResponseEvent;
#if UNITY_EDITOR
            eventSO.RemoveListener(_listenerInfo);
#endif
        }
    }

    private void ResponseEvent(T value)
    {
        responseEvent.Invoke(value);
    }

    // 在Inspector中显示监听者信息
    private void OnValidate()
    {
        if (eventSO != null)
        {
            _listenerInfo = $"{gameObject.name} ({GetType().Name})";
        }
    }
}