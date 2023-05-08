using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using System.Linq;

namespace ViitorCloud.ARModelViewer {
    public class Rotate3dGameObject : MonoBehaviour {
        private float initialFingersDistance;
        private Vector3 initialScale;
        [SerializeField] private float minSize;
        [SerializeField] private float maxSize;
        [SerializeField] private Vector3 originalSize;
        private GameObject getChild;

        private float rotatespeed = 0.5f;
        private float _startingPositionX;
        private float _startingPositionY;

        private void Awake() {
            originalSize = transform.localScale;
        }

        private void Update() {
            if (!GameManager.instance.touchStart) {
                return;
            }

            if (EnhancedTouch.Touch.activeFingers.Count == 1) {
                foreach (EnhancedTouch.Touch touch in EnhancedTouch.Touch.activeTouches) {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                        _startingPositionX = touch.delta.x;
                        _startingPositionY = touch.delta.y;
                    }                    

                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved) {
                        // transform.Rotate(touch.delta.y * Constant.rotateSpeed, -touch.delta.x * Constant.rotateSpeed, 0f);                    }

                        if (_startingPositionX > touch.delta.x) {
                            transform.RotateAroundLocal(Vector3.up, rotatespeed * Time.deltaTime);
                        } else if (_startingPositionX < touch.delta.x) {
                            transform.RotateAroundLocal(Vector3.up, -rotatespeed * Time.deltaTime);
                        }

                        //float Rotation;
                        //if (transform.eulerAngles.x <= 180f) {
                        //    Rotation = transform.eulerAngles.x;
                        //} else {
                        //    Rotation = transform.eulerAngles.x - 360f;
                        //}

                        //if (Rotation < 85f && Rotation > -85f) {
                        if (_startingPositionY > touch.delta.y) {
                            transform.RotateAroundLocal(Vector3.left, rotatespeed * Time.deltaTime);
                        } else if (_startingPositionY < touch.delta.y) {
                            transform.RotateAroundLocal(Vector3.left, -rotatespeed * Time.deltaTime);
                        }
                        //} else {
                        //    if(Rotation >= 85f) {
                        //        transform.eulerAngles = new Vector3(85f, transform.eulerAngles.y, transform.eulerAngles.z);
                        //    }
                        //    else if (Rotation <= -85f) {
                        //        transform.eulerAngles = new Vector3(-85f, transform.eulerAngles.y, transform.eulerAngles.z);
                        //    }
                        //}
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
                            var currentFingersDistance = Vector2.Distance(t1.screenPosition, t2.screenPosition);
                            var scaleFactor = currentFingersDistance / initialFingersDistance;

                            float scale = initialScale.x * scaleFactor;
                            if (scale > minSize + 0.01f && scale < maxSize - 0.01f) {
                                transform.localScale = new Vector3(scale, scale, scale);
                            }
                        }
                    }
                }
            }
        }

        public void ResetRotation() {
            transform.rotation = Quaternion.identity;
            transform.localScale = originalSize;
        }

        public void ResetPositionAndChildAlignment() {
            getChild = transform.GetChild(0).gameObject;
            Bounds bounds1 = GetCombinedBounds(getChild);
            getChild.transform.parent = null;
            float difference = transform.position.y - bounds1.min.y;
            getChild.transform.position = new Vector3(getChild.transform.position.x, transform.position.y + difference, getChild.transform.position.z);
            getChild.transform.parent = transform;
        }

        private Bounds GetCombinedBounds(GameObject parent) {
            Bounds combinedBounds = new Bounds();

            //grab all child renderers
            Renderer[] renderers = GetComponentsInChildren<Renderer>(GetComponent<Renderer>());

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