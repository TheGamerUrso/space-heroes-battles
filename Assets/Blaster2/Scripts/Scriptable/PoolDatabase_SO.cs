using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolDatabase", menuName = "Systems/Pool Database")]
public class PoolDatabase : ScriptableObject
{
    public string categoryName = "Category Name"; // e.g., "Enemies" or "UI"
    public List<PoolElement> poolElements = new List<PoolElement>();
}