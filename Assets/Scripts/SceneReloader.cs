using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// В загружаемой сцене может находиться SceneReloaderConnector, через который тамошние объекты смогут перезагружать 
// сцену sceneReference
public class SceneReloader : MonoBehaviour {

    SceneReloader instance;
    public AssetReference sceneReference;
    public AsyncOperationHandle<SceneInstance> _sceneHandle; // handle загруженной сцены

    void Awake () {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
        print("Сцена перезагружена");
    }

    public void Load () {
        // Запускаем асинхронный процесс загрузки
        // Надо всегда ставить activateOnLoad = true, потому что иначе загрузка остальных адресуемых будет заблокирована
        // https://docs.unity3d.com/Packages/com.unity.addressables@2.0/manual/LoadingScenes.html
        // activateOnLoad = false только в исключительных случаях
        _sceneHandle = Addressables.LoadSceneAsync(sceneReference, LoadSceneMode.Single, true);
        // Запускаем корутину для отслеживания состояния загрузки
        StartCoroutine(WaitForSceneLoad());
    }

    private IEnumerator WaitForSceneLoad () {
        // Ждем, пока сцена не будет загружена
        while (!_sceneHandle.IsDone) {
            // Здесь можно обновлять прогресс-бар или выполнять другие действия во время загрузки
            // Пример: progressBar.value = handle.PercentComplete;
            // Отдаем управление и продолжаем ожидание в следующем кадре
            yield return null;
        }
        // Загрузка завершена, обработаем результат
        if (_sceneHandle.Status == AsyncOperationStatus.Succeeded) {
            // Сцена успешно загружена
            Debug.Log("Scene loaded successfully.");
            print("_sceneHandle.IsValid() = " + _sceneHandle.IsValid() + ", status = " + _sceneHandle.Status);
        }
        else {
            // Произошла ошибка при загрузке сцены
            Debug.LogError("Scene loading failed: " + _sceneHandle.OperationException.Message);
        }
    }

    public void ReloadScene () {
        print("_sceneHandle.IsValid() = " + _sceneHandle.IsValid());
        // Выгружаем сцену
        Addressables.UnloadSceneAsync(_sceneHandle).Completed += operation => {
            if (operation.Status == AsyncOperationStatus.Succeeded) {
                // Загружаем сцену снова
                /* образец загрузки с делегатом Completed. Не уверен, что заработает в WebGL 
                Addressables.LoadSceneAsync(sceneReference, LoadSceneMode.Single).Completed += sceneLoadOperation => {
                    if (sceneLoadOperation.Status == AsyncOperationStatus.Succeeded) {
                        Debug.Log("Сцена была успешно перезагружена.");
                        // Обновляем handle новой загруженной сцены
                        _sceneHandle = sceneLoadOperation;
                    }
                    else {
                        Debug.LogError("Ошибка при перезагрузке сцены.");
                    }
                };
                */
                Load();
            }
            else {
                Debug.LogError("Не удалось выгрузить сцену.");
            }
        };
    }
}
