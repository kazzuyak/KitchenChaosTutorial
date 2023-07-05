using UnityEngine;
using System;

public class Player : MonoBehaviour, IKitchenObjectParent {
  public static Player Instance { get; private set; }

  public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
  public class OnSelectedCounterChangedEventArgs : EventArgs {
    public BaseCounter selectedCounter;
  }

  [SerializeField] private float moveSpeed = 7f;
  [SerializeField] private GameInput gameInput;
  [SerializeField] private LayerMask countersLayerMask;
  [SerializeField] private Transform kitchenObjectHoldPoint;

  private KitchenObject kitchenObject;

  private bool isWalking;
  private Vector3 lastInteractDir;
  private BaseCounter baseCounter;

  private void Awake() {
    if (Instance != null) {
      Debug.LogError("There is more than one Player instance");
    }
    Instance = this;
  }

  private void Start() {
    gameInput.OnInteractAction += GameInput_OnInteractAction;
  }

  private void GameInput_OnInteractAction(object sender, EventArgs e) {
    if (baseCounter != null) {
      baseCounter.Interact(this);
    }
  }

  private void Update() {
    HandleMovement();
    HandleInteractions();
  }

  public bool IsWalking() {
    return isWalking;
  }

  private void HandleInteractions() {
    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    if (moveDir != Vector3.zero) {
      lastInteractDir = moveDir;
    }

    float interactionDistance = 2f;

    if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactionDistance, countersLayerMask)) {
      if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
        if (this.baseCounter != baseCounter) {
          SetSelectedCounter(baseCounter);
        }
      }
      else {
        SetSelectedCounter(null);
      }
    }
    else {
      SetSelectedCounter(null);
    }

  }

  private void HandleMovement() {

    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);


    float moveDistance = Time.deltaTime * moveSpeed;
    float playerRadius = .7f;
    float playerHeight = 2f;

    bool canMove = !Physics.CapsuleCast(
      transform.position,
      transform.position + Vector3.up * playerHeight,
      playerRadius,
      moveDir,
      moveDistance
    );

    if (!canMove) {
      Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;

      canMove = !Physics.CapsuleCast(
        transform.position,
        transform.position + Vector3.up * playerHeight,
        playerRadius,
        moveDirX,
        moveDistance
      );

      if (canMove) {
        moveDir = moveDirX;
      }

      if (!canMove) {
        Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;


        canMove = !Physics.CapsuleCast(
          transform.position,
          transform.position + Vector3.up * playerHeight,
          playerRadius,
          moveDirZ,
          moveDistance
        );

        if (canMove) {
          moveDir = moveDirZ;
        }

      }
    }

    if (canMove) {
      transform.position += moveDir * moveDistance;
    }

    isWalking = moveDir != Vector3.zero;

    float rotateSpeed = 10f;
    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
  }

  private void SetSelectedCounter(BaseCounter baseCounter) {
    this.baseCounter = baseCounter;

    OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
      selectedCounter = baseCounter
    });
  }

  public Transform GetKitchenObjectFollowTransform() {
    return kitchenObjectHoldPoint;
  }

  public void SetKitchenObject(KitchenObject kitchenObject) {
    this.kitchenObject = kitchenObject;
  }

  public KitchenObject GetKitchenObject() {
    return this.kitchenObject;
  }

  public void ClearKitchenObject() {
    this.kitchenObject = null;
  }

  public bool HasKitchenObject() {
    return kitchenObject != null;
  }
}

