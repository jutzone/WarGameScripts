using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;
using War;

[RequireComponent(typeof(ARSessionOrigin))]
[RequireComponent(typeof(ARRaycastManager))]
public class MiniGameTracking : MonoBehaviour
{
    #region Variables
    public GameObject Siluet, ObjectToCreate;
    [HideInInspector] public bool AlreadyCreated;
    public GameObject CreatedObject;
    [Space(20)]
    [HideInInspector] public float DistanceToPlane;
    [HideInInspector] public Vector3 hitPose;
    [Header("Дистанция")]
    [Tooltip("Дистанция для правильной работы шкалы расстояния. 150 = Мелкий объект, 300 = Средний объект, 450 = Большой объект")]
    public float RulerDistance = 300;
    [Header("UI")]
    public Slider RulerInside;
    [Header("Detector Custom")]
    private float Y;
    private bool PlaneCreated;
    public GameObject TrackOnboarding, TrackAfter, CreateButton;
    public Text[] TrackQualityNames;
    public Color[] TrackQualityColors;
    [Space(20)]
    public UnityEvent OnCreateModel;
    public bool TestCreateModel, LearningOK;
    #endregion

    void Awake()
    {
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_RaycastManager = GetComponent<ARRaycastManager>();
        Camera.main.gameObject.AddComponent<CameraHitPoint>();
    }

    private void Start()
    {
        Siluet = Instantiate(Siluet);
        Siluet.SetActive(false);
        StartCoroutine(RareUpdate());
    }

    IEnumerator RareUpdate()
    {
        if (FindObjectOfType<ARPlane>() != null)
            PlaneCreated = true;

        if (PlaneCreated)
        {
            Transform TR = FindObjectOfType<ARPlane>().transform;
            Y = TR.position.y;
            Siluet.gameObject.SetActive(true);
            TrackAfter.SetActive(true);
            TrackOnboarding.SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        if (!PlaneCreated)
            StartCoroutine(RareUpdate());
    }

    void Update()
    {
        Siluet.transform.position = hitPose;
        RulerInside.value = DistanceToPlane / RulerDistance;

        RulerUISet();

        if (TestCreateModel)
        {
            CreateModel();
            TestCreateModel = false;
        }
    }

    void TestCreate()
    {
        GameObject GO = new GameObject();
        GO = Instantiate(ObjectToCreate, Siluet.transform.position, Siluet.transform.rotation, Siluet.transform);
        GO.transform.localScale = Vector3.one;

        GO.transform.SetParent(null);
        CreatedObject = GO;

        if (GetComponent<PlaneDetectionController>() != null)
            GetComponent<PlaneDetectionController>().TooglePlaneCustom(false);

        AfterCreate();
        AlreadyCreated = true;
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARSessionOrigin m_SessionOrigin;
    ARRaycastManager m_RaycastManager;

    public void CreateModel()
    {
        if (OnCreateModel != null)
            OnCreateModel.Invoke();
        TestCreate();
    }

    public void ChildrenOn() { }

    public void AfterCreate()
    {
        TrackOnboarding.SetActive(false);
        TrackAfter.SetActive(false);
        Siluet.gameObject.SetActive(false);
        CreateButton.SetActive(false);
    }

    void RulerUISet()
    {
        if (RulerInside.value < 0.295f)
        {
            SetTrackQualityNames(0);
            CreateButton.SetActive(false);
        }

        if (RulerInside.value < 0.7f && RulerInside.value > 0.295f)
        {
            SetTrackQualityNames(1);
            CreateButton.SetActive(true);
        }

        if (RulerInside.value > 0.7f)
        {
            SetTrackQualityNames(2);
            CreateButton.SetActive(false);
        }
    }

    void SetTrackQualityNames(int Number)
    {
        for (int i = 0; i < TrackQualityNames.Length; i++)
        {
            TrackQualityNames[i].color = Color.white;
        }

        TrackQualityNames[Number].color = TrackQualityColors[Number];
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}