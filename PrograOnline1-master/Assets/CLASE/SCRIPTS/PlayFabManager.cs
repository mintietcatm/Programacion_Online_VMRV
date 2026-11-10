using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

//Task representa la operacion asincrona que puede seguir ejecutandose
//UpdateUserDataResult es el TIPO de respuesta. NO LA RESPUESTA 
//UpdateUserDataRequest es la informacion que queremos enviar. 





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

        //Try and Catch 

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

        //
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

    private async void OnLogInSuccess(LoginResult result)
    {

        mensaje.text = "";

       
        try
        {
            await PostPlayerDataTask();
             await ObtenerDatosJugador();

            //las movi porque se abria el juego aunque hubiera una falla al enviar los datos
            paneldeLogin.SetActive(false);
            panelDeCrearCuenta.SetActive(false);
            entradaJuego.SetActive(true);
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

    private void OnPlayFabError(PlayFabError error)
    {
        ocupado = false;
        mensaje.text = error.ErrorMessage;

        Debug.LogError(error.GenerateErrorReport());
    }


    public async Task<UpdateUserDataResult> PostPlayerDataTask()
    {
        var taskSource = new TaskCompletionSource<UpdateUserDataResult>();
        int NivelJugador = 10;
        bool CheatsActivados = false;

        UpdateUserDataRequest request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"NombreJugador", "Pablo" },
                {"Nivel de Jugador", NivelJugador.ToString()},
                {"Trucos Activados", CheatsActivados.ToString()},
            }

        };

        PlayFabClientAPI.UpdateUserData(request, result => taskSource.SetResult(result), error => taskSource.SetException(new Exception(error.GenerateErrorReport()))); //mejor usamos ese para que Playfab nos diga que fue el error.


        return await taskSource.Task;
    }

    public async Task<GetUserDataResult> GetPlayerDataTask()
    {
        var taskSource = new TaskCompletionSource<GetUserDataResult>();

        GetUserDataRequest request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(request, result => taskSource.SetResult(result), error => taskSource.SetException(new Exception(error.GenerateErrorReport())));
        return await taskSource.Task;
    }

    public async Task ObtenerDatosJugador()
    {
        try
        {
            GetUserDataResult resultado = await GetPlayerDataTask();

            if (resultado.Data != null)
            {
                if (resultado.Data.ContainsKey("NombreJugador"))
                {
                    string nombreJugador = resultado.Data["NombreJugador"].Value;
                    Debug.Log("Nombre del jugador " + nombreJugador);
                }

                if (resultado.Data.ContainsKey("Nivel de Jugador"))
                {
                    int nivelJugador = int.Parse(resultado.Data["Nivel de Jugador"].Value);
                    Debug.Log("Nivel del Jugador " + nivelJugador);
                }

                if (resultado.Data.ContainsKey("Trucos Activados"))
                {
                    bool cheatsActivados = bool.Parse(resultado.Data["Trucos Activados"].Value);
                    Debug.Log("Trucos Activados " + cheatsActivados);
                }
            }
        }

        catch (Exception error)
        {
            mensaje.text = error.Message;
            Debug.LogException(error);
        }
    }
    

    private void OnDataReceived(GetUserDataResult result)
    {
        Debug.Log("Datos recibidos: " + result);

        if (result.Data != null)
        {

        }
    }

  
}
