using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {
  [SerializeField] private Transform counterTopPoint;

  private KitchenObject kitchenObject;

  public Transform GetKitchenObjectFollowTransform() {
    return counterTopPoint;
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

  public virtual void Interact(Player player) {
    Debug.LogError("BaseCounter.Interact();");
  }

  public virtual void InteractAlternate(Player player) {
    // Debug.LogError("BaseCounter.InteractAlternate();");
  }
}
