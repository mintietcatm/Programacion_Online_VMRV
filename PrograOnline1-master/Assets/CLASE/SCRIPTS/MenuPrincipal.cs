using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public PhotonManager photonManager;

    public GameObject uiPrincipal;
    public GameObject pantallaInicio;
    public GameObject panelMenu;
    public GameObject panelServidores;
    public GameObject panelCrear;
    public GameObject panelUnirse;

    public TMP_Text mensaje;


    public TMP_InputField nombreCrear;
    public TMP_InputField cantidadJugadores;
    public Toggle partidaPrivada;
    public TMP_InputField nombreUnirse;


    private bool ocupado;
    private bool conectado;


    private void Start()
    {
        OcultarPaneles();
        uiPrincipal.SetActive(true);
        pantallaInicio.SetActive(true);

        mensaje.text = "";

    }

    private void Update()
    {
        if (pantallaInicio.activeSelf && !ocupado)
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    EntrarAlMenu();
                }
            }
        }
    }

    private async void EntrarAlMenu()
    {
        ocupado = true;

        mensaje.text = "Conectando...";

        conectado = await photonManager.ConnectToServerLobby();

        if (conectado)
        {
            OcultarPaneles();
            panelMenu.SetActive(true);
            mensaje.text = "";
        }

        else
        {
            mensaje.text = "Error, no se pudo conectar. Intentalo otra vez";
        }
        ocupado = false;
    }

    public void AbrirMenu()
    {
        if (ocupado)
        {
            return;
        }

        OcultarPaneles();
        panelMenu.SetActive(true);

    }

    public void AbrirServidores()

    {
        if (ocupado)
        {
            return;
        }

        OcultarPaneles();
        panelServidores.SetActive(true);
    }

    public void AbrirCrear()
    {
        if (ocupado)
        {
            return;
        }

        OcultarPaneles();
        panelCrear.SetActive(true);
    }

    public void AbrirUnirse()
    {
        if (ocupado)
        {
            return;
        }

        OcultarPaneles();
        panelUnirse.SetActive(true);
    }



    public async void CrearPartida()
    {

        if (ocupado)
        {
            return;
        }
        string nombre = nombreCrear.text.Trim(); //trim quita los espacios al inicio y final del texto pa que no los cuente como parte del string
        int jugadores = 2;

        if (nombre == "") //osea, si el nombre esta vacio
        {
            mensaje.text = "Inserta nombre de la partida";
            return;
        }

        if (cantidadJugadores.text.Trim() != "")
        {
            if (!int.TryParse(cantidadJugadores.text, out jugadores)) //el try parse es como el to string. lo convierte en un entero. try pq devuelve un booleano. 
            {
                mensaje.text = "Escribe una cantidad numerica";
                return;
            }

        }

        if (jugadores < 2 || jugadores > 10) // || es or
        {
            mensaje.text = "Minimo 2 jugadores, maximo 10";
            return;
        }

        await IniciarPartida(GameMode.Host, nombre, jugadores, partidaPrivada.isOn);

    }

    public async void UnirsePartida()
    {
        string nombre = nombreUnirse.text.Trim();

        if (nombre == "")
        {
            mensaje.text = "Inserta nombre de la partida";
            return;
        }

        await IniciarPartida(GameMode.Client, nombre, 2, false);
    }

    private async Task IniciarPartida(GameMode mode, string nombre, int jugadores, bool privada)
    {
        if (ocupado)
        {
            return;
        }

        ocupado = true;
        mensaje.text = "Conectando...";

        try //intenta conectarse
        { 
        if (!conectado) //si no esta conectado se intenta conectar
        {
            conectado = await photonManager.ConnectToServerLobby();
        }

        if (!conectado) //si aun no se puede conectar es pq hay error
        {
            Debug.Log("No se pudo conectar");
            mensaje.text = "No se pudo conectar";
            return;
        }

        bool resultado = await photonManager.StartGame(mode, nombre, jugadores, privada); //await awaits (literal) espera una respuesta mientras pasa algo. (unity mostrando la pantalla)

        if (resultado)
        {
            OcultarPaneles();
                mensaje.text = "";

        }

        else
            {
                conectado = false;
                mensaje.text = "No se pudo conectar";

            }
         }
        catch (System.Exception error) // si ocurre un error lo muestra
        {
            conectado = false;
            mensaje.text = "Ocurrio un error";
            Debug.Log(error);
        }

        finally //ejecuta esto al final. aunque haya fallado 
        {
            ocupado = false;
        }
        }

    public async void PartidaRapida()
    {
        await IniciarPartida(GameMode.Host, null, 2, false); //aqui rellena lo que necesita el metodo iniciar partida
    }

    public async void PartidaRandom()
    {
        await IniciarPartida(GameMode.Client, null, 2, false); //mismo que arriba
    }


    public async void UnirseDesdeLista(string nombre)
    {
        await IniciarPartida(GameMode.Client, nombre, 2, false);
    }

    private void OcultarPaneles() //ponemos todo en false pa que no se queden puestos
    {
        pantallaInicio.SetActive(false);
        panelMenu.SetActive(false);
        panelServidores.SetActive(false);
        panelCrear.SetActive(false);
        panelUnirse.SetActive(false);
    }
}
