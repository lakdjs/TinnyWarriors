using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ghost;
    private GameObject ghostRef;
   public void SpawnGhost(Vector3 pos)
    {
       ghostRef = Instantiate(ghost, new Vector3( pos.x, pos.y + 0.5f, pos.z), Quaternion.identity);
       // Debug.Log(pos);
        StartCoroutine(DestroyGhost());
    }
    IEnumerator DestroyGhost()
    {
        yield return new WaitForSeconds(1.15f);
        Destroy(ghostRef.gameObject);
        ghostRef = null;
    }
}
