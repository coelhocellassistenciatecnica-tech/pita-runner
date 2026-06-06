using UnityEngine;
using System.Collections.Generic;

namespace PitaRunner.Portals
{
    public class PortalSpawner : MonoBehaviour
    {
        [Header("Portal Prefab")]
        [SerializeField] private Portal portalPrefab;

        [Header("Spawn Config")]
        [SerializeField] private List<ScriptableObjects.PortalData> portalDataList;
        [SerializeField] private float trackWidth = 8f;

        public void SpawnPortalPairAt(float z, ScriptableObjects.PortalData leftData, ScriptableObjects.PortalData rightData)
        {
            float xLeft = -trackWidth * 0.25f;
            float xRight = trackWidth * 0.25f;

            SpawnPortalFrom(new Vector3(xLeft, 0.5f, z), leftData);
            SpawnPortalFrom(new Vector3(xRight, 0.5f, z), rightData);
        }

        private void SpawnPortalFrom(Vector3 position, ScriptableObjects.PortalData data)
        {
            if (portalPrefab == null || data == null) return;
            var p = Instantiate(portalPrefab, position, Quaternion.identity, transform);
            p.Configure(data.portalType, data.multiplierValue, data.addValue, data.isPositive);
        }
    }
}
