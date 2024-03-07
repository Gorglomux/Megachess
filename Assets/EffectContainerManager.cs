using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectContainerManager : MonoBehaviour
{
    public GameObject effectContainerTextPrefab;

    public EffectContainer[] EffectContainers;

    public int pooledEffectContainerCount = 10;
    public int currentEffectContainerCount = 0;

    public List<EffectContainer> activeContainers;
    // Start is called before the first frame update
    void Start()
    {
        print("Start pooling");
        EffectContainers = new EffectContainer[pooledEffectContainerCount];
        for (int i = 0; i < pooledEffectContainerCount; i++)
        {
            EffectContainers[i] = GameObject.Instantiate(effectContainerTextPrefab, this.transform).GetComponent<EffectContainer>();
            EffectContainers[i].gameObject.SetActive(false);
        }
        print("Pooling done !");
    }

    public EffectContainer getNext()
    {
        currentEffectContainerCount = (currentEffectContainerCount + 1) % pooledEffectContainerCount;
        EffectContainer effectContainer = EffectContainers[currentEffectContainerCount];
        effectContainer.transform.SetParent(transform);
        effectContainer.transform.SetAsLastSibling();
        effectContainer.gameObject.SetActive(true);

        activeContainers.Add(effectContainer);
        return EffectContainers[currentEffectContainerCount];
    }


    public void DisableContainers()
    {
        foreach(EffectContainer container in activeContainers)
        {
            container.gameObject.SetActive(false);
        }
        activeContainers.Clear();
    }


}
