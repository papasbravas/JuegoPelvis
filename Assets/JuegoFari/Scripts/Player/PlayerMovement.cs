using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f; // Velocidad de movimiento del jugador
    [SerializeField] private float jumpForce = 3f; // Fuerza de salto del jugador
    [SerializeField] private float gravity = -9.81f; // Fuerza de gravedad aplicada al jugador

    private CharacterController characterController; // Componente CharacterController del jugador

    [SerializeField] private Vector2 moveInput; // Entrada de movimiento del jugador
    private float verticalVelocity; // Velocidad vertical del jugador
    private bool jumpRequest = false; // Indica si se ha solicitado un salto

    [SerializeField] private Animator animator; // Componente Animator del jugador
    private bool isGrounded; // Indica si el jugador está en el suelo

    private void Start()
    {
        animator = GetComponent<Animator>(); // Obtener el componente Animator
        characterController = GetComponent<CharacterController>(); // Obtener el componente CharacterController
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void Update()
    {
        if(characterController == null)
        {
            return;
        }

        ControlMovimiento(); // Controlar el movimiento del jugador
        //ControlAnimacion(); // Controlar la animación del jugador
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

    private void ControlMovimiento()
    {
        isGrounded = characterController.isGrounded; // Verificar si el jugador está en el suelo

        if(isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Asegurar que el jugador esté pegado al suelo
        }

        // Crear un vector de movimiento local basado en la entrada del jugador
        Vector3 localMove = new Vector3(moveInput.x, 0, moveInput.y); 

        Vector3 worldMove = transform.TransformDirection(localMove); // Convertir el vector de movimiento local a mundial

        if(worldMove.sqrMagnitude > 1f)
        {
            worldMove.Normalize(); // Normalizar el vector de movimiento si su magnitud es mayor a 1
        }

        Vector3 horizontalVelocity = worldMove * moveSpeed; // Calcular la velocidad horizontal del jugador

        if(isGrounded && jumpRequest)
        {
            //animator.SetTrigger("Jump"); // Activar la animación de salto
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity); // Calcular la velocidad vertical para el salto
            jumpRequest = false; // Resetear la solicitud de salto
        }

        verticalVelocity += gravity * Time.deltaTime; // Aplicar la gravedad a la velocidad vertical
        horizontalVelocity.y = verticalVelocity; // Asignar la velocidad vertical a la velocidad horizontal
        characterController.Move(horizontalVelocity * Time.deltaTime); // Mover el jugador usando el CharacterController

    }

}

