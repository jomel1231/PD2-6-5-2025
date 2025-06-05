using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioSceneLoad : MonoBehaviour
{
	public void LoadScenario01()
	{
		SceneManager.LoadScene("LOGIN");
	}
	public void LoadScenario02()
	{
		SceneManager.LoadScene("2 Game Scene");
	}
    public void LoadScenario03()
	{
		SceneManager.LoadScene("2 Start Scene");
	}
	public void LoadTutorial()
	{
		SceneManager.LoadScene("Tutorial Scene");
	}
}
