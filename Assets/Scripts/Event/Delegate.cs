using System;
using UnityEngine;
public class Delegate : MonoBehaviour
{
    delegate void AksiSederhana();

    void Start()
    {
        Langkah1_SimpanSatuMethod();
        Langkah2_BeberapaMethodSekaligus();
        Langkah3_ActionSiapPakai();
    }

    void Langkah1_SimpanSatuMethod()
    {
        AksiSederhana kotak = TulisHalo;
        kotak();
    }

    void Langkah2_BeberapaMethodSekaligus()
    {
        AksiSederhana kotak = TulisHalo;
        kotak += TulisDunia;
        kotak();
    }

    void Langkah3_ActionSiapPakai()
    {
        Action kotak = TulisHalo;
        kotak += TulisDunia;
        kotak();
    }

    void TulisHalo()
    {
        Debug.Log("Halo");
    }

    void TulisDunia()
    {
        Debug.Log("Dunia");
    }
}
