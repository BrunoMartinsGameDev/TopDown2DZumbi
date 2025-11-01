using UnityEngine;

public class MoveTextUp : MonoBehaviour
{
    public float timeInSeconds = 60f;    // Duração da animação

    public float distance = 5000f; // Distância que o texto irá se mover para cima

    private Vector3 initialLocalPos;

    void Awake()
    {
        initialLocalPos = transform.localPosition;
    }

    void OnEnable()
    {
        LeanTween.moveLocalY(gameObject, initialLocalPos.y + distance, timeInSeconds).setEase(LeanTweenType.once);
    }
    void OnDisable()
    {
        LeanTween.cancel(gameObject);
        transform.localPosition = initialLocalPos;
    }

}