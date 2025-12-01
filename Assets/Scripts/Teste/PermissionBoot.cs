using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using System.Collections;


public class PermissionBoot : MonoBehaviour
{
    void Start()
    {
#if UNITY_ANDROID
        StartCoroutine(CheckPermissions());
#else
        LoadMain();
#endif
    }

    IEnumerator CheckPermissions()
    {
        Permission.RequestUserPermission(Permission.Camera);
        Permission.RequestUserPermission(Permission.ExternalStorageRead);
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);

        yield return new WaitForSeconds(1f);

        LoadMain();
    }

    void LoadMain()
    {
        SceneManager.LoadScene("MainScene"); // sua cena principal
    }
}
