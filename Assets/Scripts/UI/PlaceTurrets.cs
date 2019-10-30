using UnityEngine;

public class PlaceTurrets : MonoBehaviour
{
    public ShipStats shipStats;
    public Transform[] TurrentPlaces;
    public GameObject TurretPrefab;
    public int numberOfTurret;
    public GameObject[] Turrents = new GameObject[2];

    public void CreateTurret()
    {
        Player player = PlayerManager.GetPlayer();

        for (int i = 0; i < TurrentPlaces.Length; i++)
        {
            if (TurrentPlaces[i].childCount == 0)
            {
                GameObject Turret = CreateNew(TurrentPlaces[i]);
                Turret.GetComponentInChildren<PlayerWeapon>().SetPlayerAnimation(player.PlayerAnimation());
                Turret.GetComponentInChildren<PlayerWeapon>().SetShipStatsSystem(player.GetShipStatsSystem());
                Turret.GetComponentInChildren<PlayerWeapon>().SetShipTransform(Turret.transform);
                Turret.transform.SetParent(TurrentPlaces[i]);
                Turret.transform.position = TurrentPlaces[i].position;
                Turret.transform.rotation = TurrentPlaces[i].rotation;
                Turrents[i] = Turret;
            }
        }
    }

    public GameObject CreateNew(Transform parent)
    {
        GameObject tur = Instantiate(TurretPrefab, transform.position, Quaternion.identity);

        tur.GetComponent<Turret>().SetShipStats(shipStats);
        return tur;
    }
}