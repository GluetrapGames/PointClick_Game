using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DialougeSceneChange : MonoBehaviour
{
    
    [SerializeField]
    private Animator _crossfadeAnimator;
    
    void SceneChange(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
        AkSoundEngine.StopAll();
    }
    
    IEnumerator LoadScene(string sceneName)
    {
        
        _crossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }
    
}

