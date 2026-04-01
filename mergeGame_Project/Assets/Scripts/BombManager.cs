using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance { get; private set; }
    public static bool HasInstance => Instance != null;

    private readonly List<Bomb> activeBombs = new List<Bomb>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterBomb(Bomb bomb)
    {
        if (!activeBombs.Contains(bomb))
            activeBombs.Add(bomb);
    }

    public void UnregisterBomb(Bomb bomb)
    {
        activeBombs.Remove(bomb);
    }

    public void DetonateAll()
    {
        Bomb[] bombs = activeBombs.ToArray();

        foreach (Bomb bomb in bombs)
        {
            if (bomb != null)
                bomb.Explode();
        }
    }
}