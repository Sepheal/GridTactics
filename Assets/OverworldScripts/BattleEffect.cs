using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BattleEffect : MonoBehaviour
{
    bool Active = false;
    LensDistortion MyLens;

    float FrameStart;
    readonly float FrameCooldown = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        FrameStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            //if (Time.time - FrameStart >= FrameCooldown)
            //{
            //    MyLens.intensity.value -= 5.0f;
            //    FrameStart = Time.time;
            //}
            MyLens.intensity.value -= 5.0f * Time.deltaTime * 30f;
        }
    }

    public void StartAnim()
    {
        PostProcessVolume MyVolume = GetComponent<PostProcessVolume>();
        MyVolume.profile.TryGetSettings(out MyLens);

        MyLens.intensity.value = 0.0f;
        Active = true;
    }
}
