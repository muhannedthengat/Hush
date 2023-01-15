using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    [SerializeField] private List<Renderer> avatarRenderers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignAvatarMat(Material mat)
    {
        foreach (Renderer _rend in avatarRenderers)
        {
            _rend.material = mat;
        }
    }
}
