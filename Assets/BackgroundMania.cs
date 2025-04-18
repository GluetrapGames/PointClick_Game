using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class BackgroundMania : MonoBehaviour
{
	private Image _BGImage;
	private int _DiaDM;
	private int _DiaDMPrev;
	private int _EnvDM;
	private int _EnvDMPrev;

	private void Awake()
	{
		_EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDM = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
		_EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDMPrev = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
		_BGImage = GetComponent<Image>();
	}

	private void Update()
	{
		_EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDM = DialogueLua.GetVariable("Dia_DM_Meter").asInt;

		if (_EnvDM >= _EnvDMPrev + 2 || _DiaDMPrev >= _DiaDMPrev + 2)
		{
			_BGImage.color -= new Color(0, 20, 20, 0);
			_EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
			_DiaDMPrev = DialogueLua.GetVariable("Dia_DM_Meter").asInt;
		}
	}
}
}