/*
*	TickLuck
*	All rights reserved
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DistanceSlider : MonoBehaviour
{
    #region Fields
    [SerializeField] private Transform hero;
    [SerializeField] private Transform target;

    private Slider this_slider;
    #endregion

    // Idea is to come from Distance to 0 by Slider

    #region Unity Methods
    void Start()
    {
        this_slider = GetComponent<Slider>();
        this_slider.maxValue = Vector3.Distance(hero.position, target.position);
        this_slider.value = this_slider.maxValue;
    }

    private IEnumerator UpdateSlider()
    {
        yield return new WaitForSeconds(0.2f);

        if (this_slider.value <= 5f)
        {
            this_slider.value = 0f;
        }
        else
            this_slider.value = Vector3.Distance(hero.position, target.position);
    }

    void Update()
    {
        if (PlayerStackColorController.is_Playing)
        {
            StartCoroutine(UpdateSlider());
        }
    }
    #endregion
}
