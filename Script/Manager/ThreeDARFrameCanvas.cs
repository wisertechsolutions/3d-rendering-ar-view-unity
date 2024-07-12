using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ViitorCloud.ARModelViewer {

    public class ThreeDARFrameCanvas : MonoBehaviour {
        [SerializeField] private Image bgFrame;
        [SerializeField] private Image imageFrame;
        [SerializeField] private Image frameImage;

        private float variationWhite = 80f;

        private Vector3 axis = new Vector3(0, 0, 1);
        private Vector2 frameStaticSizeDelta = new Vector2(1080, 1920);
        Vector3 screenBounds;
        float aspectratio;

        public void DataToDisplay(Sprite imageToDisplay, Color frameColor) {
            GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 1.6f, Screen.height * 1.6f);

            float aspectratio = imageToDisplay.bounds.size.x / imageToDisplay.bounds.size.y;
            imageFrame.sprite = imageToDisplay;
            ResizeWithUnits(DataForAllScene.Instance.imageDimensions.x, DataForAllScene.Instance.imageDimensions.y, DataForAllScene.Instance.imageDimensionUnit);// resize

            FrameColorChange(frameColor);
        }

        public void FrameColorChange(Color frameColor) {
            frameImage.color = frameColor;
        }

        private float FrameHeightCalcLogic(Sprite imageToDisplay) {
            return (frameStaticSizeDelta.x * imageToDisplay.texture.height / imageToDisplay.texture.width);
        }

        public void RotateTheImage() {
            bgFrame.transform.Rotate(axis, 90f);
        }

        #region Resize Image

        private float inchesToPixels = 96f;
        private float cmToPixels = 37.795275591f;
        private float metersToPixels = 3779.5275591f;

        // method to resize the image with unit conversion
        public void ResizeWithUnits(float width, float height, string unit) {
            float newWidth = width;
            float newHeight = height;

            switch (unit) {
                case "in":
                    newWidth *= inchesToPixels;
                    newHeight *= inchesToPixels;
                    break;

                case "cm":
                    newWidth *= cmToPixels;
                    newHeight *= cmToPixels;
                    break;

                case "m":
                    newWidth *= metersToPixels;
                    newHeight *= metersToPixels;
                    break;

                default:
                    Debug.LogError("Invalid unit provided. Default inch taken");
                    newWidth *= inchesToPixels;
                    newHeight *= inchesToPixels;
                    break;
            }

            RectTransform rectTransform = bgFrame.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(newWidth, newHeight);

            ResizeOutofBoundImage();
        }


        private void ResizeOutofBoundImage() {
            RectTransform canvasRect = GetComponent<RectTransform>();
            RectTransform imageRect = bgFrame.GetComponent<RectTransform>();
            Bounds imageBounds = new Bounds(imageRect.position, imageRect.sizeDelta);
            
            float image_aspectratio = imageBounds.size.x / imageBounds.size.y;
            //aspectratio
            if (imageRect.sizeDelta.x > canvasRect.sizeDelta.x) {
                imageRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, canvasRect.sizeDelta.x / image_aspectratio);
            }
            if (imageRect.sizeDelta.y > canvasRect.sizeDelta.y) {
                imageRect.sizeDelta = new Vector2(canvasRect.sizeDelta.y * image_aspectratio, canvasRect.sizeDelta.y);
            }
        }

        #endregion Resize Image
    }
}