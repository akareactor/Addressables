using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;


public class Load : MonoBehaviour {

	public AssetReference[] music;
	public AssetReference sphere;
	public SpriteRenderer sprite;
	public string url = "https://akareactor.github.io/spacestation/Textures/smile.png";
	GameObject currentMusic;
	// взято из примера PrefabSpawnerSample.cs
	AsyncOperationHandle<GameObject> handle;
	 
	IEnumerator Start() {
		//Addressables.InstantiateAsync("Figure");
		// yield return Addressables.LoadAssetsAsync<GameObject>("Figures", go => { UnityEngine.Debug.Log("Loaded asset: " + go.name); });
		//List<string> keys = new List<string>() { "Figures" };
		Addressables.LoadAssetsAsync<GameObject>("Figures", null).Completed += objects =>  { foreach (var go in objects.Result) {
				GameObject clone = Instantiate(go, Random.onUnitSphere * Random.RandomRange(1, 4), Quaternion.identity);
				clone.transform.SetParent(transform);
				Debug.Log($"Addressable Loaded: {go.name}");
			}
		};
		yield return null;
		// теперь загрузка текстуры, полезно для проверки работоспособности ресурса
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		yield return www.SendWebRequest();
		if (www.isNetworkError || www.isHttpError) {
			Debug.LogError(www.error);
		} else {
			Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
			sprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2());
		}
		// ссылка
		if (sphere != null) sphere.InstantiateAsync(this.transform);
	}

 	private void OnDestroy() {
        if (sphere != null && sphere.IsValid())
            sphere.ReleaseAsset();
    }

	public void Update () {
		transform.Rotate(13f * Time.deltaTime, 19f * Time.deltaTime, 28f * Time.deltaTime, Space.World);
	}

	public void LoadMusic (int i) {
		if (music[i] != null) {
			if (currentMusic) {
				Addressables.Release(handle); // что если только это делать, без Destroy?
				//Destroy(currentMusic); // память не освобождается, это видно в профайлере!
				//currentMusic.GetComponent<AudioSource>().Stop(); // память не освобождается, это видно в профайлере!
			} 
			// Нужно правильно освобождать память после InstantiateAsync
			// https://docs.unity3d.com/Packages/com.unity.addressables@1.14/manual/MemoryManagement.html
			handle = music[i].InstantiateAsync();
			handle.Completed += clone => {
				currentMusic = clone.Result;
			};
		}
	}


}
