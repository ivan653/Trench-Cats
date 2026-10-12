using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TechManager : NetworkBehaviour
{
    public static TechManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private List<TechNodeSO> technologiesSO;
    [SerializeField] private List<TechNode> technologies = new();
    [SerializeField] private TechNode currentTechnology;
    [SerializeField] private float techPoints = 1;
    [SerializeField] private float researchTimer = 0;

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        UpdateTechTree(technologies);
    }*/

    void Start()
    {
        for (int i = 0; i < technologiesSO.Count; i++)
        {
            if (technologiesSO[i] != null)
            {
                technologies.Add(new TechNode(technologiesSO[i]));
            }
        }

        UpdateTechTree(technologies);
    }


    private void InitializeTechTree()
    {
        
    }

    private void UpdateTechTree(List<TechNode> techTree)
    {
        foreach (var tech in techTree)
        {
            if(tech.prerequisites.Count == 0 && tech.status != TechStatus.Researched)
            {
                tech.status = TechStatus.Available;
            }
            else
            {
                for(int i = 0; i < tech.prerequisites.Count; i++)
                {
                    if(tech.prerequisites[i].status != TechStatus.Researched)
                    {
                        tech.status = TechStatus.Locked;
                        break;
                    }

                    tech.status = TechStatus.Available;
                }
            }
            
        }

        ResearchTabController.Instance.DrawTechTree(technologies);
    }



    public void SelectTechnology(TechNode technology)
    {
        currentTechnology = technology;
        StartCoroutine(Research());
    }

    IEnumerator Research()
    {
        var wait = new WaitForSeconds(1f);

        while (researchTimer < currentTechnology.cost)
        {
            yield return wait;
            
            researchTimer += techPoints;
            ResearchTabController.Instance.UpdateTechNodeProgressBar(currentTechnology.id, researchTimer);
        }

        Debug.Log("R");
        currentTechnology.status = TechStatus.Researched;
        UpdateTechTree(technologies);

        researchTimer = 0;
    }
}