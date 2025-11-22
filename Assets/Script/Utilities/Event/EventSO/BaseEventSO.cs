using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BaseEventSO<T> : ScriptableObject
{
    //事件描述
    [TextArea]
    public string description;

#if UNITY_EDITOR
    [Space(20)]
    [Header("=== 事件触发记录 ===")]
    [SerializeField] private List<string> _senders = new List<string>();
    [SerializeField] private int _maxDisplay = 5; // 控制最大显示数量

    [Space(10)]
    [Header("=== 当前监听者 ===")]
    [SerializeField] private List<string> _listeners = new List<string>();
#endif

    public UnityAction<T> OnEventRaised;
    public string lastSender;

    // 事件触发方法
    public void RaiseEvent(T value, object sender = null)
    {
        // 如果事件不为空，则触发事件
        OnEventRaised?.Invoke(value);

        // 记录发送者信息
#if UNITY_EDITOR
        RecordSender(sender?.ToString() ?? "Unknown");
        lastSender = sender?.ToString() ?? "Unknown";
#else
        lastSender = sender?.ToString() ?? "Unknown";
#endif
    }

#if UNITY_EDITOR
    private void RecordSender(string sender)
    {
        // 保持列表长度
        while (_senders.Count >= _maxDisplay)
        {
            _senders.RemoveAt(0);
        }

        _senders.Add($"{sender} ({System.DateTime.Now:HH:mm:ss})");
        UnityEditor.EditorUtility.SetDirty(this);
    }

    public void AddListener(string listenerInfo)
    {
        if (!_listeners.Contains(listenerInfo))
        {
            _listeners.Add(listenerInfo);
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    public void RemoveListener(string listenerInfo)
    {
        if (_listeners.Contains(listenerInfo))
        {
            _listeners.Remove(listenerInfo);
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    // 编辑器专用方法，获取记录信息
    public List<string> GetSenderRecords() => new List<string>(_senders);
    public List<string> GetCurrentListeners() => new List<string>(_listeners);
    public void ClearSenderRecords() { _senders.Clear(); UnityEditor.EditorUtility.SetDirty(this); }
    public void ClearListenerRecords() { _listeners.Clear(); UnityEditor.EditorUtility.SetDirty(this); }
#endif
}