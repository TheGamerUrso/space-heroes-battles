using System.Collections.Generic;

[System.Serializable]
public class ObjectiveListData
{
    public List<ObjectiveData> ListOfOnGoingObjectives;

    public ObjectiveListData(List<ObjectiveData> Missions)
    {
        ListOfOnGoingObjectives = Missions;
    }
}
