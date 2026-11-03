using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    public GameObject panelDeCrearCuenta;
    public GameObject paneldeLogin;
    public GameObject entradaJuego;

    public TMP_InputField usuarioRegistro;
    public TMP_InputField correoRegistro;
    public TMP_InputField passwordRegistro;

    public TMP_InputField correoLogin;
    public TMP_InputField passwordLogin;

    public TMP_Text mensaje;

    private bool ocupado;

    private void Start()
    {
        panelDeCrearCuenta.SetActive(false);
        paneldeLogin.SetActive(true);
        entradaJuego.SetActive(false);
        mensaje.text = ""; //vacio
    }

    public void AbrirRegistro()
    {
        if (ocupado)
        {
            return;
        }

        paneldeLogin.SetActive(false);
        panelDeCrearCuenta.SetActive(true);
        mensaje.text = "";
    }

    public void AbrirLogin()
    {
        if (ocupado)
        {
            return;
        }

        panelDeCrearCuenta.SetActive(false);
        paneldeLogin.SetActive(true);
        mensaje.text = "";
    }

    public async void RegisterUserInPlayFab()
    {
        if (ocupado)
        {
            return;
        }

        if (usuarioRegistro.text.Trim() == ""
            || correoRegistro.text.Trim() == ""
            || passwordRegistro.text == "")
        {
            mensaje.text = "Completa todos los campos";
            return;
        }

        ocupado = true;
        mensaje.text = "Creando cuenta...";

        try
        {
            await RegisterUserInPlayFabTask();

            panelDeCrearCuenta.SetActive(false);
            paneldeLogin.SetActive(true);

            mensaje.text = "Cuenta creada. Ahora inicia sesion";
        }
        catch (Exception error)
        {
            mensaje.text = error.Message;
            Debug.LogException(error);
        }
        finally
        {
            ocupado = false;
        }
    }

    private async Task<RegisterPlayFabUserResult>
        RegisterUserInPlayFabTask()
    {
        TaskCompletionSource<RegisterPlayFabUserResult> taskResult =
            new TaskCompletionSource<RegisterPlayFabUserResult>();

        var request = new RegisterPlayFabUserRequest()
        {
            Username = usuarioRegistro.text.Trim(),
            Email = correoRegistro.text.Trim(),
            Password = passwordRegistro.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(
            request,
            result => taskResult.SetResult(result),
            error => taskResult.SetException(
                new Exception(error.GenerateErrorReport())
            )
        );

        return await taskResult.Task;
    }

    public void Login()
    {
        if (ocupado)
        {
            return;
        }

        if (correoLogin.text.Trim() == ""
            || passwordLogin.text == "")
        {
            mensaje.text = "Escribe tu correo y password";
            return;
        }

        ocupado = true;
        mensaje.text = "Iniciando sesion...";

        var request = new LoginWithEmailAddressRequest()
        {
            Email = correoLogin.text.Trim(),
            Password = passwordLogin.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(
            request,
            OnLogInSuccess,
            OnPlayFabError
        );
    }

    private void OnLogInSuccess(LoginResult result)
    {
        ocupado = false;
        mensaje.text = "";

        paneldeLogin.SetActive(false);
        panelDeCrearCuenta.SetActive(false);
        entradaJuego.SetActive(true);
    }

    private void OnPlayFabError(PlayFabError error)
    {
        ocupado = false;
        mensaje.text = error.ErrorMessage;

        Debug.LogError(error.GenerateErrorReport());
    }
}
