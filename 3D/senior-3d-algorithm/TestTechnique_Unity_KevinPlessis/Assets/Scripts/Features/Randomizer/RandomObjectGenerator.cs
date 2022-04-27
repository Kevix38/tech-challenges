using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RandomObjectGenerator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectToUses;

    [SerializeField]
    private int nbObjectToShow = 30;

    [SerializeField]
    private Vector3 MaxBoxSize = new Vector3(10.0f,10.0f,10.0f);
    [SerializeField]
    private float MaxObjectSize = 2.0f;

    public UnityEvent onfinish;
    // Start is called before the first frame update

    [CanExecuteInEditor]
    public void GenerateRandomObject()
    {
        int i = 0;
        int childs = transform.childCount;
        for (i = childs - 1; i >= 0; i--) 
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject );

        for(i = 0; i < nbObjectToShow; i++)
        {
            MeshRenderer rd = Instantiate(objectToUses[Random.Range(0,objectToUses.Count)],
            Vector3.right * Random.Range(-MaxBoxSize.x*0.5f,MaxBoxSize.x*0.5f) + 
            Vector3.up * Random.Range(-MaxBoxSize.y*0.5f,MaxBoxSize.y*0.5f) + 
            Vector3.forward * Random.Range(-MaxBoxSize.z*0.5f,MaxBoxSize.z*0.5f),
            Quaternion.Euler(Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f),Random.Range(0.0f,360.0f))).GetComponent<MeshRenderer>();

            rd.sharedMaterial.SetColor("_Color",Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            rd.transform.localScale = Vector3.one * Random.Range(1.0f, MaxObjectSize);
            rd.transform.SetParent(transform);
        }
        onfinish?.Invoke();
    }
}
