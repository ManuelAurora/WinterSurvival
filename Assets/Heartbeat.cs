using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Heartbeat : MonoBehaviour
{
    public float scaleAmount = 1.5f; // How much the heart scales up
    public float beatDuration = 0.3f; // Duration of one beat
    public float pauseDuration = 0.3f; // Pause between beats
    public float appearDuration = 0.1f; // Duration for heart to appear and disappear
    
    public void AnimateHeart(TweenCallback completion)
    {
        // Set initial scale to zero for the appearance effect
        transform.localScale = Vector3.one;

        // Create a sequence for the heartbeat animation
         Sequence heartbeatSequence = DOTween.Sequence();

        // Heart appears
        heartbeatSequence.Append(transform.DOScale(Vector3.one, appearDuration));

        // First beat
        heartbeatSequence.Append(transform.DOScale(scaleAmount, beatDuration).SetEase(Ease.InOutSine));
        heartbeatSequence.Append(transform.DOScale(Vector3.one, beatDuration).SetEase(Ease.OutBounce));
        heartbeatSequence.Append(transform.DOScale(1.25f , beatDuration / 2).SetEase(Ease.InOutSine));
        heartbeatSequence.Append(transform.DOScale(Vector3.one, beatDuration).SetEase(Ease.OutBounce));
        
        // Pause
        heartbeatSequence.AppendInterval(pauseDuration);

        // Second beat
        heartbeatSequence.Append(transform.DOScale(scaleAmount, beatDuration).SetEase(Ease.InOutSine));
        heartbeatSequence.Append(transform.DOScale(Vector3.one, beatDuration).SetEase(Ease.OutBounce));
        heartbeatSequence.Append(transform.DOScale(1.25f, beatDuration / 2).SetEase(Ease.InOutSine));
        heartbeatSequence.Append(transform.DOScale(Vector3.one, beatDuration).SetEase(Ease.OutBounce));

        // Pause
        heartbeatSequence.AppendInterval(pauseDuration);

        // Heart disappears
        heartbeatSequence.Append(transform.DOScale(Vector3.zero, appearDuration).SetEase(Ease.InBack));

        // Start the sequence
        heartbeatSequence.Play();
        heartbeatSequence.onComplete = completion;
    }
    
}