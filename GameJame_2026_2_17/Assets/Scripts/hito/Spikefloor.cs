using UnityEngine;

public class Spikefloor : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    public void Activate(GameObject player)
    {
        if (player == null) return;

        var col = player.GetComponent<Collider2D>();
        var target = col != null && col.attachedRigidbody != null
            ? col.attachedRigidbody.gameObject
            : player;

        Destroy(target);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        Activate(collision.gameObject);
    }
}