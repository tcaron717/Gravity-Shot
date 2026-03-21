using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private bool _isInitialized;
    
    private void Awake()
    {
        _isInitialized = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isInitialized)
        {
            return;
        }

        if (collision == null || collision.gameObject == null)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with the player");
            DamageModel.TakeDamage(gameObject.name, collision.gameObject.name);
        }
    }
}
