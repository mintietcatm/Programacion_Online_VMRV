using Fusion;
using TMPro;
using UnityEngine;

public class EntradaServidor : MonoBehaviour
{
    public TMP_Text textoNombre;
    public TMP_Text textoJugadores;
    public TMP_Text textoCapacidad;

    public MenuPrincipal menu;
    public UnityEngine.UI.Button botondeunirse;

    public string nombreSesion;

    public void SetInfo(SessionInfo session)
    {
        nombreSesion = session.Name;

        textoNombre.text = session.Name;
        textoJugadores.text =""+ session.PlayerCount;
        textoCapacidad.text =""+ session.MaxPlayers;

        botondeunirse.interactable = session.IsOpen && session.PlayerCount < session.MaxPlayers; 



    }

    public void Unirse()
    {
        menu.UnirseDesdeLista(nombreSesion);
    }

    }
