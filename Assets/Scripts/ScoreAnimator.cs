using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.UI;

public class ScoreAnimator : MonoBehaviour
{

    [SerializeField] TMP_Text AddedScoreText;
    [SerializeField] GameObject bonusPrefab;
    [SerializeField] Transform pointParent;
    [SerializeField] RectTransform endPoint;
    [SerializeField] Color green;
    [SerializeField] Color red;

public void AnimateScoreChange(int change) {
    
    var inst = Instantiate(bonusPrefab, Vector3.zero, Quaternion.identity);
    inst.transform.SetParent(pointParent, false);
    
    RectTransform rect = inst.GetComponent<RectTransform>();
    TMP_Text text = inst.GetComponent<TMP_Text>();
    text.text = (change > 0 ? "+ " : "") + change.ToString();
    text.color = change > 0 ? green : red;

    StartCoroutine(MoveAndFade(rect, text));
}

private IEnumerator MoveAndFade(RectTransform rect, TMP_Text text) {
    float duration = 1.25f;
    float elapsed = 0;
    Vector3 startPosition = rect.localPosition;
    Vector3 endPosition = new(rect.localPosition.x - 100, endPoint.anchoredPosition.y, rect.localPosition.z);  

    Color startColor = text.color;
    Color endColor = new(startColor.r, startColor.g, startColor.b, 0); 

    while (elapsed < duration) {
        float t = elapsed / duration;
        rect.localPosition = Vector3.Lerp(startPosition, endPosition, t);
        text.color = Color.Lerp(startColor, endColor, t);
        elapsed += Time.deltaTime;
        yield return null;
    }

    text.color = endColor;
    rect.localPosition = endPosition;
    Destroy(rect.gameObject);
}
}
