using GlueTrap.Utilities;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace GlueTrap
{
public class BackgroundMania : MonoBehaviour
{
	private Color _BgOriginalColor;
	private Image _BGImage;
	private int _DiaDM;
	private int _DiaDMPrev;
	private int _EnvDM;
	private int _EnvDMPrev;


	private void Awake()
	{
		_EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDM = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;
		_EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDMPrev = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;
		_BGImage = GetComponent<Image>();
		_BgOriginalColor = _BGImage.color;
	}

	private void Update()
	{
		_EnvDM = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDM = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;

		if (_EnvDM >= _EnvDMPrev + 2 || _DiaDMPrev >= _DiaDMPrev + 2)
		{
			_BGImage.color -= new Color(0f, 0.01f, 0.01f, 0f);
			_EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
			_DiaDMPrev = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;
		}
		else if (_EnvDM <= _EnvDMPrev - 2 || _DiaDMPrev <= _DiaDMPrev - 2)
		{
			_BGImage.color += new Color(0f, 0.01f, 0.01f, 0f);
			_EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
			_DiaDMPrev = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;
		}
	}

	private void OnEnable()
	{
		Utils.GetGameManager().m_OnGameReset.AddListener(OnGameReset);
	}

	private void OnDisable()
	{
		Utils.GetGameManager().m_OnGameReset.RemoveListener(OnGameReset);
	}

	public void updateAlpha()
	{
		_BGImage.color += new Color(0f, 0f, 0f, 0.12f);
	}

	private void OnGameReset()
	{
		_EnvDMPrev = DialogueLua.GetVariable("Env_DM_Meter").asInt;
		_DiaDMPrev = DialogueLua.GetVariable("Dialogue_DM_Meter").asInt;
		_BGImage.color = _BgOriginalColor;
	}
}
}