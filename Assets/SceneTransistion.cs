using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransistion : MonoBehaviour
{
    
    [SerializeField]
    private String _sceneToTransitionTo;
    
    [SerializeField]
    private Animator _crossfadeAnimator;
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        StartCoroutine(LoadScene(_sceneToTransitionTo));

    }

    IEnumerator LoadScene(string sceneName)
    {
        
        _crossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
    }
    
}
