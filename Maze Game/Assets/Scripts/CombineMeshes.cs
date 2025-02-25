using UnityEngine;
using System;
using System.Collections;

public class CombineMeshes : MonoBehaviour {

    public bool renderMeshCollider = true;
    private MazeGenerator MazeGenerator;


    void Start(){
        MazeGenerator = GameObject.FindWithTag("MazeGenerator").GetComponent<MazeGenerator>();
        if (MazeGenerator.enableMeshCombining) Combine();
    }

 



    public GameObject[] Objects;
        
    [ContextMenu("Combine")]
    public void Combine(){
        Quaternion oldRot = transform.rotation;
        Vector3 oldPos = transform.position;
        Vector3 oldScl = new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z);

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;


        // Find all mesh filter submeshes and separate them by their cooresponding materials
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        
        foreach( GameObject obj in Objects ) 
        {
            if(!obj)
                continue;
            
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
             
            foreach( MeshFilter meshFilter in meshFilters )
            {
                MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                
                // Handle bad input
                if(!meshRenderer) { 
                    Debug.LogError("MeshFilter does not have a coresponding MeshRenderer."); 
                    continue; 
                }
                if(meshRenderer.materials.Length != meshFilter.sharedMesh.subMeshCount) { 
                    Debug.LogError("Mismatch between material count and submesh count. Is this the correct MeshRenderer?"); 
                    continue; 
                }
                
                for(int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
                {
                    int materialArrayIndex = 0;
                    for(materialArrayIndex = 0; materialArrayIndex < materials.Count; materialArrayIndex++)
                    {
                        if(materials[materialArrayIndex] == meshRenderer.sharedMaterials[s])
                            break;
                    }
                    
                    if(materialArrayIndex == materials.Count)
                    {
                        materials.Add(meshRenderer.sharedMaterials[s]);
                        combineInstanceArrays.Add(new ArrayList());
                    }
                    
                    CombineInstance combineInstance = new CombineInstance();
                    combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                    combineInstance.subMeshIndex = s;
                    combineInstance.mesh = meshFilter.sharedMesh;
                    (combineInstanceArrays[materialArrayIndex] as ArrayList).Add( combineInstance );
                }
            }
        }
        
        // For MeshFilter
        {
            // Get / Create mesh filter
            MeshFilter meshFilterCombine = gameObject.GetComponent<MeshFilter>();
            if(!meshFilterCombine)
                meshFilterCombine = gameObject.AddComponent<MeshFilter>();
            
            // Combine by material index into per-material meshes
            // also, Create CombineInstance array for next step
            Mesh[] meshes = new Mesh[materials.Count];
            CombineInstance[] combineInstances = new CombineInstance[materials.Count];
            
            for( int m = 0; m < materials.Count; m++ )
            {
                CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
                meshes[m] = new Mesh();
                meshes[m].CombineMeshes( combineInstanceArray, true, true );
                
                combineInstances[m] = new CombineInstance();
                combineInstances[m].mesh = meshes[m];
                combineInstances[m].subMeshIndex = 0;
            }
            
            // Combine into one
            meshFilterCombine.sharedMesh = new Mesh();
            meshFilterCombine.sharedMesh.CombineMeshes( combineInstances, false, false );
            
            if (renderMeshCollider){
                // create mesh collider
                MeshCollider meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                meshc.sharedMesh = meshFilterCombine.sharedMesh; // assign mesh
            }
            
            // Destroy other meshes
            foreach( Mesh mesh in meshes )
            {
                mesh.Clear();
                DestroyImmediate(mesh);
            }
        }
        
        // For MeshRenderer
        {
            // Get / Create mesh renderer
            MeshRenderer meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
            if(!meshRendererCombine)
                meshRendererCombine = gameObject.AddComponent<MeshRenderer>();    
            
            // Assign materials
            Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
            meshRendererCombine.materials = materialsArray;    
        }

        transform.rotation = oldRot;
        transform.position = oldPos;
        transform.localScale = oldScl;
        foreach (Transform child in gameObject.transform) GameObject.Destroy(child.gameObject);

        if (MazeGenerator.enableCulling){
            gameObject.GetComponent<Renderer>().enabled = false;    // Disable renderer

            foreach(Transform tr in gameObject.transform.parent){
                if(tr.tag == "OccludableOther"){
                    tr.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}


