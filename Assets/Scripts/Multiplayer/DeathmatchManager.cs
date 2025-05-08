using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DeathmatchManager : MonoBehaviour
{
    [Header("Match Settings")]
    public int minWinScore = 10;
    public Vector3[] worldSpawnPoints;

    [Header("Player Settings")]
    [SerializeField] internal bool quickLooking;
    [SerializeField] protected float labelSize = 250f;
    [HideInInspector] public BasePlayer winner;
    protected InputAction quickLookAction;

    public List<BasePlayer> players = new();

    private void Start()
    {
        quickLookAction = InputSystem.actions.FindAction("Quick Look");
    }

    private void Update()
    {
        if (quickLookAction != null && quickLookAction.IsPressed()) quickLooking = true;
        else quickLooking = false;

        if (winner = null) CheckForWinner(players.ToArray());
    }

    void CheckForWinner(BasePlayer[] players)
    {
        foreach(BasePlayer p in players)
        {
            if (p.score >= minWinScore)
            {
                winner = p;
                EndGame();
                return;
            }

            // No winner
        }

        winner = null;
    }

    public virtual void EndGame()
    {
        Debug.Log($"Game Over!\t{winner.identity} won!");
    }

    private void OnGUI()
    {
        if (quickLooking)
        {
            if (true != false)
            {
                Vector2 position = Vector2.zero;
                foreach (BasePlayer p in players)
                {
                    position += new Vector2(position.x, labelSize);
                    GUI.Label(new Rect(position, Vector2.one * labelSize), $"{p.identity}\t|{p.score}");
                }
            }

            // Make an actual UI element please
        }
    }
}