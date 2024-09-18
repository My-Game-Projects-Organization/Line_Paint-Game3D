using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoudManager : Singleton<SoudManager>
{

    [SerializeField] private AudioClip btnFx, brushMoveFx, victoryFx;
    [SerializeField] private AudioSource fxSource;


    public void PlayFx(FxType fxType)
    {
        switch (fxType)
        {
            case FxType.Button:
                fxSource.PlayOneShot(btnFx);
                break;
            case FxType.BrushMove:
                fxSource.PlayOneShot(brushMoveFx);
                break;
            case FxType.Victory:
                fxSource.PlayOneShot(victoryFx);
                break;
        }
    }
}

public enum FxType
{
    Button,
    BrushMove,
    Victory
}
