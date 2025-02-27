using EditorAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempEnd : MonoBehaviour
{
	[SceneDropdown]
	public string m_SceneTransition;


	public void ChangeScene()
	{
		SceneManager.LoadScene(m_SceneTransition);
	}
}