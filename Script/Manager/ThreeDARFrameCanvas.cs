using UnityEngine;
using UnityEngine.UI;

namespace ViitorCloud.ARModelViewer {

    public class ThreeDARFrameCanvas : MonoBehaviour {
        [SerializeField] private Image imageFrame;
        [SerializeField] private Image frameImage;
        private float variation = 20f;

        public void DataToDisplay(Sprite imageToDisplay, Color frameColor) {
            imageFrame.sprite = imageToDisplay;
            Vector2 scale = new Vector2(imageToDisplay.texture.width + variation, imageToDisplay.texture.height + variation);
            frameImage.rectTransform.sizeDelta = scale;

            FrameColorChange(frameColor);
        }

        public void FrameColorChange(Color frameColor) {
            frameImage.color = frameColor;
        }
    }
}