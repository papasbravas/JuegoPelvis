using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject panelInicio; // Panel de inicio del menú
    [SerializeField] private GameObject panelOpciones; // Panel de opciones del menú

    [Header("Musica de Fondo")]
    [SerializeField] private AudioSource musicaFondo; // Música de fondo del menú
    [SerializeField] private float fade = 2f; // Duración del fundido de la música de fondo

    [Header("Configuración de Volumen")]
    [SerializeField] private Slider sliderVolumen; // Slider para controlar el volumen de la música de fondo
    private float volumenActual = 1f; // Volumen inicial de la música de fondo


    private void Start()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex; // Obtener el índice de la escena actual

        if(sceneIndex == 0) // Si estamos en la escena del menú principal
        {
            if(panelInicio != null && panelOpciones != null)
            {
                panelInicio.SetActive(true); // Activar el panel de inicio
                panelOpciones.SetActive(false); // Desactivar el panel de opciones
            }

            if(musicaFondo != null)
            {
                musicaFondo.volume = volumenActual; // Establecer el volumen inicial de la música de fondo
                musicaFondo.loop = true; // Configurar la música para que se repita en bucle
                musicaFondo.Play(); // Reproducir la música de fondo
            }
        }

        if(sliderVolumen != null)
        {
            volumenActual = PlayerPrefs.GetFloat("Volumen", 1f); // Cargar el volumen guardado en las preferencias del jugador
            sliderVolumen.value = volumenActual; // Establecer el valor del slider al volumen actual
            ActualizarVolumen(volumenActual); // Actualizar el volumen global del audio
            sliderVolumen.onValueChanged.AddListener(ActualizarVolumen); // Añadir un listener para actualizar el volumen cuando el slider cambie
        }
    }


    public void ActualizarVolumen(float valor)
    {
        volumenActual = valor;
        AudioListener.volume = volumenActual; // Actualizar el volumen global del audio
        PlayerPrefs.SetFloat("Volumen", volumenActual); // Guardar el volumen en las preferencias del jugador
    }

    public void VolverAlMenu()
    {
        panelInicio.SetActive(true); // Activar el panel de inicio
        panelOpciones.SetActive(false); // Desactivar el panel de opciones
    }

    public void AbrirOpciones()
    {
        panelInicio.SetActive(false); // Desactivar el panel de inicio
        panelOpciones.SetActive(true); // Activar el panel de opciones
    }

    public void Load()
    {
        StartCoroutine(FadeOutYLoad()); // Iniciar la corrutina para hacer el fade out y cargar la escena del nivel
    }

    private IEnumerator FadeOutYLoad()
    {
        if (musicaFondo != null) // Si hay musica de fondo asignada
        {
            float volumenInicial = musicaFondo.volume; // Guardar el volumen inicial de la musica

            for (float t = 0; t < fade; t += Time.deltaTime) // Hacer un bucle durante la duracion del fade
            {
                musicaFondo.volume = Mathf.Lerp(volumenInicial, 0, t / fade); // Interpolar el volumen de la musica
                yield return null;
            }

            musicaFondo.volume = 0; // Asegurarse de que el volumen sea 0 al final del fade
            musicaFondo.Stop(); // Detener la musica de fondo
        }

        SceneManager.LoadScene("Juego"); // Cargar la escena del nivel
    }


    public void VolverAlMenuPrincipal()
    {
        SceneManager.LoadScene("MenuPrincipal"); // Cargar la escena del menú principal
    }

    public void Cargar()
    {
        SceneManager.LoadScene("Juego"); // Cargar la escena del juego
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");

        // Directiva de preprocesador
        #if UNITY_EDITOR
                // Si estamos en el editor de Unity, usamos el comando para detener el juego.
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                                        // Si estamos en un ejecutable (Build), cerramos la aplicación.
                                        Application.Quit();
        #endif
    }
}
