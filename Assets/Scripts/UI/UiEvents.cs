using UnityEngine;
using UnityEngine.EventSystems;
public class UiEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Aumenta um pouco a escala do botão ao passar o mouse sobre ele de forma suave
        LeanTween.scale(gameObject, originalScale * 1.1f, 0.2f).setEase(LeanTweenType.easeOutBack);
        //Toca um som ao passar o mouse sobre o botão
        SoundsManager.Instance.PlaySFX(hoverSound, Camera.main.transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Restaura a escala original do botão ao retirar o mouse
        LeanTween.scale(gameObject, originalScale, 0.2f).setEase(LeanTweenType.easeOutBack);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Aumenta um pouco a escala do botão ao passar o mouse sobre ele de forma suave
        LeanTween.scale(gameObject, originalScale * 1.1f, 0.2f).setEase(LeanTweenType.easeOutBack);
        //Toca um som ao passar o mouse sobre o botão
        SoundsManager.Instance.PlaySFX(hoverSound, Camera.main.transform.position);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Restaura a escala original do botão ao retirar o mouse
        LeanTween.scale(gameObject, originalScale, 0.2f).setEase(LeanTweenType.easeOutBack);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Toca um som ao clicar no botão
        SoundsManager.Instance.PlaySFX(clickSound, Camera.main.transform.position);
    }

}
