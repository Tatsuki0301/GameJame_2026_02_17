using UnityEngine;

public class Spikefloor : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    public void Activate(GameObject player)
    {
        if (player == null) return;

        var rb = player.GetComponent<Rigidbody2D>();
        Destroy(rb != null ? rb.gameObject : player);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag)) return;

        Activate(collision.gameObject);
    }
}