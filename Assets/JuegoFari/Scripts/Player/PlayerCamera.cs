using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Referencias de cámara")]
    public Transform cameraTransform; // Referencia al transform de la cámara

    [Header("Configuración de cámara")]
    public float sensitivity = 60f; // Sensibilidad del mouse para la cámara
    public float minPitch = -30f; // Ángulo mínimo de pitch
    public float maxPitch = 30f; // Ángulo máximo de pitch

    private Vector2 lookInput; // Entrada de mirada del mouse
    private float cameraPitch; // Ángulo de pitch de la cámara

    [SerializeField] private PlayerInput playerInput; // Referencia al componente PlayerInput
    [SerializeField] private float delaySeconds = 2f; // Tiempo de retraso antes de activar la cámara

    private Renderer[] renderers; // Array de renderizadores del objeto


    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null) // Asignar la cámara principal si no se ha asignado ninguna
            cameraTransform = Camera.main.transform;

        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>(); // Obtener el componente PlayerInput si no se ha asignado
        }

        renderers = GetComponentsInChildren<Renderer>(); // Obtener todos los renderizadores hijos

        Ocultar(); // Ocultar el objeto al inicio
    }

    private void Ocultar()
    {
        foreach (var r in renderers) // Desactivar la visibilidad de cada renderer
        {
            r.enabled = false;
        }
    }
    void Start()
    {
        float yaw = transform.eulerAngles.y; // Guardamos el valor actual de yaw (rotación en Y)
        transform.rotation = Quaternion.Euler(0, yaw, 0); // Reseteamos la rotación en X y Z, manteniendo Y
        cameraPitch = 0f; // Reseteamos el pitch de la cámara
        lookInput = Vector2.zero; // Reseteamos la entrada de mirada
        if (cameraTransform != null) // Reseteamos la rotación local de la cámara
            cameraTransform.localRotation = Quaternion.identity;

        StartCoroutine("StartInput"); // Iniciar la corrutina para activar la entrada del jugador después de un retraso
    }

    IEnumerator StartInput()
    {
        yield return new WaitForSeconds(delaySeconds); // Esperar el tiempo especificado

        Mostrar(); // Mostrar el objeto

        if (playerInput != null) // Verificar si playerInput no es nulo
            playerInput.ActivateInput(); // Activar la entrada del jugador
    }

    private void Mostrar()
    {
        foreach (var r in renderers) // Activar la visibilidad de cada renderer
        {
            r.enabled = true;
        }
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    void Update()
    {
        if (cameraTransform == null) // Verificar si la referencia de la cámara es nula
            return;
        HandleLook(); // Manejar la entrada de mirada del mouse
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * sensitivity * Time.deltaTime; // Movimiento horizontal del ratón
        float mouseY = lookInput.y * sensitivity * Time.deltaTime; // Movimiento vertical del ratón

        transform.Rotate(0f, mouseX, 0f); // Rotamos el jugador en Y (yaw)

        cameraPitch -= mouseY; // Ajustamos el pitch de la cámara

        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch); // Limitamos el pitch

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f); // Aplicamos la rotación a la cámara
    }
}
