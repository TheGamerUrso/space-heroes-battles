using UnityEngine;

public class PlaceTurrets : MonoBehaviour
{
    [SerializeField] private Transform[] TurrentPlaces;
    [SerializeField] private GameObject TurretPrefab;
    private GameObject[] Turrents = new GameObject[2];

    public void DeactiveTurret()
    {
        for (int i = 0; i < Turrents.Length; i++)
        {
            Turret Turret = Turrents[i].GetComponent<Turret>();
            Turret.Deactivate();
        }
    }
    public void DeployTurret()
    {
        for (int i = 0; i < TurrentPlaces.Length; i++)
        {
            GameObject Turret = CreateTurret();
            Turret.transform.SetParent(TurrentPlaces[i]);
            Turret.transform.position = TurrentPlaces[i].position;
            Turret.transform.rotation = TurrentPlaces[i].rotation;
            Turrents[i] = Turret;
        }
    }

    public GameObject CreateTurret()
    {
        GameObject tur = Instantiate(TurretPrefab, transform.position, Quaternion.identity);
        return tur;
    }
}