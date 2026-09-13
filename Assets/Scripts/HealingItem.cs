using UnityEngine;

public class HealingItem : MonoBehaviour
{
    [SerializeField] private int _healAmount = 1;
    private bool _isConsumed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryHeal(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryHeal(collision);
    }

    private void TryHeal(Collider2D collision)
    {
        if (_isConsumed) return;

        if (collision.TryGetComponent<IHealth>(out var target))
        {
            // Destroy the health pickup if it was consumed
            if (target.Heal(_healAmount))
            {
                _isConsumed = true;

                // Disable collider immediately so no further collisions are queued
                if (TryGetComponent<Collider2D>(out var col))
                {
                    col.enabled = false;
                }

                Destroy(gameObject);
            }
        }
    }

}
