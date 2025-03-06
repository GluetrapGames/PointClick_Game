using EditorAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GlueTrap.Utilities
{
	public class TempEnd : MonoBehaviour
	{
		[SceneDropdown]
		public string m_SceneTransition;


		public void ChangeScene()
		{
			SceneManager.LoadScene(m_SceneTransition);
		}
	}
}