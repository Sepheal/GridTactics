using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSetters : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            Material Mat = tile.GetComponent<MeshRenderer>().material;
            Mat.mainTextureScale = new Vector2(0.2f * tile.transform.localScale.x, 0.2f * tile.transform.localScale.z);
        }
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("FakeTile"))
        {
            Material Mat = tile.GetComponent<MeshRenderer>().material;
            Mat.mainTextureScale = new Vector2(0.2f * tile.transform.localScale.x, 0.2f * tile.transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
