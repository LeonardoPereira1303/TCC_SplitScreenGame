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
    public Transform playerListPanel; // Painel para exibir os jogadores
    public GameObject playerInfoPrefab; // Prefab para as informações do jogador
    public Button startGameButton;
    public TMP_Text playerCountText;

    [Header("Lobby Settings")]
    public int maxPlayers = 4;
    public string gameSceneName; // Nome da cena do jogo

    private List<PlayerInput> players = new List<PlayerInput>();

    void Start()
    {
        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(StartGame);
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
        UpdateLobbyUI();
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        if (players.Count < maxPlayers)
        {
            players.Add(playerInput);
            AddPlayerToUI(playerInput);
        }
        else
        {
            Destroy(playerInput.gameObject);
        }
        UpdateLobbyUI();
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput);
        RemovePlayerFromUI(playerInput);
        UpdateLobbyUI();
    }

    void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void AddPlayerToUI(PlayerInput playerInput)
    {
        GameObject playerInfo = Instantiate(playerInfoPrefab, playerListPanel);
        Text playerNameText = playerInfo.transform.Find("PlayerNameText").GetComponent<Text>();
        Image playerIcon = playerInfo.transform.Find("PlayerIcon").GetComponent<Image>();

        playerNameText.text = "Player " + (playerInput.playerIndex + 1);
        playerIcon.color = Random.ColorHSV();
        playerInfo.name = "PlayerInfo_" + playerInput.playerIndex;
    }

    void RemovePlayerFromUI(PlayerInput playerInput)
    {
        Transform playerInfo = playerListPanel.Find("PlayerInfo_" + playerInput.playerIndex);
        if (playerInfo != null)
        {
            Destroy(playerInfo.gameObject);
        }
    }

    void UpdateLobbyUI()
    {
        playerCountText.text = "Players: " + players.Count + "/" + maxPlayers;
        startGameButton.interactable = players.Count >= 2;
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
