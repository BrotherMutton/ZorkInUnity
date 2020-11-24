using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Zork;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string GameFilename = "Zork";

    [SerializeField]
    private UnityInputService InputService;

    [SerializeField]
    private UnityOutputService OutputService;

    [SerializeField]
    private TextMeshProUGUI LocationText;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI MovesText;

    private void Start()
    {
        TextAsset gameTextAsset = Resources.Load<TextAsset>(GameFilename);
        _game = JsonConvert.DeserializeObject<Game>(gameTextAsset.text);
        _game.Initalize(InputService, OutputService);
        LocationText.text = _game.Player.Location.Name;

        _game.Player.LocationChanged += Player_LocationChanged;
        _game.Player.MovesChanged += (sender, e) => MovesText.text = $"Moves: {e}";
        _game.Player.ScoreChanged += (sender, e) => ScoreText.text = $"Score: {e}";

        InputService.InputField.Select();
        InputService.InputField.ActivateInputField();
    }

    private void Player_LocationChanged(object sender, LocationChangedEventArgs e)
    {
        LocationText.text = e.NewLocation != null ? e.NewLocation.Name : "Unknown";
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return) && string.IsNullOrEmpty(InputService.InputField.text) == false)
        {
            InputService.ProcessInput();
            InputService.InputField.Select();
            InputService.InputField.ActivateInputField();
        }


        if (_game.IsRunning == false)
        {
#if UNITY_EDITOR
            // Application.Quit does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private Game _game;
}
