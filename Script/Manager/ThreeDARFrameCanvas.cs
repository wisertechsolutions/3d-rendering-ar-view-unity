using UnityEngine;
using UnityEngine.UI;

namespace ViitorCloud.ARModelViewer {

    public class ThreeDARFrameCanvas : MonoBehaviour {
        [SerializeField] private Image imageFrame;
        [SerializeField] private Image frameImage;

        [SerializeField] private Image frameImageWhite;

        private Vector3 axis = new Vector3(0, 0, 1);
        private Vector2 frameStaticSizeDelta = new Vector2(1080, 1920);

        public void DataToDisplay(Sprite imageToDisplay, Color frameColor) {
            imageFrame.sprite = imageToDisplay;
            Vector2 scale = new Vector2(frameImage.rectTransform.sizeDelta.x + Constant.variation, FrameHeightCalcLogic(imageToDisplay) + Constant.variation);
            frameImage.rectTransform.sizeDelta = scale;

            //scale = new Vector2(frameImageWhite.rectTransform.sizeDelta.x - variationWhite, FrameHeightCalcLogic(imageToDisplay) - variationWhite);
            frameImageWhite.rectTransform.sizeDelta = scale - new Vector2(Constant.variationWhite, Constant.variationWhite);

            FrameColorChange(frameColor);
        }

        public void FrameColorChange(Color frameColor) {
            frameImage.color = frameColor;
        }

        private float FrameHeightCalcLogic(Sprite imageToDisplay) {
            return (frameStaticSizeDelta.x * imageToDisplay.texture.height / imageToDisplay.texture.width);
        }

        public void RotateTheImage() {
            imageFrame.transform.Rotate(axis, 90f);
        }
    }
}