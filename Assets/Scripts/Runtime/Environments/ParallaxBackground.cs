using UnityEngine;

namespace Starblast.Environments
{
    public class ParallaxBackground : MonoBehaviour
    {
        public float parallaxSpeed;
        private float length;
        private float startpos;
        public GameObject cam;

        void Start()
        {
            startpos = transform.position.x;
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        void LateUpdate()
        {
            float temp = (cam.transform.position.x * (1 - parallaxSpeed));
            float dist = (cam.transform.position.x * parallaxSpeed);

            transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

            // Loop the background
            if (temp > startpos + length) startpos += length;
            else if (temp < startpos - length) startpos -= length;
        }
    }
}