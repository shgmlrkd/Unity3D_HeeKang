using UnityEngine;

namespace StylizedAxe
{
    public class DemoRotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotDir = new Vector3(0f, 1f, 0f);
        [SerializeField] private float rotSpeed = 200f;


        private void Update()
        {
            transform.Rotate(rotDir * rotSpeed * Time.deltaTime);
        }
    }
}
