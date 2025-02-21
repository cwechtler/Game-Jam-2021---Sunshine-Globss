﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public static LevelManager instance = null;

	[SerializeField] private string webglQuitURL = "about:blank";

	public string currentScene { get; private set; }

	private void Awake(){
		if (instance == null){
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if (instance != this){
			Destroy(gameObject);
		}
	}

	void Start(){
		currentScene = SceneManager.GetActiveScene().name;
	}

	private void Update(){
		if (SceneManager.GetActiveScene().name != currentScene) {
			currentScene = SceneManager.GetActiveScene().name;
		}
	}

	public void StartNewGame()
	{
		GameController.instance.StartGame();
		StartCoroutine(LoadScene(3, .9f));
	}

	public void Continue()
	{
		GameController.instance.Continue();
	}

	public void LoadLevel (string name){
		Debug.Log("Level load requested for: " + name);
		SceneManager.LoadScene(name);
	}

	public void LoadLevel(int levelIndex, float waitTime) {
		StartCoroutine(LoadScene(levelIndex, waitTime));
	}

	public void LoadLevelAdditive(string name) {
		SceneManager.LoadScene(name, LoadSceneMode.Additive);
	}
	
	public void LoadNextLevel() {
		StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1, .9f));
		currentScene = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name;
	}

	private IEnumerator LoadScene(int sceneToLoad, float waitTime)
	{
		print("Load " + sceneToLoad);
		yield return new WaitForSeconds(waitTime);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
		yield return new WaitUntil(() => asyncOperation.isDone);
		print("Scene " + currentScene + " Loaded");
		//if (currentScene == "Game Level 1") {
		//	GameController.instance.LoadSceneObjects();
		//}	
	}

	public IEnumerator UnloadScene(float waitTime, string name)
	{
		float counter = 0f;

		while (counter < waitTime) {
			counter += GameController.instance.timeDeltaTime;
		}
		if (counter >= waitTime) {
			print("Unload");
			SceneManager.UnloadSceneAsync(name);
		}
		yield return null;
	}

	public void QuitRequest()
	{
		Debug.Log("Level Quit Request");

		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#elif UNITY_WEBGL
			Application.OpenURL(webglQuitURL);
		#else
			Application.Quit();
		#endif
	}
}
