using System.Collections.Generic;

public class GeneratorWeight
{
    public string id;
    public float weight = 100f;
}

public class RandomCharacterGenerator
{

    public RandomCharacterGenerator()
    {
    }

    public List<GeneratorWeight> weights = new List<GeneratorWeight>();

    public void wipe(bool allSpeciesOn = true)
    {
        weights = new List<GeneratorWeight>();
    }

    public void setWeight(string id, float val)
    {
        for (int w = 0; w < weights.Count; w++)
        {
            if (weights[w].id == id)
            {
                weights[w].weight = val;
                return;
            }
        }
        weights.Add(new GeneratorWeight());
        weights[weights.Count - 1].id = id;
        weights[weights.Count - 1].weight = val;
    }

    public string rollWeights()
    {
        float totalWeight = 0.0f;
        for (int w = 0; w < weights.Count; w++)
        {
            totalWeight += weights[w].weight;
        }
        float roll = UnityEngine.Random.value * totalWeight;
        int rolledWeight = 0;
        while (roll > weights[rolledWeight].weight)
        {
            roll -= weights[rolledWeight].weight;
            rolledWeight++;
        }
        return weights[rolledWeight].id;
    }
}