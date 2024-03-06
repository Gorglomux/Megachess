using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatManager : MonoBehaviour
{
    public List<GameObject> splats;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Cleanup()
    {
        foreach(GameObject splat in splats)
        {
            splat.transform.DOScale(Vector3.zero, 0.5f).onComplete += () =>
            {
                Destroy(splat);

            };

        }
        splats.Clear();
    }
}
