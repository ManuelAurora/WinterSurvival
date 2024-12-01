using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreMountains.Feedbacks;
using UnityEngine;

public class CoverView : MonoBehaviour
{
    public RectTransform introScreenImage;
    public RectTransform background;
    public MMF_Player openFeedback;
    public MMF_Player closeFeedback;

    public void Open()
    {
        introScreenImage.localScale = Vector3.one * .1f;
        background.gameObject.SetActive(true);
        introScreenImage.gameObject.SetActive(true);
        openFeedback.PlayFeedbacks();
        var duration = openFeedback.TotalDuration;
        StopFeedbackAfter(duration, closeFeedback);
        DisableAfter(duration);
    }

    public void Close()
    {
        introScreenImage.localScale = Vector3.one * 40;
        background.gameObject.SetActive(true);
        introScreenImage.gameObject.SetActive(true);
        closeFeedback.PlayFeedbacks();
    }
    
    async private void StopFeedbackAfter(float duration, MMF_Player player)
    {
        await Task.Delay(TimeSpan.FromSeconds(duration));
        player.StopFeedbacks(true);
    }

    async private void DisableAfter(float duration)
    {
        await Task.Delay(TimeSpan.FromSeconds(duration));
        introScreenImage.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
