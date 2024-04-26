using UnityEngine;


public class IbitsuMovement : MonoBehaviour
{
    [SerializeField] KatamariMovement katamariMovement;
    [SerializeField] SphereCollider sphereCollider; //塊オブジェクトのコライダーをとってくる
    [SerializeField] Transform katamariRigidbody; //塊オブジェクトのtransformをとってくる
    [SerializeField] Transform doMakikomi; //巻き込み判定用のsphere

    private float moveSpeed = 5f;

    public Vector3[] GetBoxColliderVertices(BoxCollider boxCollider)
    {
        Vector3 boxSize = boxCollider.size;
        Vector3 boxCenter = boxCollider.center;

        Vector3[] vertices = new Vector3[8];

        vertices[0] = boxCenter + new Vector3(boxSize.x, boxSize.y, boxSize.z) * 0.5f;
        vertices[1] = boxCenter + new Vector3(boxSize.x, boxSize.y, -boxSize.z) * 0.5f;
        vertices[2] = boxCenter + new Vector3(boxSize.x, -boxSize.y, boxSize.z) * 0.5f;
        vertices[3] = boxCenter + new Vector3(boxSize.x, -boxSize.y, -boxSize.z) * 0.5f;
        vertices[4] = boxCenter + new Vector3(-boxSize.x, boxSize.y, boxSize.z) * 0.5f;
        vertices[5] = boxCenter + new Vector3(-boxSize.x, boxSize.y, -boxSize.z) * 0.5f;
        vertices[6] = boxCenter + new Vector3(-boxSize.x, -boxSize.y, boxSize.z) * 0.5f;
        vertices[7] = boxCenter + new Vector3(-boxSize.x, -boxSize.y, -boxSize.z) * 0.5f;

        //Debug.Log(vertices[1]);

        return vertices; //モノのローカル座標で表された、８頂点の座標（これは塊が転がっても不変な値）

    }

    public void IbitsuMove(Vector3[] vertices, Transform monoTransform)
    {
        Vector3[] worldVertices = new Vector3[8];

        //モノのローカル座標をワールド座標に変換する
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = monoTransform.TransformPoint(vertices[i]);
        }

        //頂点の中で一番y座標が低いものを探す
        Vector3 lowestVertex = worldVertices[0];
        for (int i = 1; i < worldVertices.Length; i++)
        {
            if (worldVertices[i].y < lowestVertex.y)
            {
                lowestVertex = worldVertices[i];
            }
        }


        Vector3 katamariLowestPoint = katamariRigidbody.position - sphereCollider.radius * Vector3.up; //塊の最も低い点

        //Debug.Log("lowestVertex.y: " + lowestVertex.y + " , katamariLowestPoint.y: " + katamariLowestPoint.y);

        //塊をいびつなモノの分だけ浮かせる
        Vector3 targetPosition;
        if (lowestVertex.y < katamariLowestPoint.y) //もし塊の最も低い点より頂点が下にいたら
        {
            targetPosition = katamariRigidbody.position + Vector3.up * (katamariLowestPoint.y - lowestVertex.y);
            //Debug.Log("Delta up. Up ratio: " + (lowestVertex.y - katamariLowestPoint.y) / sphereCollider.radius);
        }
        else
        {
           targetPosition = katamariRigidbody.position;
        }
        doMakikomi.position = Vector3.Lerp(doMakikomi.position, targetPosition, moveSpeed * Time.deltaTime);
    }


    //private void GetIbitusVertics()
    //{
    //    if(katamariMovement.ibitsuCollider != null)
    //    {
    //        BoxCollider boxCollider = katamariMovement.ibitsuCollider.gameObject.GetComponent<BoxCollider>();
    //        Transform transform = katamariMovement.ibitsuCollider.gameObject.GetComponent<Transform>();

    //        Vector3 boxSize = boxCollider.size;
    //        Vector3 boxCenter = boxCollider.center;

    //        Vector3[] vertices = new Vector3[8];

    //        vertices[0] = boxCenter + new Vector3(boxSize.x, boxSize.y, boxSize.z) * 0.5f;
    //        vertices[1] = boxCenter + new Vector3(boxSize.x, boxSize.y, -boxSize.z) * 0.5f;
    //        vertices[2] = boxCenter + new Vector3(boxSize.x, -boxSize.y, boxSize.z) * 0.5f;
    //        vertices[3] = boxCenter + new Vector3(boxSize.x, -boxSize.y, -boxSize.z) * 0.5f;
    //        vertices[4] = boxCenter + new Vector3(-boxSize.x, boxSize.y, boxSize.z) * 0.5f;
    //        vertices[5] = boxCenter + new Vector3(-boxSize.x, boxSize.y, -boxSize.z) * 0.5f;
    //        vertices[6] = boxCenter + new Vector3(-boxSize.x, -boxSize.y, boxSize.z) * 0.5f;
    //        vertices[7] = boxCenter + new Vector3(-boxSize.x, -boxSize.y, -boxSize.z) * 0.5f;

    //        //モノのローカル座標をワールド座標に変換する
    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            vertices[i] = transform.TransformPoint(vertices[i]);
    //        }

    //        //頂点の中で一番y座標が低いものを探す
    //        Vector3 lowestVertex = vertices[0];
    //        for (int i = 1; i < vertices.Length; i++)
    //        {
    //            if (vertices[i].y < lowestVertex.y)
    //            {
    //                lowestVertex = vertices[i];
    //            }
    //        }

    //        Vector3 katamariLowestPoint = sphereCollider.center - sphereCollider.radius * Vector3.up; //塊の最も低い点

    //        //塊をいびつなモノの分だけ浮かせる
    //        if (lowestVertex.y < katamariLowestPoint.y) //もし塊の最も低い点より頂点が下にいたら
    //        {
    //            katamariTransform.position = katamariTransform.position + Vector3.up * (lowestVertex.y - katamariLowestPoint.y);
    //        }

    //    }


    //}



}
