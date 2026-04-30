using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PrepareUI : Singleton<PrepareUI> {
    public float actionSpeed = 2f;          // duration for scale/color anim
    public float holdTime = 0.5f;           // time to hold each sprite after anim
    public Image image;
    public List<Sprite> sprites;            // 3, 2, 1, Fighting
    public Sprite fighting;
    private void Reset() {
        image = GetComponent<Image>();
    }

    public IEnumerator Act() {
        gameObject.SetActive(true);
        for(int i = 0; i < sprites.Count; ++i) {
            var sprite = sprites[i];
            // set new sprite
            image.sprite = sprite;

            // reset start state
            image.color = new Color(1f, 1f, 1f, 0f);
            image.transform.localScale = Vector3.one * 2f;

            float t = 0f;
            while(t < 1f) {
                t += Time.deltaTime * actionSpeed;

                // animate alpha
                var c = image.color;
                c.a = Mathf.Lerp(0f, 1f, t);
                image.color = c;

                // animate scale
                image.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one, t);

                yield return null;
            }

            // small pause before next sprite
            yield return new WaitForSeconds(holdTime);
        }
    }
    public IEnumerator Fighting() {
        image.sprite = fighting;
        yield return new WaitForSeconds(0.75f);
        this.gameObject.SetActive(false);
    }
}
