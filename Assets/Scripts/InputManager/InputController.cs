using UnityEngine;

// TODO: Check how the namespace works
namespace InputManager {
    public class InputController : MonoBehaviour {
        private enum InputMapType {
            Player,
            Menu
        }
        
        // Auto-generated Actions
        private PlayerInputActions playerInputActions;
        
        // Variables
        [Header("Current Input Map")]
        [SerializeField] private InputMapType currentInputMap;
    }
}
