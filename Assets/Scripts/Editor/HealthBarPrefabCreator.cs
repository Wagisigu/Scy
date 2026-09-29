using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

public class HealthBarPrefabCreator : EditorWindow
{
    [MenuItem("GameObject/UI/Health Bar", false, 10)]
    static void CreateHealthBar()
    {
        // Create health bar root
        GameObject healthBarRoot = new GameObject("HealthBar");
        RectTransform rootRect = healthBarRoot.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(100, 10);

        // Add background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(healthBarRoot.transform, false);
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // Add fill (this is what HealthBarUI animates)
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(healthBarRoot.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        // Select the created object
        Selection.activeGameObject = healthBarRoot;

        Debug.Log("Health Bar created! Drag the root object to Project view to create a prefab.");
    }
}
#endif
