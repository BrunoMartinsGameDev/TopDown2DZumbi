using UnityEngine;
using UnityEngine.EventSystems;
public class UiEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Aumenta um pouco a escala do botão ao passar o mouse sobre ele de forma suave
        LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.2f).setEase(LeanTweenType.easeOutBack);
        //Toca um som ao passar o mouse sobre o botão
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Restaura a escala original do botão ao retirar o mouse
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Aumenta um pouco a escala do botão ao passar o mouse sobre ele de forma suave
        LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.2f).setEase(LeanTweenType.easeOutBack);
        //Toca um som ao passar o mouse sobre o botão
        AudioSource.PlayClipAtPoint(hoverSound, Camera.main.transform.position);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Restaura a escala original do botão ao retirar o mouse
        LeanTween.scale(gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //Toca um som ao clicar no botão
        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
    }

}
