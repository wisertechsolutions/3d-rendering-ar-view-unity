using System;
//using UnityEditor;
using UnityEngine;

public class BoundsRecalculations : MonoBehaviour {

    #region MenuItem

    //[MenuItem("Model Viewer/Create Manager")]
    private static void CreateManager() {
        GameObject Manager = new GameObject();
        Manager.name = "Model Manager";
        Manager.AddComponent<BoundsRecalculations>();
    }

    #endregion MenuItem

    #region Declarations

    private GameObject objParent; // parent will be automatically start with default values position (0,0,0) , scale (1,1,1) , Roatation (0,0,0)   

    private int margin; // margin must be 20% as lower and highest 50%

    private Camera maincamera;
    private GameObject objChild;
    private Vector2 difference;
    private float tempScale;

    private Bounds bounds1;
    private RaycastHit hit;
    private RaycastHit hit2;
    private GameObject tempInstatiate;

    public enum SideSelect {
        Bottom, // 0
        Top, // 1
        Left, // 2
        Right, // 3
        Center, // 4
        TopLeft, // 5
        TopRight, // 6
        BottomLeft, // 7
        BottomRight, // 8
    };

    private SideSelect sideSelect;

    public static BoundsRecalculations instance = null;

    #endregion Declarations

    #region Public Methods

    /// <summary>
    /// To reset the postion and scalling according to the use case
    /// </summary>
    /// <param name="parentObj">Pass the parent object which always have default transform values</param>
    /// <param name="scalling">Pass the value of bool to enable or disable scalling</param>
    /// <param name="margins">Pass the value of boundary margins between 20-50 in percent</param>
    /// <param name="sideSelectParent">Pass the value of new pivot position from given list</param>
    public void ChildAlignmentAndScalling(GameObject parentObj, bool scalling, int margins, SideSelect sideSelectParent) {
        objParent = parentObj;
        sideSelect = sideSelectParent;          

        Init();
        //tempInstatiate.transform.position = objParent.transform.position;
        objChild = objParent.transform.GetChild(0).gameObject;
        bounds1 = GetCombinedBounds(objChild);
        objChild.transform.parent = null;

        switch ((int)sideSelect) {
            case 0:
                difference = new Vector2(0, objParent.transform.position.y - bounds1.min.y);
                objChild.transform.position = new Vector3(objChild.transform.position.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;

            case 1:
                difference = new Vector2(0, objParent.transform.position.y - bounds1.max.y);
                objChild.transform.position = new Vector3(objChild.transform.position.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;

            case 2:
                difference = new Vector2(objParent.transform.position.x - bounds1.min.x, 0);
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y, objChild.transform.position.z);
                break;

            case 3:
                difference = new Vector2(objParent.transform.position.x - bounds1.max.x, 0);
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y, objChild.transform.position.z);
                break;

            case 4:
                difference = new Vector2(objParent.transform.position.x - ((bounds1.min.x + bounds1.max.x) / 2), objParent.transform.position.y - ((bounds1.min.y + bounds1.max.y) / 2));
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;

            case 5:
                difference = new Vector2(objParent.transform.position.x - bounds1.min.x, objParent.transform.position.y - bounds1.max.y);
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;

            case 6:
                difference = new Vector2(objParent.transform.position.x - bounds1.max.x, objParent.transform.position.y - bounds1.max.y);
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;

            case 7:
                difference = new Vector2(objParent.transform.position.x - bounds1.min.x, objParent.transform.position.y - bounds1.min.y);
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;

            case 8:
                difference = new Vector2(objParent.transform.position.x - bounds1.max.x, objParent.transform.position.y - bounds1.min.y);
                objChild.transform.position = new Vector3(objChild.transform.position.x + difference.x, objParent.transform.position.y + difference.y, objChild.transform.position.z);
                break;
        }
        objChild.transform.parent = objParent.transform;

        bounds1 = GetCombinedBounds(objChild);
        if (scalling) {
            if (margins < 0) {
                margin = 20;
            } else if (margins > 50) {
                margin = 50;
            } else {
                margin = margins;
            }

            tempInstatiate.SetActive(true);
            Vector2 diffrenceScreenSize = RayCastStart();
            float scale = GetTheScaleOfParent(MathF.Max(bounds1.size.x, bounds1.size.y), bounds1, diffrenceScreenSize);
            objParent.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void Awake() {
        instance = this;
    }

    /// <summary>
    /// OnStart takes the camera and instantiate the plane for measuring the screen bounds by raycast
    /// </summary>
    private void Start() {
        //maincamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //tempInstatiate = CreatePlane(Vector3.zero);
    }

    private void Init() {
        //objParent.transform.position = Vector3.zero;
        objParent.transform.rotation = new Quaternion(0, 0, 0, 0);
        objParent.transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// scalling the object according to the use case
    /// </summary>
    /// <param name="maxValue">Maximum size of bound between X and Y</param>
    /// <param name="boundsOld">old bounds</param>
    /// <param name="diffrenceScreenSize">Difference between screen size max min points on X and Y</param>
    /// <returns> The scale of the parent in float</returns>
    private float GetTheScaleOfParent(float maxValue, Bounds boundsOld, Vector2 diffrenceScreenSize) {
        if (maxValue == boundsOld.size.x) {
            tempScale = diffrenceScreenSize.x / maxValue;
        } else {
            tempScale = diffrenceScreenSize.y / maxValue;
        }
        return tempScale;
    }

    /// <summary>
    /// To measure the screen bounds size of X is width and Y is height
    /// </summary>
    /// <returns>The bounds size of Screen X width and Y height</returns>
    private Vector2 RayCastStart() {
        var radAngle = maincamera.fieldOfView * Mathf.Deg2Rad;
        var radHFOV = 2 * Math.Atan(Mathf.Tan(radAngle / 2) * maincamera.aspect);
        var hFOV = Mathf.Rad2Deg * radHFOV;

        Vector3 noAngle = maincamera.transform.forward;
        Quaternion spreadAngle = Quaternion.AngleAxis((float)hFOV / 2f, Vector3.up);
        Vector3 newVector = spreadAngle * noAngle;

        Quaternion spreadAngle2 = Quaternion.AngleAxis((float)hFOV / -2f, Vector3.up);
        Vector3 newVector2 = spreadAngle2 * noAngle;

        Physics.Raycast(maincamera.transform.position, newVector, out hit);
        Debug.Log(hit.collider.gameObject);
        Debug.Log("Hit1 Point HR : " + hit.point);

        Physics.Raycast(maincamera.transform.position, newVector2, out hit2);
        Debug.Log(hit2.collider.gameObject);
        Debug.Log("Hit2 Point HR : " + hit2.point);

        float differenceX = Mathf.Abs(hit.point.x - hit2.point.x);
        differenceX = differenceX - ((float)margin / 100 * differenceX);

        noAngle = maincamera.transform.forward;
        spreadAngle = Quaternion.AngleAxis(maincamera.fieldOfView / 2f, Vector3.right);
        newVector = spreadAngle * noAngle;

        spreadAngle2 = Quaternion.AngleAxis(maincamera.fieldOfView / -2f, Vector3.right);
        newVector2 = spreadAngle2 * noAngle;

        Physics.Raycast(maincamera.transform.position, newVector, out hit);
        Debug.Log(hit.collider.gameObject);
        Debug.Log("Hit1 Point VR : " + hit.point);

        Physics.Raycast(maincamera.transform.position, newVector2, out hit2);
        Debug.Log(hit2.collider.gameObject);
        Debug.Log("Hit2 Point VR : " + hit2.point);

        float differenceY = Mathf.Abs(hit.point.y - hit2.point.y);
        differenceY = differenceY - ((float)margin / 100 * differenceY);

        tempInstatiate.SetActive(false);
        return new Vector2(differenceX, differenceY);
    }

    /// <summary>
    /// For getting combine bound details
    /// </summary>
    /// <param name="parent"> give the child object which have some mesh</param>
    /// <returns>The bounds of all combined meshes</returns>
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

    /// <summary>
    /// Creating the plane on start
    /// </summary>
    /// <param name="position">position where you want to create the plane</param>
    /// <returns></returns>
    private GameObject CreatePlane(Vector3 position) {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.GetComponent<MeshRenderer>().enabled = false;
        plane.transform.position = position;
        plane.transform.localScale = new Vector3(500f, 500f, 500f);
        plane.transform.Rotate(new Vector3(-90f, 0f, 0f), Space.Self);

        return plane;
    }

    #endregion Private Methods
}