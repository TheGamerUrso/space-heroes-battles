using UnityEngine;

public class PlaceTurrets : MonoBehaviour
{
    private PlayerShip player;
    public Transform[] TurrentPlaces;
    public GameObject TurretPrefab;
    public int numberOfTurret;
    public GameObject[] Turrents = new GameObject[2];

    public void DeactiveTurret()
    {
        for (int i = 0; i < Turrents.Length; i++)
        {
            Turret Turret = Turrents[i].GetComponent<Turret>();
            Turret.Deactivate();
        }
    }
    public void CreateTurret()
    {
        Turrents = new GameObject[2];
        player = PlayerManager.GetPlayer();
        for (int i = 0; i < TurrentPlaces.Length; i++)
        {
            GameObject Turret = CreateNew(TurrentPlaces[i]);
            Turret.GetComponentInChildren<PlayerWeapon>().SetShipTransform(Turret.transform);
            Turret.transform.SetParent(TurrentPlaces[i]);
            Turret.transform.position = TurrentPlaces[i].position;
            Turret.transform.rotation = TurrentPlaces[i].rotation;
            Turrents[i] = Turret;
        }
    }

    public GameObject CreateNew(Transform parent)
    {
        player = PlayerManager.GetPlayer();
        GameObject tur = Instantiate(TurretPrefab, transform.position, Quaternion.identity);
        return tur;
    }
}