using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARRaycastManager))]
public class MakeAppearOnPlaneCustom : MonoBehaviour
{
    public GameObject ObjectToCreate;
    public bool AlreadyCreated;
    public GameObject CreatedObject;
    [Space(20)]
    public Transform FakePlanePoint;
    public bool TestCreateModel;

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (TestCreateModel == true)
        {
            TestCreate();
            TestCreateModel = false;
        }
        if (Input.touchCount == 0 || ObjectToCreate == null)
            return;

        var touch = Input.GetTouch(0);

        if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;
            if (!AlreadyCreated)
            {
                GameObject GO = Instantiate(ObjectToCreate, hitPose.position, Quaternion.identity);
                CreatedObject = GO;
                //FindObjectOfType<SceneController>().TrackableCreatedPrefab = GO;
                if (GetComponent<PlaneDetectionController>() != null)
                    GetComponent<PlaneDetectionController>().TooglePlaneCustom(false);

                AlreadyCreated = true;
            }
            else{
                //m_SessionOrigin.MakeContentAppearAt(CreatedObject.transform, hitPose.position, Quaternion.identity);
            }
        }
    }

    void TestCreate()
    {
        GameObject GO = Instantiate(ObjectToCreate, FakePlanePoint.position, Quaternion.identity);
        CreatedObject = GO;
        //FindObjectOfType<SceneController>().TrackableCreatedPrefab = GO;   
        if (GetComponent<PlaneDetectionController>() != null)
            GetComponent<PlaneDetectionController>().TooglePlaneCustom(false);

        AlreadyCreated = true;
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARSessionOrigin m_SessionOrigin;
    ARRaycastManager m_RaycastManager;
}