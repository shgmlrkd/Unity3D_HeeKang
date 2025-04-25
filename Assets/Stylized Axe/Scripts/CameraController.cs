using UnityEngine;

namespace StylizedAxe
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Vector3[] camPositions;
        [SerializeField] private float lerpSpeed = 4f;
        [SerializeField] private int index;

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, camPositions[index], lerpSpeed * Time.deltaTime);

            if((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && index >= 0)
                DecreaseIndex();
            else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                IncreaseIndex();

        }

        public void IncreaseIndex()
        {
            if (index >= 4)
                index = 4;
            else
                index++;
        }
        public void DecreaseIndex()
        {
            if (index <= 0)
                index = 0;
            else
                index--;
        }
    }
}
