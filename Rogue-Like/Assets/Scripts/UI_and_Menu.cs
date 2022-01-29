using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_and_Menu : MonoBehaviour
{
    //public void PlayGame()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

    public GameObject loadingScreen;
    public GameObject keyW;
    private bool startLevel;
    public Slider slider;
    public int nextLevel;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            keyW.SetActive(true);
            startLevel = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            keyW.SetActive(false);
            startLevel = false;
        }
    }

    public void LoadLevel(int scene)  //poner manualmente en Unity, la escena a cargar.
    {                                   
        StartCoroutine(LoadAsync(scene));
    }

    IEnumerator LoadAsync (int scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            yield return null;
        }
    }

    private void Update()
    {
        if (startLevel==true)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                LoadLevel(nextLevel);  //LoadLevel(SceneManager.GetActiveScene().buildIndex + 1); 
            }
        }
    }

}
