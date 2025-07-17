using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// скрипт располагается в загружаемой сцене
// При загрузке сцены автоматически отыскивает объект SceneReloader, который хранит хэндл текущей загруженной сцены и поэтому может её перезагрузить
// Объекты на этой сцене могут обращаться к методу Reload, чтобы перезагрузить эту сцену

public class SceneReloaderConnector : MonoBehaviour {

    SceneReloader reloader;

    void Awake () {
        reloader = FindObjectOfType<SceneReloader>();
    }

    public void Reload () {
        if (reloader) reloader.ReloadScene(); else print("Не найден SceneReloader!");
    }

}
