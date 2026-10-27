using UnityEngine;

public class SampleRayHit : MonoBehaviour
{
  public void RaycastHitCheck(RaycastHit hit)
    {
        GameObject obj = hit.collider.gameObject;

        if(obj.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
        {
            enemyAI.TakeDamage(1.0f);
        }
    }
}
