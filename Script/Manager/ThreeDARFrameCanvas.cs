using UnityEngine;
using UnityEngine.UI;

namespace ViitorCloud.ARModelViewer {

    public class ThreeDARFrameCanvas : MonoBehaviour {
        [SerializeField] private Image imageFrame;
        [SerializeField] private Image frameImage;

        public void DataToDisplay(Sprite imageToDisplay, Color frameColor) {
            imageFrame.sprite = imageToDisplay;

            FrameColorChange(frameColor);
        }

        public void FrameColorChange(Color frameColor) {
            frameImage.color = frameColor;
        }
    }
}