using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ViitorCloud.ARModelViewer {
    public class ARFrame3D : MonoBehaviour {
        [SerializeField] private Material paintingMaterial;
        [SerializeField] private Material frameMaterial;
        [SerializeField] private Transform frame_transform;

        //[SerializeField] private Image frameImageWhite;
        private float variationWhite = 80f;

        private Vector3 axis = new Vector3(0, 0, 1);
        private Vector2 frameStaticSizeDelta = new Vector2(1080, 1920);
        Vector3 screenBounds;
        float aspectratio;

        public void DataToDisplay(Texture imageToDisplay, Color frameColor) {
            paintingMaterial.mainTexture = imageToDisplay;
            ResizeWithUnits(DataForAllScene.Instance.imageDimensions.x, DataForAllScene.Instance.imageDimensions.y, DataForAllScene.Instance.imageDimensionUnit);// resize
            FrameColorChange(frameColor);
        }

        public void FrameColorChange(Color frameColor) {
            frameMaterial.color = frameColor;
        }

        private float FrameHeightCalcLogic(Sprite imageToDisplay) {
            return (frameStaticSizeDelta.x * imageToDisplay.texture.height / imageToDisplay.texture.width);
        }

        public void RotateTheImage() {
            frame_transform.Rotate(axis, 90f);
        }
        public void ResetRotation() {
            frame_transform.rotation = Quaternion.identity;
        }


        #region Resize Frame

        private float inchesToMeter = 0.0254f;
        private float cmToMeter = 0.01f;

        // method to resize the image with unit conversion
        public void ResizeWithUnits(float width, float height, string unit) {
            float newWidth = width;
            float newHeight = height;

            switch (unit) {
                case "in":
                    newWidth *= inchesToMeter;
                    newHeight *= inchesToMeter;
                    break;

                case "cm":
                    newWidth *= cmToMeter;
                    newHeight *= cmToMeter;
                    break;

                case "m":
                    break;

                default:
                    Debug.LogError("Invalid unit provided. Default inch taken");
                    newWidth *= inchesToMeter;
                    newHeight *= inchesToMeter;
                    break;
            }

            transform.localScale = new Vector3(newWidth, newHeight, transform.localScale.z);

        }
        #endregion Resize Image
    }
}
