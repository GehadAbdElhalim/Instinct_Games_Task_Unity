using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{

    public int xSize, ySize, cellScale;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake()
    {
        Generate();
        //StartCoroutine(Generate2());
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        //for (int i = 0, y = 0; y <= ySize; y++) {
        //	for (int x = 0; x <= xSize; x++, i++) {
        //		vertices[i] = new Vector3(x, transform.position.y, y);
        //		uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
        //		tangents[i] = tangent;
        //	}
        //}
        int index = 0;

        for (int i = 0; i <= xSize; i++)
        {
            for (int j = 0; j <= ySize; j++)
            {
                float X = -((xSize - 1) / 2.0f) + i;
                X *= cellScale;
                X -= cellScale / 2.0f;
                float Y = -((ySize - 1) / 2.0f) + j;
                Y *= cellScale;
                Y -= cellScale / 2.0f;

                vertices[index + j] = new Vector3(Y, 0, X);
                print(vertices[index + j]);
                uv[index + j] = new Vector2((float)Y / ySize, (float)X / xSize);
                tangents[index + j] = tangent;
            }
            index += ySize + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private IEnumerator Generate2()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        int index = 0;

        for (int i = 0; i <= xSize; i++)
        {
            for (int j = 0; j <= ySize; j++)
            {
                float X = -((xSize - 1) / 2.0f) + i;
                X *= cellScale;
                X -= cellScale / 2.0f;
                float Y = -((ySize - 1) / 2.0f) + j;
                Y *= cellScale;
                Y -= cellScale / 2.0f;

                vertices[index + j] = new Vector3(Y, 0, X);
                print(vertices[index + j]);
                print(index + j);
                yield return wait;
            }
            index += ySize + 1;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.black;
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Gizmos.DrawSphere(vertices[i], 0.1f);
        //}
    }
}