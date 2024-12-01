using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskUI : MaskableGraphic
{
    // Start is called before the first frame update
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        
        vh.Clear();
        Vector3 vec_00 = new Vector3(0, 0);
        Vector3 vec_01 = new Vector3(0, 150);
        Vector3 vec_10 = new Vector3(150, 0);
        Vector3 vec_11 = new Vector3(150, 150);
        
        vh.AddUIVertexQuad(new UIVertex[]
        {
            new UIVertex() {position = vec_00, color = Color.green},
            new UIVertex() {position = vec_01, color = Color.green},
            new UIVertex() {position = vec_10, color = Color.green},
            new UIVertex() {position = vec_11, color = Color.green}
        });
    }
}
