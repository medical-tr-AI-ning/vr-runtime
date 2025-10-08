using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
    public class AddMeshCollider : MonoBehaviour
    {
        [SerializeField] private float borderMeter = 0f;
        private float newScaleX;
        private float newScaleZ;

        IEnumerator Start()
        {
            float pSizeX = 0f, pSizeZ = 0f;
            var chaperone = OpenVR.Chaperone;
            chaperone.GetPlayAreaSize(ref pSizeX, ref pSizeZ);
            Debug.Log("x: " + pSizeX + " z: " + pSizeZ);

            while (pSizeX == 0f || pSizeZ == 0f)
            {
                yield return new WaitForSeconds(1);
                chaperone.GetPlayAreaSize(ref pSizeX, ref pSizeZ);
                Debug.Log("x: " + pSizeX + " z: " + pSizeZ);
            }

            Rescale(pSizeX, pSizeZ);

            AddCollider();
        }


        private void Rescale(float pSizeX, float pSizeZ)
        {
            borderMeter = borderMeter * 2;
            newScaleX = (pSizeX - borderMeter) / pSizeX;
            newScaleZ = (pSizeZ - borderMeter) / pSizeZ;
            gameObject.transform.localScale = new Vector3(transform.localScale.x * newScaleX, transform.localScale.y, transform.localScale.z * newScaleZ);
        }

        private void AddCollider()
        {
            MeshCollider sc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
            sc.convex = true;
        }
    }
}