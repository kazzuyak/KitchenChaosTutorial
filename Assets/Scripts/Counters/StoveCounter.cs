using System;
using UnityEngine;

public class StoveCounter : BaseCounter {
  public event EventHandler<OnStateChangedArs> OnStateChanged;
  public class OnStateChangedArs : EventArgs {
    public State state;
  }

  public enum State {
    Idle,
    Frying,
    Fried,
    Burned
  }

  [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
  [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

  private float fryingTimer;
  private float burningTimer;
  private State state;
  private FryingRecipeSO fryingRecipeSO;
  private BurningRecipeSO burningRecipeSO;


  private void Start() {
    state = State.Idle;
  }

  private void Update() {
    if (HasKitchenObject()) {
      switch (state) {
        case State.Idle:
          break;
        case State.Frying:
          fryingTimer += Time.deltaTime;

          if (fryingTimer > fryingRecipeSO.fryingTimerMax) {
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

            burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
            burningTimer = 0f;
            state = State.Fried;

            OnStateChanged?.Invoke(this, new OnStateChangedArs {
              state = state
            });

          }

          break;
        case State.Fried:
          burningTimer += Time.deltaTime;

          if (burningTimer > burningRecipeSO.burningTimerMax) {
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

            state = State.Burned;
          }
          break;
        case State.Burned:
          break;
      }
    }
  }

  public override void Interact(Player player) {
    if (!HasKitchenObject()) {
      if (player.HasKitchenObject()) {
        if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
          player.GetKitchenObject().SetKitchenObjectParent(this);
          fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
          state = State.Frying;
          fryingTimer = 0f;
        }
      }
    }
    else {
      if (!player.HasKitchenObject()) {
        GetKitchenObject().SetKitchenObjectParent(player);
        state = State.Idle;
      }
    }
  }

  private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
    FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

    return fryingRecipeSO != null;
  }

  private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
    FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

    if (fryingRecipeSO != null) {
      return fryingRecipeSO.output;
    }

    return null;
  }

  private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray) {
      if (fryingRecipeSO.input == inputKitchenObjectSO) {
        return fryingRecipeSO;
      }
    }

    return null;
  }

  private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
    foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray) {
      if (burningRecipeSO.input == inputKitchenObjectSO) {
        return burningRecipeSO;
      }
    }

    return null;
  }
}
