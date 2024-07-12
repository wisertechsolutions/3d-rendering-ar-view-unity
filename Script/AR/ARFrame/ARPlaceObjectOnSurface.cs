using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ViitorCloud.ARModelViewer {
    public class ARPlaceObjectOnSurface : MonoBehaviour {
        public ARRaycastManager m_RaycastManager;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        public GameObject objectToPlace;


        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            TryPlaceObject();
        }

        private void TryPlaceObject() {
            if (!MouseOverUILayerObject.IsPointerOverUIObject()) {

                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                    Debug.Log("Touch detected");
                    Vector2 touchPosition = Input.GetTouch(0).position;
                    if (m_RaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon)) {
                    Debug.Log("Ray Hit");
                        Pose hitPose = hits[0].pose;
                        
                        GameObject g = Instantiate(objectToPlace, hitPose.position, hitPose.rotation);
                        g.transform.forward = -hitPose.up;
                        
                    }
                }
            }
        }

        private void PlaceObject() {

        }
    }
}
