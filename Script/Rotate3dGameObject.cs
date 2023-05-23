using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using System.Linq;

namespace ViitorCloud.ARModelViewer {
    public class Rotate3dGameObject : MonoBehaviour {
        private float initialFingersDistance;
        private Vector3 initialScale;
        private GameObject objChild;
        [SerializeField] private float minSize;
        [SerializeField] private float maxSize;
        [SerializeField] private Vector3 originalSize;
        [SerializeField] private bool ifAR;
        public Quaternion originalRotation;

        private void Awake() {
            originalSize = transform.localScale;
        }        

        private void Update() {
            if (GameManager.instance.touchStart && EnhancedTouch.Touch.activeFingers.Count == 1) {
                foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) { 
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                        transform.Rotate(0f, -touch.delta.x * Constant.rotateSpeed, 0f);
                    }
                }
            }

            if (EnhancedTouch.Touch.activeFingers.Count == 2) {
                if (transform.localScale.x > minSize && transform.localScale.x < maxSize) {
                    foreach (var (t1, t2) in from EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches let t1 = EnhancedTouch.Touch.activeTouches[0] let t2 = EnhancedTouch.Touch.activeTouches[1] select (t1, t2)) {

                        if (t1.phase == UnityEngine.InputSystem.TouchPhase.Began || t2.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                            initialFingersDistance = Vector2.Distance(t1.screenPosition, t2.screenPosition);
                            initialScale = transform.localScale;
                        } else if (t1.phase == UnityEngine.InputSystem.TouchPhase.Moved || t2.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                            //var currentFingersDistance = Vector2.Distance(t1.screenPosition, t2.screenPosition);
                            //var scaleFactor = currentFingersDistance / initialFingersDistance;

                            //float scale = initialScale.x * scaleFactor;

                            float scale = initialScale.x * (Vector2.Distance(t1.screenPosition, t2.screenPosition) / initialFingersDistance);

                            if (scale > minSize + 0.01f && scale < maxSize - 0.01f) {
                                transform.localScale = new Vector3(scale, scale, scale);
                            }
                        }
                    }
                }
            }
        }

        public void ResetRotation() {
            transform.rotation = ifAR ? originalRotation : Quaternion.identity;
            transform.localScale = originalSize;
        }

        public void ResetPositionAndChildAlignment() {
            objChild = DataForAllScene.Instance.model3d;
            objChild.transform.localPosition = new Vector3(0, 0, 0);
            objChild.transform.localScale = new Vector3(1, 1, 1);
            Bounds bounds = GetCombinedBounds(objChild);
            objChild.transform.parent = null;
            float difference = transform.position.y - bounds.min.y;
            objChild.transform.position = new Vector3(objChild.transform.position.x, transform.position.y + difference, objChild.transform.position.z);
            objChild.transform.parent = transform;

            //float size = (4 / bounds.size.y) * originalSize.x;
            //transform.localScale = new Vector3(size, size, size);   
        }

        private Bounds GetCombinedBounds(GameObject parent) {
            Bounds combinedBounds = new Bounds();

            //grab all child renderers
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>(GetComponent<Renderer>());

            //grow combined bounds with every children renderer
            foreach (Renderer rendererChild in renderers) {
                if (combinedBounds.size == Vector3.zero) {
                    combinedBounds = rendererChild.bounds;
                }
                combinedBounds.Encapsulate(rendererChild.bounds);
            }
            //at this point combinedBounds should be size of renderer and all its renderers children
            return combinedBounds;
        }
    }
}