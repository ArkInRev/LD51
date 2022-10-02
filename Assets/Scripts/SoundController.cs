using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private SphereCollider sc;
    private MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onTick += gameTick;
        sc = GetComponentInChildren<SphereCollider>();
        mr = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void gameTick()
    {
        this.transform.localScale -= Vector3.one;
        if (this.transform.localScale.x <= 0.5f) {
            this.transform.position = new Vector3(0f, -1000f, 0f);
            mr.enabled = false;
            sc.enabled = false;
            Destroy(this.gameObject, 3);

        }
    }











    public void OnDisable()
    {
        GameManager.Instance.onTick -= gameTick;
    }
}
