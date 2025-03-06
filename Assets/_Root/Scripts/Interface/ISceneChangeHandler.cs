using UnityEngine.SceneManagement;

namespace GlueTrap
{
public interface ISceneChangeHandler
{
	void OnSceneChange(Scene scene, LoadSceneMode mode);
}
}