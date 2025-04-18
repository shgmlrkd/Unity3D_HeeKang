using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    private Transform _player;

    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        direction.y = 0;

        if (direction.sqrMagnitude > 0)
        {
            transform.Translate(direction * 5 * Time.deltaTime, Space.World);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
        }
    }
}
