using UnityEngine;

public class SetAlphaOfChildren : MonoBehaviour
{

    public void SetAlpha(float alphaValue) {
        SpriteRenderer[] children = this.GetComponentsInChildren<SpriteRenderer>();
        Color newColor;
        foreach (SpriteRenderer child in children) {
            newColor = child.color;
            newColor.a = alphaValue;
            child.color = newColor;
        }
    }

}
