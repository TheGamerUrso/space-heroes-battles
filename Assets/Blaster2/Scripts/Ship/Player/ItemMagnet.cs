using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemMagnet : MonoBehaviour
{
    public PlayerShip playerShip;
    public PlayerShipData playerShipData;
    [SerializeField] private LayerMask itemLayer;
    private HashSet<IPickable> activePulledItems = new HashSet<IPickable>();


    private void Start()
    {
        playerShipData = playerShip.GetShipData<PlayerShipData>();
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, playerShipData.MagnetDistance, itemLayer);

        HashSet<IPickable> currentFrameItems = new HashSet<IPickable>();
        foreach (var col in hitColliders)
        {
            if (col.TryGetComponent<IPickable>(out IPickable pickable))
            {
                currentFrameItems.Add(pickable);

                if (!activePulledItems.Contains(pickable))
                {
                    pickable.IsPulled = true;
                    activePulledItems.Add(pickable);
                }

                // Move item toward player position
                if (col.TryGetComponent<Transform>(out Transform itemTransform))
                {
                    itemTransform.position = Vector3.MoveTowards(
                        itemTransform.position,
                        transform.position,
                        playerShipData.MagnetPower * Time.deltaTime
                    );
                }
            }

            // 2. Check items that left the radius this frame
            List<IPickable> itemsToRemove = new List<IPickable>();
            foreach (var pulledItem in activePulledItems)
            {
                if (!currentFrameItems.Contains(pulledItem))
                {
                    pulledItem.IsPulled = false;
                    itemsToRemove.Add(pulledItem);
                }
            }

            // Clean up tracking list
            foreach (var item in itemsToRemove)
            {
                activePulledItems.Remove(item);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerShipData.MagnetDistance);
    }
}
