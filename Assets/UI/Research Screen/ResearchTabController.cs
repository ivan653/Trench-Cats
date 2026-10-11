using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResearchTabController : MonoBehaviour
{
    public static ResearchTabController Instance;

    [SerializeField] private PanelRenderer panelRenderer;
    [SerializeField] private VisualTreeAsset researchElementAsset;

    [Header("Grid Configuration")]
    [SerializeField] private float nodeWidth = 160f;
    [SerializeField] private float nodeHeight = 200f;
    [SerializeField] private float spacingX = 80f;
    [SerializeField] private float spacingY = 40f;

    private VisualElement techTree;
    private VisualElement linesContainer;

    private float StepX => nodeWidth + spacingX;
    private float StepY => nodeHeight + spacingY;

    // Кеш стану та черги
    private bool _isUIReloaded = false;
    private List<TechNode> _cachedTechnologies;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
    {
        techTree = root.Q<VisualElement>("TechTree");
        techTree.Clear();

        techTree.AddManipulator(new CustomManipulator());

        _isUIReloaded = true;

        // Якщо ззовні вже надходив запит на малювання — виконуємо його зараз
        if (_cachedTechnologies != null)
        {
            ExecuteDrawTechTree(_cachedTechnologies);
        }
    }

    /// <summary>
    /// Безпечний публічний вхід для зовнішніх скриптів
    /// </summary>
    public void DrawTechTree(List<TechNode> technologies)
    {   
        // Зберігаємо актуальні дані
                _cachedTechnologies = technologies;

        // Якщо верстка ще не готова — чекаємо на OnUIReload
        if (!_isUIReloaded || techTree == null)
        {
            return;
        }

        ExecuteDrawTechTree(_cachedTechnologies);
    }

    private void ExecuteDrawTechTree(List<TechNode> technologies)
    {
        if (technologies == null) return;

        techTree.Clear();

        // 1. Окремий фоновий шар для всіх ліній зв'язку
        linesContainer = new VisualElement();
        linesContainer.pickingMode = PickingMode.Ignore;
        linesContainer.style.position = Position.Absolute;
        linesContainer.style.width = Length.Percent(100);
        linesContainer.style.height = Length.Percent(100);
        techTree.Add(linesContainer);

        // 2. Генерація зв'язків
        DrawConnections(technologies);

        // 3. Генерація нодів
        DrawNodes(technologies);
    }

    private void DrawNodes(List<TechNode> technologies)
    {
        for (int i = 0; i < technologies.Count; i++)
        {
            var tech = technologies[i];
            VisualElement nodeElement = researchElementAsset.Instantiate();

            nodeElement.style.position = Position.Absolute;
            nodeElement.style.width = nodeWidth;
            nodeElement.style.height = nodeHeight;

            Vector2 pixelPos = GridToPixelPosition(tech.gridPosition);
            nodeElement.style.left = pixelPos.x;
            nodeElement.style.top = pixelPos.y;
            
            if (tech.status == TechStatus.Locked)
            {
                VisualElement icon = nodeElement.Q<VisualElement>("ActvePanel");
                if (icon != null)
                {
                    icon.style.opacity = 0.3f;
                }
            }

            techTree.Add(nodeElement);
        }
    }

    private void DrawConnections(List<TechNode> technologies)
    {
        for (int i = 0; i < technologies.Count; i++)
        {
            var childTech = technologies[i];
            if (childTech.prerequisites == null) continue;

            Vector2 endPoint = GetNodeCenterPin(childTech.gridPosition);

            for (int p = 0; p < childTech.prerequisites.Count; p++)
            {
                var parentTech = childTech.prerequisites[p];
                if (parentTech == null) continue;

                Vector2 startPoint = GetNodeCenterPin(parentTech.gridPosition);

                SimpleLine line = new SimpleLine();
                line.style.position = Position.Absolute;
                line.style.width = Length.Percent(100);
                line.style.height = Length.Percent(100);

                line.UpdatePoints(startPoint, endPoint);
                linesContainer.Add(line);
            }
        }
    }

    private Vector2 GridToPixelPosition(Vector2Int grid)
    {
        return new Vector2(grid.x * StepX, grid.y * StepY);
    }

    private Vector2 GetNodeRightPin(Vector2Int grid)
    {
        Vector2 topLeft = GridToPixelPosition(grid);
        return new Vector2(topLeft.x + nodeWidth, topLeft.y + (nodeHeight * 0.5f));
    }

    private Vector2 GetNodeCenterPin(Vector2Int grid)
    {
        Vector2 topLeft = GridToPixelPosition(grid);
        return new Vector2(topLeft.x + nodeWidth * 0.5f, topLeft.y + (nodeHeight * 0.5f));
    }

    private Vector2 GetNodeLeftPin(Vector2Int grid)
    {
        Vector2 topLeft = GridToPixelPosition(grid);
        return new Vector2(topLeft.x, topLeft.y + (nodeHeight * 0.5f));
    }
}