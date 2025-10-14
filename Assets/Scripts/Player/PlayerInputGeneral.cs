using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputGeneral : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Weapon weapon;
    void Awake()
    {
        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();
        if (weapon == null)
            weapon = GetComponentInChildren<Weapon>();

        if (playerMovement == null)
            Debug.LogError("PlayerInputGeneral: PlayerMovement component not found!");
        if (weapon == null)
            Debug.LogError("PlayerInputGeneral: Weapon component not found!");
    }

    #region Movement Input Callbacks
    // Para movimento do personagem
    public void OnMove(InputValue value)
    {
        playerMovement.OnMove(value.Get<Vector2>());
    }

    // Para mouse position
    public void OnLook(InputValue value)
    {
        playerMovement.OnLook(value.Get<Vector2>());
    }

    // Para analógico direito
    public void OnAim(InputValue value)
    {
        playerMovement.OnAim(value.Get<Vector2>());
    }
    #endregion

    #region Weapon Input Callbacks
    public void OnFire(InputValue value)
    {
        weapon.OnFire(value.isPressed);
    }
    public void OnReload(InputValue value)
    {
        weapon.OnReload(value.isPressed);
    }
    public void OnChangeWeapon(InputValue value)
    {
        weapon.OnChangeWeapon(value.isPressed);
    }
    #endregion
}
