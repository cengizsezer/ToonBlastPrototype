using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIEffectManager : MonoSingleton<UIEffectManager>
{
    public enum CanvasLayer
    {
        OverGridUnderUI,
        OverEverything,
    }

    [BHeader("References")]
    [SerializeField] private RectTransform onGridUnderUiLayerParent;
    [SerializeField] private RectTransform overEverythingLayerParent;
    [BHeader("Point References")]
    [SerializeField] private List<RectTransform> pointReferenceRects = new List<RectTransform>();
  
    public void CreateCurvyFlyingSprite(Sprite sprite, Vector2 spriteSize, Vector2 spawnPos, Vector2 targetPos, CanvasLayer layer, Action onComplete = null)
    {
        GameObject flyingObj = PoolManager.I.GetObject<FlyingSprite>().gameObject;
        Debug.Log(flyingObj);
        flyingObj.transform.position = spawnPos;
        Debug.Log(overEverythingLayerParent);
        flyingObj.transform.SetParent(overEverythingLayerParent);
        flyingObj.GetComponent<RectTransform>().sizeDelta = spriteSize;
        flyingObj.GetComponent<Image>().sprite = sprite;
        Tween flyingTween = TweenHelper.CurvingMoveTo(flyingObj.transform, targetPos, onComplete, 1f, .2f, Ease.InOutCubic, Ease.InBack);
        flyingTween.onComplete += () => flyingObj.GetComponent<PoolObject>().OnDeactive();
    }

    public void CreateLinearFlyingSprite(Sprite sprite, Vector2 spriteSize, Vector2 spawnPos, Vector2 targetPos, CanvasLayer layer, Action onComplete = null)
    {
        GameObject flyingObj = PoolManager.I.GetObject<FlyingSprite>().gameObject;
        flyingObj.transform.position = spawnPos;
        flyingObj.transform.SetParent(GetLayerParent(layer));
        flyingObj.GetComponent<RectTransform>().sizeDelta = spriteSize;
        flyingObj.GetComponent<Image>().sprite = sprite;
        Tween flyingTween = TweenHelper.LinearMoveTo(flyingObj.transform, targetPos, onComplete);
        flyingTween.onComplete += () => flyingObj.GetComponent<PoolObject>().OnDeactive();
    }

    public void CreatePassingByFlyingText(string text, float fontSize, Vector2 spawnPos, Vector2 waitingPos, Vector2 targetPos, CanvasLayer layer, float moveDuration, float waitDuration, Action onComplete = null)
    {
        GameObject flyingObj = PoolManager.I.GetObject<FlyingSprite>().gameObject;
        flyingObj.transform.position = spawnPos;
        flyingObj.transform.SetParent(GetLayerParent(layer));
        TMPro.TMP_Text textComponent = flyingObj.GetComponent<TMPro.TMP_Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        Tween flyingTween = TweenHelper.PassingBy(flyingObj.transform, spawnPos, waitingPos, targetPos, moveDuration, waitDuration, onComplete);
        flyingTween.onComplete += () => flyingObj.GetComponent<PoolObject>().OnDeactive();
    }

    public RectTransform GetLayerParent(CanvasLayer layer)
    {
        switch (layer)
        {
            case CanvasLayer.OverGridUnderUI:
                return onGridUnderUiLayerParent;
            case CanvasLayer.OverEverything:
                return overEverythingLayerParent;
            default:
                Debug.LogWarning("Unknown CanvasLayer: " + layer);
                return null;
        }
    }

    public Vector2 GetReferencePointByName(string referenceName)
    {
        foreach (RectTransform rect in pointReferenceRects)
        {
            if (rect.name != referenceName) continue;
            return rect.position;
        }
        Debug.LogError("Reference point with Name not found: " + referenceName);
        return Vector2.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (RectTransform rect in pointReferenceRects)
        {
            if (rect == null) continue;
            Gizmos.DrawSphere(rect.position, 50f);
            DrawString(rect.name, rect.position);
        }
    }

    public void DrawString(string text, Vector3 worldPos, Color? textColor = null)
    {
        if (textColor.HasValue) UnityEditor.Handles.color = textColor.Value;
        UnityEditor.Handles.Label(worldPos, text);

    }
#endif
}
