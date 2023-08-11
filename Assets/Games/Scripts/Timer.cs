using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {

	public float startTime = 10.0f;
	private float timeRemaining = 0.0f;

	public TextMeshProUGUI timeRemainingLabel;

	public void StartClock() {
		timeRemaining = startTime;
	}

	private void Update() {
		if (!GameManager.instance.GameIsPlaying) {
			return;
		}

		timeRemaining -= Time.deltaTime;
		
		if (timeRemaining <= 0.0f) {
			GameManager.instance.GameOver();
			timeRemaining = 0;
		}

		if (timeRemaining >= 0.0f) {
			var labelString = $"{(int)timeRemaining}";
			timeRemainingLabel.SetText(labelString);
		}
	}
}
