using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupMenu : MonoBehaviour {

    [SerializeField] Canvas firstCanvas;
    [SerializeField] Canvas secondCanvas;
    [SerializeField] Canvas thirdCanvas;
    [SerializeField] GameObject MenuCanvas;

    [SerializeField]
    GameObject coquiblue;


    private float timeStart;

    private float firstCanvasApparitionDelay = 2.75f;
    private float secondCanvasApparitionDelay = 3f;
    private float thirdCanvasApparitionDelay = 3.25f;
    private float nextSceneDelay = 4;

    private bool loadScene = false;
    [SerializeField] int scene;
    [SerializeField] Text loadingText;


    void Start()
    {
        timeStart = Time.time;
    }

    void Update()
    {
        float firstCanvasOpacity = (Time.time - timeStart - 0) / firstCanvasApparitionDelay;
        if (firstCanvasOpacity >= 0 && firstCanvasOpacity <= 1)
            firstCanvas.GetComponent<CanvasGroup>().alpha = firstCanvasOpacity;

        float firstCanvasOpacityWhenDisappearing = 1 - (Time.time - timeStart) / secondCanvasApparitionDelay;
        if (firstCanvasOpacityWhenDisappearing >= 0 && firstCanvasOpacityWhenDisappearing <= 1)
            firstCanvas.GetComponent<CanvasGroup>().alpha = firstCanvasOpacityWhenDisappearing;

        float secondCanvasOpacity = (Time.time - timeStart / secondCanvasApparitionDelay);
        if (secondCanvasOpacity >= 0 && secondCanvasOpacity <= 1)
            secondCanvas.GetComponent<CanvasGroup>().alpha = secondCanvasOpacity;

        float secondCanvasOpacityWhenDisappearing = 1 - (Time.time - timeStart) / thirdCanvasApparitionDelay;
        if (secondCanvasOpacityWhenDisappearing >= 0 && secondCanvasOpacityWhenDisappearing <= 1)
            secondCanvas.GetComponent<CanvasGroup>().alpha = secondCanvasOpacityWhenDisappearing;

        float thirdCanvasOpacity = (Time.time - timeStart /thirdCanvasApparitionDelay);
        if (thirdCanvasOpacity >= 0 && thirdCanvasOpacity <= 1)
            thirdCanvas.GetComponent<CanvasGroup>().alpha = thirdCanvasOpacity;

        float thirdCanvasOpacityWhenDisappearing = 1 - (Time.time - timeStart) / nextSceneDelay;
        if (thirdCanvasOpacityWhenDisappearing >= 0 && thirdCanvasOpacityWhenDisappearing <= 1)
            thirdCanvas.GetComponent<CanvasGroup>().alpha = thirdCanvasOpacityWhenDisappearing;

        if(Time.time - timeStart > nextSceneDelay)
        {
            MenuCanvas.SetActive(true);
        }

            //source : http://blog.teamtreehouse.com/make-loading-screen-unity

        if (Input.GetKeyUp(KeyCode.Space) && loadScene == false)
        {
            loadScene = true;
            loadingText.text = "Procedural map generation in process ...";
            StartCoroutine(LoadNewScene());
        }
        if (loadScene == true)
        {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        }
    }
    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(3);
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);
        while (!async.isDone)
        {
            yield return null;
        }
    }
    public void AnimCoquBlue(GameObject prmAni)
    {
        prmAni.GetComponent<Animator>().SetBool("isClick", true);

    }
    public void AnimCoquOran1(GameObject prmAni)
    {
        prmAni.GetComponent<Animator>().SetBool("isClick", true);
    }
    public void AnimCoquOran2(GameObject prmAni)
    {
        prmAni.GetComponent<Animator>().SetBool("isClick", true);
    }
}

