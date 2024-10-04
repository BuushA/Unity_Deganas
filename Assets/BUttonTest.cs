using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BUttonTest : MonoBehaviour
{
	[SerializeField] private string newGameLevel = "Level12";
	public void NewGameButton()
	{
		SceneManager.LoadScene(newGameLevel);


	}




}
