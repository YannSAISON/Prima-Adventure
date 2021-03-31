using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwagBar : MonoBehaviour
{
    private PlayerStateManager _playerState;

    public GameObject _swagBar;
    // Start is called before the first frame update
    void Start()
    {
        _playerState = GameObject.FindWithTag("Player").GetComponent<PlayerStateManager>();
        Debug.Log($"_playerState: {_playerState == null} - _swagBar: {_swagBar == null}");
        _swagBar.GetComponent<Slider>().maxValue = _playerState.swagMax;
        // _swagBar.maxValue = _playerState.swagMax;
    }

    // Update is called once per frame
    void Update()
    {
        _swagBar.GetComponent<Slider>().value = _playerState.GetSwagValue();
        // _swagBar.value = _playerState.GetSwagValue();
    }
}
