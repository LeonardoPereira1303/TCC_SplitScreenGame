using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text lobbyStatusText;
    public Button startGameButton;
    public string sceneGameLoad;

    [Header("Lobby Settings")]
    public int maxPlayers = 4;

    private List<PlayerInput> players = new List<PlayerInput>();

    void Start()
    {
        // Initialize UI
        UpdateLobbyStatus();

        // Disable start button initially
        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(StartGame);

        // Subscribe to player joining events
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        if (players.Count < maxPlayers)
        {
            players.Add(playerInput);
            Debug.Log(playerInput.playerIndex + " joined the lobby.");
            UpdateLobbyStatus();
        }
        else
        {
            Debug.Log("Lobby is full. Player " + playerInput.playerIndex + " cannot join.");
            Destroy(playerInput.gameObject);
        }
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput);
        Debug.Log(playerInput.playerIndex + " left the lobby.");
        UpdateLobbyStatus();
    }

    void StartGame()
    {
        Debug.Log("Game is starting with players:");
        foreach (var player in players)
        {
            Debug.Log("Player " + player.playerIndex);
        }

        SceneManager.LoadScene(sceneGameLoad);
    }

    void UpdateLobbyStatus()
    {
        lobbyStatusText.text = "Players in Lobby: " + players.Count + "/" + maxPlayers;

        // Enable or disable start button based on player count
        startGameButton.interactable = players.Count > 1;
    }

    void OnDestroy()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
            PlayerInputManager.instance.onPlayerLeft -= OnPlayerLeft;
        }
    }
}
