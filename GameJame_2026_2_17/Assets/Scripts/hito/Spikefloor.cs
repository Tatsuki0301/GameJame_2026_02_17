using UnityEngine;

public class Spikefloor : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        // プレイヤー即死
        var target = collision.attachedRigidbody != null ? collision.attachedRigidbody.gameObject : collision.gameObject;
        Destroy(target);
    }
}