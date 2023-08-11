using System.Collections;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public event System.Action OnEventTriggered;

    private void OnTriggerEnter(Collider other)
    {
        // NOTE: enabled の状態に関わらず OnTrigger は呼ばれてしまうためチェックする
        if (!enabled) return;

        if (other.CompareTag("Player"))
        {
            OnEventTriggered?.Invoke();
        }
    }
}