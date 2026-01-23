using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))] // Asegura que el GameObject tenga un CharacterController
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")] public float moveSpeed = 5f; // Velocidad de movimiento del jugador

    [Header("Salto / Gravedad")] public float jumpHeight = 2f; // Altura del salto
    public float gravity = -9.81f; // Aceleración debida a la gravedad

    private CharacterController characterController; // Referencia al componente CharacterController

    [SerializeField] private Vector2 moveInput; // Entrada de movimiento del jugador
    private float verticalVelocity; // Velocidad vertical del jugador
    private bool jumpRequested = false; // Indica si se ha solicitado un salto

    [SerializeField] private AudioSource salto; // Sonido de salto
    [SerializeField] private AudioSource pasos; // Sonido de pasos
    [SerializeField] private int minSpeedSound = 1; // Velocidad mínima para reproducir el sonido de pasos

    [SerializeField] private Animator animator; // Referencia al componente Animator

    private bool isGrounded; // Variable para verificar si el jugador está en el suelo

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>(); // Obtener el componente Animator
        characterController = GetComponent<CharacterController>(); // Obtener el componente CharacterController
    }

    private void OnMove(InputValue value) 
    {
        moveInput = value.Get<Vector2>(); // Obtener la entrada de movimiento del jugador
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController == null) // Verificar si el CharacterController es nulo
            return;
        ControlMovimiento();
        ControlAnimacion();
        SonidoPasos();
    }

    private void ControlAnimacion()
    {
        Vector3 velocidad = characterController.velocity; // Obtener la velocidad del CharacterController
        Vector3 movimientoLocal = characterController.transform.InverseTransformDirection(velocidad); // Convertir la velocidad a espacio local

        animator.SetFloat("X", movimientoLocal.x); // Actualizar el parámetro "X" del Animator
        animator.SetFloat("Y", movimientoLocal.z); // Actualizar el parámetro "Y" del Animator
        animator.SetBool("EnSuelo", characterController.isGrounded); // Actualizar el parámetro "EnSuelo" del Animator
        animator.SetFloat("Z", verticalVelocity); // Actualizar el parámetro "Z" del Animator
    }

    private void SonidoPasos() // Reproducir sonido de pasos al moverse
    {
        if (pasos == null)
        {
            return;
        }
        Vector3 v = characterController.velocity; // Obtener la velocidad del CharacterController
        v.y = 0; // Ignorar la componente vertical de la velocidad
        bool andando = characterController.isGrounded && v.magnitude > minSpeedSound; // Comprobar si el jugador está en el suelo y se está moviendo

        if (andando)
        {
            if (!pasos.isPlaying) // Reproducir el sonido de pasos si no se está reproduciendo
            {
                pasos.Play();
            }
            else if (pasos.isPlaying) // Detener el sonido de pasos si no se está moviendo
            {
                pasos.Stop();
            }
        }
    }

    private void OnJump(InputValue value)
    {
        // Solicitar un salto si el botón de salto está presionado y el jugador está en el suelo
        if (value.isPressed && characterController.isGrounded) 
            jumpRequested = true;
    }

    private void ControlMovimiento()
    {
        // Si no está en el suelo, no permitimos que quede un salto pendiente
        if (!isGrounded)
            jumpRequested = false;

        isGrounded = characterController.isGrounded;
        //Reset vertical al tocar suelo
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        //Movimiento local XZ
        Vector3 localMove = new Vector3(moveInput.x, 0, moveInput.y);

        //convertir de local a mundo
        Vector3 worldMove = transform.TransformDirection(localMove);

        if (worldMove.sqrMagnitude > 1f)
            worldMove.Normalize();

        Vector3 horizontalVelocity = worldMove * moveSpeed;
        //Salto
        if (isGrounded && jumpRequested)
        {
            if (salto != null)
            {
                salto.Play();
            }
            animator.SetTrigger("Saltar"); // Activar la animación de salto
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calcular la velocidad vertical para el salto
            jumpRequested = false; // Resetear la solicitud de salto
        }


        /////////Salto
        verticalVelocity += gravity * Time.deltaTime; // Aplicar gravedad a la velocidad vertical
        //  Vector3 velocity = horizontalVelocity;
        // velocity.y = verticalVelocity;
        horizontalVelocity.y = verticalVelocity; // Asignar la velocidad vertical al movimiento horizontal
        characterController.Move(horizontalVelocity * Time.deltaTime); // Mover el CharacterController
    }
}
