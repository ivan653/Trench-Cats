using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResearchTabController : MonoBehaviour
{
    public static ResearchTabController Instance;
    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private VisualTreeAsset researchElementAsset;
    private VisualElement techTree;

    
    [SerializeField] private List<TechNodeSO> researches = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Підписуємося на оновлення та побудову UI дерева
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        //endLabel = root.Q<Label>("EndText");
        //menuButton = root.Q<Button>("MenuButton");

        techTree = root.Q<VisualElement>("TechTree");

        techTree.AddManipulator(new CustomManipulator());

        DrawTechTree();
    }

    private void DrawTechTree()
    {
        
        
        for (int i = 0; i < researches.Count; i++)
        {
            VisualElement nodeElement = researchElementAsset.Instantiate();
        
            // 2. Додавання стилю та абсолютного позиціонування
            nodeElement.AddToClassList("tech-node");
            nodeElement.style.position = Position.Absolute;

            // 3. Задання координат
            nodeElement.style.left = 200 * i;
            nodeElement.style.top = 200 * i;

            // 4. Додавання в контейнер
            techTree.Add(nodeElement);
        }

        SimpleLine simpleLine = new SimpleLine();
        simpleLine.style.position = Position.Absolute;
        simpleLine.style.width = Length.Percent(100);
        simpleLine.style.height = Length.Percent(100);
        simpleLine.UpdatePoints(new Vector2(10, 10), new Vector2(300, 200));
        techTree.Add(simpleLine);

    }

}