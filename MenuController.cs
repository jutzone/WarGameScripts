using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using War;

public class MenuController : MonoBehaviour
{
    #region Variables
    public int CurrentQuest, Year;
    public float Speed = 10;
    public Transform Cam;
    private Transform CurrentLerpPos;
    public Transform[] LerpPoses, LerpPoses1;
    public GameObject[] Models, Models1, ModelsQuads;
    private bool CanSelect = true;
    public static int ReturnNumber;
    [Header("ПАНЕЛИ, ОНБОРДИНГ")]
    public Text Name;
    public Text Description;
    [TextArea]
    public string[] Names;
    [TextArea]
    public string[] Descriptions;
    [TextArea]
    public string[] Names1;
    [TextArea]
    public string[] Descriptions1;
    public Button Prev, Next;
    public GameObject[] WhiteCircle, WhiteCircle1;
    public bool CircleFill;
    public float FillSpeed;
    private Vector3 BoardStartPos;
    public Transform Board;
    public float BoardSpeed;
    [Header("КАРТА")]
    public GameObject[] Light, Light1;
    public GameObject HandPointer, LearningBackgr;
    public GameObject Panel0, Panel1, FirstGO41, FirstGO42;
    [Header("СМЕНА ЛОКАЦИИ")]
    public static int StaticYear;
    public CanvasGroup[] YearButtons;//0 = 1941, 1 = 1942
    public GameObject[] YearsContent;
    public GameObject[] YearsCanvasContent;
    [Header("ЗВУКИ")]
    public AudioSource Clicks;
    public AudioClip Button;
    public AudioClip Point;
    [Header("ВИДЕО")]
    public VideoController Video;
    public VideoClip[] Videos;
    public int CurrentVideoNumberMap;
    #endregion
    private void Start()
    {
        Application.targetFrameRate = 60;
        BoardStartPos = Board.position;

        if (StaticYear == 0)
        {
            SetYear(1941);
        }
        else
        {
            SetYear(StaticYear);
        }
        ChangeContent(ReturnNumber);

        if (PlayerPrefs.GetInt("Learning") == 0)
        {
            LearningBackgr.SetActive(true);
            Panel0.SetActive(true);
            FirstGO41.GetComponent<BoxCollider>().enabled = false;
            FirstGO42.GetComponent<BoxCollider>().enabled = false;
        }
    }  

    private void Update()
    {
        if (CurrentLerpPos != null)
            Cam.position = Vector3.Lerp(Cam.position, CurrentLerpPos.position, Speed * Time.deltaTime);

        if (PlayerPrefs.GetInt("FirstGame") == 0 && PlayerPrefs.GetInt("Learning") == 1)
        {
            //Debug.Log(Board.position.y);
            if (Board.position.y > 220)
            {
                HandPointer.SetActive(true);
            }
            else
            {
                HandPointer.SetActive(false);
            }
        }
            if (CircleFill)
            switch (Year)
            {
                case 1941:
                    Light[CurrentQuest].SetActive(true);
                    break;
                case 1942:
                    Light1[CurrentQuest].SetActive(true);
                    break;
            }
        
        if (Board.position != BoardStartPos)
            Board.position = Vector3.Lerp(Board.position, BoardStartPos, BoardSpeed * Time.deltaTime);
    }

    public void SetLearningPrefs()
    {
        PlayerPrefs.SetInt("Learning", 1);
        FirstGO41.GetComponent<BoxCollider>().enabled = true;
        FirstGO42.GetComponent<BoxCollider>().enabled = true;
    }

    public void ChangeContent(int Number)
    {
        Board.position = new Vector3(Board.position.x, -1000, Board.position.z);
        CurrentQuest += Number;

        if (CurrentQuest > 8)
            CurrentQuest = 0;
        if (CurrentQuest < 0)
            CurrentQuest = 8;

        switch (Year)
        {
            case 1941:
                CurrentLerpPos = LerpPoses[CurrentQuest];
                ArrayState(WhiteCircle, CurrentQuest);
                ArrayState(Models, CurrentQuest);
                break;
            case 1942:
                CurrentLerpPos = LerpPoses1[CurrentQuest];
                ArrayState(WhiteCircle1, CurrentQuest);
                ArrayState(Models1, CurrentQuest);
                break;
        }
        SetContent();
    }

    void SetContent()
    {
        if (Year == 1941)
        {
            Name.text = Names[CurrentQuest];
            Description.text = Descriptions[CurrentQuest];
        }
        if (Year == 1942)
        {
            Name.text = Names1[CurrentQuest];
            Description.text = Descriptions1[CurrentQuest];
        }

        if (CurrentQuest == 0)
        {
            Prev.interactable = false;
            Next.interactable = true;
        }

        if(CurrentQuest > 0 && CurrentQuest < 8)
        {
            Prev.interactable = true;
            Next.interactable = true;
        }

        if (CurrentQuest == 8)
        {
            Next.interactable = false;
            Prev.interactable = true;
        }
    }

    public void OnSelectModel()
    {
        if (CanSelect)
        {
            StartCoroutine(OnSelectModelIE());
            CanSelect = false;
        }
    }

    public void ButtonClick()
    {
        Clicks.PlayOneShot(Button);
    }
    public void PointClick()
    {
        Clicks.PlayOneShot(Point);
    }

    IEnumerator OnSelectModelIE()
    {
        switch (Year)
        {
            case 1941:
                SceneControllerFoundation.ModelIndex = CurrentQuest;
                break;
            case 1942:
                SceneControllerFoundation.ModelIndex = CurrentQuest + 9;
                break;
        }
        ReturnNumber = CurrentQuest;
        CircleFill = true;
        Prev.interactable = false;
        Next.interactable = false;
        PointClick();

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("WarFoundation");
    }

    void ArrayState(GameObject[] Obj, int Number)
    {
        for (int i = 0; i < Obj.Length; i++)
        {
            Obj[i].SetActive(false);
        }

        Obj[Number].SetActive(true);
    }
    
    public void SetYear(int year)
    {
        Year = year;
        StaticYear = year;

        if (Year == 1941)
        {
            YearButtons[0].alpha = 1;
            YearButtons[1].alpha = 0;

            YearsContent[0].SetActive(true);
            YearsContent[1].SetActive(false);

            YearsCanvasContent[0].SetActive(true);
            YearsCanvasContent[1].SetActive(false);
        }
        if (Year == 1942)
        {
            YearButtons[0].alpha = 0;
            YearButtons[1].alpha = 1;

            YearsContent[0].SetActive(false);
            YearsContent[1].SetActive(true);

            YearsCanvasContent[0].SetActive(false);
            YearsCanvasContent[1].SetActive(true);
        }

        CurrentQuest = 0;
        ChangeContent(CurrentQuest);

        switch (Year)
        {
            case 1941:
                CurrentLerpPos = LerpPoses[CurrentQuest];
                break;
            case 1942:
                CurrentLerpPos = LerpPoses1[CurrentQuest];
                break;
        }

        SetContent();
    }

    public void VideoPlay(VideoClip Clip)
    {
        Screen.orientation = ScreenOrientation.Landscape;
        Video.PlayVideo(Clip);
    }
    
    public void VideoEnd()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    public void NumberOfVideoMap(int Number)
    {
        CurrentVideoNumberMap = Number;
    }
    public void VideoViewedMap()
    {
        PlayerPrefs.SetInt(FindObjectOfType<MenuController>().CurrentVideoNumberMap.ToString(), 1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
