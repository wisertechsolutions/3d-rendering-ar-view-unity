using UnityEngine;
using UnityEngine.UI;

namespace ViitorCloud.ARModelViewer {

    public class ThreeDARFrameCanvas : MonoBehaviour {
        [SerializeField] private Image imageFrame;
        [SerializeField] private Outline outline;

        private static float divisionValueOutline = 20f; // We Found 100 unit => 5 then this is divisionValue

        public void DataToDisplay(Sprite imageToDisplay, Color frameColor) {
            imageFrame.sprite = imageToDisplay;

            FrameColorChange(frameColor);

            if (imageToDisplay.texture.width > imageToDisplay.texture.height) {
                outline.effectDistance = OutLineCalculations(imageToDisplay.texture.width);
            } else {
                outline.effectDistance = OutLineCalculations(imageToDisplay.texture.height);
            }
        }

        public void FrameColorChange(Color frameColor) {
            outline.effectColor = frameColor;
        }

        private Vector2 OutLineCalculations(float sizeImage) {
            return new Vector2((sizeImage / divisionValueOutline), (sizeImage / divisionValueOutline));
        }
    }
}