using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kizuna {
    public class PlayerFollow : MonoBehaviour {
        [Header("Object to follow")] 
        [SerializeField] private GameObject player;

        [Header("Offset Constraints")] 
        [SerializeField] private float offsetX;
        [SerializeField] private float offsetDelay;
        [SerializeField] private int maximumOffsets;
        [SerializeField] private float velocityThreshold;

        // Private variables
        private Rigidbody2D objRb2d;
        private float timer;
        private float currentOffsetX;
        private int currentMaxOffset;

        void Start() {
            objRb2d = player.GetComponent<Rigidbody2D>();
            currentOffsetX = transform.localPosition.x;
            currentMaxOffset = 0;
        }
        
        void LateUpdate() {
            // Timer is always running
            if ((objRb2d.velocity.x > velocityThreshold) || (objRb2d.velocity.x < -velocityThreshold)) { 
                timer += Time.deltaTime;
            } else {
                timer = 0;
            }

            // Going to the right
            if (objRb2d.velocity.x > 0 && timer >= offsetDelay && currentMaxOffset < maximumOffsets) {
                // Tween the interaction
                LeanTween.cancel(gameObject);
                LeanTween.moveLocalX(gameObject, currentOffsetX - offsetX, 0.2f)
                    .setEaseOutExpo()
                    .setOnComplete(() => {
                        currentOffsetX -= offsetX;
                        currentMaxOffset++;
                    });

                timer = 0;
            } else if (objRb2d.velocity.x < 0 && timer >= offsetDelay && currentMaxOffset > -maximumOffsets) {
                LeanTween.cancel(gameObject);
                LeanTween.moveLocalX(gameObject, currentOffsetX + offsetX, 0.2f)
                    .setEaseOutExpo()
                    .setOnComplete(() => {
                        currentOffsetX += offsetX;
                        currentMaxOffset--;
                    });

                timer = 0;
            }
        }
    }
}
